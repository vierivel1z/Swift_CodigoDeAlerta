using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;
using Swift.UI;
using Swift.Game;
using Swift.Bank;
using Swift.Interaction;
namespace Swift.EditorTools
{
    // Local, opt-in file queue. Does nothing unless a command file exists.
    [InitializeOnLoad]
    public static class OperationsLightingReview
    {
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Out="Assets/_Project/OfficeVisual/LightingReview";
        const string Queue="Artifacts/LightingReview/command.txt";
        static bool busy;
        static OperationsLightingReview() { EditorApplication.update += Tick; }
        static void Tick()
        {
            if(busy || EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(Queue)) return;
            string command=File.ReadAllText(Queue).Trim(); File.Delete(Queue); busy=true;
            try
            {
                if(command=="inspect") Inspect();
                else if(command=="before") Capture("Before");
                else if(command=="apply") Apply();
                else if(command=="bake") Bake();
                else if(command=="checkpoint") Checkpoint();
                else if(command=="refine") Refine();
                else if(command=="after") { Validate(); Capture("After"); }
                else if(command=="play") BeginPlayCheck();
                else if(command=="reopen") { CheckedScene(); EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single); Validate(); Capture("After"); }
                else throw new Exception("Unknown command: "+command);
                File.WriteAllText("Artifacts/LightingReview/last-result.txt",command+" PASS "+DateTime.UtcNow.ToString("O"));
            }
            catch(Exception e) { File.WriteAllText("Artifacts/LightingReview/last-result.txt",command+" FAIL "+e); Debug.LogException(e); }
            finally { busy=false; }
        }
        static Scene CheckedScene()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play before editing.");
            if(SceneManager.sceneCount!=1 || SceneManager.GetActiveScene().path!=ScenePath) throw new Exception("Open only BancoCorresponsal.");
            var scene=SceneManager.GetActiveScene();
            if(scene.isDirty) throw new Exception("Unsaved scene: preserving user edits. Save/review before applying.");
            return scene;
        }
        static T[] All<T>() where T:Component => SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static void Inspect()
        {
            var lines=new System.Collections.Generic.List<string>();
            lines.Add("Unity "+Application.unityVersion+" GPU "+SystemInfo.graphicsDeviceType+" pipeline "+AssetDatabase.GetAssetPath(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline));
            lines.Add("Scene "+SceneManager.GetActiveScene().path+" dirty="+SceneManager.GetActiveScene().isDirty+" playing="+EditorApplication.isPlaying);
            lines.Add("Lightmaps="+LightmapSettings.lightmaps.Length+" ReflectionProbes="+All<ReflectionProbe>().Length+" LightProbeGroups="+All<LightProbeGroup>().Length);
            foreach(var light in All<Light>()) lines.Add("Light "+light.name+" active="+light.gameObject.activeInHierarchy+" type="+light.type+" intensity="+light.intensity+" bake="+light.lightmapBakeType);
            foreach(var r in All<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy)) lines.Add("Mesh "+r.name+" position="+r.transform.position+" bounds="+r.bounds+" materials="+string.Join(",",r.sharedMaterials.Select(m=>m==null?"NULL":m.name+"/"+m.shader.name)));
            File.WriteAllLines("Artifacts/LightingReview/inspection.txt",lines);
        }
        static Transform Group(Transform parent,string name) { var g=new GameObject(name); Undo.RegisterCreatedObjectUndo(g,"Operations lighting"); g.transform.SetParent(parent,false); return g.transform; }
        static Material Material(string name,Color color,bool emissive=false)
        {
            var path=Out+"/"+name+".mat"; var m=AssetDatabase.LoadAssetAtPath<Material>(path); if(m!=null) return m;
            m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name}; m.SetColor("_BaseColor",color); m.SetFloat("_Smoothness",.16f);
            if(emissive) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor",color*1.2f); m.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive; }
            AssetDatabase.CreateAsset(m,path); return m;
        }
        static GameObject Box(Transform parent,string name,Vector3 pos,Vector3 scale,Material mat)
        {
            var obj=GameObject.CreatePrimitive(PrimitiveType.Cube); Undo.RegisterCreatedObjectUndo(obj,"Operations lighting"); obj.name=name; obj.transform.SetParent(parent,false); obj.transform.position=pos; obj.transform.localScale=scale;
            obj.GetComponent<Renderer>().sharedMaterial=mat;
            // Fixtures are decoration and do not block the existing gaze raycasts.
            UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>()); return obj;
        }
        [MenuItem("SWIFT/Centro de operaciones/Aplicar UI e iluminacion nocturna")]
        public static void Apply()
        {
            var scene=CheckedScene();
            if(GameObject.Find("BankOfficeVisualV1/07_OperationsLighting")!=null) throw new Exception("Already applied; edit existing objects instead of duplicating.");
            Directory.CreateDirectory(Out); Directory.CreateDirectory("Assets/_Project/UI/OperationsCenter/Prefabs");
            var root=GameObject.Find("BankOfficeVisualV1").transform;
            var backup=Path.Combine(Path.GetTempPath(),"SWIFT-Lighting-Editor-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff")); Directory.CreateDirectory(backup);
            File.Copy(ScenePath,Path.Combine(backup,"BancoCorresponsal.unity")); File.Copy(ScenePath+".meta",Path.Combine(backup,"BancoCorresponsal.unity.meta"));
            File.WriteAllText("Artifacts/LightingReview/editor-backup.txt",backup);
            var lighting=Group(root,"07_OperationsLighting");
            var housing=Material("Luminaire_Slate",new Color(.12f,.17f,.20f));
            var diffuser=Material("Luminaire_Neutral",new Color(.9f,.86f,.74f),true);
            var wall=Material("Wall_Neutral",new Color(.66f,.69f,.68f));
            var ceiling=Material("Ceiling_Neutral",new Color(.72f,.73f,.70f));
            var floor=Material("Floor_BlueGrey",new Color(.20f,.25f,.29f));
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy))
            {
                Undo.RecordObject(r,"Baked office lighting");
                if(r.name=="Floor") r.sharedMaterial=floor;
                else if(r.name=="Ceiling") r.sharedMaterial=ceiling;
                else if(r.transform.parent.name=="01_Architecture" && (r.name.Contains("Wall") || r.name.Contains("infill") || r.name=="EntranceLintel")) r.sharedMaterial=wall;
                r.shadowCastingMode=ShadowCastingMode.On; r.receiveShadows=true;
                GameObjectUtility.SetStaticEditorFlags(r.gameObject,GameObjectUtility.GetStaticEditorFlags(r.gameObject)|StaticEditorFlags.ContributeGI);
                // Generate UV2 only for models actually used, retaining source meshes/materials.
                var mf=r.GetComponent<MeshFilter>(); if(mf==null || mf.sharedMesh==null) continue;
                string modelPath=AssetDatabase.GetAssetPath(mf.sharedMesh);
                var importer=AssetImporter.GetAtPath(modelPath) as ModelImporter;
                if(importer!=null && !importer.generateSecondaryUV)
                {
                    string meta=modelPath+".meta"; string copy=Path.Combine(backup,modelPath.Replace('/','_')+".meta"); if(File.Exists(meta)&&!File.Exists(copy)) File.Copy(meta,copy);
                    importer.generateSecondaryUV=true; importer.SaveAndReimport();
                }
            }
            foreach(var key in root.GetComponentsInChildren<Light>())
            { Undo.RecordObject(key,"Balanced night fill"); key.color=new Color(.9f,.94f,1); key.intensity=.32f; key.shadows=LightShadows.None; key.lightmapBakeType=LightmapBakeType.Realtime; RenderSettings.sun=key; }
            RenderSettings.skybox=null; RenderSettings.ambientMode=AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.15f,.19f,.25f); RenderSettings.ambientEquatorColor=new Color(.13f,.16f,.19f); RenderSettings.ambientGroundColor=new Color(.08f,.09f,.11f);
            RenderSettings.ambientIntensity=1; RenderSettings.reflectionIntensity=.25f; RenderSettings.fog=false;
            int id=0;
            foreach(float x in new[]{-2.7f,2.7f}) foreach(float z in new[]{-2.4f,.8f,3.5f})
            {
                ++id; Box(lighting,"Ceiling fixture "+id,new Vector3(x,3.27f,z),new Vector3(1.3f,.08f,.55f),housing);
                Box(lighting,"Neutral diffuser "+id,new Vector3(x,3.22f,z),new Vector3(1.18f,.025f,.43f),diffuser);
                var obj=new GameObject("Baked ceiling light "+id,typeof(Light)); obj.transform.SetParent(lighting,false); obj.transform.position=new Vector3(x,3.13f,z); obj.transform.rotation=Quaternion.Euler(90,0,0);
                var light=obj.GetComponent<Light>(); light.type=LightType.Spot; light.spotAngle=118; light.innerSpotAngle=72; light.range=7; light.intensity=4.2f; light.color=new Color(1,.92f,.80f); light.lightmapBakeType=LightmapBakeType.Baked; light.shadows=LightShadows.Soft; light.bounceIntensity=1;
            }
            var work=new GameObject("Baked inspection soft fill",typeof(Light)); work.transform.SetParent(lighting,false); work.transform.position=new Vector3(0,2.7f,1.25f);
            var task=work.GetComponent<Light>(); task.type=LightType.Point; task.range=3.4f; task.intensity=1.2f; task.color=new Color(1,.95f,.87f); task.lightmapBakeType=LightmapBakeType.Baked; task.shadows=LightShadows.Soft;
            // Probe interpolation is useful for the rig/reticle; no realtime reflection probe.
            var probes=new GameObject("Baked light probes",typeof(LightProbeGroup)); probes.transform.SetParent(lighting,false);
            probes.GetComponent<LightProbeGroup>().probePositions=(from x in new[]{-3.5f,0,3.5f} from z in new[]{-3.5f,0,3.5f} from y in new[]{.45f,1.6f,2.8f} select new Vector3(x,y,z)).ToArray();
            foreach(var r in lighting.GetComponentsInChildren<MeshRenderer>()) GameObjectUtility.SetStaticEditorFlags(r.gameObject,StaticEditorFlags.ContributeGI);
            var ui=All<UIManager>().Single(x=>x.gameObject.activeInHierarchy); Undo.RecordObject(ui,"Operations UI"); ui.OperationsCenterStyle=true; ui.RefreshPresentation();
            GeneratePrefabs(ui);
            var settings=new LightingSettings { name="OfficeNight_Baked",bakedGI=true,realtimeGI=false,lightmapper=LightingSettings.Lightmapper.ProgressiveCPU,lightmapResolution=16,lightmapMaxSize=1024,lightmapPadding=4,indirectSampleCount=32,directSampleCount=32,environmentSampleCount=32,maxBounces=2,ao=true,aoMaxDistance=.65f,aoExponentIndirect=1,aoExponentDirect=.25f };
            AssetDatabase.CreateAsset(settings,Out+"/OfficeNight_Baked.lighting"); Lightmapping.lightingSettings=settings;
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); if(!EditorSceneManager.SaveScene(scene)) throw new Exception("Could not save scene");
            Inspect(); Debug.Log("SWIFT_OPERATIONS_LIGHTING_APPLIED");
        }
        [MenuItem("SWIFT/Centro de operaciones/Generar siete prefabs UI")]
        public static void GeneratePrefabsMenu()
        {
            CheckedScene(); GeneratePrefabs(All<UIManager>().Single(x=>x.gameObject.activeInHierarchy)); AssetDatabase.SaveAssets();
        }
        static void GeneratePrefabs(UIManager live)
        {
            string dest="Assets/_Project/UI/OperationsCenter/Prefabs/";
            var temp=new GameObject("OC_Expedientes");
            try
            {
                var ui=temp.AddComponent<UIManager>(); ui.Font=live.Font; ui.PanelAnchor=temp.transform; ui.CompactMonitor=true; ui.OperationsCenterStyle=true; ui.BuildInterface();
                PrefabUtility.SaveAsPrefabAsset(temp,dest+"OC_Expedientes.prefab");
                var canvas=temp.GetComponentInChildren<Canvas>();
                foreach(var child in new[]{"OC Chrome/Header","OC Chrome/Document well","OC Chrome/Feedback well","APROBAR","RETENER","DEVOLVER"})
                {
                    var source=canvas.transform.Find(child).gameObject; var clone=UnityEngine.Object.Instantiate(source); clone.name="OC_"+source.name.Replace(" ","_");
                    // Decision pieces contain their collider/action; assign Manager when composing separately.
                    PrefabUtility.SaveAsPrefabAsset(clone,dest+clone.name+".prefab"); UnityEngine.Object.DestroyImmediate(clone);
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(temp); }
        }
        [MenuItem("SWIFT/Centro de operaciones/Ajustar segunda pasada de luz")]
        public static void Refine()
        {
            var scene=CheckedScene(); Checkpoint();
            var root=GameObject.Find("BankOfficeVisualV1").transform;
            foreach(var light in root.GetComponentsInChildren<Light>())
            {
                Undo.RecordObject(light,"Night lighting second pass");
                if(light.type==LightType.Directional) light.intensity=.44f;
                else if(light.name.StartsWith("Baked ceiling")) light.intensity=9.5f;
                else if(light.name=="Baked inspection soft fill")
                {
                    light.type=LightType.Spot; light.spotAngle=105; light.innerSpotAngle=70; light.intensity=6.5f; light.range=5;
                    light.transform.position=new Vector3(0,3.13f,1.25f); light.transform.rotation=Quaternion.Euler(90,0,0);
                }
            }
            var zone=root.Find("07_OperationsLighting");
            if(zone.Find("Inspection ceiling fixture")==null)
            {
                Box(zone,"Inspection ceiling fixture",new Vector3(0,3.27f,1.25f),new Vector3(1.3f,.08f,.55f),AssetDatabase.LoadAssetAtPath<Material>(Out+"/Luminaire_Slate.mat"));
                Box(zone,"Inspection neutral diffuser",new Vector3(0,3.22f,1.25f),new Vector3(1.18f,.025f,.43f),AssetDatabase.LoadAssetAtPath<Material>(Out+"/Luminaire_Neutral.mat"));
            }
            foreach(var r in zone.GetComponentsInChildren<MeshRenderer>()) GameObjectUtility.SetStaticEditorFlags(r.gameObject,StaticEditorFlags.ContributeGI);
            var oak=Material("Desk_LightOak",new Color(.47f,.32f,.19f)); var oakDark=Material("Desk_OakInset",new Color(.29f,.19f,.11f));
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>().Where(x=>x.name=="Desk"||x.name.StartsWith("Desk_Drawer")))
            {
                Undo.RecordObject(r,"Readable oak desk"); r.sharedMaterials=r.sharedMaterials.Select(m=>m.name.Contains("3F2815")?oak:m.name.Contains("291A0E")?oakDark:m).ToArray();
            }
            RenderSettings.ambientSkyColor=new Color(.20f,.24f,.30f); RenderSettings.ambientEquatorColor=new Color(.17f,.20f,.23f); RenderSettings.ambientGroundColor=new Color(.10f,.12f,.14f);
            var ui=All<UIManager>().Single(x=>x.gameObject.activeInHierarchy); ui.RefreshPresentation(); GeneratePrefabs(ui);
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }
        static void Checkpoint()
        {
            var scene=SceneManager.GetActiveScene();
            if(EditorApplication.isPlayingOrWillChangePlaymode || scene.path!=ScenePath || SceneManager.sceneCount!=1) throw new Exception("Invalid checkpoint scene");
            string recovery=AssetDatabase.GenerateUniqueAssetPath("Assets/_Project/Scenes/Backups/BancoCorresponsal_LightingRecovery.unity");
            if(!EditorSceneManager.SaveScene(scene,recovery,true)) throw new Exception("Recovery copy failed");
            if(!EditorSceneManager.SaveScene(scene)) throw new Exception("Save failed");
            File.AppendAllText("Artifacts/LightingReview/checkpoints.txt",recovery+"\n");
        }
        [MenuItem("SWIFT/Centro de operaciones/Hornear luces de oficina")]
        public static void Bake()
        {
            CheckedScene();
            Lightmapping.bakeCompleted-=BakeFinished; Lightmapping.bakeCompleted+=BakeFinished;
            if(!Lightmapping.BakeAsync()) throw new Exception("Lightmapping refused bake");
            File.WriteAllText("Artifacts/LightingReview/bake.txt","RUNNING "+DateTime.UtcNow.ToString("O"));
        }
        static void BakeFinished()
        {
            Lightmapping.bakeCompleted-=BakeFinished; AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            File.WriteAllText("Artifacts/LightingReview/bake.txt","COMPLETE lightmaps="+LightmapSettings.lightmaps.Length+" "+DateTime.UtcNow.ToString("O"));
        }
        static void Validate()
        {
            CheckedScene();
            var bad=All<MeshRenderer>().Where(r=>r.gameObject.activeInHierarchy).SelectMany(r=>r.sharedMaterials).Where(m=>m==null||m.shader==null||m.shader.name=="Hidden/InternalErrorShader"||!m.shader.isSupported).ToArray();
            if(bad.Length!=0) throw new Exception("Unsupported/missing renderer materials: "+bad.Length);
            var actions=All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy).ToArray();
            if(actions.Count(x=>x.Action==BankAction.Teleport)!=4) throw new Exception("Four teleport destinations required");
            foreach(var action in new[]{BankAction.Inspect,BankAction.Registry,BankAction.Alerts,BankAction.Defense,BankAction.End}) if(!actions.Any(x=>x.Action==action)) throw new Exception("Missing action "+action);
            if(LightmapSettings.lightmaps.Length==0) throw new Exception("Lighting has not baked");
            File.WriteAllText("Artifacts/LightingReview/validation.txt","PASS: renderer shaders, five interactions, four teleports, baked lightmaps="+LightmapSettings.lightmaps.Length+", prefabs="+Directory.GetFiles("Assets/_Project/UI/OperationsCenter/Prefabs","*.prefab").Length+". No Android measurement.");
        }
        static SceneReviewCapture.View[] Views() => new[]
        {
            new SceneReviewCapture.View {file="01-oficina.png",position=new Vector3(0,2.65f,-4.5f),target=new Vector3(0,1.1f,1),fieldOfView=85},
            new SceneReviewCapture.View {file="02-escritorio.png",position=new Vector3(1.9f,2.2f,-.9f),target=new Vector3(0,1.1f,1.6f)},
            new SceneReviewCapture.View {file="03-computadora-expedientes.png",position=new Vector3(-.15f,1.85f,-.3f),target=new Vector3(-.15f,1.4f,1.4f),fieldOfView=65},
            new SceneReviewCapture.View {file="04-distribucion-superior.png",position=new Vector3(0,12,0),target=Vector3.zero,orthographicSize=5.7f,hideCeiling=true},
            new SceneReviewCapture.View {file="05-iluminacion.png",position=new Vector3(-4.25f,2.0f,-2.8f),target=new Vector3(.8f,1.6f,2.6f),fieldOfView=78}
        };
        [Serializable] class Receipt { public string utc,scene,sceneSha256,unity,pipeline,quality; public string[] files,sha256; public string note="Real Unity Editor renders; top view hides Ceiling temporarily. Not Android or Play evidence."; }
        static string Hash(string path) { using(var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant(); }
        static void Capture(string label)
        {
            var scene=CheckedScene(); if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null) throw new Exception("GPU required");
            string output="Docs/Previews/LightingReview/"+label+"-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"); Directory.CreateDirectory(output);
            var views=Views(); Canvas.ForceUpdateCanvases();
            var render=typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static);
            foreach(var view in views)
            {
                var fixtures=All<MeshRenderer>().Where(r=>r.transform.parent!=null && r.transform.parent.name=="07_OperationsLighting").ToArray();
                var previous=fixtures.Select(r=>r.forceRenderingOff).ToArray();
                try { if(view.hideCeiling) foreach(var r in fixtures) r.forceRenderingOff=true; render.Invoke(null,new object[]{scene,view,Path.Combine(output,view.file)}); }
                finally { for(int i=0;i<fixtures.Length;i++) fixtures[i].forceRenderingOff=previous[i]; }
            }
            var report=new Receipt{utc=DateTime.UtcNow.ToString("O"),scene=ScenePath,sceneSha256=Hash(ScenePath),unity=Application.unityVersion,pipeline=AssetDatabase.GetAssetPath(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline),quality=QualitySettings.names[QualitySettings.GetQualityLevel()],files=views.Select(v=>v.file).ToArray(),sha256=views.Select(v=>Hash(Path.Combine(output,v.file))).ToArray()};
            File.WriteAllText(Path.Combine(output,"capture.json"),JsonUtility.ToJson(report,true));
            File.WriteAllText(Path.Combine(output,"README.md"),"# "+label+" — BancoCorresponsal\n\nCapturas reales del proyecto principal, Unity "+report.unity+". Perfil "+report.quality+". No certifican Android; techo oculto solo en planta.\n\n"+string.Join("\n\n",views.Select(v=>"!["+v.file+"]("+v.file+")")));
            File.WriteAllText("Artifacts/LightingReview/"+label.ToLowerInvariant()+"-capture.txt",output);
        }
        static void BeginPlayCheck()
        {
            CheckedScene(); SessionState.SetBool("SWIFT_LIGHTING_PLAY_CHECK",true);
            EditorApplication.playModeStateChanged-=PlayState; EditorApplication.playModeStateChanged+=PlayState; EditorApplication.EnterPlaymode();
        }
        [InitializeOnLoadMethod] static void RestorePlayCheck() { if(SessionState.GetBool("SWIFT_LIGHTING_PLAY_CHECK",false)) { EditorApplication.playModeStateChanged-=PlayState; EditorApplication.playModeStateChanged+=PlayState; } }
        static void PlayState(PlayModeStateChange state)
        {
            if(state!=PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("SWIFT_LIGHTING_PLAY_CHECK",false)) return;
            SessionState.SetBool("SWIFT_LIGHTING_PLAY_CHECK",false); EditorApplication.delayCall+=PlayCheck;
        }
        static void PlayCheck()
        {
            try
            {
                var game=UnityEngine.Object.FindFirstObjectByType<GameManager>(); var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();
                game.StartScenario(ScenarioMode.Attack); Canvas.ForceUpdateCanvases();
                var targets=ui.PanelAnchor.GetComponentsInChildren<BankInteractable>(true).Where(x=>x.Action==BankAction.Approve||x.Action==BankAction.Hold||x.Action==BankAction.Return).ToArray();
                if(targets.Length!=3||targets.Any(x=>!x.gameObject.activeInHierarchy)) throw new Exception("Decision buttons missing");
                Physics.SyncTransforms();
                foreach(var target in targets)
                {
                    var origin=new Vector3(0,1.6f,-.65f); var delta=target.transform.position-origin;
                    if(!Physics.Raycast(origin,delta.normalized,out var hit,delta.magnitude+.2f) || hit.collider.GetComponent<BankInteractable>()!=target) throw new Exception("Inspection gaze ray blocked: "+target.name);
                }
                game.RegisterEvidence(); game.InspectRegistry(); game.InspectAlerts();
                // Render runtime dossier with actual session data using the real UI, without saving it.
                var scene=SceneManager.GetActiveScene(); string dir="Docs/Previews/LightingReview/Play-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"); Directory.CreateDirectory(dir);
                var render=typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static);
                render.Invoke(null,new object[]{scene,Views()[2],dir+"/expediente-activo.png"});
                for(int i=0;i<6;i++) targets.Single(x=>x.Action==BankAction.Approve).Activate();
                if(!game.Session.Ended||game.LastAttackLoss!=965000||targets.Any(x=>x.gameObject.activeSelf)) throw new Exception("Attack/results lifecycle failed");
                game.StartScenario(ScenarioMode.Defense); game.ApplyDefense();
                if(targets.Any(x=>!x.gameObject.activeSelf)) throw new Exception("Buttons did not restore");
                for(int i=0;i<6;i++) targets.Single(x=>x.Action==BankAction.Approve).Activate();
                if(!game.Session.Ended||game.LastDefenseLoss!=0) throw new Exception("Defense comparison failed");
                Canvas.ForceUpdateCanvases(); render.Invoke(null,new object[]{scene,Views()[2],dir+"/resultados.png"});
                var receipt=new Receipt {utc=DateTime.UtcNow.ToString("O"),scene=ScenePath,sceneSha256=Hash(ScenePath),unity=Application.unityVersion,pipeline=AssetDatabase.GetAssetPath(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline),quality=QualitySettings.names[QualitySettings.GetQualityLevel()],files=new[]{"expediente-activo.png","resultados.png"},sha256=new[]{Hash(dir+"/expediente-activo.png"),Hash(dir+"/resultados.png")},note="Real Play Mode renders of dynamic fictional data. Automated actions from inspection point; no Android measurement."};
                File.WriteAllText(dir+"/capture.json",JsonUtility.ToJson(receipt,true));
                File.WriteAllText("Artifacts/LightingReview/play-capture.txt",dir);
                File.WriteAllText("Artifacts/LightingReview/play-validation.txt","PASS: inspection raycasts,  actual Play mode, dynamic dossier, three collider actions, six decisions, results hide/restore, evidence, registry, alerts, attack 965000 / defense 0. Not Android.");
            }
            catch(Exception e) { File.WriteAllText("Artifacts/LightingReview/play-validation.txt","FAIL: "+e); Debug.LogException(e); }
            finally { EditorApplication.ExitPlaymode(); }
        }
    }
}

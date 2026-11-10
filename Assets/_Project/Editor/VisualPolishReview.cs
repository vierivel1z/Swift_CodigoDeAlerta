using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;
using Swift.UI;
using Swift.Game;
using Swift.Bank;
using Swift.Interaction;
using Swift.VR;

namespace Swift.EditorTools {
    // Local opt-in commands only. No scene generation or work on import.
    [InitializeOnLoad] public static class VisualPolishReview {
        const string Dir="Artifacts/VisualPolish", Queue=Dir+"/command.txt";
        const string Gallery="Docs/Previews/VisualPolishReview/20261103-BankPolish";
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity", Out="Assets/_Project/OfficeVisual/VisualPolish";
        static string command;static IEnumerator routine;static bool ownsPlay;static int errors,lastFrame=-1;
        static VisualPolishReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static void Check(bool ok,string s){if(!ok)throw new Exception(s);File.AppendAllText(Dir+"/checks.txt","PASS "+s+"\n");}
        static void Tick(){
            if(routine!=null){if(lastFrame==Time.frameCount)return;lastFrame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning||!File.Exists(Queue))return;
            string requested;try{requested=File.ReadAllText(Queue).Trim();File.Delete(Queue);}catch(IOException){return;}
            try{command=requested;errors=0;
                Check(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==ScenePath,"Only BancoCorresponsal open");
                if(command=="save"){Checkpoint("ExplicitSave");Finish(null);return;}
                if(command=="before"){Checkpoint("Before");Inspect("Before");Views("Before");Finish(null);return;}
                Check(!SceneManager.GetActiveScene().isDirty,"No unsaved scene discarded");
                if(command=="inspect"){Inspect("Current");Finish(null);}
                else if(command=="placement"){Placement();Finish(null);}
                else if(command=="surfaces"){Surfaces();Finish(null);}
                else if(command=="refine"){Refine();Finish(null);}
                else if(command=="finish-details"){Checkpoint("BeforeFinalDetails");foreach(var t in All<Transform>().Where(t=>t.gameObject.activeInHierarchy&&t.name.StartsWith("Cubicle route "))){Undo.RecordObject(t.gameObject,"Remove redundant old floor plate");t.gameObject.SetActive(false);}Save();Inspect("After");Views("After");Finish(null);}
                else if(command=="bake"){Checkpoint("BeforeBake");Lightmapping.bakeCompleted+=Baked;Check(Lightmapping.BakeAsync(),"Bake accepted");File.WriteAllText(Dir+"/bake-result.txt","RUNNING");}
                else if(command=="after"){Inspect("After");Views("After");Finish(null);}
                else if(command=="play"){SessionState.SetBool("SWIFT_POLISH_PLAY",true);EditorApplication.EnterPlaymode();}
                else throw new Exception("Unknown command "+command);
            }catch(Exception e){Finish(e);}
        }
        static void Baked(){Lightmapping.bakeCompleted-=Baked;Save();Finish(null);}
        static void PlayChanged(PlayModeStateChange s){if(s!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("SWIFT_POLISH_PLAY",false))return;SessionState.EraseBool("SWIFT_POLISH_PLAY");command="play";ownsPlay=true;errors=0;Application.logMessageReceived+=Log;routine=Play();}
        static void Log(string s,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;var s=(e==null&&errors==0?"PASS ":"FAIL ")+command+" "+DateTime.UtcNow.ToString("O")+" "+(e?.ToString()??("errors="+errors))+" EDITOR ONLY; ANDROID NOT TESTED";File.WriteAllText(Dir+"/"+command+"-result.txt",s);Debug.Log(s);if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();ownsPlay=false;}
        static void Save(){AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Scene saved through Unity APIs");}
        static void Checkpoint(string phase){var root=File.ReadAllText(Dir+"/backup-path.txt").Trim().Trim('\uFEFF');var dest=Path.Combine(root,phase+"-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));Directory.CreateDirectory(dest);File.Copy(ScenePath,dest+"/Scene-before-save.unity");File.Copy(ScenePath+".meta",dest+"/BancoCorresponsal.unity.meta");CopyTree(Path.ChangeExtension(ScenePath,null),dest+"/Lighting");Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Saved current memory with recovery copy");AssetDatabase.SaveAssets();File.Copy(ScenePath,dest+"/Scene-saved.unity");File.AppendAllText(Dir+"/checkpoints.txt",dest+"\n");}
        static void CopyTree(string a,string b){Directory.CreateDirectory(b);foreach(var p in Directory.GetFiles(a))File.Copy(p,Path.Combine(b,Path.GetFileName(p)));foreach(var p in Directory.GetDirectories(a))CopyTree(p,Path.Combine(b,Path.GetFileName(p)));}
        static string PathOf(Transform t)=>t.parent==null?t.name:PathOf(t.parent)+"/"+t.name;
        static Bounds BoundsOf(GameObject g){var rs=g.GetComponentsInChildren<Renderer>().Where(x=>x is MeshRenderer).ToArray();Check(rs.Length>0,"Mesh bounds "+g.name);var b=rs[0].bounds;foreach(var r in rs.Skip(1))b.Encapsulate(r.bounds);return b;}
        static void Inspect(string phase){
            Directory.CreateDirectory(Gallery+"/"+phase);Physics.SyncTransforms();var lines=new List<string>();
            foreach(var t in All<Transform>().Where(x=>x.gameObject.activeInHierarchy))lines.Add(PathOf(t)+" p="+t.position+" rot="+t.eulerAngles+" scale="+t.lossyScale+(t.TryGetComponent<MeshRenderer>(out var r)?" bounds="+r.bounds+" GI="+r.receiveGI+" map="+r.lightmapIndex+" flags="+GameObjectUtility.GetStaticEditorFlags(t.gameObject)+" mats="+string.Join(",",r.sharedMaterials.Select(m=>m?.name)):""));
            File.WriteAllLines(Dir+"/"+phase+"-scene.txt",lines);
            var chairs=All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.name.ToLowerInvariant().Contains("chair")&&x.GetComponent<MeshRenderer>()!=null).ToArray();
            File.WriteAllLines(Dir+"/"+phase+"-chair-mesh.txt",chairs.Select(t=>{var mesh=t.GetComponent<MeshFilter>().sharedMesh;var v=mesh.vertices;var hi=v.Where(p=>p.y>mesh.bounds.min.y+mesh.bounds.size.y*.74f).ToArray();var b=mesh.bounds;return PathOf(t)+" mesh="+AssetDatabase.GetAssetPath(mesh)+" localBounds="+b+" highCentroid="+(hi.Length>0?hi.Aggregate(Vector3.zero,(s,p)=>s+p)/hi.Length:Vector3.zero)+" worldForward="+t.forward;}));
            var activeMeshes=All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy).ToArray();long triangles=0;foreach(var f in All<MeshFilter>().Where(f=>f.gameObject.activeInHierarchy&&f.sharedMesh!=null&&f.TryGetComponent<Renderer>(out var r)&&r.enabled))for(int j=0;j<f.sharedMesh.subMeshCount;j++)triangles+=(long)f.sharedMesh.GetIndexCount(j)/3;
            long allTriangles=0;foreach(var f in All<MeshFilter>().Where(f=>f.gameObject.activeInHierarchy&&f.sharedMesh!=null))for(int j=0;j<f.sharedMesh.subMeshCount;j++)allTriangles+=(long)f.sharedMesh.GetIndexCount(j)/3;
            File.WriteAllText(Dir+"/"+phase+"-geometry.txt","All active MeshFilters (same method as expansion baseline)="+allTriangles+"; enabled Renderer triangles="+triangles+"; these are inventory counts, not rendered frame GPU counts.");
            File.WriteAllText(Dir+"/"+phase+"-metrics.json","{\"meshRenderers\":"+activeMeshes.Length+",\"meshMaterials\":"+activeMeshes.SelectMany(r=>r.sharedMaterials).Distinct().Count()+",\"triangles\":"+triangles+",\"bakedLights\":"+All<Light>().Count(l=>l.isActiveAndEnabled&&l.lightmapBakeType==LightmapBakeType.Baked)+",\"realtimeLights\":"+All<Light>().Count(l=>l.isActiveAndEnabled&&l.lightmapBakeType!=LightmapBakeType.Baked)+",\"lightmaps\":"+LightmapSettings.lightmaps.Length+",\"probes\":"+LightmapSettings.lightProbes?.count+",\"reflectionProbes\":"+All<ReflectionProbe>().Count(p=>p.gameObject.activeInHierarchy)+"}");
            File.WriteAllLines(Dir+"/"+phase+"-quality.txt",new[]{"Platform="+EditorUserBuildSettings.activeBuildTarget+" quality="+QualitySettings.names[QualitySettings.GetQualityLevel()]+" Aniso="+QualitySettings.anisotropicFiltering,"Active RP="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),"Graphics default="+AssetDatabase.GetAssetPath(GraphicsSettings.defaultRenderPipeline),"Lightmaps="+LightmapSettings.lightmaps.Length+" probes="+LightmapSettings.lightProbes?.count}.Concat(All<Light>().Where(l=>l.isActiveAndEnabled).Select(l=>l.name+" "+l.lightmapBakeType+" "+l.intensity)));
            File.WriteAllLines(Dir+"/"+phase+"-reflection.txt",All<ReflectionProbe>().Where(p=>p.gameObject.activeInHierarchy).Select(p=>p.name+" mode="+p.mode+" resolution="+p.resolution+" bakedTexture="+AssetDatabase.GetAssetPath(p.bakedTexture)));
            Check(All<MonoBehaviour>().All(x=>x!=null),"No missing scripts");BankNavigationValidation.Check(All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy));
        }
        static void Views(string phase){
            Capture(phase+"/01-general.png",new Vector3(8.8f,2.2f,-6.9f),new Vector3(0,1.3f,3.6f));
            Capture(phase+"/02-cubiculos.png",new Vector3(-3.6f,1.65f,-3.1f),new Vector3(-6.6f,1.05f,0));
            Capture(phase+"/03-silla-jugador.png",new Vector3(-2.5f,1.6f,-1.2f),new Vector3(0,.8f,.6f));
            Capture(phase+"/04-documentacion.png",new Vector3(-1.5f,1.65f,1),new Vector3(-4.2f,1.2f,4.3f));
            Capture(phase+"/05-supervision-cierre.png",new Vector3(2.7f,1.6f,4.3f),new Vector3(7.4f,1.4f,8.2f));
            Capture(phase+"/06-puesto.png",new Vector3(-1.8f,1.8f,-1.1f),new Vector3(.25f,1.2f,1.7f));
            Capture(phase+"/07-materiales-luz.png",new Vector3(-8.8f,1.6f,-2.4f),new Vector3(-6.6f,1.05f,-.8f));
            Capture(phase+"/08-nocturna.png",new Vector3(0,1.75f,-7.1f),new Vector3(0,1.5f,2.4f));
            Capture(phase+"/09-planta.png",new Vector3(0,21,1),new Vector3(0,0,1),11);
        }
        static void Capture(string file,Vector3 eye,Vector3 look,float ortho=0){
            var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var reticle=gaze!=null?gaze.Reticle:null;bool originalReticleActive=reticle!=null&&reticle.gameObject.activeSelf;bool hideObserverReticle=reticle!=null&&gaze.View!=null&&Vector3.Distance(eye,gaze.View.transform.position)>.25f;
            var path=Gallery+"/"+file;Directory.CreateDirectory(Path.GetDirectoryName(path));Canvas.ForceUpdateCanvases();var ceiling=All<Renderer>().Where(x=>x.name=="Ceiling"||x.name=="Modular ceiling grid").ToArray();var off=ceiling.Select(x=>x.forceRenderingOff).ToArray();var active=RenderTexture.active;var g=new GameObject("Temporary polish review camera"){hideFlags=HideFlags.HideAndDontSave};RenderTexture rt=null;Texture2D tex=null;
            try{if(hideObserverReticle)reticle.gameObject.SetActive(false);var c=g.AddComponent<Camera>();c.enabled=false;c.stereoTargetEye=StereoTargetEyeMask.None;c.nearClipPlane=.05f;c.farClipPlane=60;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(.08f,.1f,.12f);c.aspect=1600f/900;c.fieldOfView=60;c.orthographic=ortho>0;c.orthographicSize=ortho;c.transform.position=eye;var d=look-eye;c.transform.rotation=Quaternion.LookRotation(d,Mathf.Abs(Vector3.Dot(d.normalized,Vector3.up))>.99f?Vector3.forward:Vector3.up);var data=c.GetUniversalAdditionalCameraData();data.renderPostProcessing=false;data.renderShadows=true;rt=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32);Check(rt.Create(),"Render target allocated");if(ortho>0)foreach(var r in ceiling)r.forceRenderingOff=true;var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};Check(RenderPipeline.SupportsRenderRequest(c,request),"URP render request supported");RenderPipeline.SubmitRenderRequest(c,request);RenderTexture.active=rt;tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());File.AppendAllText(Gallery+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+file+" eye="+eye+" look="+look+" fov=60 ortho="+ortho+" Play="+EditorApplication.isPlaying+" NOT_ANDROID\n");}
            finally{if(hideObserverReticle&&reticle!=null)reticle.gameObject.SetActive(originalReticleActive);for(int i=0;i<ceiling.Length;i++)ceiling[i].forceRenderingOff=off[i];RenderTexture.active=active;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}UnityEngine.Object.DestroyImmediate(g);}
        }
        static Material wood,metal,fabric,polymer,paper,accent,stone;
        static Transform Root=>GameObject.Find("VisualPolishV1").transform;
        static Transform Group(Transform parent,string name){var g=new GameObject(name);Undo.RegisterCreatedObjectUndo(g,"Visual polish");g.transform.SetParent(parent,false);return g.transform;}
        static Material Mat(string name,Color color,float smooth=.2f,float metallic=0,bool unlit=false){
            var path=Out+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit")){name=name};AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",color);if(!unlit){m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",metallic);}return m;
        }
        static void Materials(){Directory.CreateDirectory(Out+"/Materials");Directory.CreateDirectory(Out+"/Geometry");Directory.CreateDirectory(Out+"/Textures");AssetDatabase.Refresh();wood=Mat("Oak_Satin",new Color(.60f,.40f,.23f),.24f);metal=Mat("Steel_Powdercoat",new Color(.27f,.32f,.36f),.32f,.2f);fabric=Mat("Acoustic_Woven",new Color(.32f,.40f,.43f),.04f);polymer=Mat("Equipment_Matte",new Color(.065f,.09f,.12f),.2f);paper=Mat("Paper_Warm",new Color(.88f,.86f,.79f),.05f);accent=Mat("Brushed_Champagne",new Color(.58f,.47f,.29f),.35f,.45f);stone=Mat("Stone_Planter",new Color(.52f,.54f,.51f),.08f);}
        static void SetMaterials(GameObject g,params Material[] mats){foreach(var r in g.GetComponentsInChildren<MeshRenderer>()){Undo.RecordObject(r,"Scene material overrides");r.sharedMaterials=r.sharedMaterials.Select((m,i)=>mats[Mathf.Min(i,mats.Length-1)]).ToArray();PrefabUtility.RecordPrefabInstancePropertyModifications(r);}}
        static void Pose(Transform t,Vector3 point,float yaw){Undo.RecordObject(t,"Visual polish pose");t.SetPositionAndRotation(point,Quaternion.Euler(0,yaw,0));PrefabUtility.RecordPrefabInstancePropertyModifications(t);}
        static void Ground(Transform t,float y=0){var b=BoundsOf(t.gameObject);Undo.RecordObject(t,"Support contact");t.position+=Vector3.up*(y-b.min.y);}
        static GameObject Box(Transform parent,string name,Vector3 p,Vector3 size,Material mat,bool collider=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);Undo.RegisterCreatedObjectUndo(g,"Visual polish detail");g.name=name;g.transform.SetParent(parent,true);g.transform.position=p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.ContributeGI|StaticEditorFlags.BatchingStatic);if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
        static GameObject Native(string file,string name,Transform parent,Vector3 p,float height,float yaw=0){
            var root=Group(parent,name);var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/KenneyFurniture/Models/FBX format/"+file+".fbx");Check(source!=null,"Verified Kenney source "+file);var child=(GameObject)PrefabUtility.InstantiatePrefab(source);child.transform.SetParent(root,false);var b=BoundsOf(child);child.transform.localScale*=height/b.size.y;b=BoundsOf(child);child.transform.position-=new Vector3(b.center.x,b.min.y,b.center.z);Pose(root,p,yaw);SetMaterials(root.gameObject,wood,metal);return root.gameObject;
        }
        static GameObject Reuse(string name,string instance,Transform parent,Vector3 p,float yaw=0){var src=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/Prefabs/"+name+".prefab");Check(src!=null,"Reused verified prefab "+name);var g=(GameObject)PrefabUtility.InstantiatePrefab(src);Undo.RegisterCreatedObjectUndo(g,"Visual polish furniture");g.name=instance;g.transform.SetParent(parent,false);Pose(g.transform,p,yaw);Ground(g.transform,p.y);return g;}
        static void Placement(){
            Check(GameObject.Find("VisualPolishV1")==null,"No duplicate polish stage");Checkpoint("BeforePlacement");Materials();Group(null,"VisualPolishV1");
            // Kenney chairDesk's upper back vertices lie at local +Z: seat front is -Z.
            // Rotate ONLY the player's visual model; sit/stand references and chair origin stay unchanged.
            var main=GameObject.Find("Analyst chair").transform;var visual=main.Find("chairDesk");var center=BoundsOf(visual.gameObject).center;var floor=BoundsOf(visual.gameObject).min.y;
            Undo.RecordObject(visual,"Measured chair front axis");visual.RotateAround(new Vector3(center.x,floor,center.z),Vector3.up,180);Ground(visual);
            var booths=GameObject.Find("02_SecondaryCubicles").transform;
            foreach(Transform booth in booths){if(!booth.name.StartsWith("Cubicle "))continue;var chair=booth.Find("Office chair");if(chair==null)continue;var desk=booth.Find("Shared desk");var aim=BoundsOf(desk.gameObject).center-BoundsOf(chair.gameObject).center;aim.y=0;var before=BoundsOf(chair.gameObject);Undo.RecordObject(chair,"Orient seat toward own desk");chair.rotation=Quaternion.LookRotation(-aim.normalized);var after=BoundsOf(chair.gameObject);chair.position+=new Vector3(before.center.x-after.center.x,-after.min.y,before.center.z-after.center.z);
                foreach(string n in new[]{"Keyboard","Mouse","Secondary workstation display","Desk lamp - no dynamic light","Ordered papers"}){var obj=booth.Find(n);if(obj==null)continue;if(n=="Keyboard"||n=="Secondary workstation display")Pose(obj,obj.position,0);Ground(obj,BoundsOf(desk.gameObject).max.y);}
            }
            foreach(string n in new[]{"Interactive Archive 1","Interactive Archive 2"}){var t=GameObject.Find(n).transform;var b=BoundsOf(t.gameObject);Pose(t,t.position,270);var c=BoundsOf(t.gameObject);t.position+=new Vector3(b.center.x-c.center.x,-c.min.y,b.center.z-c.center.z);}
            foreach(var pair in new[]{("Plant west",new Vector3(-8.65f,0,-6.5f)),("Plant east",new Vector3(9.15f,0,-5.95f)),("Rear corner plant",new Vector3(9.1f,0,9.35f))}){var t=GameObject.Find(pair.Item1).transform;Pose(t,pair.Item2,0);Ground(t);}
            Pose(GameObject.Find("Visitor seating").transform,new Vector3(8.3f,0,-6.2f),90);Ground(GameObject.Find("Visitor seating").transform);Pose(GameObject.Find("Visitor table").transform,new Vector3(7.5f,0,-5.6f),0);Ground(GameObject.Find("Visitor table").transform);Pose(GameObject.Find("Reference books").transform,new Vector3(7.5f,.68f,-5.6f),0);Ground(GameObject.Find("Reference books").transform,.68f);
            // A real work surface and monitor justify the existing End action.
            var supervisor=Group(Root,"Supervision workstation");var deskModel=Native("desk","Supervisor desk",supervisor,new Vector3(6.65f,0,9.0f),.78f);var deskBounds=BoundsOf(deskModel);Check(deskBounds.size.x<3.2f&&deskBounds.size.z<1.6f,"Supervisor desk proportions fit room");
            var mon=Native("computerScreen","Supervisor terminal housing",supervisor,new Vector3(6.55f,.78f,8.84f),.73f);SetMaterials(mon,polymer,metal);
            var end=All<BankInteractable>().Single(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.End);Undo.SetTransformParent(end.transform,supervisor,"Existing end action integrated");Pose(end.transform,new Vector3(6.55f,1.19f,8.64f),0);Undo.RecordObject(end.transform,"Terminal target size");end.transform.localScale=Vector3.one;Undo.RecordObject(end.GetComponent<BoxCollider>(),"Terminal selection surface");end.GetComponent<BoxCollider>().size=new Vector3(1.02f,.64f,.12f);Undo.RecordObject(end.GetComponent<MeshRenderer>(),"Old arbitrary cube retained but hidden");end.GetComponent<MeshRenderer>().enabled=false;
            var face=Box(supervisor,"Closure terminal screen",new Vector3(6.55f,1.19f,8.56f),new Vector3(1.09f,.70f,.025f),polymer);
            var marker=Box(supervisor,"Closure amber status edge",new Vector3(6.55f,.90f,8.54f),new Vector3(.94f,.018f,.012f),accent);var focus=marker.AddComponent<ArchitecturalFocusVisual>();focus.Target=end;focus.Surface=marker.GetComponent<Renderer>();focus.Idle=new Color(.58f,.47f,.29f);
            var label=GameObject.Find("End label").transform;Pose(label,new Vector3(6.55f,1.20f,8.535f),0);Undo.RecordObject(label,"Readable closure terminal label");label.localScale=Vector3.one*.002f;var rt=label.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(490,270);foreach(var image in label.GetComponentsInChildren<UnityEngine.UI.Image>())image.color=new Color(.045f,.075f,.10f,1);var tmp=label.GetComponentInChildren<TextMeshProUGUI>();tmp.rectTransform.sizeDelta=new Vector2(460,245);tmp.fontSize=30;tmp.text="SUPERVISION\nCIERRE DE OPERACIONES\n\nCERRAR TURNO\n\nResuelve todos los expedientes";
            Undo.SetTransformParent(label,supervisor,"Closure label belongs to terminal");
            var key=Reuse("Kenney_Keyboard","Supervisor keyboard",supervisor,new Vector3(6.5f,.78f,8.45f));SetMaterials(key,polymer,metal);
            Pose(GameObject.Find("Supervisor reference table").transform,new Vector3(8.7f,0,8.1f),0);Ground(GameObject.Find("Supervisor reference table").transform);Pose(GameObject.Find("Supervisor books").transform,new Vector3(8.7f,.68f,8.1f),0);Ground(GameObject.Find("Supervisor books").transform,.68f);
            var shelf=Native("bookcaseOpen","Document reference shelf",Root,new Vector3(-4.2f,0,6.75f),1.7f,270);SetMaterials(shelf,metal,wood);
            // The large legible monitor needs a plausible keyboard/clipboard ledge below it.
            Box(Root,"Analyst keyboard worktop",new Vector3(0,.775f,1.52f),new Vector3(2.60f,.05f,.70f),wood);
            var kb=GameObject.Find("Keyboard1").transform;Undo.RecordObject(kb,"Keyboard visible in front of monitor");kb.position+=new Vector3(.26f,0,-.54f);Ground(kb,.8f);
            var mouse=GameObject.Find("Comp_Mouse").transform;Undo.RecordObject(mouse,"Mouse next to keyboard");mouse.position+=new Vector3(.13f,0,-.4f);Ground(mouse,.8f);
            var clipboard=GameObject.Find("Analyst review clipboard").transform;Undo.RecordObject(clipboard,"Clipboard on accessible worktop");clipboard.position=new Vector3(-1.02f,clipboard.position.y,1.45f);Ground(clipboard,.8f);
            var lamp=GameObject.Find("Analyst task lamp").transform;Pose(lamp,new Vector3(-1.08f,.8f,2.75f),180);Ground(lamp,.8f);
            Save();Inspect("Placement");Views("Placement");
        }
        static Texture2D Pattern(string name,int kind){
            var path=Out+"/Textures/"+name+".png";const int n=256;var tex=new Texture2D(n,n,TextureFormat.RGB24,true);var pixels=new Color[n*n];
            for(int y=0;y<n;y++)for(int x=0;x<n;x++){float noise=Mathf.PerlinNoise(x*.17f,y*.21f)-.5f;float v=kind==0?.92f+.025f*Mathf.Sin(x*.18f+3*Mathf.Sin(y*.025f))+.035f*noise:kind==1?.94f+.0125f*((x%4<2?1:-1)+(y%4<2?1:-1))+.02f*noise:.91f+.035f*noise+.025f*((x/64+y/64)%2);pixels[y*n+x]=new Color(v,v,v);}
            tex.SetPixels(pixels);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=true;importer.wrapMode=TextureWrapMode.Repeat;importer.filterMode=FilterMode.Trilinear;importer.anisoLevel=2;importer.maxTextureSize=256;importer.textureCompression=TextureImporterCompression.Compressed;importer.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        static void JoinedBoxes(string name,Material mat,IEnumerable<(Vector3 p,Vector3 size)> parts,bool gi=true){
            var primitive=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=primitive.GetComponent<MeshFilter>().sharedMesh;var combines=parts.Select(p=>new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p.p,Quaternion.identity,p.size)}).ToArray();var mesh=new Mesh{name=name};mesh.CombineMeshes(combines,true,true);UnityEngine.Object.DestroyImmediate(primitive);if(gi)Unwrapping.GenerateSecondaryUVSet(mesh);AssetDatabase.CreateAsset(mesh,Out+"/Geometry/"+name.Replace(' ','_')+".asset");var go=Group(Root,name).gameObject;go.AddComponent<MeshFilter>().sharedMesh=mesh;var r=go.AddComponent<MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=gi?ShadowCastingMode.On:ShadowCastingMode.Off;GameObjectUtility.SetStaticEditorFlags(go,StaticEditorFlags.BatchingStatic|(gi?StaticEditorFlags.ContributeGI:0));
        }
        static void TeleportPresentation(){
            var ringMat=Mat("Navigation_Inlay",new Color(.45f,.54f,.57f),unlit:true);var vertices=new List<Vector3>();var triangles=new List<int>();
            for(int i=0;i<40;i++){float a=2*Mathf.PI*i/40,b=2*Mathf.PI*(i+1)/40;int n=vertices.Count;vertices.Add(new Vector3(Mathf.Cos(a)*.31f,0,Mathf.Sin(a)*.31f));vertices.Add(new Vector3(Mathf.Cos(a)*.285f,0,Mathf.Sin(a)*.285f));vertices.Add(new Vector3(Mathf.Cos(b)*.285f,0,Mathf.Sin(b)*.285f));vertices.Add(new Vector3(Mathf.Cos(b)*.31f,0,Mathf.Sin(b)*.31f));triangles.AddRange(new[]{n,n+1,n+2,n,n+2,n+3,n+2,n+1,n,n+3,n+2,n});}
            var mesh=new Mesh{name="Navigation inlay ring"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Out+"/Geometry/Navigation_Ring.asset");
            foreach(var pad in All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.Teleport)){
                var original=pad.GetComponent<Renderer>();Undo.RecordObject(original,"Retain collider, replace green block visual");original.enabled=false;
                var g=Group(Root,pad.name+" floor inlay").gameObject;g.transform.position=pad.transform.position+Vector3.up*.031f;g.AddComponent<MeshFilter>().sharedMesh=mesh;var r=g.AddComponent<MeshRenderer>();r.sharedMaterial=ringMat;r.shadowCastingMode=ShadowCastingMode.Off;var focus=g.AddComponent<ArchitecturalFocusVisual>();focus.Target=pad;focus.Surface=r;focus.HideWhenColliderDisabled=true;
                string label=pad.name.Contains("Inspection")?"PUESTO":pad.name.Contains("Evidence")?"REGISTRO":pad.name.Contains("Security")?"SEGURIDAD":pad.name.Contains("West")?"ANALISTAS O":pad.name.Contains("East")?"ANALISTAS E":pad.name.Contains("Reception")?"RECEPCION":pad.name.Contains("Gallery")?"GALERIA":"ENTRADA";
                FloorText(pad.name+" route",pad.transform.position+new Vector3(0,.035f,-.43f),label,new Vector2(.72f,.16f));
            }
            foreach(var t in All<Transform>().Where(t=>t.gameObject.activeInHierarchy&&(t.name=="Go to analyst station"||t.name=="Cubicle route"||t.name=="Teleport Reception label"||t.name=="Teleport Rear Gallery label"))){Undo.RecordObject(t.gameObject,"Retain previous floor plaques");t.gameObject.SetActive(false);}
        }
        static void FloorText(string name,Vector3 p,string value,Vector2 physical){
            var go=new GameObject(name,typeof(RectTransform),typeof(Canvas));Undo.RegisterCreatedObjectUndo(go,"Floor wayfinding");var canvas=go.transform;canvas.SetParent(Root,false);go.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;canvas.position=p;canvas.rotation=Quaternion.Euler(90,0,0);canvas.localScale=Vector3.one*.001f;var ui=All<UIManager>().Single(x=>x.gameObject.activeInHierarchy);var tmp=ui.Text(canvas,"Direction",Vector2.zero,physical*1000,42);tmp.text=value;tmp.alignment=TextAlignmentOptions.Center;tmp.color=new Color(.60f,.66f,.67f);
        }
        static void Plaque(string name,string text){foreach(var t in All<Transform>().Where(x=>x.name==name&&x.gameObject.activeInHierarchy)){foreach(var im in t.GetComponentsInChildren<UnityEngine.UI.Image>()){Undo.RecordObject(im,"Institutional plaque finish");im.color=new Color(.68f,.72f,.71f,1);}foreach(var tmp in t.GetComponentsInChildren<TextMeshProUGUI>()){Undo.RecordObject(tmp,"Concise institutional signage");tmp.color=new Color(.035f,.075f,.11f);if(text!=null)tmp.text=text;}}}
        static void Surfaces(){
            Checkpoint("BeforeSurfaces");Materials();Check(GameObject.Find("Polished surfaces checkpoint")==null,"No duplicate surface stage");Group(Root,"Polished surfaces checkpoint");
            wood.SetTexture("_BaseMap",Pattern("Oak_Grain_256",0));fabric.SetTexture("_BaseMap",Pattern("Acoustic_Weave_256",1));var carpet=Mat("Carpet_BlueGrey",new Color(.30f,.36f,.42f),.02f);carpet.SetTexture("_BaseMap",Pattern("Carpet_Tile_256",2));carpet.SetTextureScale("_BaseMap",new Vector2(20,18));
            var ceiling=Mat("Ceiling_Acoustic",new Color(.65f,.65f,.61f),.02f);var walls=Mat("Wall_Mineral",new Color(.63f,.67f,.69f),.1f);var chair=Mat("Seat_Upholstery",new Color(.17f,.24f,.28f),.08f);chair.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Out+"/Textures/Acoustic_Weave_256.png"));var leaves=Mat("Foliage_Olive",new Color(.27f,.38f,.17f),.05f);var soil=Mat("Potting_Soil",new Color(.12f,.085f,.055f),.02f);
            var inset=Mat("Oak_DrawerInset",new Color(.38f,.27f,.18f),.18f);inset.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Out+"/Textures/Oak_Grain_256.png"));
            foreach(var r in All<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy&&x.enabled)){
                var path=PathOf(r.transform);Undo.RecordObject(r,"Recognizable surface families");
                r.sharedMaterials=r.sharedMaterials.Select(m=>{if(m==null)return m;var n=m.name;
                    if(r.name=="Floor")return carpet;if(r.name=="Ceiling")return ceiling;
                    if(r.transform.parent?.name=="01_Architecture"&&(r.name.Contains("Wall")||r.name.Contains("infill")||r.name=="EntranceLintel"))return walls;
                    if(path.Contains("Shared desk")||path.Contains("Main analyst desk"))return n.Contains("291A0E")||n.Contains("Inset")?inset:wood;
                    if(r.name=="plant")return leaves;if(r.name=="pottedPlant")return n.Contains("AD744")?soil:stone;
                    if(path.Contains("Office chair")||path.Contains("Analyst chair/chairDesk"))return n.Contains("Fabric")||n.Contains("2ED")?chair:metal;
                    if(r.name=="Fabric acoustic back"||r.name=="Acoustic side"||r.name=="Supervisor side divider"||r.name=="Supervisor front partition")return fabric;
                    if(n=="Oak_Worksurface")return wood;if(n=="Painted_Metal"||n.Contains("Luminaire_Slate"))return metal;if(n=="Equipment_Polymer")return polymer;
                    if(path.Contains("Interactive Archive")||path.Contains("Supervisor files"))return n.Contains("BDD2")?accent:metal;
                    return m;}).ToArray();
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            foreach(var t in All<Transform>().Where(t=>t.gameObject.activeInHierarchy&&(t.name=="Ceiling cross beam"||t.name=="Supervision canopy"))){Undo.RecordObject(t.GetComponent<Renderer>(),"Lighter finished ceiling structure");t.GetComponent<Renderer>().sharedMaterial=metal;}
            var trim=new List<(Vector3,Vector3)>();foreach(float x in new[]{-9.88f,9.88f}){trim.Add((new Vector3(x,.07f,1),new Vector3(.045f,.14f,17.85f)));trim.Add((new Vector3(x,3.58f,1),new Vector3(.06f,.07f,17.85f)));}trim.Add((new Vector3(0,.07f,9.88f),new Vector3(19.8f,.14f,.045f)));foreach(float x in new[]{-5.55f,5.55f})trim.Add((new Vector3(x,.07f,-7.88f),new Vector3(8.65f,.14f,.045f)));JoinedBoxes("Architectural skirting and cornice",metal,trim);
            var grid=new List<(Vector3,Vector3)>();for(int i=1;i<10;i++)grid.Add((new Vector3(-10+i*2,3.703f,1),new Vector3(.018f,.022f,17.8f)));for(int i=1;i<9;i++)grid.Add((new Vector3(0,3.703f,-8+i*2),new Vector3(19.8f,.022f,.018f)));JoinedBoxes("Modular ceiling grid",metal,grid);
            var edge=new List<(Vector3,Vector3)>();foreach(var t in All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&(x.name=="Fabric acoustic back"||x.name=="Acoustic side"))){var b=t.GetComponent<Renderer>().bounds;if(b.size.x>b.size.z)foreach(float x in new[]{b.min.x,b.max.x})edge.Add((new Vector3(x,b.center.y,b.center.z),new Vector3(.028f,b.size.y,.08f)));else foreach(float z in new[]{b.min.z,b.max.z})edge.Add((new Vector3(b.center.x,b.center.y,z),new Vector3(.08f,b.size.y,.028f)));}JoinedBoxes("Cubicle edge channels",metal,edge);
            // Opaque city panes, restrained lit windows; one shared mesh/material, no realtime reflection.
            var city=new List<(Vector3,Vector3)>();for(int side=-1;side<=1;side+=2)for(int bay=0;bay<3;bay++)for(int building=0;building<5;building++)for(int row=0;row<3;row++)if((bay+building+row)%3!=0){float z=-4.2f+bay*5.2f-1.65f+building*.76f;city.Add((new Vector3(side*9.805f,1.45f+row*.15f,z),new Vector3(.008f,.055f,.07f)));}JoinedBoxes("Distant city office windows",Mat("City_WarmWindows",new Color(.50f,.43f,.29f),unlit:true),city,false);
            TeleportPresentation();
            foreach(var n in new[]{"Cubicle identifier","Evidence zone label","Bank title","Supervision identity","Welcome","Model credits"})Plaque(n,null);
            Plaque("Evidence zone label","DOCUMENTACION\nREGISTRO Y EVIDENCIAS");Plaque("Supervision identity","SUPERVISION\nCIERRE DE OPERACIONES");Plaque("Welcome","SWIFT / OPERACIONES INTERNACIONALES\n2016 - SIMULACION EDUCATIVA\nElige modo. Mira los aros para trasladarte.");
            var supervisor=GameObject.Find("Supervision workstation").transform;Undo.RecordObject(supervisor,"Terminal visible through supervision entrance");supervisor.position+=Vector3.left*.55f;
            var supervisorChair=Reuse("Kenney_OfficeChair","Supervisor chair",supervisor,new Vector3(6.0f,0,8.0f),180);foreach(var r in supervisorChair.GetComponentsInChildren<MeshRenderer>()){r.sharedMaterials=r.sharedMaterials.Select((m,i)=>r.name=="chair"&&i==0?chair:metal).ToArray();}
            // Populate the documentation shelf using already attributed objects at actual shelf heights.
            var book=Reuse("Kenney_Books","Procedure reference volumes",Root,new Vector3(-4.1f,.77f,6.75f),270);SetMaterials(book,inset,paper,metal);
            // GI UVs are derived only when absent, keeping all downloaded source/importers intact.
            int index=0;foreach(var r in Root.GetComponentsInChildren<MeshRenderer>().Where(r=>r.enabled&&!r.sharedMaterials.Any(m=>m.shader.name.EndsWith("/Unlit")))){
                if(r.GetComponent<ArchitecturalFocusVisual>()!=null)continue;var f=r.GetComponent<MeshFilter>();if(f==null)continue;if(f.sharedMesh.uv2.Length==0){var copy=UnityEngine.Object.Instantiate(f.sharedMesh);copy.name=f.sharedMesh.name+" GI copy";Unwrapping.GenerateSecondaryUVSet(copy);AssetDatabase.CreateAsset(copy,Out+"/Geometry/DerivedGI_"+(index++)+".asset");Undo.RecordObject(f,"Derived UV2 mesh without changing imported source");f.sharedMesh=copy;}
                GameObjectUtility.SetStaticEditorFlags(r.gameObject,GameObjectUtility.GetStaticEditorFlags(r.gameObject)|StaticEditorFlags.ContributeGI|StaticEditorFlags.BatchingStatic);
            }
            foreach(var lamp in All<Light>().Where(x=>x.gameObject.activeInHierarchy&&x.lightmapBakeType==LightmapBakeType.Baked)){Undo.RecordObject(lamp,"Warm work hierarchy");if(lamp.name=="Baked analyst task light"){lamp.transform.position=new Vector3(-.92f,1.25f,2.75f);lamp.transform.rotation=Quaternion.Euler(90,0,0);lamp.intensity=4;lamp.range=3;lamp.color=new Color(1,.75f,.47f);}}
            foreach(Transform booth in GameObject.Find("02_SecondaryCubicles").transform){var desk=booth.Find("Shared desk");if(desk==null)continue;var b=BoundsOf(desk.gameObject);BakedArea(booth.name+" warm work light",new Vector3(b.center.x,2.8f,b.center.z),new Vector2(1.8f,.9f),2.2f);}
            BakedArea("Supervision work light",new Vector3(6.55f,2.9f,8.6f),new Vector2(1.7f,.8f),3.0f);
            var probes=All<LightProbeGroup>().Single();Undo.RecordObject(probes,"Probe coverage at work surfaces");var points=probes.probePositions.ToList();foreach(var p in new[]{new Vector3(0,1.0f,1.3f),new Vector3(-6.6f,1.0f,-.8f),new Vector3(-6.8f,1.0f,4.5f),new Vector3(6.4f,1.0f,.4f),new Vector3(6.8f,1.0f,6.3f),new Vector3(6.55f,1.2f,8.3f)})points.Add(probes.transform.InverseTransformPoint(p));probes.probePositions=points.ToArray();
            Save();Inspect("Surfaces");Views("Surfaces");
        }
        static void Refine(){
            Checkpoint("BeforeRefinement");Materials();wood.SetTexture("_BaseMap",Pattern("Oak_Grain_256",0));fabric.SetTexture("_BaseMap",Pattern("Acoustic_Weave_256",1));
            var texture=new Texture2D(128,128,TextureFormat.RGB24,true);for(int y=0;y<128;y++)for(int x=0;x<128;x++){var c=new Color(.035f,.075f,.11f);if(y>103&&y<112&&x>9&&x<90)c=new Color(.30f,.39f,.43f);if(y>45&&y<96&&x>10&&x<82&&(y%11<3))c=new Color(.14f,.23f,.27f);if(x>92&&x<118&&y>45&&y<96)c=new Color(.10f,.19f,.23f);texture.SetPixel(x,y,c);}texture.Apply();string path=Out+"/Textures/Auxiliary_Workstation_128.png";File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.mipmapEnabled=true;importer.filterMode=FilterMode.Trilinear;importer.maxTextureSize=128;importer.textureCompression=TextureImporterCompression.Compressed;importer.SaveAndReimport();var screen=Mat("Auxiliary_Screen",Color.white,unlit:true);screen.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            int i=0;foreach(var display in All<Transform>().Where(x=>x.name=="Secondary workstation display"&&x.gameObject.activeInHierarchy))foreach(var r in display.GetComponentsInChildren<MeshRenderer>()){
                var filter=r.GetComponent<MeshFilter>();var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);mesh.name="Auxiliary screen UV copy";var v=mesh.vertices;var b=mesh.bounds;mesh.uv=v.Select(p=>new Vector2((p.x-b.min.x)/b.size.x,(p.y-b.min.y)/b.size.y)).ToArray();AssetDatabase.CreateAsset(mesh,Out+"/Geometry/AuxiliaryScreenUV_"+(i++)+".asset");Undo.RecordObject(filter,"Screen presentation UV only");filter.sharedMesh=mesh;Undo.RecordObject(r,"Decorative display active, no banking data");r.sharedMaterials=new[]{polymer,screen};
            }
            if(All<ReflectionProbe>().All(p=>!p.gameObject.activeInHierarchy)){
                var g=Group(Root,"Baked room reflection - 128");g.position=new Vector3(0,1.6f,1);var probe=g.gameObject.AddComponent<ReflectionProbe>();probe.mode=ReflectionProbeMode.Baked;probe.resolution=128;probe.size=new Vector3(20,3.8f,18);probe.center=new Vector3(0,.3f,0);probe.boxProjection=true;probe.intensity=.65f;probe.clearFlags=ReflectionProbeClearFlags.SolidColor;probe.backgroundColor=new Color(.08f,.12f,.17f);
            }
            var shelf=GameObject.Find("Document reference shelf");var levels=new List<float>();foreach(var mf in shelf.GetComponentsInChildren<MeshFilter>()){var v=mf.sharedMesh.vertices;var tri=mf.sharedMesh.triangles;for(int t=0;t<tri.Length;t+=3){var a=mf.transform.TransformPoint(v[tri[t]]);var b=mf.transform.TransformPoint(v[tri[t+1]]);var c=mf.transform.TransformPoint(v[tri[t+2]]);var cross=Vector3.Cross(b-a,c-a);if(cross.magnitude>.002f&&Vector3.Dot(cross.normalized,Vector3.up)>.99f&&a.y>.4f&&a.y<1.35f)levels.Add(a.y);}}Check(levels.Count>0,"Actual bookshelf support surfaces found");float level=levels.OrderBy(y=>Mathf.Abs(y-.77f)).First();Ground(GameObject.Find("Procedure reference volumes").transform,level);File.WriteAllText(Dir+"/shelf-contact.txt","Book support from actual upward shelf triangles: y="+level);
            foreach(string name in new[]{"Supervisor desk","Document reference shelf"}){var go=GameObject.Find(name);var b=BoundsOf(go);if(go.GetComponent<Collider>()==null){var collider=go.AddComponent<BoxCollider>();collider.center=go.transform.InverseTransformPoint(b.center);collider.size=b.size;}}
            foreach(string name in new[]{"Fictional transfer documents","Stapler"})Ground(GameObject.Find(name).transform,.8f);
            Save();Inspect("Refinement");
        }
        static void BakedArea(string name,Vector3 p,Vector2 size,float power){var g=Group(Root,name);g.position=p;g.rotation=Quaternion.Euler(90,0,0);var l=g.gameObject.AddComponent<Light>();l.type=LightType.Rectangle;l.areaSize=size;l.lightmapBakeType=LightmapBakeType.Baked;l.intensity=power;l.color=new Color(1,.80f,.59f);l.shadows=LightShadows.Soft;}

        static bool Reaches(BankInteractable target,Vector3 origin){Physics.SyncTransforms();foreach(var col in target.GetComponentsInChildren<Collider>()){if(!col.enabled)continue;var d=col.bounds.center-origin;if(d.magnitude<=12&&Physics.Raycast(origin,d.normalized,out var hit,d.magnitude+.05f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<BankInteractable>()==target)return true;}return false;}
        static void ChairAudit(){
            var output=new List<string>{"chairDesk: back is local +Z from upper mesh centroid; seat front is -Z. Player visual is independent of sit/stand target origin."};
            foreach(var visual in All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.name=="chairDesk")){
                var parent=visual.parent;var main=parent.name=="Analyst chair";var desk=main?GameObject.Find("Main analyst desk").transform:parent.name=="Supervisor chair"?GameObject.Find("Supervisor desk").transform:parent.parent.Find("Shared desk");if(desk==null)continue;
                var b=BoundsOf(visual.gameObject);var table=BoundsOf(desk.gameObject);var d=table.center-b.center;d.y=0;float dot=Vector3.Dot(-visual.forward,d.normalized);Check(dot>.95f,"Chair faces its desk: "+PathOf(parent));Check(Mathf.Abs(b.min.y)<.005f,"Chair wheels on floor: "+parent.name);Check(b.size.y>.9f&&b.size.y<1.25f,"Chair physical height plausible: "+parent.name);Check(!b.Intersects(new Bounds(table.center,new Vector3(table.size.x,table.size.y,.8f*table.size.z))),"Chair does not intersect desk: "+parent.name);output.Add(PathOf(parent)+" height="+b.size.y+" floor="+b.min.y+" facingDot="+dot+" chairBounds="+b+" deskBounds="+table);
            }File.WriteAllLines(Dir+"/chair-audit.txt",output);
        }
        static IEnumerator Play(){
            yield return null;yield return null;var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var eye=gaze.View;var rig=eye.transform.parent;var end=All<BankInteractable>().Single(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.End);ChairAudit();
            game.StartScenario(ScenarioMode.Attack);yield return null;var pads=All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.Teleport).ToArray();var origins=pads.Where(x=>Reaches(end,x.Destination+Vector3.up*1.6f)).ToArray();Check(origins.Length>0,"Closure terminal reachable from existing navigation");File.WriteAllLines(Dir+"/closure-reachable-from.txt",origins.Select(x=>x.name));
            rig.position=origins.OrderBy(x=>Vector3.Distance(x.Destination,end.transform.position)).First().Destination;eye.transform.rotation=Quaternion.LookRotation(end.GetComponent<Collider>().bounds.center-eye.transform.position);int activated=0;Action<BankInteractable> onActivation=t=>{if(t==end)activated++;};gaze.Activated+=onActivation;gaze.SimulateGazeInEditor=true;
            float gazeDeadline=Time.realtimeSinceStartup+5;while(activated==0&&Time.realtimeSinceStartup<gazeDeadline)yield return null;Check(activated==1&&!game.Session.Ended,"1.5-second closure gaze activates once and cannot close incomplete turn");Check(game.Message.Contains("Resuelve todos"),"Original incomplete-turn explanation retained");float holdUntil=Time.realtimeSinceStartup+.5f;while(Time.realtimeSinceStartup<holdUntil)yield return null;Check(activated==1,"No repeated closure per fixation");gaze.Activated-=onActivation;gaze.SimulateGazeInEditor=false;
            Capture("Play/01-cierre-por-mirada.png",eye.transform.position,end.transform.position);Capture("Play/02-supervision-terminal.png",new Vector3(5.45f,1.65f,7.75f),end.transform.position);
            rig.position=new Vector3(0,0,-.65f);eye.transform.localRotation=Quaternion.identity;game.StartScenario(ScenarioMode.Defense);yield return null;Capture("Play/03-puesto-de-pie.png",eye.transform.position,new Vector3(.3f,1.35f,1.6f));
            var seat=AnalystSeat.Current;Check(seat.Request(true),"Polished chair accepts sit");while(seat.IsTransitioning)yield return null;Check(seat.IsSeated,"Polished chair reaches seated state");
            foreach(var action in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return,BankAction.FormatReference,BankAction.Notebook,BankAction.Stand})Check(All<BankInteractable>().Any(t=>t.gameObject.activeInHierarchy&&t.Action==action&&Reaches(t,eye.transform.position)),"Seated polished raycast "+action);
            Check(eye.GetComponent<HeadPoseAdapter>().enabled,"Head pose provider preserved");Capture("Play/04-puesto-sentado.png",eye.transform.position,new Vector3(.3f,1.15f,1.7f));
            var notebook=All<BankInteractable>().Single(t=>t.Action==BankAction.Notebook);Capture("Play/05-libreta-teclado.png",eye.transform.position,notebook.transform.position+Vector3.right*.5f);
            Canvas.ForceUpdateCanvases();foreach(var tmp in ui.PanelAnchor.GetComponentsInChildren<TextMeshProUGUI>()){tmp.ForceMeshUpdate();Check(!tmp.isTextOverflowing,"Polished monitor TMP fits "+tmp.name);}foreach(var tmp in GameObject.Find("End label").GetComponentsInChildren<TextMeshProUGUI>()){tmp.ForceMeshUpdate();Check(!tmp.isTextOverflowing,"Closure terminal TMP fits");}
            var originalPipeline=QualitySettings.renderPipeline;var rows=new List<string>();
            try{foreach(string profile in new[]{"PC","Mobile"}){
                var rp=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/"+profile+"_RPAsset.asset");Check(rp!=null,"Existing comparison profile "+profile);QualitySettings.renderPipeline=rp;yield return null;yield return null;
                string counters;try{counters="wholeEditorDrawCalls="+UnityStats.drawCalls+" batches="+UnityStats.batches+" triangles="+UnityStats.triangles;}catch(Exception e){counters="Editor counters unavailable: "+e.GetType().Name;}
                rows.Add("Profile="+profile+" path="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline)+" renderScale="+rp.renderScale+" MSAA="+rp.msaaSampleCount+" shadow="+rp.mainLightShadowmapResolution+" "+counters+" NOT_ANDROID; includes Editor rendering, not isolated GPU cost");Capture("Profiles/"+profile+"-same-camera.png",eye.transform.position,ui.PanelAnchor.position);
            }}finally{QualitySettings.renderPipeline=originalPipeline;}File.WriteAllLines(Dir+"/profile-comparison.txt",rows);
            Check(seat.Request(false),"Polished chair accepts stand");while(seat.IsTransitioning)yield return null;Check(!seat.IsSeated&&seat.FadeOpacity<.01f,"Stand/fade restored");
            Check(errors==0,"No runtime errors during polish review");
        }
    }
}


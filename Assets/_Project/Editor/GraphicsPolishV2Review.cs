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
    [InitializeOnLoad] public static class GraphicsPolishV2Review {
        const string Dir="Artifacts/GraphicsPolishV2", Queue=Dir+"/command.txt";
        const string Gallery="Docs/Previews/GraphicsPolishV2";
        const string ScenePath="Assets/_Project/Scenes/Review/BancoCorresponsal_GraphicsV2.unity", Out="Assets/_Project/OfficeVisual/GraphicsPolishV2";
        static string command;static IEnumerator routine;static bool ownsPlay;static int errors,lastFrame=-1;
        static GraphicsPolishV2Review(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static void Check(bool ok,string s){if(!ok)throw new Exception(s);File.AppendAllText(Dir+"/checks.txt","PASS "+s+"\n");}
        static void Baked(){Lightmapping.bakeCompleted-=Baked;Save();Finish(null);}
        static void Log(string s,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;var s=(e==null&&errors==0?"PASS ":"FAIL ")+command+" "+DateTime.UtcNow.ToString("O")+" "+(e?.ToString()??("errors="+errors))+" EDITOR ONLY; ANDROID NOT TESTED";File.WriteAllText(Dir+"/"+command+"-result.txt",s);File.AppendAllText(Dir+"/result-history.txt",s+"\n");Debug.Log(s);if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();ownsPlay=false;}
        static void Save(){AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Scene saved through Unity APIs");}
        static void Checkpoint(string phase){var root=File.ReadAllText(Dir+"/backup-path.txt").Trim().Trim('\uFEFF');var dest=Path.Combine(root,phase+"-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));Directory.CreateDirectory(dest);File.Copy(ScenePath,dest+"/Scene-before-save.unity");File.Copy(ScenePath+".meta",dest+"/BancoCorresponsal.unity.meta");if(Directory.Exists(Path.ChangeExtension(ScenePath,null)))CopyTree(Path.ChangeExtension(ScenePath,null),dest+"/Lighting");Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Saved current memory with recovery copy");AssetDatabase.SaveAssets();File.Copy(ScenePath,dest+"/Scene-saved.unity");File.AppendAllText(Dir+"/checkpoints.txt",dest+"\n");}
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
        static void Capture(string file,Vector3 eye,Vector3 look,float ortho=0){
            var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var reticle=gaze!=null?gaze.Reticle:null;bool originalReticleActive=reticle!=null&&reticle.gameObject.activeSelf;bool hideObserverReticle=reticle!=null&&gaze.View!=null&&Vector3.Distance(eye,gaze.View.transform.position)>.25f;
            var path=Gallery+"/"+file;Directory.CreateDirectory(Path.GetDirectoryName(path));Canvas.ForceUpdateCanvases();var ceiling=All<Renderer>().Where(x=>x.name=="Ceiling"||x.name=="Modular ceiling grid"||x.name=="V2 modular ceiling joints").ToArray();var off=ceiling.Select(x=>x.forceRenderingOff).ToArray();var active=RenderTexture.active;var g=new GameObject("Temporary polish review camera"){hideFlags=HideFlags.HideAndDontSave};RenderTexture rt=null;Texture2D tex=null;
            try{if(hideObserverReticle)reticle.gameObject.SetActive(false);var c=g.AddComponent<Camera>();c.enabled=false;c.stereoTargetEye=StereoTargetEyeMask.None;c.nearClipPlane=.05f;c.farClipPlane=60;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(.08f,.1f,.12f);c.aspect=1600f/900;c.fieldOfView=60;c.orthographic=ortho>0;c.orthographicSize=ortho;c.transform.position=eye;var d=look-eye;c.transform.rotation=Quaternion.LookRotation(d,Mathf.Abs(Vector3.Dot(d.normalized,Vector3.up))>.99f?Vector3.forward:Vector3.up);var data=c.GetUniversalAdditionalCameraData();data.renderPostProcessing=false;data.renderShadows=true;rt=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32);Check(rt.Create(),"Render target allocated");if(ortho>0)foreach(var r in ceiling)r.forceRenderingOff=true;var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};Check(RenderPipeline.SupportsRenderRequest(c,request),"URP render request supported");RenderPipeline.SubmitRenderRequest(c,request);RenderTexture.active=rt;tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());File.AppendAllText(Gallery+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+file+" eye="+eye+" look="+look+" fov=60 ortho="+ortho+" Play="+EditorApplication.isPlaying+" NOT_ANDROID\n");}
            finally{if(hideObserverReticle&&reticle!=null)reticle.gameObject.SetActive(originalReticleActive);for(int i=0;i<ceiling.Length;i++)ceiling[i].forceRenderingOff=off[i];RenderTexture.active=active;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}UnityEngine.Object.DestroyImmediate(g);}
        }
        static Material wood,metal,fabric,polymer,paper,accent,stone;
        static Transform Root=>GameObject.Find("GraphicsPolishV2").transform;
        static Transform Group(Transform parent,string name){var g=new GameObject(name);Undo.RegisterCreatedObjectUndo(g,"Visual polish");g.transform.SetParent(parent,false);return g.transform;}
        static Material Mat(string name,Color color,float smooth=.2f,float metallic=0,bool unlit=false){
            var path=Out+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit")){name=name};AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",color);if(!unlit){m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",metallic);}return m;
        }
        static void SetMaterials(GameObject g,params Material[] mats){foreach(var r in g.GetComponentsInChildren<MeshRenderer>()){Undo.RecordObject(r,"Scene material overrides");r.sharedMaterials=r.sharedMaterials.Select((m,i)=>mats[Mathf.Min(i,mats.Length-1)]).ToArray();PrefabUtility.RecordPrefabInstancePropertyModifications(r);}}
        static void Pose(Transform t,Vector3 point,float yaw){Undo.RecordObject(t,"Visual polish pose");t.SetPositionAndRotation(point,Quaternion.Euler(0,yaw,0));PrefabUtility.RecordPrefabInstancePropertyModifications(t);}
        static void Ground(Transform t,float y=0){var b=BoundsOf(t.gameObject);Undo.RecordObject(t,"Support contact");t.position+=Vector3.up*(y-b.min.y);}
        static GameObject Box(Transform parent,string name,Vector3 p,Vector3 size,Material mat,bool collider=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);Undo.RegisterCreatedObjectUndo(g,"Visual polish detail");g.name=name;g.transform.SetParent(parent,true);g.transform.position=p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.ContributeGI|StaticEditorFlags.BatchingStatic);if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
        static GameObject Native(string file,string name,Transform parent,Vector3 p,float height,float yaw=0){
            var root=Group(parent,name);var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/KenneyFurniture/Models/FBX format/"+file+".fbx");Check(source!=null,"Verified Kenney source "+file);var child=(GameObject)PrefabUtility.InstantiatePrefab(source);child.transform.SetParent(root,false);var b=BoundsOf(child);child.transform.localScale*=height/b.size.y;b=BoundsOf(child);child.transform.position-=new Vector3(b.center.x,b.min.y,b.center.z);Pose(root,p,yaw);SetMaterials(root.gameObject,wood,metal);return root.gameObject;
        }
        static GameObject Reuse(string name,string instance,Transform parent,Vector3 p,float yaw=0){var src=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/Prefabs/"+name+".prefab");Check(src!=null,"Reused verified prefab "+name);var g=(GameObject)PrefabUtility.InstantiatePrefab(src);Undo.RegisterCreatedObjectUndo(g,"Visual polish furniture");g.name=instance;g.transform.SetParent(parent,false);Pose(g.transform,p,yaw);Ground(g.transform,p.y);return g;}
        static void JoinedBoxes(string name,Material mat,IEnumerable<(Vector3 p,Vector3 size)> parts,bool gi=true){
            var primitive=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=primitive.GetComponent<MeshFilter>().sharedMesh;var combines=parts.Select(p=>new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p.p,Quaternion.identity,p.size)}).ToArray();var mesh=new Mesh{name=name};mesh.CombineMeshes(combines,true,true);UnityEngine.Object.DestroyImmediate(primitive);if(gi)Unwrapping.GenerateSecondaryUVSet(mesh);AssetDatabase.CreateAsset(mesh,Out+"/Geometry/"+name.Replace(' ','_')+".asset");var go=Group(Root,name).gameObject;go.AddComponent<MeshFilter>().sharedMesh=mesh;var r=go.AddComponent<MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=gi?ShadowCastingMode.On:ShadowCastingMode.Off;GameObjectUtility.SetStaticEditorFlags(go,StaticEditorFlags.BatchingStatic|(gi?StaticEditorFlags.ContributeGI:0));
        }
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
            var notebook=All<BankInteractable>().Single(t=>t.Action==BankAction.Notebook);Capture("Play/05-libreta-teclado.png",eye.transform.position,notebook.transform.position+Vector3.right*.5f);Capture("Play/06-monitor-centrado.png",eye.transform.position,ui.PanelAnchor.position);
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
        const string MainScene="Assets/_Project/Scenes/BancoCorresponsal.unity";
        static void Tick(){
            if(routine!=null){if(lastFrame==Time.frameCount)return;lastFrame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning||!File.Exists(Queue))return;
            string next;try{next=File.ReadAllText(Queue).Trim();File.Delete(Queue);}catch(IOException){return;}
            command=next;errors=0;
            try{
                Check(SceneManager.sceneCount==1,"Single scene open");
                if(command=="prepare"){
                    Check(SceneManager.GetActiveScene().path==MainScene&&!SceneManager.GetActiveScene().isDirty,"Main scene saved; no memory edits discarded");
                    Check(!File.Exists(ScenePath),"No review scene overwritten");Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));AssetDatabase.Refresh();
                    File.WriteAllText(Dir+"/main-scene-sha.txt",Hash(MainScene));
                    Check(AssetDatabase.CopyAsset(MainScene,ScenePath),"Isolated scene with new GUID copied through AssetDatabase");
                    EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);Inspect("Before");Views("Before");Save();Finish(null);return;
                }
                Check(SceneManager.GetActiveScene().path==ScenePath,"Only isolated GraphicsV2 scene modified");
                if(command=="save"){Checkpoint("Save");Finish(null);return;}
                Check(!SceneManager.GetActiveScene().isDirty,"No unsaved changes overwritten");
                if(command=="materials"){Checkpoint("BeforeMaterials");MaterialsV2();Save();Inspect("Materials");Views("Materials");Finish(null);}
                else if(command=="architecture"){Checkpoint("BeforeArchitecture");ArchitectureV2();Save();Inspect("Architecture");Views("Architecture");Finish(null);}
                else if(command=="finish"){Checkpoint("BeforeFinalFinish");FinalFinish();Save();Finish(null);}
                else if(command=="protected"){ValidateProtected();Finish(null);}
                else if(command=="seal"){foreach(var test in new[]{"bake","after","play","flow","office","smoke","environment","protected"})Check(File.ReadAllText(Dir+"/"+test+"-result.txt").StartsWith("PASS "),"Final test "+test);File.WriteAllText(Dir+"/validated-review-sha.txt",Hash(ScenePath));File.WriteAllText(Dir+"/validated-dependencies.txt",AssetDatabase.GetAssetDependencyHash(ScenePath).ToString());Finish(null);}
                else if(command=="lighting"){Checkpoint("BeforeLighting");LightingV2();Save();Finish(null);}
                else if(command=="bake"){Checkpoint("BeforeBake");Lightmapping.lightingDataAsset=null;Lightmapping.bakeCompleted+=Baked;Check(Lightmapping.BakeAsync(),"Bake accepted for isolated scene");File.WriteAllText(Dir+"/bake-result.txt","RUNNING");}
                else if(command=="after"){Inspect("After");Views("After");QualityAudit();Check(All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy).All(r=>r.sharedMaterials.All(m=>m!=null&&m.shader!=null&&m.shader.isSupported)),"All visible mesh materials use supported shaders");Finish(null);}
                else if(new[]{"play","flow","office","smoke","environment"}.Contains(command)){SessionState.SetString("SWIFT_GRAPHICSV2_COMMAND",command);EditorApplication.EnterPlaymode();}
                else throw new Exception("Unknown command "+command);
            }catch(Exception e){Finish(e);}
        }
        static string Hash(string path){using(var sha=System.Security.Cryptography.SHA256.Create())return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","");}
        static void PlayChanged(PlayModeStateChange s){
            if(s!=PlayModeStateChange.EnteredPlayMode)return;var pending=SessionState.GetString("SWIFT_GRAPHICSV2_COMMAND","");if(pending=="")return;SessionState.EraseString("SWIFT_GRAPHICSV2_COMMAND");command=pending;ownsPlay=true;errors=0;Application.logMessageReceived+=Log;
            if(command=="play")routine=Play();
            else if(command=="environment")routine=(IEnumerator)typeof(EnvironmentRedesign).GetMethod("Review",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,null);
            else routine=(IEnumerator)typeof(UXGameplayReview).GetMethod("Probe",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,new object[]{command});
        }
        static void Views(string phase){
            Capture(phase+"/01-entrada.png",new Vector3(0,1.6f,-6.6f),new Vector3(0,1.45f,3.1f));
            Capture(phase+"/02-general.png",new Vector3(8.8f,2.2f,-6.9f),new Vector3(0,1.3f,3.6f));
            Capture(phase+"/03-cubiculos.png",new Vector3(-3.6f,1.65f,-3.1f),new Vector3(-6.6f,1.05f,0));
            Capture(phase+"/04-puesto.png",new Vector3(-1.8f,1.8f,-1.1f),new Vector3(.25f,1.2f,1.7f));
            Capture(phase+"/05-documentacion.png",new Vector3(-1.5f,1.65f,1),new Vector3(-4.2f,1.2f,4.3f));
            Capture(phase+"/06-supervision.png",new Vector3(2.7f,1.6f,4.3f),new Vector3(7.4f,1.4f,8.2f));
            Capture(phase+"/07-nocturna.png",new Vector3(0,1.6f,8.3f),new Vector3(7.7f,2,-.3f));
            Capture(phase+"/08-planta.png",new Vector3(0,21,1),new Vector3(0,0,1),11);
        }
        static Material FinishMaterial(string name,Color color,float smooth,float metallic=0,string texture=null){
            var m=Mat(name,color,smooth,metallic);if(texture!=null)m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/OfficeVisual/VisualPolish/Textures/"+texture+".png"));return m;
        }
        static void MaterialsV2(){
            Check(GameObject.Find("GraphicsPolishV2")==null,"Materials stage runs once");Directory.CreateDirectory(Out+"/Materials");Directory.CreateDirectory(Out+"/Geometry");AssetDatabase.Refresh();Group(null,"GraphicsPolishV2");
            wood=FinishMaterial("Wood_DesaturatedOak",new Color(.46f,.365f,.275f),.28f,0,"Oak_Grain_256");
            var inset=FinishMaterial("Wood_WalnutInset",new Color(.30f,.255f,.21f),.23f,0,"Oak_Grain_256");
            metal=FinishMaterial("Metal_GraphitePaint",new Color(.27f,.31f,.34f),.38f,.35f);
            var archive=FinishMaterial("Metal_ArchiveEnamel",new Color(.48f,.52f,.53f),.32f,.15f);
            var trim=FinishMaterial("Metal_AluminiumTrim",new Color(.48f,.52f,.54f),.38f,.65f);
            polymer=FinishMaterial("Plastic_EquipmentMatte",new Color(.08f,.095f,.115f),.18f);
            var keys=FinishMaterial("Plastic_Keycaps",new Color(.25f,.28f,.30f),.12f);
            fabric=FinishMaterial("Fabric_AcousticSlate",new Color(.27f,.35f,.38f),.04f,0,"Acoustic_Weave_256");
            var chair=FinishMaterial("Fabric_SeatCharcoal",new Color(.12f,.17f,.20f),.06f,0,"Acoustic_Weave_256");
            paper=FinishMaterial("Paper_Ivory",new Color(.83f,.81f,.735f),.025f);
            var edge=FinishMaterial("Paper_StackEdges",new Color(.66f,.665f,.62f),.025f);
            var walls=FinishMaterial("Architecture_Mineral",new Color(.60f,.64f,.66f),.08f);
            var ceiling=FinishMaterial("Architecture_CeilingIvory",new Color(.76f,.77f,.73f),.025f);
            var carpet=FinishMaterial("Architecture_CarpetSlate",new Color(.285f,.33f,.37f),.015f,0,"Carpet_Tile_256");carpet.SetTextureScale("_BaseMap",new Vector2(20,18));
            var aisle=FinishMaterial("Architecture_CarpetAisle",new Color(.32f,.365f,.40f),.015f,0,"Carpet_Tile_256");aisle.SetTextureScale("_BaseMap",new Vector2(1,16));
            var map=new Dictionary<string,Material>{{"Oak_Satin",wood},{"Oak_DrawerInset",inset},{"Oak_Worksurface",wood},{"Steel_Powdercoat",metal},{"Painted_Metal",metal},{"Window_Frame",metal},{"Acoustic_Woven",fabric},{"Seat_Upholstery",chair},{"Equipment_Matte",polymer},{"Equipment_Polymer",polymer},{"Paper_Warm",paper},{"Wall_Mineral",walls},{"Ceiling_Acoustic",ceiling},{"Carpet_BlueGrey",carpet},{"Aisle_Slate",aisle}};
            foreach(var r in All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy)){
                var path=PathOf(r.transform);Undo.RecordObject(r,"V2 shared material family");
                r.sharedMaterials=r.sharedMaterials.Select((m,i)=>{
                    if(m==null)return m;
                    // Specific object roles precede generic prior material names.
                    if(path.Contains("Interactive Archive")||path.Contains("Supervisor files"))return i==0?archive:trim;
                    if(path.Contains("Ordered papers")||path.Contains("Fictional transfer documents"))return i==0?paper:edge;
                    if(path.Contains("Keyboard")&&!path.Contains("Keyboard1"))return i==0?polymer:keys;
                    if(path.Contains("Mouse")&&!path.Contains("Comp_Mouse"))return polymer;
                    if(r.name=="Ceiling cross beam"||r.name=="Supervision canopy")return metal;
                    return map.TryGetValue(m.name,out var replacement)?replacement:m;
                }).ToArray();PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            File.WriteAllLines(Dir+"/material-library.txt",AssetDatabase.FindAssets("t:Material",new[]{Out}).Select(g=>AssetDatabase.GUIDToAssetPath(g)));
        }
        static void ArchitectureV2(){
            if(GameObject.Find("V2 finished architecture")==null)Group(Root,"V2 finished architecture");
            metal=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Metal_GraphitePaint.mat");wood=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Wood_DesaturatedOak.mat");paper=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Paper_Ivory.mat");
            var trim=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Metal_AluminiumTrim.mat");
            if(GameObject.Find("V2 modular ceiling joints")==null){
            var oldGrid=GameObject.Find("Modular ceiling grid").GetComponent<Renderer>();Undo.RecordObject(oldGrid,"Retain original grid for recovery");oldGrid.enabled=false;
            var parts=new List<(Vector3,Vector3)>();for(float x=-8.8f;x<9.8f;x+=1.2f)parts.Add((new Vector3(x,3.706f,1),new Vector3(.012f,.012f,17.8f)));for(float z=-6.8f;z<9.8f;z+=1.2f)parts.Add((new Vector3(0,3.706f,z),new Vector3(19.8f,.012f,.012f)));JoinedBoxes("V2 modular ceiling joints",metal,parts);
            parts.Clear();foreach(var r in All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy&&r.name=="Window transom")){var b=r.bounds;if(b.center.y<2)parts.Add((b.center+new Vector3(0,-.055f,0),new Vector3(.17f,.045f,b.size.z)));}JoinedBoxes("V2 window sill reveals",trim,parts);
            parts.Clear();foreach(var r in All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy&&(r.name=="Fabric acoustic back"||r.name=="Acoustic side"))){var b=r.bounds;parts.Add((new Vector3(b.center.x,.055f,b.center.z),new Vector3(b.size.x+.025f,.11f,b.size.z+.025f)));}JoinedBoxes("V2 cubicle plinth channels",metal,parts);
            }
            // Variation uses existing verified props; desk, chair, monitor and collider poses stay unchanged.
            int index=0;foreach(Transform booth in GameObject.Find("02_SecondaryCubicles").transform){var desk=booth.Find("Shared desk");if(desk==null)continue;var b=BoundsOf(desk.gameObject);var docs=booth.Find("Ordered papers");if(docs!=null){Undo.RecordObject(docs,"Moderate paper arrangement");docs.rotation=Quaternion.Euler(0,index%2==0?-7:9,0);docs.position=new Vector3(b.center.x+.78f,docs.position.y,b.center.z-.12f);Ground(docs,b.max.y);PrefabUtility.RecordPrefabInstancePropertyModifications(docs);}
                var existingBooks=booth.Find("Reference books");if(existingBooks!=null){Undo.RecordObject(existingBooks,"Reference books supported by own desk");existingBooks.position=new Vector3(b.center.x+.80f,b.max.y,b.center.z+.1f);Ground(existingBooks,b.max.y);PrefabUtility.RecordPrefabInstancePropertyModifications(existingBooks);}
                if(index==2&&GameObject.Find("V2 ordered reference books "+index)==null){var books=Reuse("Kenney_Books","V2 ordered reference books "+index,Root,new Vector3(b.center.x+.82f,b.max.y,b.center.z+.22f),index==1?90:0);SetMaterials(books,wood,paper,metal);Ground(books.transform,b.max.y);}
                index++;
            }
            // Sparse opaque city lights placed inside each actual silhouette, not a separate distant city.
            var oldCity=GameObject.Find("Distant city office windows").GetComponent<Renderer>();Undo.RecordObject(oldCity,"Retain previous city dots");oldCity.enabled=false;
            var windows=new List<(Vector3,Vector3)>();int building=0;foreach(var r in All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy&&r.name=="City silhouette - geometry")){var b=r.bounds;float x=b.center.x-Mathf.Sign(b.center.x)*.012f;for(float y=b.min.y+.14f;y<b.max.y-.08f;y+=.16f)for(int c=0;c<2;c++)if((building+c+Mathf.RoundToInt(y*10))%3!=0)windows.Add((new Vector3(x,y,b.center.z+(c==0?-.13f:.13f)),new Vector3(.006f,.045f,.055f)));building++;}
            JoinedBoxes("V2 sparse city lights",Mat("Exterior_WarmWindows",new Color(.46f,.40f,.29f),unlit:true),windows,false);
            Check(index==4,"Four cubicles decorated without changing layout");
        }
        static void LightingV2(){
            foreach(var l in All<Light>().Where(l=>l.isActiveAndEnabled)){
                Undo.RecordObject(l,"V2 restrained work hierarchy");
                if(l.lightmapBakeType==LightmapBakeType.Baked){l.shadows=LightShadows.Soft;if(l.name=="Baked analyst task light"){l.color=new Color(1,.85f,.66f);l.intensity=3.2f;}
                    else if(l.name.Contains("warm work light")||l.name=="Supervision work light"){l.color=new Color(.94f,.96f,1);l.intensity=l.name=="Supervision work light"?2.8f:2.3f;}
                    else{l.color=new Color(.93f,.965f,1);}
                }
            }
            Check(All<Light>().Count(l=>l.isActiveAndEnabled&&l.lightmapBakeType!=LightmapBakeType.Baked)==1,"No additional realtime lights");
        }
        static void QualityAudit(){
            var rows=new List<string>{"No quality/pipeline/XR settings modified. Overdraw/GPU/FPS not measured on Android."};
            foreach(var profile in new[]{"PC","Mobile"}){var p=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/"+profile+"_RPAsset.asset");rows.Add(profile+": renderScale="+p.renderScale+" MSAA="+p.msaaSampleCount+" shadow="+p.mainLightShadowmapResolution+" shadowDistance="+p.shadowDistance+" additionalShadows="+p.supportsAdditionalLightShadows);}
            foreach(var lm in LightmapSettings.lightmaps)foreach(var tex in new[]{lm.lightmapColor,lm.lightmapDir})if(tex!=null)rows.Add("Lightmap "+AssetDatabase.GetAssetPath(tex)+" "+tex.width+"x"+tex.height+" format="+tex.format+" memoryBytes="+UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(tex));
            foreach(var guid in AssetDatabase.FindAssets("t:Texture",new[]{"Assets/_Project/OfficeVisual/VisualPolish/Textures"})){var path=AssetDatabase.GUIDToAssetPath(guid);var i=AssetImporter.GetAtPath(path) as TextureImporter;if(i!=null)rows.Add(path+" filter="+i.filterMode+" aniso="+i.anisoLevel+" mipmaps="+i.mipmapEnabled+" compression="+i.textureCompression);}
            rows.Add("Opaque city surfaces; no new full-screen transparent overlay or post effect. Frame Debugger/overdraw capture pending.");File.WriteAllLines(Dir+"/quality-audit.txt",rows);
        }

        static void FinalFinish(){
            var stone=FinishMaterial("Architecture_ColumnMineral",new Color(.56f,.60f,.62f),.10f);
            var joints=FinishMaterial("Architecture_CeilingJoint",new Color(.49f,.52f,.52f),.04f);
            var metal=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Metal_GraphitePaint.mat");
            var trim=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Metal_AluminiumTrim.mat");
            var caps=new List<(Vector3,Vector3)>();
            foreach(var r in All<MeshRenderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy)){
                var path=PathOf(r.transform);Undo.RecordObject(r,"V2 final architectural finish");
                if(r.name=="Perimeter column"){r.sharedMaterial=stone;var b=r.bounds;caps.Add((new Vector3(b.center.x,.12f,b.center.z),new Vector3(.325f,.24f,.325f)));caps.Add((new Vector3(b.center.x,3.53f,b.center.z),new Vector3(.325f,.085f,.325f)));}
                else if(r.name=="V2 modular ceiling joints")r.sharedMaterial=joints;
                else if(path.Contains("Entrance door frame")&&r.name=="doorwayFront")r.sharedMaterial=trim;
                else if(path.Contains("Interactive Archive")||path.Contains("Supervisor files"))r.sharedMaterials=r.sharedMaterials.Select((m,i)=>m.name=="Metal_AluminiumTrim"?trim:m).ToArray();
                PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
            if(GameObject.Find("V2 column base and capital collars")==null)JoinedBoxes("V2 column base and capital collars",metal,caps);
        }
        static string[] InteractionState(Scene scene){
            var rows=new List<string>();foreach(var t in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<BankInteractable>(true))){
                rows.Add(PathOf(t.transform)+" action="+t.Action+" p="+t.transform.position.ToString("F5")+" q="+t.transform.rotation.ToString("F5")+" scale="+t.transform.lossyScale.ToString("F5")+" destination="+t.Destination.ToString("F5"));
                foreach(var c in t.GetComponentsInChildren<Collider>(true)){var box=c as BoxCollider;rows.Add(PathOf(c.transform)+" collider="+c.GetType().Name+" enabled="+c.enabled+" trigger="+c.isTrigger+" layer="+c.gameObject.layer+" p="+c.transform.position.ToString("F5")+" scale="+c.transform.lossyScale.ToString("F5")+(box!=null?" center="+box.center.ToString("F5")+" size="+box.size.ToString("F5"):""));}
            }return rows.OrderBy(x=>x,StringComparer.Ordinal).ToArray();
        }
        static void ValidateProtected(){
            Check(Hash(MainScene)==File.ReadAllText(Dir+"/main-scene-sha.txt"),"Main scene bytes unchanged during isolated review");
            var review=SceneManager.GetActiveScene();var current=InteractionState(review);Scene original=default;
            try{original=EditorSceneManager.OpenScene(MainScene,OpenSceneMode.Additive);var baseline=InteractionState(original);File.WriteAllLines(Dir+"/interaction-baseline.txt",baseline);File.WriteAllLines(Dir+"/interaction-current.txt",current);Check(baseline.SequenceEqual(current),"All bank actions, poses, destinations and action colliders preserved exactly");}
            finally{if(original.IsValid())EditorSceneManager.CloseScene(original,true);SceneManager.SetActiveScene(review);}
        }
        [MenuItem("SWIFT/Graphics Polish V2/Integrar revision validada (sin otra sesion editando)")]
        public static void IntegrateToMain(){
            Check(!EditorApplication.isPlayingOrWillChangePlaymode&&!Lightmapping.isRunning,"Editor idle for integration");
            Check(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==ScenePath&&!SceneManager.GetActiveScene().isDirty,"Saved V2 review is active");
            Check(Hash(MainScene)==File.ReadAllText(Dir+"/main-scene-sha.txt"),"Main scene unchanged; concurrent changes require manual reconciliation");
            Check(Hash(ScenePath)==File.ReadAllText(Dir+"/validated-review-sha.txt")&&AssetDatabase.GetAssetDependencyHash(ScenePath).ToString()==File.ReadAllText(Dir+"/validated-dependencies.txt"),"Review scene and dependencies match validated state");
            foreach(var test in new[]{"bake","after","play","flow","office","smoke","environment","protected"})Check(File.ReadAllText(Dir+"/"+test+"-result.txt").StartsWith("PASS "),"Validation before integration: "+test);
            var backup=File.ReadAllText(Dir+"/backup-path.txt").Trim().Trim('\uFEFF')+"/BeforeIntegration-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");Directory.CreateDirectory(backup);File.Copy(MainScene,backup+"/BancoCorresponsal.unity");File.Copy(MainScene+".meta",backup+"/BancoCorresponsal.unity.meta");CopyTree(Path.ChangeExtension(MainScene,null),backup+"/Lighting");
            Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),MainScene,true),"V2 saved to main through Unity with existing scene GUID");
            EditorSceneManager.OpenScene(MainScene,OpenSceneMode.Single);Lightmapping.lightingDataAsset=null;Lightmapping.bakeCompleted+=IntegratedBake;Check(Lightmapping.BakeAsync(),"Main-scene GUID requires its own bake after integration");File.WriteAllText(Dir+"/integration-result.txt","RUNNING main bake; integration validation pending");
        }
        static void IntegratedBake(){
            Lightmapping.bakeCompleted-=IntegratedBake;
            try{Check(SceneManager.GetActiveScene().path==MainScene,"Main active after integration bake");AssetDatabase.SaveAssets();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Main bake saved");File.WriteAllText(Dir+"/integration-result.txt","BAKE_COMPLETE "+DateTime.UtcNow.ToString("O")+" main="+Hash(MainScene)+"; repeat gameplay probes on main before APK");}
            catch(Exception e){File.WriteAllText(Dir+"/integration-result.txt","FAIL "+e);Debug.LogException(e);}
        }
    }
}

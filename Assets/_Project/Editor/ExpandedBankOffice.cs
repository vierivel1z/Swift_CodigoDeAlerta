using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using Swift.Interaction;
using Swift.Game;
using Swift.Bank;
using Swift.UI;
using Swift.VR;

namespace Swift.EditorTools
{
    [InitializeOnLoad]
    public static class ExpandedBankOffice
    {
        const string Dir="Artifacts/ExpandedBankOffice",Queue=Dir+"/command.txt",Pending="SWIFT_EXPANDED_CAPTURE";
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Out="Assets/_Project/OfficeVisual/ExpandedBankOffice";
        const string Gallery="Docs/Previews/ExpandedBankOffice/20261103-OperationsRoom";
        static string command;static IEnumerator routine;static bool ownsPlay;static int errors;
        static UIManager ui;static GameManager game;static Material navy,metal,wood,fabric,window,sky,lightStrip;
        [Serializable] public class Metrics {
            public string utc,scene,phase,circulationMethod="0.25 m grid, body capsule radius 0.25 m from y=0.30 to y=1.60; teleport/seat markers ignored; prefab colliders conservatively count as occupied",drawCalls="NOT MEASURED; Android profiling pending";
            public Vector3 floorCenter,floorSize;public float width,length,grossArea,freeArea,entryConnectedArea,grid=.25f;
            public int freeSamples,connectedSamples,meshRenderers,meshMaterials,teleports,cubicles,zones=6,bakedLights,realtimeLights,lightmaps;public long triangles;
        }
        static ExpandedBankOffice(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static void Tick(){
            if(routine!=null){try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning||!File.Exists(Queue))return;
            try{
                command=File.ReadAllText(Queue).Trim();File.Delete(Queue);errors=0;
                Check(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==ScenePath,"Only bank scene open");
                Check(File.Exists(Dir+"/backup-path.txt"),"External recovery receipt exists");
                if(command=="save"){Checkpoint("ExplicitSave");Finish(null);return;}
                Check(!SceneManager.GetActiveScene().isDirty,"No unsaved scene discarded");
                ui=All<UIManager>().Single(x=>x.gameObject.activeInHierarchy);game=All<GameManager>().Single(x=>x.gameObject.activeInHierarchy);
                if(command=="before"){Checkpoint("Before");Measure("Before");Capture("01-planta-antes.png",new Vector3(0,21,1),new Vector3(0,0,1),60,11,true);Capture("03a-entrada-antes.png",new Vector3(0,1.75f,-4.4f),new Vector3(0,1.5f,2),60);Finish(null);}
                else if(command=="architecture"){Architecture();Finish(null);}
                else if(command=="workstation"){Workstation();Finish(null);}
                else if(command=="relight"){Relight();Finish(null);}
                else if(command=="bake"){
                    Check(!Lightmapping.isRunning,"No concurrent bake");Checkpoint("BeforeBake");Lightmapping.bakeCompleted+=Baked;Check(Lightmapping.BakeAsync(),"Bake started");File.WriteAllText(Dir+"/bake-result.txt","RUNNING");
                }
                else if(command=="after"){Validate();Measure("After");CaptureAfter();Finish(null);}
                else if(command=="play-captures"){SessionState.SetBool(Pending,true);EditorApplication.EnterPlaymode();}
                else throw new Exception("Unknown explicit expansion command: "+command);
            }catch(Exception e){Finish(e);}
        }
        static void Baked(){Lightmapping.bakeCompleted-=Baked;AssetDatabase.SaveAssets();Save();Finish(null);}
        static void PlayChanged(PlayModeStateChange s){if(s!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool(Pending,false))return;SessionState.EraseBool(Pending);ownsPlay=true;command="play-captures";errors=0;Application.logMessageReceived+=Log;routine=PlayCaptures();}
        static void Log(string s,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;var result=(e==null&&errors==0?"PASS ":"FAIL ")+command+" "+DateTime.UtcNow.ToString("O")+" "+(e?.ToString()??("errors="+errors))+" EDITOR ONLY; ANDROID NOT TESTED";Directory.CreateDirectory(Dir);File.WriteAllText(Dir+"/"+command+"-result.txt",result);Debug.Log(result);if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();ownsPlay=false;}
        static void Check(bool ok,string label){if(!ok)throw new Exception(label);Directory.CreateDirectory(Dir);File.AppendAllText(Dir+"/checks.txt","PASS "+label+"\n");}
        static void Save(){AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Scene saved via Unity");}
        static void Checkpoint(string phase){
            string root=File.ReadAllText(Dir+"/backup-path.txt").Trim().Trim('\uFEFF');string dest=Path.Combine(root,phase+"-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));Directory.CreateDirectory(dest);
            File.Copy(ScenePath,Path.Combine(dest,"BancoCorresponsal-before-save.unity"));File.Copy(ScenePath+".meta",Path.Combine(dest,"BancoCorresponsal.unity.meta"));
            if(Directory.Exists(Path.ChangeExtension(ScenePath,null)))CopyTree(Path.ChangeExtension(ScenePath,null),Path.Combine(dest,"Lighting"));
            Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Memory scene saved with external recovery copy");AssetDatabase.SaveAssets();File.Copy(ScenePath,Path.Combine(dest,"BancoCorresponsal-saved.unity"));File.AppendAllText(Dir+"/checkpoints.txt",dest+"\n");
        }
        static void CopyTree(string from,string to){Directory.CreateDirectory(to);foreach(var p in Directory.GetFiles(from))File.Copy(p,Path.Combine(to,Path.GetFileName(p)));foreach(var p in Directory.GetDirectories(from))CopyTree(p,Path.Combine(to,Path.GetFileName(p)));}
        static bool Occupied(Vector3 p){
            foreach(var c in Physics.OverlapCapsule(p+Vector3.up*.30f,p+Vector3.up*1.6f,.25f,~0,QueryTriggerInteraction.Ignore)){
                var a=c.GetComponentInParent<BankInteractable>();if(a!=null&&(a.Action==BankAction.Teleport||a.Action==BankAction.Sit||a.Action==BankAction.Stand))continue;
                return true;
            }return false;
        }
        static void Measure(string phase){
            Physics.SyncTransforms();var floor=GameObject.Find("Floor").GetComponent<Renderer>().bounds;var size=floor.size;
            var m=new Metrics{utc=DateTime.UtcNow.ToString("O"),scene=ScenePath,phase=phase,floorCenter=floor.center,floorSize=size,width=size.x,length=size.z,grossArea=size.x*size.z};
            int nx=Mathf.FloorToInt(size.x/m.grid),nz=Mathf.FloorToInt(size.z/m.grid);var free=new bool[nx*nz];
            for(int z=0;z<nz;z++)for(int x=0;x<nx;x++)if(!Occupied(new Vector3(floor.min.x+(x+.5f)*m.grid,0,floor.min.z+(z+.5f)*m.grid))){free[z*nx+x]=true;m.freeSamples++;}
            int sx=Mathf.Clamp(Mathf.FloorToInt((0-floor.min.x)/m.grid),0,nx-1),sz=Mathf.Clamp(Mathf.FloorToInt((-3.3f-floor.min.z)/m.grid),0,nz-1);var seen=new bool[free.Length];var q=new Queue<int>();
            if(free[sz*nx+sx]){q.Enqueue(sz*nx+sx);seen[sz*nx+sx]=true;}
            while(q.Count>0){int n=q.Dequeue();m.connectedSamples++;int x=n%nx,z=n/nx;foreach(var d in new[]{Vector2Int.left,Vector2Int.right,Vector2Int.up,Vector2Int.down}){int xx=x+d.x,zz=z+d.y;if(xx<0||xx>=nx||zz<0||zz>=nz)continue;int k=zz*nx+xx;if(!free[k]||seen[k])continue;seen[k]=true;q.Enqueue(k);}}
            m.freeArea=m.freeSamples*m.grid*m.grid;m.entryConnectedArea=m.connectedSamples*m.grid*m.grid;
            var rs=All<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy&&x.enabled).ToArray();m.meshRenderers=rs.Length;m.meshMaterials=rs.SelectMany(x=>x.sharedMaterials).Distinct().Count();
            foreach(var f in All<MeshFilter>().Where(x=>x.gameObject.activeInHierarchy&&x.sharedMesh!=null))for(int i=0;i<f.sharedMesh.subMeshCount;i++)m.triangles+=(long)f.sharedMesh.GetIndexCount(i)/3;
            m.cubicles=All<Transform>().Count(x=>x.gameObject.activeInHierarchy&&x.parent?.name=="02_SecondaryCubicles"&&x.name.StartsWith("Cubicle "));m.teleports=All<BankInteractable>().Count(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.Teleport);m.zones=GameObject.Find("ExpandedBankOfficeV2")!=null?6:5;
            m.bakedLights=All<Light>().Count(x=>x.gameObject.activeInHierarchy&&x.enabled&&x.lightmapBakeType==LightmapBakeType.Baked);m.realtimeLights=All<Light>().Count(x=>x.gameObject.activeInHierarchy&&x.enabled&&x.lightmapBakeType!=LightmapBakeType.Baked);m.lightmaps=LightmapSettings.lightmaps.Length;
            Directory.CreateDirectory(Gallery);File.WriteAllText(Dir+"/"+phase+"-metrics.json",JsonUtility.ToJson(m,true));File.WriteAllText(Gallery+"/"+phase+"-metrics.json",JsonUtility.ToJson(m,true));
            File.WriteAllLines(Dir+"/"+phase+"-architecture.txt",All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&(x.parent?.name=="01_Architecture"||x.name=="Ceiling"||x.name=="Floor")).Select(x=>x.name+" position="+x.position+" scale="+x.localScale+(x.TryGetComponent<Renderer>(out var r)?" bounds="+r.bounds:"")));
            Check(m.entryConnectedArea>0,"Entry-connected floor sampling completed");
        }
        static Transform Group(Transform parent,string name){var g=new GameObject(name);Undo.RegisterCreatedObjectUndo(g,"Expand bank office");g.transform.SetParent(parent,false);return g.transform;}
        static Material Mat(string name,Color c,bool unlit=false,bool emission=false){
            string path=Out+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m!=null)return m;m=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",c);
            if(!unlit){m.SetFloat("_Smoothness",.15f);if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*.7f);m.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;}}
            AssetDatabase.CreateAsset(m,path);return m;
        }
        static void Materials(){navy=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/EnvironmentRedesign/Materials/Institutional_Navy.mat");metal=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/EnvironmentRedesign/Materials/Painted_Metal.mat");wood=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/EnvironmentRedesign/Materials/Oak_Worksurface.mat");fabric=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/EnvironmentRedesign/Materials/Chair_Fabric.mat");window=Mat("Night_Window",new Color(.035f,.11f,.20f),true);sky=Mat("City_Silhouette",new Color(.019f,.045f,.08f),true);lightStrip=Mat("Warm_Diffuser",new Color(1,.78f,.48f),false,true);}
        static GameObject Box(Transform parent,string name,Vector3 point,Vector3 size,Material m,bool collider=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);Undo.RegisterCreatedObjectUndo(g,"Expand bank office");g.name=name;g.transform.SetParent(parent,true);g.transform.position=point;g.transform.localScale=size;var r=g.GetComponent<MeshRenderer>();r.sharedMaterial=m;r.shadowCastingMode=m.shader.name.EndsWith("/Unlit")?ShadowCastingMode.Off:ShadowCastingMode.On;
            GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.BatchingStatic|(m.shader.name.EndsWith("/Lit")?StaticEditorFlags.ContributeGI:0));if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
        static void Move(Transform t,Vector3 p){Undo.RecordObject(t,"Expand bank office");t.position=p;}
        static void Resize(string name,Vector3 p,Vector3 size){var g=GameObject.Find(name);Check(g!=null,"Found architectural object "+name);Undo.RecordObject(g.transform,"Expand architecture");g.transform.position=p;g.transform.localScale=size;}
        static void Shift(string name,Vector3 delta){var g=GameObject.Find(name);Check(g!=null,"Preserved functional object "+name);Move(g.transform,g.transform.position+delta);}
        static Bounds BoundsOf(GameObject g){var rs=g.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var r in rs.Skip(1))b.Encapsulate(r.bounds);return b;}
        static GameObject Place(string prefab,Transform parent,string name,Vector3 p){var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/Prefabs/"+prefab+".prefab");var g=(GameObject)PrefabUtility.InstantiatePrefab(asset);Undo.RegisterCreatedObjectUndo(g,"Expanded bank furnishing");g.name=name;g.transform.SetParent(parent,true);var b=BoundsOf(g);g.transform.position+=p-new Vector3(b.center.x,b.min.y,b.center.z);return g;}
        static Transform Surface(Transform parent,string name,Vector3 p,Vector2 size,float yaw=0){var g=new GameObject(name,typeof(RectTransform),typeof(Canvas));Undo.RegisterCreatedObjectUndo(g,"Expanded bank UI");g.transform.SetParent(parent,false);g.transform.SetPositionAndRotation(p,Quaternion.Euler(0,yaw,0));g.transform.localScale=Vector3.one*.001f;g.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;g.GetComponent<RectTransform>().sizeDelta=size*1000;OperationsCenterStyle.Panel(g.transform,"Surface",Vector2.zero,size*1000,OperationsCenterStyle.Navy);return g.transform;}
        static TMP_Text Label(Transform parent,string name,string value,Vector2 size,float font){var t=ui.Text(parent,name,Vector2.zero,size,font);t.text=value;t.alignment=TextAlignmentOptions.Center;t.color=Color.white;return t;}
        static void Architecture(){
            Check(GameObject.Find("ExpandedBankOfficeV2")==null,"No duplicate expansion");Checkpoint("BeforeArchitecture");Directory.CreateDirectory(Out+"/Materials");Directory.CreateDirectory(Out+"/Geometry");AssetDatabase.Refresh();Materials();
            var root=Group(null,"ExpandedBankOfficeV2");
            Resize("Floor",new Vector3(0,-.1f,1),new Vector3(20,.2f,18));Resize("Ceiling",new Vector3(0,3.8f,1),new Vector3(20,.16f,18));
            Resize("NorthWall",new Vector3(0,1.85f,10),new Vector3(20,3.7f,.2f));Resize("WestWall",new Vector3(-10,1.85f,1),new Vector3(.2f,3.7f,18));Resize("EastWall",new Vector3(10,1.85f,1),new Vector3(.2f,3.7f,18));
            Resize("EntranceWallLeft",new Vector3(-5.6f,1.85f,-8),new Vector3(8.8f,3.7f,.2f));Resize("EntranceWallRight",new Vector3(5.6f,1.85f,-8),new Vector3(8.8f,3.7f,.2f));
            foreach(string n in new[]{"EntranceLintel","Entrance infill left","Entrance infill right","Entrance door frame","Entrance door leaf provisional"})Shift(n,Vector3.back*3);
            Resize("EntranceLintel",new Vector3(0,3.1f,-8),new Vector3(2.4f,1.2f,.2f));
            Resize("NorthBlueBand",new Vector3(0,.45f,9.87f),new Vector3(19.8f,.9f,.04f));foreach(var t in All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.name=="SideBlueBand"))ResizeInstance(t,new Vector3(Mathf.Sign(t.position.x)*9.87f,.45f,1),new Vector3(.04f,.9f,17.8f));
            foreach(var t in All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.name=="Perimeter pilaster")){Undo.RecordObject(t.gameObject,"Retain previous pilasters");t.gameObject.SetActive(false);}
            for(int side=-1;side<=1;side+=2)for(int i=0;i<4;i++)Box(root,"Perimeter column",new Vector3(side*9.75f,1.85f,-6.8f+i*5.2f),new Vector3(.30f,3.7f,.30f),metal);
            for(int i=0;i<3;i++)Box(root,"Ceiling cross beam",new Vector3(0,3.62f,-4.5f+i*5.5f),new Vector3(19.6f,.18f,.18f),metal);
            var wallArt=GameObject.Find("ART - Bank Wall Graphic").transform;Undo.RecordObject(wallArt,"Institutional focal wall");wallArt.SetPositionAndRotation(new Vector3(0,2.27f,9.83f),Quaternion.identity);wallArt.localScale=Vector3.one*1.65f;
            Move(GameObject.Find("Bank title").transform,new Vector3(0,3.48f,9.80f));Move(GameObject.Find("Welcome").transform,new Vector3(-9.82f,1.8f,-5.5f));
            foreach(string n in new[]{"Wall clock provisional","Clock hour hand","Clock minute hand"})Shift(n,Vector3.forward);
            var booths=GameObject.Find("02_SecondaryCubicles").transform;var points=new[]{new Vector3(-6.6f,0,-.8f),new Vector3(-6.8f,0,4.5f),new Vector3(6.4f,0,.4f),new Vector3(6.8f,0,6.3f)};
            for(int i=0;i<4;i++){var booth=booths.Find("Cubicle "+(i+1));var desk=booth.Find("Shared desk");var b=BoundsOf(desk.gameObject);Move(booth,booth.position+points[i]-new Vector3(b.center.x,b.min.y,b.center.z));}
            Shift("Interactive Printer",Vector3.left*1.3f);Shift("Evidence table",new Vector3(-1.3f,0,2.2f));Shift("Fictional printed evidence",new Vector3(-1.3f,-.016f,2.2f));
            var oldSecurity=GameObject.Find("05_Zone_D_Security").transform;var pivot=Group(oldSecurity,"Relocated security zone");pivot.position=new Vector3(3.65f,0,4.9f);foreach(var child in oldSecurity.Cast<Transform>().Where(x=>x!=pivot).ToArray())Undo.SetTransformParent(child,pivot,"Preserve security references");Undo.RecordObject(pivot,"Security zone orientation");pivot.SetPositionAndRotation(new Vector3(8.6f,0,4.2f),Quaternion.Euler(0,90,0));
            var secPad=GameObject.Find("Teleport Security").GetComponent<BankInteractable>();Move(secPad.transform,secPad.Destination+Vector3.up*.025f);
            var partition=GameObject.Find("Security partition").transform;Undo.RecordObject(partition,"Aligned security backing");partition.SetPositionAndRotation(new Vector3(8.72f,1.2f,4.2f),Quaternion.Euler(0,90,0));
            Move(GameObject.Find("Supervision identity").transform,new Vector3(7.1f,3.25f,9.80f));
            Windows(root);Background(root);Console(root);Lighting(root);
            var input=All<EditorInputAdapter>().Single(x=>x.gameObject.activeInHierarchy);Undo.RecordObject(input,"Expanded debug bounds");input.MovementMin=new Vector2(-9.5f,-7.5f);input.MovementMax=new Vector2(9.5f,9.5f);
            Move(input.Rig,new Vector3(0,0,-6.6f));
            NewPad(root,"Teleport Reception",new Vector3(0,0,-6.6f),"RECEPCION");NewPad(root,"Teleport Rear Gallery",new Vector3(0,0,8.3f),"GALERIA");
            var inspection=GameObject.Find("Teleport Inspection").transform;Move(inspection,new Vector3(-2,.025f,-2.9f));Move(GameObject.Find("Go to analyst station").transform,new Vector3(-2,.055f,-2.9f));
            foreach(var t in All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.name=="Aisle inlay")){Undo.RecordObject(t.gameObject,"Retain old inlay");t.gameObject.SetActive(false);}
            var aisle=Mat("Aisle_Slate",new Color(.25f,.30f,.34f));for(int side=-1;side<=1;side+=2)Box(root,"Circulation ribbon",new Vector3(side*3.0f,.005f,1),new Vector3(1.0f,.008f,16),aisle);
            Save();BankNavigationValidation.Check(All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy));
        }
        static void ResizeInstance(Transform t,Vector3 p,Vector3 s){Undo.RecordObject(t,"Expanded side band");t.position=p;t.localScale=s;}
        static void Windows(Transform root){
            var group=Group(root,"F - Night windows simplified");var dark=Mat("Window_Frame",new Color(.045f,.08f,.105f));
            for(int side=-1;side<=1;side+=2)for(int i=0;i<3;i++){
                float z=-4.2f+i*5.2f;Box(group,"Opaque night glazing",new Vector3(side*9.85f,2.25f,z),new Vector3(.025f,2.05f,4.15f),window);
                foreach(float dz in new[]{-2.1f,0,2.1f})Box(group,"Window mullion",new Vector3(side*9.79f,2.25f,z+dz),new Vector3(.08f,2.16f,.075f),dark);
                foreach(float y in new[]{1.18f,3.32f})Box(group,"Window transom",new Vector3(side*9.79f,y,z),new Vector3(.08f,.075f,4.28f),dark);
                for(int n=0;n<5;n++){float h=.44f+.15f*((n+i)%4);Box(group,"City silhouette - geometry",new Vector3(side*9.82f,1.25f+h/2,z-1.65f+n*.76f),new Vector3(.012f,h,.53f),sky);}
            }
        }
        static void Background(Transform root){
            var g=Group(root,"F - Supervision depth");Box(g,"Supervisor side divider",new Vector3(4.8f,.55f,8.7f),new Vector3(.10f,1.1f,2.5f),fabric,true);
            Box(g,"Supervisor front partition",new Vector3(7.7f,.55f,7.6f),new Vector3(3.2f,1.1f,.10f),fabric,true);
            foreach(var p in new[]{new Vector3(4.8f,1.55f,7.6f),new Vector3(6.1f,1.55f,7.6f),new Vector3(9.3f,1.55f,7.6f),new Vector3(9.3f,1.55f,9.6f)})Box(g,"Supervision upright",p,new Vector3(.08f,3.1f,.08f),metal,true);
            Box(g,"Supervision lintel",new Vector3(7.05f,3.08f,7.6f),new Vector3(4.6f,.12f,.12f),metal);Box(g,"Supervision canopy",new Vector3(7.05f,3.15f,8.65f),new Vector3(4.6f,.10f,2.2f),metal);
            Place("Kenney_SideTable",g,"Supervisor reference table",new Vector3(7.2f,0,8.8f));Place("Kenney_Archive",g,"Supervisor files",new Vector3(8.7f,0,9.15f));Place("Kenney_Books",g,"Supervisor books",new Vector3(7.2f,.685f,8.8f));
            Place("Kenney_Plant",g,"Supervision plant",new Vector3(5.5f,0,9.2f));
        }
        static void Console(Transform root){
            var station=All<ModeStationVisual>().Single(x=>x.gameObject.activeInHierarchy);var face=Group(root,"A - Inclined mode console face");face.SetPositionAndRotation(new Vector3(0,1.12f,-2.1f),Quaternion.Euler(25,0,0));
            var frame=GameObject.Find("Integrated portal screen").transform;Undo.SetTransformParent(frame,face,"Inclined console frame");Undo.RecordObject(frame,"Inclined console frame");frame.localPosition=Vector3.zero;frame.localRotation=Quaternion.identity;frame.localScale=new Vector3(2.85f,1.22f,.16f);
            var names=new[]{"Attack card","Defense card"};var actions=new[]{station.Attack,station.Defense};var lamps=new[]{station.AttackLamp,station.DefenseLamp};
            for(int i=0;i<2;i++){
                var t=actions[i].transform;Undo.SetTransformParent(t,face,"Preserve mode action");Undo.RecordObject(t,"Tilt mode target");t.localPosition=new Vector3(i==0?-.71f:.71f,0,-.07f);t.localRotation=Quaternion.identity;t.localScale=Vector3.one;
                var card=GameObject.Find(names[i]).transform;Undo.SetTransformParent(card,face,"Tilt native mode UI");Undo.RecordObject(card,"Tilt native mode UI");card.localPosition=new Vector3(i==0?-.71f:.71f,0,-.095f);card.localRotation=Quaternion.identity;card.localScale=Vector3.one*.001f;
                var lamp=lamps[i].transform;Undo.SetTransformParent(lamp,face,"Mode status lighting");Undo.RecordObject(lamp,"Mode status lighting");lamp.localPosition=new Vector3(i==0?-.71f:.71f,-.43f,-.118f);lamp.localRotation=Quaternion.identity;
            }
            var header=GameObject.Find("Mode console header").transform;Undo.SetTransformParent(header,face,"Console header");Undo.RecordObject(header,"Console header");header.localPosition=new Vector3(0,.54f,-.095f);header.localRotation=Quaternion.identity;header.localScale=Vector3.one*.001f;
            var vertices=new Vector3[8];int k=0;foreach(var xy in new[]{new Vector2(-1.43f,-.62f),new Vector2(1.43f,-.62f),new Vector2(1.43f,.62f),new Vector2(-1.43f,.62f)}){var p=face.TransformPoint(new Vector3(xy.x,xy.y,.10f));vertices[k]=p;vertices[k+4]=new Vector3(p.x,.12f,p.z);k++;}
            var mesh=new Mesh{name="Mode console solid wedge"};mesh.vertices=vertices;mesh.triangles=new[]{0,2,1,0,3,2,4,5,6,4,6,7,0,1,5,0,5,4,1,2,6,1,6,5,2,3,7,2,7,6,3,0,4,3,4,7};mesh.RecalculateNormals();mesh.RecalculateBounds();Unwrapping.GenerateSecondaryUVSet(mesh);AssetDatabase.CreateAsset(mesh,Out+"/Geometry/ModeConsole_Wedge.asset");
            var body=Group(root,"Solid mode console carcass").gameObject;body.AddComponent<MeshFilter>().sharedMesh=mesh;body.AddComponent<MeshRenderer>().sharedMaterial=navy;body.AddComponent<MeshCollider>().sharedMesh=mesh;GameObjectUtility.SetStaticEditorFlags(body,StaticEditorFlags.ContributeGI|StaticEditorFlags.BatchingStatic);
            var plinth=GameObject.Find("Console plinth");Undo.RecordObject(plinth,"Preserve previous plinth");plinth.SetActive(false);Resize("Console base",new Vector3(0,.06f,-2.1f),new Vector3(2.95f,.12f,1.2f));
            Box(root,"Console amber apron",new Vector3(0,.145f,-2.57f),new Vector3(2.62f,.025f,.025f),AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/EnvironmentRedesign/Materials/Signal_Amber.mat"));
        }
        static void Lighting(Transform root){
            var ls=All<Light>().Where(x=>x.gameObject.activeInHierarchy&&x.name.StartsWith("Baked ceiling light")).OrderBy(x=>x.name).ToArray();
            var points=new[]{new Vector3(-6.7f,3.55f,-3.8f),new Vector3(-6.7f,3.55f,1.6f),new Vector3(-6.7f,3.55f,6.9f),new Vector3(6.7f,3.55f,-3.8f),new Vector3(6.7f,3.55f,1.6f),new Vector3(6.7f,3.55f,6.9f)};
            for(int i=0;i<ls.Length;i++){Undo.RecordObject(ls[i],"Expanded baked coverage");Move(ls[i].transform,points[i]);ls[i].range=8;ls[i].intensity=12;ls[i].spotAngle=115;ls[i].color=new Color(1,.91f,.79f);Move(GameObject.Find("Ceiling fixture "+(i+1)).transform,points[i]+Vector3.up*.12f);Move(GameObject.Find("Neutral diffuser "+(i+1)).transform,points[i]+Vector3.up*.07f);}
            var rear=All<Light>().Where(x=>x.gameObject.activeInHierarchy&&x.name.StartsWith("Rear baked ceiling")).OrderBy(x=>x.name).ToArray();
            for(int i=0;i<rear.Length;i++){Move(rear[i].transform,new Vector3(i==0?-2.5f:2.5f,3.55f,8.3f));Undo.RecordObject(rear[i],"Rear coverage");rear[i].range=7;}
            var oldFixtures=All<Transform>().Where(x=>x.gameObject.activeInHierarchy&&(x.name=="Rear ceiling fixture"||x.name=="Rear neutral diffuser")).ToArray();foreach(var t in oldFixtures){Move(t,new Vector3(t.position.x, t.name=="Rear ceiling fixture"?3.67f:3.62f,8.3f));}
            var fill=All<Light>().Single(x=>x.name=="Baked inspection soft fill");Move(fill.transform,new Vector3(0,3.55f,2));Undo.RecordObject(fill,"Warm analyst hierarchy");fill.color=new Color(1,.78f,.55f);fill.intensity=10;fill.range=6;
            Move(GameObject.Find("Inspection ceiling fixture").transform,new Vector3(0,3.67f,2));Move(GameObject.Find("Inspection neutral diffuser").transform,new Vector3(0,3.62f,2));
            for(int i=0;i<2;i++){var p=new Vector3(i==0?-2.4f:2.4f,3.55f,-6.3f);var g=Group(root,"Reception baked luminaire "+i);g.SetPositionAndRotation(p,Quaternion.Euler(90,0,0));var l=g.gameObject.AddComponent<Light>();l.type=LightType.Spot;l.lightmapBakeType=LightmapBakeType.Baked;l.range=7;l.spotAngle=115;l.intensity=10;l.color=new Color(1,.90f,.77f);Box(root,"Reception fixture",p+Vector3.up*.12f,new Vector3(1.3f,.08f,.55f),metal);Box(root,"Reception diffuser",p+Vector3.up*.07f,new Vector3(1.18f,.025f,.43f),lightStrip);}
        }
        static void Relight(){
            Checkpoint("BeforeLightingRefinement");
            var probes=All<LightProbeGroup>().Single();Undo.RecordObject(probes,"Expanded room probe coverage");
            probes.probePositions=(from x in new[]{-8f,-4f,0f,4f,8f} from z in new[]{-6.5f,-3f,1f,5f,8.5f} from y in new[]{.45f,1.6f,3.2f} select probes.transform.InverseTransformPoint(new Vector3(x,y,z))).ToArray();
            foreach(var l in All<Light>().Where(x=>x.gameObject.activeInHierarchy&&x.enabled)){
                Undo.RecordObject(l,"Readable night office lighting");
                if(l.lightmapBakeType==LightmapBakeType.Baked){
                    if(l.name.StartsWith("Baked ceiling light"))l.intensity=18;
                    else if(l.name=="Baked inspection soft fill")l.intensity=16;
                    else if(l.name=="Baked analyst task light")l.intensity=3;
                    else l.intensity=Mathf.Max(l.intensity,14);
                }else if(l.type==LightType.Directional)l.intensity=.65f;
            }
            RenderSettings.ambientSkyColor=new Color(.30f,.33f,.38f);RenderSettings.ambientEquatorColor=new Color(.27f,.29f,.32f);RenderSettings.ambientGroundColor=new Color(.17f,.18f,.20f);
            File.WriteAllLines(Dir+"/lighting-final.txt",All<Light>().Select(l=>l.name+" "+l.type+" intensity="+l.intensity+" rotation="+l.transform.eulerAngles+" bake="+l.lightmapBakeType).Concat(new[]{"Probe positions="+probes.probePositions.Length}));
            Save();
        }
        static void NewPad(Transform root,string name,Vector3 point,string label){var m=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/Materials/Interactive_Teal.mat");var g=Box(root,name,point+Vector3.up*.025f,new Vector3(.65f,.05f,.65f),m,true);var t=g.AddComponent<BankInteractable>();t.Manager=game;t.Action=BankAction.Teleport;t.Player=All<VRInteractionManager>().Single(x=>x.gameObject.activeInHierarchy).View.transform.parent;t.Destination=point;var s=Surface(root,name+" label",point+Vector3.up*.06f,new Vector2(.62f,.28f));s.rotation=Quaternion.Euler(90,0,0);Label(s,"Route",label+"\nMIRA 1,5 s",new Vector2(590,250),54);}
        static void Workstation(){
            Check(GameObject.Find("ExpandedBankOfficeV2")!=null,"Architecture expansion present");Checkpoint("BeforeWorkstation");Materials();var root=GameObject.Find("ExpandedBankOfficeV2").transform;var work=GameObject.Find("03_AnalystWorkstation").transform;
            Check(GameObject.Find("B - Horizontal workstation complete")==null,"No duplicate workstation update");Group(root,"B - Horizontal workstation complete");
            foreach(string n in new[]{"Main analyst desk","Interactive Computer","Fictional transfer documents","Stapler"})Shift(n,Vector3.forward*.2f);Move(work,work.position+Vector3.forward*.2f);
            var frame=GameObject.Find("Main monitor frame").transform;Undo.RecordObject(frame,"Horizontal primary monitor");frame.localScale=new Vector3(2.22f,1.32f,.055f);
            var reference=GameObject.Find("Secondary format monitor").transform;Undo.RecordObject(reference,"Reference display on right");reference.SetPositionAndRotation(new Vector3(1.57f,reference.position.y,reference.position.z),Quaternion.Euler(0,48,0));
            var surface=reference.Find("Format reference UI");Undo.RecordObject(surface,"Reference plane aligned");surface.SetPositionAndRotation(reference.position,reference.rotation);
            var refFrame=reference.Find("Secondary monitor shell");Undo.RecordObject(refFrame,"Reference shell aligned");refFrame.SetPositionAndRotation(reference.position+reference.forward*.04f,reference.rotation);
            foreach(string n in new[]{"Secondary display stand","Reference side worktop","Reference worktop leg"}){var t=GameObject.Find(n).transform;Move(t,new Vector3(-t.position.x,t.position.y,t.position.z));}
            var canvas=ui.PanelAnchor.GetComponentInChildren<Canvas>();Undo.RecordObject(canvas.GetComponent<RectTransform>(),"Horizontal UI canvas");canvas.GetComponent<RectTransform>().sizeDelta=new Vector2(1400,800);
            var backdrop=canvas.transform.Find("Fondo de lectura");if(backdrop!=null){Undo.RecordObject(backdrop.GetComponent<RectTransform>(),"Monitor backdrop");backdrop.GetComponent<RectTransform>().sizeDelta=new Vector2(1400,800);}
            var task=Group(root,"Baked analyst task light");task.SetPositionAndRotation(new Vector3(1.0f,1.2f,2.9f),Quaternion.Euler(68,-35,0));var lamp=task.gameObject.AddComponent<Light>();lamp.type=LightType.Spot;lamp.lightmapBakeType=LightmapBakeType.Baked;lamp.range=2.4f;lamp.intensity=1.4f;lamp.spotAngle=85;lamp.color=new Color(1,.76f,.46f);
            Save();
        }
        static void Validate(){
            BankNavigationValidation.Check(All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy));Check(All<MonoBehaviour>().All(x=>x!=null),"No missing scripts");Check(AssetDatabase.GetDependencies(ScenePath,true).Where(x=>x.StartsWith("Assets/")).All(File.Exists),"All native dependencies exist");
            Check(All<Camera>().Count(x=>x.gameObject.activeInHierarchy)==1,"One persisted player camera");Check(LightmapSettings.lightmaps.Length>0&&LightmapSettings.lightmaps.All(x=>x.lightmapColor!=null),"Baked lighting available");
            var floor=GameObject.Find("Floor").GetComponent<Renderer>().bounds;Check(Mathf.Abs(floor.size.x-20)<.01f&&Mathf.Abs(floor.size.z-18)<.01f,"Actual floor is 20 by 18 metres");
            var b=JsonUtility.FromJson<Metrics>(File.ReadAllText(Dir+"/Before-metrics.json"));Check(floor.size.x*floor.size.z>b.grossArea,"Architectural footprint grew beyond baseline");File.WriteAllLines(Dir+"/dependencies-final.txt",AssetDatabase.GetDependencies(ScenePath,true));
        }
        static void CaptureAfter(){
            Capture("02-planta-despues.png",new Vector3(0,21,1),new Vector3(0,0,1),60,11,true);
            Capture("03b-entrada-misma-camara.png",new Vector3(0,1.75f,-4.4f),new Vector3(0,1.5f,2),60);
            Capture("03-entrada-ampliada.png",new Vector3(0,1.75f,-7.1f),new Vector3(0,1.5f,2.4f),60);
            Capture("04-cubiculos.png",new Vector3(-2.0f,1.75f,-4.6f),new Vector3(-5.5f,1.2f,3.1f),60);
            Capture("06-fondo-hacia-entrada.png",new Vector3(0,1.75f,9.1f),new Vector3(0,1.25f,-2),60);
            var station=All<ModeStationVisual>().Single(x=>x.gameObject.activeInHierarchy);Capture("07-consola-modos.png",new Vector3(0,1.6f,-3.6f),(station.Attack.transform.position+station.Defense.transform.position)*.5f,60);
            Capture("09-iluminacion-general.png",new Vector3(8.8f,2.2f,-6.9f),new Vector3(0,1.3f,3.6f),60);
        }
        static bool Reaches(BankInteractable t,Vector3 eye){Physics.SyncTransforms();foreach(var c in t.GetComponentsInChildren<Collider>()){if(!c.enabled)continue;var delta=c.bounds.center-eye;if(Physics.Raycast(eye,delta.normalized,out var hit,delta.magnitude+.04f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<BankInteractable>()==t)return true;}return false;}
        static IEnumerator PlayCaptures(){
            yield return null;yield return null;ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();game=UnityEngine.Object.FindFirstObjectByType<GameManager>();game.StartScenario(ScenarioMode.Defense);yield return null;
            var seat=AnalystSeat.Current;var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var eye=gaze.View;var rig=eye.transform.parent;rig.position=new Vector3(0,0,-.65f);eye.transform.localRotation=Quaternion.identity;
            Capture("05-puesto-principal-de-pie.png",eye.transform.position,ui.PanelAnchor.position,60);
            Check(seat.Request(true),"Seat request accepted after enlargement");while(seat.IsTransitioning)yield return null;Check(seat.IsSeated,"Seat transition completed");
            foreach(var a in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return})Check(All<BankInteractable>().Any(x=>x.gameObject.activeInHierarchy&&x.Action==a&&Reaches(x,eye.transform.position)),"Seated bank raycast "+a);
            Check(Reaches(All<BankInteractable>().Single(x=>x.Action==BankAction.FormatReference),eye.transform.position),"Right consultation monitor is raycast reachable");Check(Reaches(seat.StandTarget,eye.transform.position),"Stand marker remains visible");
            Capture("08-escritorio-sentado.png",eye.transform.position,new Vector3(.35f,1.42f,1.75f),60);
            Capture("10-monitor-principal.png",eye.transform.position,ui.PanelAnchor.position,60);
            Capture("11-consulta-derecha.png",eye.transform.position,UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>().transform.position,60);
            var notebook=All<BankInteractable>().Single(x=>x.Action==BankAction.Notebook).transform.position;
            Capture("12-libreta-y-materiales.png",notebook+new Vector3(-.35f,.85f,-.25f),notebook,60);
            Capture("13-levantarse.png",eye.transform.position,seat.StandTarget.transform.position,60);
            Check(Mathf.Abs(ui.PanelAnchor.GetComponentInChildren<Canvas>().GetComponent<RectTransform>().sizeDelta.x-1400)<.1f,"Horizontal UI width is 1400; height and font sizes retained");
            Check(seat.Request(false),"Standing request accepted");while(seat.IsTransitioning)yield return null;Check(!seat.IsSeated,"Standing restored safely");
            Check(errors==0,"No runtime capture errors");
        }
        static void Capture(string file,Vector3 eye,Vector3 look,float fov=60,float ortho=0,bool hideCeiling=false){
            Directory.CreateDirectory(Gallery);Canvas.ForceUpdateCanvases();Physics.SyncTransforms();var rs=All<Renderer>().Where(x=>x.name=="Ceiling").ToArray();var oldVisibility=rs.Select(x=>x.forceRenderingOff).ToArray();var active=RenderTexture.active;
            var g=new GameObject("Expanded office temporary review camera"){hideFlags=HideFlags.HideAndDontSave};RenderTexture rt=null;Texture2D image=null;
            try{var c=g.AddComponent<Camera>();c.enabled=false;c.stereoTargetEye=StereoTargetEyeMask.None;c.cameraType=CameraType.Game;c.nearClipPlane=.05f;c.farClipPlane=60;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(.08f,.1f,.12f);c.aspect=1600f/900;c.fieldOfView=fov;c.orthographic=ortho>0;c.orthographicSize=ortho;c.transform.position=eye;var direction=look-eye;c.transform.rotation=Quaternion.LookRotation(direction,Mathf.Abs(Vector3.Dot(direction.normalized,Vector3.up))>.99f?Vector3.forward:Vector3.up);
                var data=c.GetUniversalAdditionalCameraData();data.renderPostProcessing=false;data.renderShadows=true;data.antialiasing=AntialiasingMode.None;rt=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32){antiAliasing=1};Check(rt.Create(),"Capture texture allocated");if(hideCeiling)foreach(var r in rs)r.forceRenderingOff=true;
                var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};Check(RenderPipeline.SupportsRenderRequest(c,request),"Existing URP supports render request");RenderPipeline.SubmitRenderRequest(c,request);RenderTexture.active=rt;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(Gallery+"/"+file,image.EncodeToPNG());
                File.AppendAllText(Gallery+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+file+" eye="+eye+" look="+look+" temporaryFOV="+fov+" ortho="+ortho+" Play="+EditorApplication.isPlaying+" 1600x900 NOT_ANDROID\n");
            }finally{for(int i=0;i<rs.Length;i++)if(rs[i]!=null)rs[i].forceRenderingOff=oldVisibility[i];RenderTexture.active=active;if(image!=null)UnityEngine.Object.DestroyImmediate(image);if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}UnityEngine.Object.DestroyImmediate(g);}
        }
    }
}

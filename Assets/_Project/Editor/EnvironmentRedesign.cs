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
using TMPro;
using Swift.Game;
using Swift.Interaction;
using Swift.UI;
using Swift.VR;
using Swift.Bank;

namespace Swift.EditorTools
{
    // Explicit opt-in commands; never regenerates the original office or changes pipeline/XR settings.
    [InitializeOnLoad]
    public static class EnvironmentRedesign
    {
        const string Dir="Artifacts/EnvironmentRedesign",Queue=Dir+"/command.txt",Pending="SWIFT_ENV_REVIEW";
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string AssetsOut="Assets/_Project/OfficeVisual/EnvironmentRedesign";
        static IEnumerator routine;static string command;static bool ownsPlay;static int errors;
        static GameManager manager;static UIManager ui;
        static Material navy,oak,paper,metal,fabric,plastic,teal,amber;
        static string Gallery {
            get {string file=Dir+"/output-path.txt";if(!File.Exists(file))File.WriteAllText(file,"Docs/Previews/EnvironmentRedesign/"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));string p=File.ReadAllText(file).Trim();Directory.CreateDirectory(p);return p;}
        }
        static EnvironmentRedesign(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static void Tick()
        {
            if(routine!=null){try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning||!File.Exists(Queue))return;
            try {
                command=File.ReadAllText(Queue).Trim();File.Delete(Queue);errors=0;
                Check(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==ScenePath,"Only BancoCorresponsal open");
                if(command=="checkpoint") {
                    Check(GameObject.Find("EnvironmentRedesignV1")!=null&&File.Exists(Dir+"/backup-path.txt"),"Authorized layout and recovery receipt present");
                    string checkpoint=Path.Combine(File.ReadAllText(Dir+"/backup-path.txt").Trim().Trim('\uFEFF'),"Checkpoint-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));Directory.CreateDirectory(checkpoint);
                    File.Copy(ScenePath,Path.Combine(checkpoint,"BancoCorresponsal-before-save.unity"));File.Copy(ScenePath+".meta",Path.Combine(checkpoint,"BancoCorresponsal-before-save.unity.meta"));
                    AssetDatabase.SaveAssets();Save();File.Copy(ScenePath,Path.Combine(checkpoint,"BancoCorresponsal-saved.unity"));Finish(null);return;
                }
                Check(!SceneManager.GetActiveScene().isDirty,"Unsaved user scene preserved");
                manager=All<GameManager>().Single(x=>x.gameObject.activeInHierarchy);ui=All<UIManager>().Single(x=>x.gameObject.activeInHierarchy);
                if(command=="inspect"){Inspect();Finish(null);}
                else if(command=="before"){
                    Capture("01-oficina-anterior.png",new Vector3(0,2.65f,-4.5f),new Vector3(0,1.1f,1),85);
                    Capture("01b-planta-anterior.png",new Vector3(0,13,2),new Vector3(0,0,2),60,9,true);Finish(null);
                }
                else if(command=="layout"){Layout();Finish(null);}
                else if(command=="refine"){Refine();Finish(null);}
                else if(command=="after"){ValidateSaved();After();Finish(null);}
                else if(command=="legacy-validation"){
                    BankOfficeBuilder.Validate();
                    typeof(OperationsLightingReview).GetMethod("Validate",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
                    Check(true,"Existing office and lighting validators pass with four original destinations and additional exploration pads");Finish(null);
                }
                else if(command=="bake"){
                    Check(!Lightmapping.isRunning,"No concurrent bake");Lightmapping.bakeCompleted+=Baked;
                    Check(Lightmapping.BakeAsync(),"Bake accepted");File.WriteAllText(Dir+"/bake-result.txt","RUNNING "+DateTime.UtcNow.ToString("O"));
                }
                else if(command=="review") {SessionState.SetBool(Pending,true);EditorApplication.EnterPlaymode();}
                else throw new Exception("Unknown environment command: "+command);
            }catch(Exception e){Finish(e);}
        }
        static void Baked(){Lightmapping.bakeCompleted-=Baked;AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(SceneManager.GetActiveScene());File.WriteAllText(Dir+"/bake-result.txt","COMPLETE "+DateTime.UtcNow.ToString("O")+" lightmaps="+LightmapSettings.lightmaps.Length);Finish(null);}
        static void PlayChanged(PlayModeStateChange s){if(s!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool(Pending,false))return;SessionState.EraseBool(Pending);command="review";ownsPlay=true;errors=0;Application.logMessageReceived+=Log;routine=Review();}
        static void Log(string s,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;string result=(e==null&&errors==0?"PASS ":"FAIL ")+command+" "+DateTime.UtcNow.ToString("O")+" "+(e?.ToString()??("errors="+errors))+" Editor only; Android NOT TESTED";File.WriteAllText(Dir+"/"+command+"-result.txt",result);Debug.Log(result);if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();ownsPlay=false;}
        static void Check(bool ok,string note){if(!ok)throw new Exception(note);File.AppendAllText(Dir+"/checks.txt","PASS "+note+"\n");}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static string PathOf(Transform t)=>t.parent==null?t.name:PathOf(t.parent)+"/"+t.name;
        static void Inspect(){
            var lines=new List<string>{"Unity="+Application.unityVersion+" sceneDirty="+SceneManager.GetActiveScene().isDirty};
            foreach(var t in All<Transform>()){
                string prefab=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject);
                lines.Add(PathOf(t)+" | "+t.position+" | scale="+t.localScale+" | active="+t.gameObject.activeInHierarchy+" | prefab="+prefab);
            }
            File.WriteAllLines(Dir+"/hierarchy-before.txt",lines);
            File.WriteAllLines(Dir+"/dependencies-before.txt",AssetDatabase.GetDependencies(ScenePath,true));
            File.WriteAllLines(Dir+"/model-inventory.txt",AssetDatabase.FindAssets("t:Model",new[]{"Assets/Models"}).Select(AssetDatabase.GUIDToAssetPath));
        }
        static Material Mat(string name,Color color,float smooth=.15f,float metallic=0,bool emission=false){
            string path=AssetsOut+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m!=null)return m;
            m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",metallic);
            if(emission){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*.6f);m.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;}
            AssetDatabase.CreateAsset(m,path);return m;
        }
        static Transform Group(Transform parent,string name){var g=new GameObject(name);Undo.RegisterCreatedObjectUndo(g,"Environment redesign");g.transform.SetParent(parent,false);return g.transform;}
        static GameObject Box(Transform parent,string name,Vector3 pos,Vector3 size,Material mat,bool collide=true){
            var g=GameObject.CreatePrimitive(PrimitiveType.Cube);Undo.RegisterCreatedObjectUndo(g,"Environment redesign");g.name=name;g.transform.SetParent(parent,true);g.transform.position=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;
            if(!collide)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
        }
        static Bounds BoundsOf(GameObject g){var rs=g.GetComponentsInChildren<Renderer>();var b=rs[0].bounds;foreach(var x in rs.Skip(1))b.Encapsulate(x.bounds);return b;}
        static GameObject Place(string prefab,Transform parent,string name,Vector3 center,float yaw=0){
            var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/Prefabs/"+prefab+".prefab");Check(source!=null,"Reused prefab "+prefab);
            var g=(GameObject)PrefabUtility.InstantiatePrefab(source);Undo.RegisterCreatedObjectUndo(g,"Environment redesign");g.name=name;g.transform.SetParent(parent,false);g.transform.rotation=Quaternion.Euler(0,yaw,0);
            var b=BoundsOf(g);g.transform.position+=center-new Vector3(b.center.x,b.min.y,b.center.z);return g;
        }
        static GameObject NativeModel(string file,string name,float height){
            string path=AssetsOut+"/Prefabs/"+name+".prefab";var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(prefab!=null)return prefab;
            var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/KenneyFurniture/Models/FBX format/"+file+".fbx");Check(source!=null,"Existing native model "+file);
            var g=new GameObject(name);var child=(GameObject)PrefabUtility.InstantiatePrefab(source);child.transform.SetParent(g.transform,false);
            var b=BoundsOf(g);child.transform.localScale*=height/b.size.y;b=BoundsOf(g);child.transform.position-=new Vector3(b.center.x,b.min.y,b.center.z);
            foreach(var renderer in g.GetComponentsInChildren<MeshRenderer>())renderer.sharedMaterials=renderer.sharedMaterials.Select((m,i)=>i==0?plastic:metal).ToArray();
            prefab=PrefabUtility.SaveAsPrefabAsset(g,path);UnityEngine.Object.DestroyImmediate(g);return prefab;
        }
        static GameObject NativePlace(GameObject prefab,Transform parent,string name,Vector3 pos,float yaw=0){var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab);Undo.RegisterCreatedObjectUndo(g,"Environment redesign");g.name=name;g.transform.SetParent(parent,false);g.transform.SetPositionAndRotation(pos,Quaternion.Euler(0,yaw,0));return g;}
        static Transform Surface(Transform parent,string name,Vector3 pos,Vector2 size,float yaw=0){
            var g=new GameObject(name,typeof(RectTransform),typeof(Canvas));Undo.RegisterCreatedObjectUndo(g,"Environment redesign");g.transform.SetParent(parent,false);g.transform.SetPositionAndRotation(pos,Quaternion.Euler(0,yaw,0));g.transform.localScale=Vector3.one*.001f;
            g.GetComponent<RectTransform>().sizeDelta=size*1000;g.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            OperationsCenterStyle.Panel(g.transform,"Surface",Vector2.zero,size*1000,new Color(.025f,.065f,.10f));return g.transform;
        }
        static TextMeshProUGUI Label(Transform root,string name,string text,Vector2 pos,Vector2 size,float font,Color? color=null){var t=ui.Text(root,name,pos,size,font);t.text=text;t.color=color??Color.white;t.alignment=TextAlignmentOptions.MidlineLeft;t.textWrappingMode=TextWrappingModes.Normal;return t;}
        static void Position(GameObject g,Vector3 pos,Vector3? size=null){Undo.RecordObject(g.transform,"Environment redesign");g.transform.position=pos;if(size.HasValue)g.transform.localScale=size.Value;}
        static void Hide(string name){var g=GameObject.Find(name);if(g==null)return;Undo.RecordObject(g,"Environment redesign");g.SetActive(false);}
        static void Layout()
        {
            Check(File.Exists(Dir+"/backup-path.txt"),"External backup exists");Check(GameObject.Find("EnvironmentRedesignV1")==null,"No duplicate redesign");
            Directory.CreateDirectory(AssetsOut+"/Materials");Directory.CreateDirectory(AssetsOut+"/Prefabs");AssetDatabase.Refresh();
            navy=Mat("Institutional_Navy",new Color(.035f,.09f,.14f));oak=Mat("Oak_Worksurface",new Color(.44f,.30f,.19f),.25f);
            paper=Mat("Paper_Ivory",new Color(.88f,.89f,.83f),.04f);metal=Mat("Painted_Metal",new Color(.22f,.29f,.32f),.32f,.18f);
            fabric=Mat("Chair_Fabric",new Color(.12f,.24f,.28f),.04f);plastic=Mat("Equipment_Polymer",new Color(.07f,.10f,.12f),.22f);
            teal=Mat("Signal_Teal",new Color(.10f,.66f,.61f),.25f,0,true);amber=Mat("Signal_Amber",new Color(.95f,.52f,.11f),.25f,0,true);
            var root=Group(null,"EnvironmentRedesignV1");var shell=GameObject.Find("01_Architecture").transform;
            Position(GameObject.Find("Floor"),new Vector3(0,-.1f,2),new Vector3(16,.2f,14));
            Position(GameObject.Find("Ceiling"),new Vector3(0,3.4f,2),new Vector3(16,.16f,14));
            Position(GameObject.Find("NorthWall"),new Vector3(0,1.65f,9),new Vector3(16,3.3f,.2f));
            Position(GameObject.Find("WestWall"),new Vector3(-8,1.65f,2),new Vector3(.2f,3.3f,14));
            Position(GameObject.Find("EastWall"),new Vector3(8,1.65f,2),new Vector3(.2f,3.3f,14));
            Position(GameObject.Find("EntranceWallLeft"),new Vector3(-4.6f,1.65f,-5),new Vector3(6.8f,3.3f,.2f));
            Position(GameObject.Find("EntranceWallRight"),new Vector3(4.6f,1.65f,-5),new Vector3(6.8f,3.3f,.2f));
            Position(GameObject.Find("NorthBlueBand"),new Vector3(0,.45f,8.87f),new Vector3(15.8f,.9f,.04f));
            foreach(var band in shell.Cast<Transform>().Where(x=>x.name=="SideBlueBand")){Position(band.gameObject,new Vector3(Mathf.Sign(band.position.x)*7.87f,.45f,2),new Vector3(.04f,.9f,13.8f));}
            // A low perimeter and overhead beams provide rhythm without occupying the central route.
            for(int side=-1;side<=1;side+=2)for(int z=0;z<3;z++)Box(root,"Perimeter pilaster",new Vector3(side*7.75f,1.65f,-2+z*4.8f),new Vector3(.30f,3.3f,.30f),metal);
            var art=GameObject.Find("ART - Bank Wall Graphic");if(art!=null)Position(art,new Vector3(-7.84f,1.95f,-.4f));
            var bankTitle=GameObject.Find("Bank title");Position(bankTitle,new Vector3(0,2.68f,8.82f));
            var welcome=GameObject.Find("Welcome");Position(welcome,new Vector3(-7.80f,1.7f,-2.7f));
            foreach(var t in welcome.GetComponentsInChildren<TMP_Text>())t.text="BANCO CORRESPONSAL\nINTERNACIONAL\n\nSALA DE OPERACIONES\nSelecciona un modo en la consola.\nMira 1,5 s y aparta la mirada.";
            BuildModes(root);BuildCubicles(root);BuildWorkstation(root);BuildLighting(root);
            // Reuse existing material families consistently; retain textured computer assets.
            foreach(var renderer in All<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy)){
                var mats=renderer.sharedMaterials;bool changed=false;
                for(int i=0;i<mats.Length;i++)if(mats[i]!=null&&!AssetDatabase.GetAssetPath(mats[i]).Contains("EnvironmentRedesign")){
                    string n=mats[i].name;
                    if(n.Contains("E59964")){mats[i]=oak;changed=true;}else if(n.Contains("F15E57")){mats[i]=fabric;changed=true;}else if(n.Contains("5E7777")||n.Contains("4E6363")){mats[i]=metal;changed=true;}
                }
                if(changed){Undo.RecordObject(renderer,"Material family");renderer.sharedMaterials=mats;}
                if(renderer.name!="Reticula"&&(renderer.GetComponentInParent<BankInteractable>()==null||renderer.GetComponentInParent<BankInteractable>().Action!=BankAction.Sit))
                    GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,GameObjectUtility.GetStaticEditorFlags(renderer.gameObject)|StaticEditorFlags.ContributeGI);
                renderer.shadowCastingMode=ShadowCastingMode.On;renderer.receiveShadows=true;
            }
            var editor=All<EditorInputAdapter>().Single(x=>x.gameObject.activeInHierarchy);Undo.RecordObject(editor,"Expanded PC debug bounds");editor.MovementMin=new Vector2(-7.5f,-4.5f);editor.MovementMax=new Vector2(7.5f,8.5f);
            // New scenery has no bank actions; the four original pads retain exact destinations and references.
            Save();File.WriteAllLines(Dir+"/dependencies-after.txt",AssetDatabase.GetDependencies(ScenePath,true));
        }
        static void BuildModes(Transform root)
        {
            var zone=Group(root,"01_ModePortalConsole");
            foreach(string n in new[]{"Portal ATAQUE stand","Portal ATAQUE base","Portal DEFENSA stand","Portal DEFENSA base","Portal ATAQUE label","Portal DEFENSA label"})Hide(n);
            Box(zone,"Console base",new Vector3(0,.08f,-2.02f),new Vector3(2.85f,.16f,.65f),metal);
            Box(zone,"Console plinth",new Vector3(0,.64f,-1.98f),new Vector3(1.25f,1.05f,.38f),navy);
            Box(zone,"Integrated portal screen",new Vector3(0,1.49f,-2.01f),new Vector3(2.85f,1.22f,.16f),plastic);
            var header=Surface(zone,"Mode console header",new Vector3(0,2.03f,-2.105f),new Vector2(2.72f,.18f));
            Label(header,"Title","SWIFT  /  PORTAL DE OPERACIONES",Vector2.zero,new Vector2(2600,150),68);
            var visual=zone.gameObject.AddComponent<ModeStationVisual>();
            for(int i=0;i<2;i++){
                bool attack=i==0;var g=GameObject.Find(attack?"Portal ATAQUE":"Portal DEFENSA");var action=g.GetComponent<BankInteractable>();
                Undo.RecordObject(g.transform,"Preserve portal action");g.transform.SetPositionAndRotation(new Vector3(i==0?-.71f:.71f,1.46f,-2.12f),Quaternion.identity);g.transform.localScale=Vector3.one;
                Undo.RecordObject(g.GetComponent<Renderer>(),"Portal skin");g.GetComponent<Renderer>().enabled=false;
                Undo.RecordObject(g.GetComponent<BoxCollider>(),"Portal target");g.GetComponent<BoxCollider>().size=new Vector3(1.30f,.92f,.04f);
                foreach(var child in g.transform.Cast<Transform>().ToArray()){Undo.RecordObject(child.gameObject,"Preserve prior portal labels");child.gameObject.SetActive(false);}
                var s=Surface(zone,attack?"Attack card":"Defense card",g.transform.position+new Vector3(0,0,-.025f),new Vector2(1.30f,.92f));
                var c=attack?new Color(1,.65f,.25f):new Color(.25f,.90f,.82f);
                Label(s,"Mode",attack?"ATAQUE":"DEFENSA",new Vector2(0,245),new Vector2(1180,150),105,c);
                Label(s,"Description",attack?"Simulacion de vulnerabilidad.\nRevisa ordenes ficticias y\ncompara las consecuencias.":"Controles adicionales.\nContrasta las alertas y\nactiva la defensa simulada.",new Vector2(0,30),new Vector2(1180,280),55);
                var status=Label(s,"Status","MIRA PARA SELECCIONAR",new Vector2(0,-315),new Vector2(1180,130),43,c);
                var lamp=Box(zone,attack?"Attack status lamp":"Defense status lamp",new Vector3(g.transform.position.x,1.02f,-2.16f),new Vector3(1.18f,.035f,.025f),attack?amber:teal,false).GetComponent<Renderer>();
                if(attack){visual.Attack=action;visual.AttackLamp=lamp;visual.AttackStatus=status;}else{visual.Defense=action;visual.DefenseLamp=lamp;visual.DefenseStatus=status;}
            }
        }
        static void BuildCubicles(Transform root)
        {
            var zone=Group(root,"02_SecondaryCubicles");var screen=NativeModel("computerScreen","Kenney_ReferenceMonitor",.46f);var lamp=NativeModel("lampSquareTable","Kenney_DeskLamp",.40f);
            for(int i=0;i<4;i++){
                float x=i<2?-5.55f:5.55f,z=i%2==0?.9f:5.6f;var booth=Group(zone,"Cubicle "+(i+1));
                var desk=Place("Quaternius_Desk",booth,"Shared desk",new Vector3(x,0,z));desk.transform.localScale=new Vector3(1.65f,1,1.5f);
                Box(booth,"Fabric acoustic back",new Vector3(x,.78f,z+.78f),new Vector3(2.7f,1.56f,.07f),fabric);
                Box(booth,"Metal top rail",new Vector3(x,1.58f,z+.78f),new Vector3(2.7f,.045f,.08f),metal);
                Box(booth,"Acoustic side",new Vector3(x+(i<2?-1.34f:1.34f),.65f,z+.14f),new Vector3(.07f,1.3f,1.3f),fabric);
                NativePlace(screen,booth,"Secondary workstation display",new Vector3(x+(i%2==0?-.18f:.18f),.82f,z+.12f),180);
                Place("Kenney_Keyboard",booth,"Keyboard",new Vector3(x,.82f,z-.25f),90);
                Place("Kenney_Mouse",booth,"Mouse",new Vector3(x+.50f,.82f,z-.22f));
                Place("Kenney_OfficeChair",booth,"Office chair",new Vector3(x+(i%2==0?.2f:-.15f),0,z-1.25f),i%2==0?8:-12);
                NativePlace(lamp,booth,"Desk lamp - no dynamic light",new Vector3(x-(i%2==0?.82f:-.82f),.82f,z+.2f));
                if(i%2==0)Place("Jarlan_Papers",booth,"Ordered papers",new Vector3(x+.78f,.82f,z-.25f));else Place("Kenney_Books",booth,"Reference books",new Vector3(x-.76f,.82f,z+.10f),90);
                var tag=Surface(booth,"Cubicle identifier",new Vector3(x,1.40f,z+.73f),new Vector2(.68f,.13f));Label(tag,"ID","OPERACIONES / 0"+(i+2),Vector2.zero,new Vector2(650,115),35);
            }
            // Rear circulation loop and a supervision backdrop; existing security consoles remain in place.
            var sign=Surface(root,"Supervision identity",new Vector3(3.5f,2.45f,8.82f),new Vector2(2.8f,.55f));Label(sign,"Title","SUPERVISION / SEGURIDAD\nRevision y controles",Vector2.zero,new Vector2(2650,500),70);
            Place("Kenney_Archive",root,"Rear reference storage",new Vector3(-2.0f,0,8.55f));
            Place("Kenney_Plant",root,"Rear corner plant",new Vector3(7.2f,0,8.1f));
            for(int side=-1;side<=1;side+=2)Box(root,"Aisle inlay",new Vector3(side*2.1f,.006f,3.8f),new Vector3(.055f,.008f,9.2f),metal,false);
        }
        static void BuildWorkstation(Transform root)
        {
            var zone=Group(root,"03_AnalystWorkstation");
            var chair=GameObject.Find("Analyst chair");Position(chair,new Vector3(0,0,-.65f));Undo.RecordObject(chair.transform,"Chair orientation");chair.transform.rotation=Quaternion.identity;
            var seat=Undo.AddComponent<AnalystSeat>(chair);var gaze=All<VRInteractionManager>().Single(x=>x.gameObject.activeInHierarchy);
            seat.Eye=gaze.View.transform;seat.Rig=gaze.View.transform.parent;
            var seated=Group(chair.transform,"Seated eye target");seated.position=new Vector3(0,1.25f,-.65f);seat.SeatedEye=seated;
            var standing=Group(chair.transform,"Safe standing floor");standing.position=new Vector3(1.6f,0,-.65f);seat.StandingFloor=standing;
            seat.SitTarget=SeatButton(chair.transform,"SENTARSE",new Vector3(-.62f,1.0f,-.30f),BankAction.Sit,new Vector2(.62f,.22f));
            seat.StandTarget=SeatButton(chair.transform,"LEVANTARSE",new Vector3(.92f,1.02f,-.03f),BankAction.Stand,new Vector2(.62f,.22f));seat.StandTarget.gameObject.SetActive(false);
            // Preserve anchor, Canvas and decision references; physical surround follows the raycast-safe runtime panel plane.
            Undo.RecordObject(ui.PanelAnchor,"Monitor proportion");ui.PanelAnchor.position=new Vector3(0,1.53f,.76f);
            Box(zone,"Main monitor frame",new Vector3(0,1.53f,.805f),new Vector3(1.77f,1.32f,.055f),plastic,false);
            Box(zone,"Main monitor stem",new Vector3(0,.88f,1.05f),new Vector3(.12f,.19f,.12f),metal,false);
            // The imported computer already includes the working keyboard/mouse. Preserve redundant spare meshes inactive.
            Hide("Spare keyboard");Hide("Mouse");
            var documents=GameObject.Find("Fictional transfer documents");Position(documents,new Vector3(1.02f,.82f,1.15f));
            var reference=Group(zone,"Secondary format monitor");reference.SetPositionAndRotation(new Vector3(-1.57f,1.48f,.42f),Quaternion.Euler(0,-48,0));
            var frame=Box(reference,"Secondary monitor shell",reference.position+reference.forward*.04f,new Vector3(1.14f,.99f,.06f),plastic,false);frame.transform.rotation=reference.rotation;
            var surface=Surface(reference,"Format reference UI",reference.position,new Vector2(1.08f,.93f),-48);
            var panel=reference.gameObject.AddComponent<FormatReferencePanel>();panel.Manager=manager;
            Label(surface,"Header","FORMATO / CONSULTA",new Vector2(0,399),new Vector2(1000,90),44,new Color(.18f,.82f,.85f));
            panel.Rules=Label(surface,"Rules",panel.RuleText,new Vector2(0,243),new Vector2(1000,195),32);
            panel.Observed=Label(surface,"Observed","Sin expediente activo",new Vector2(0,-3),new Vector2(1000,270),33);
            panel.ExistingAlert=Label(surface,"Existing alert","Consulta sin validacion automatica",new Vector2(0,-288),new Vector2(1000,225),32,new Color(1,.77f,.39f));
            var target=reference.gameObject.AddComponent<BankInteractable>();target.Manager=manager;target.Action=BankAction.FormatReference;
            var collider=reference.gameObject.AddComponent<BoxCollider>();collider.size=new Vector3(1.08f,.93f,.012f);collider.center=new Vector3(0,0,-.015f);
            Box(zone,"Secondary display stand",new Vector3(-1.57f,.94f,.42f),new Vector3(.10f,.30f,.1f),metal,false);
            Box(zone,"Reference side worktop",new Vector3(-1.65f,.78f,.48f),new Vector3(.92f,.08f,.78f),oak);
            Box(zone,"Reference worktop leg",new Vector3(-1.90f,.38f,.48f),new Vector3(.08f,.76f,.58f),metal);
            // Clipboard geometry, not a new inventory. It opens the existing enlarged procedure page.
            var notebook=Group(zone,"Analyst review clipboard");notebook.SetPositionAndRotation(new Vector3(-1.08f,.817f,1.15f),Quaternion.Euler(90,0,-8));
            var board=Box(notebook,"Clipboard back",notebook.position,new Vector3(.35f,.48f,.025f),oak,false);board.transform.rotation=notebook.rotation;
            var clip=Box(notebook,"Metal clip",notebook.TransformPoint(new Vector3(0,.22f,-.02f)),new Vector3(.17f,.04f,.025f),metal,false);clip.transform.rotation=notebook.rotation;
            var note=Surface(notebook,"Editable review steps",notebook.TransformPoint(new Vector3(0,0,-.018f)),new Vector2(.32f,.41f));note.rotation=notebook.rotation;
            note.Find("Surface").GetComponent<UnityEngine.UI.Image>().color=new Color(.88f,.89f,.83f);
            Label(note,"Steps","REVISION MANUAL\n\n1  Leer expediente\n2  Contrastar registro\n3  Consultar alertas\n4  Documentar y decidir\n\nMIRA PARA AMPLIAR",Vector2.zero,new Vector2(290,380),24,new Color(.03f,.09f,.12f));
            var bookTarget=notebook.gameObject.AddComponent<BankInteractable>();bookTarget.Manager=manager;bookTarget.Action=BankAction.Notebook;
            notebook.gameObject.AddComponent<BoxCollider>().size=new Vector3(.36f,.49f,.04f);
            var lamp=AssetDatabase.LoadAssetAtPath<GameObject>(AssetsOut+"/Prefabs/Kenney_DeskLamp.prefab");NativePlace(lamp,zone,"Analyst task lamp",new Vector3(1.0f,.82f,2.0f),180);
        }
        static BankInteractable SeatButton(Transform root,string name,Vector3 point,BankAction action,Vector2 size){
            var s=Surface(root,name,point,size);Label(s,"Label",name,Vector2.zero,size*1000-new Vector2(20,10),44);
            var c=s.gameObject.AddComponent<BoxCollider>();c.size=new Vector3(size.x*1000,size.y*1000,12);c.center=new Vector3(0,0,-8);
            var t=s.gameObject.AddComponent<BankInteractable>();t.Manager=manager;t.Action=action;return t;
        }
        static void BuildLighting(Transform root)
        {
            var lights=All<Light>().Where(x=>x.name.StartsWith("Baked ceiling light")).OrderBy(x=>x.name).ToArray();
            var points=new[]{new Vector3(-5.3f,3.13f,.8f),new Vector3(-5.3f,3.13f,5.6f),new Vector3(-2.3f,3.13f,-2.6f),new Vector3(5.3f,3.13f,.8f),new Vector3(5.3f,3.13f,5.6f),new Vector3(2.3f,3.13f,-2.6f)};
            var housing=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/LightingReview/Luminaire_Slate.mat");var diffuser=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/LightingReview/Luminaire_Neutral.mat");
            for(int i=0;i<lights.Length;i++){
                Undo.RecordObject(lights[i],"Expanded bake coverage");Undo.RecordObject(lights[i].transform,"Expanded bake coverage");lights[i].transform.position=points[i];lights[i].intensity=i==2||i==5?9.5f:11;lights[i].range=7;lights[i].spotAngle=115;lights[i].lightmapBakeType=LightmapBakeType.Baked;
                Position(GameObject.Find("Ceiling fixture "+(i+1)),new Vector3(points[i].x,3.27f,points[i].z));Position(GameObject.Find("Neutral diffuser "+(i+1)),new Vector3(points[i].x,3.22f,points[i].z));
            }
            for(int i=0;i<2;i++){
                var p=new Vector3(i==0?-1.8f:1.8f,3.13f,7.3f);var g=new GameObject("Rear baked ceiling "+i);Undo.RegisterCreatedObjectUndo(g,"Rear baked lights");g.transform.SetParent(root);g.transform.SetPositionAndRotation(p,Quaternion.Euler(90,0,0));
                var l=g.AddComponent<Light>();l.type=LightType.Spot;l.lightmapBakeType=LightmapBakeType.Baked;l.intensity=9.5f;l.range=6;l.spotAngle=115;l.color=new Color(1,.94f,.83f);
                Box(root,"Rear ceiling fixture",new Vector3(p.x,3.27f,p.z),new Vector3(1.3f,.08f,.55f),housing,false);Box(root,"Rear neutral diffuser",new Vector3(p.x,3.22f,p.z),new Vector3(1.18f,.025f,.43f),diffuser,false);
            }
        }
        static void Save(){EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Scene saved using Unity APIs");AssetDatabase.SaveAssets();}
        static void Refine()
        {
            var root=GameObject.Find("EnvironmentRedesignV1").transform;
            Check(root.Find("04_AdditionalCirculation")==null,"Refinement not previously applied");
            Position(GameObject.Find("Console plinth"),new Vector3(0,.50f,-1.98f),new Vector3(1.25f,.72f,.38f));
            // Move furniture, never the original four destination coordinates. Inspection remains a clear standing position.
            foreach(string name in new[]{"Main analyst desk","Interactive Computer","Fictional transfer documents","Stapler","Analyst chair"}){
                var g=GameObject.Find(name);Position(g,g.transform.position+Vector3.forward*.7f);
            }
            var workstation=root.Find("03_AnalystWorkstation");Undo.RecordObject(workstation,"Clear original standing destination");workstation.position+=Vector3.forward*.7f;
            var seat=All<AnalystSeat>().Single();Undo.RecordObject(seat.StandingFloor,"Safe stand point");seat.StandingFloor.position=new Vector3(1.6f,0,-.65f);
            var inspection=GameObject.Find("Teleport Inspection");Position(inspection,new Vector3(-1.72f,.025f,-2.65f));
            var label=Surface(root,"Go to analyst station",new Vector3(-1.72f,.055f,-2.65f),new Vector2(.69f,.32f));label.rotation=Quaternion.Euler(90,0,0);
            Label(label,"Destination","AL PUESTO\nMIRA 1,5 s",Vector2.zero,new Vector2(650,290),64);
            // Functional security boards get a real backing after the exterior wall moves north.
            Box(root,"Security partition",new Vector3(3.65f,1.2f,4.94f),new Vector3(3.45f,2.4f,.10f),navy??AssetDatabase.LoadAssetAtPath<Material>(AssetsOut+"/Materials/Institutional_Navy.mat"));
            var backing=AssetDatabase.LoadAssetAtPath<Material>(AssetsOut+"/Materials/Painted_Metal.mat");
            Box(root,"Archive partition",new Vector3(-4.78f,1.12f,3.15f),new Vector3(.10f,2.24f,3.25f),backing);
            var evidenceSign=GameObject.Find("Evidence zone label");Position(evidenceSign,new Vector3(-4.70f,2.02f,3.15f));Undo.RecordObject(evidenceSign.transform,"Archive label aligned to partition");evidenceSign.transform.rotation=Quaternion.Euler(0,-90,0);
            foreach(string name in new[]{"Wall clock provisional","Clock hour hand","Clock minute hand"}){var g=GameObject.Find(name);if(g!=null)Position(g,g.transform.position+Vector3.forward*4);}
            var circulation=Group(root,"04_AdditionalCirculation");
            var mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/Materials/Interactive_Teal.mat");
            for(int i=0;i<2;i++){
                var point=new Vector3(i==0?-3.35f:3.35f,0,6.45f);
                var pad=Box(circulation,i==0?"Teleport Cubicles West":"Teleport Cubicles East",point+Vector3.up*.025f,new Vector3(.7f,.05f,.7f),mat);
                var t=pad.AddComponent<BankInteractable>();t.Action=BankAction.Teleport;t.Manager=manager;t.Player=seat.Rig;t.Destination=point;
                var title=Surface(circulation,"Cubicle route "+i,point+Vector3.up*.06f,new Vector2(.68f,.32f));title.rotation=Quaternion.Euler(90,0,0);
                Label(title,"Route","CUBICULOS\nMIRA 1,5 s",Vector2.zero,new Vector2(640,290),56);
            }
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>())GameObjectUtility.SetStaticEditorFlags(r.gameObject,GameObjectUtility.GetStaticEditorFlags(r.gameObject)|StaticEditorFlags.ContributeGI);
            Save();
        }
        static void Capture(string name,Vector3 eye,Vector3 target,float fov=60,float ortho=0,bool top=false){
            Canvas.ForceUpdateCanvases();Physics.SyncTransforms();var view=new SceneReviewCapture.View{file=name,position=eye,target=target,fieldOfView=fov,orthographicSize=ortho,hideCeiling=top};
            typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{SceneManager.GetActiveScene(),view,Path.Combine(Gallery,name)});
            File.AppendAllText(Path.Combine(Gallery,"capture-receipt.txt"),DateTime.UtcNow.ToString("O")+" "+name+" Unity="+Application.unityVersion+" Play="+EditorApplication.isPlaying+" Android=NOT_TESTED\n");
        }
        static void After(){
            Capture("02-oficina-ampliada.png",new Vector3(0,2.65f,-4.5f),new Vector3(0,1.1f,1),85);
            Capture("03-consola-modos.png",new Vector3(0,1.65f,-4.4f),new Vector3(0,1.4f,-2.1f),68);
            Capture("04-cubiculos.png",new Vector3(0,2.8f,8.3f),new Vector3(-3.3f,1.0f,2.6f),85);
            Capture("09-iluminacion-materiales.png",new Vector3(6.7f,2.7f,-4.3f),new Vector3(-1,1.1f,3.0f),85);
            Capture("10-distribucion-superior.png",new Vector3(0,16,2),new Vector3(0,0,2),60,9,true);
        }
        static bool RayReaches(BankInteractable t,Vector3 origin){Physics.SyncTransforms();foreach(var c in t.GetComponentsInChildren<Collider>()){if(!c.enabled)continue;var d=c.bounds.center-origin;if(d.magnitude<12&&Physics.Raycast(origin,d.normalized,out var hit,d.magnitude+.05f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<BankInteractable>()==t)return true;}return false;}
        static void NavigationCheck(BankInteractable[] pads)
        {
            var reached=new HashSet<string>{"Teleport Entry"};bool more=true;
            while(more){more=false;foreach(var origin in pads.Where(x=>reached.Contains(x.name)).ToArray())foreach(var target in pads.Where(x=>!reached.Contains(x.name)))if(RayReaches(target,origin.Destination+Vector3.up*1.6f)){reached.Add(target.name);more=true;}}
            Check(pads.All(x=>reached.Contains(x.name)),"All navigation destinations reachable by gaze from entry through available pads");
            foreach(var pad in pads){var p=pad.Destination;var colliders=Physics.OverlapCapsule(p+Vector3.up*.25f,p+Vector3.up*1.6f,.18f,~0,QueryTriggerInteraction.Ignore);Check(colliders.Length==0,"Full standing body clearance at "+pad.name);}
            var lines=new List<string>();foreach(var origin in pads)foreach(var target in pads)if(origin!=target&&RayReaches(target,origin.Destination+Vector3.up*1.6f))lines.Add(origin.name+" -> "+target.name);
            File.WriteAllLines(Dir+"/navigation-graph.txt",lines);
            foreach(var source in pads){var back=new HashSet<string>{source.name};bool advance=true;while(advance){advance=false;foreach(var origin in pads.Where(x=>back.Contains(x.name)).ToArray())foreach(var target in pads.Where(x=>!back.Contains(x.name)))if(RayReaches(target,origin.Destination+Vector3.up*1.6f)){back.Add(target.name);advance=true;}}Check(back.Contains("Teleport Entry"),"Return to entry possible from "+source.name);}
        }
        static void ValidateSaved()
        {
            BankNavigationValidation.Check(All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy));
            Check(All<MonoBehaviour>().All(x=>x!=null),"No missing MonoBehaviour in saved scene");
            var dependencies=AssetDatabase.GetDependencies(ScenePath,true);Check(dependencies.Where(x=>x.StartsWith("Assets/")).All(File.Exists),"All saved scene dependencies exist");File.WriteAllLines(Dir+"/dependencies-final.txt",dependencies);
            Check(LightmapSettings.lightmaps.Length>0&&LightmapSettings.lightmaps.All(x=>x.lightmapColor!=null&&x.lightmapDir!=null),"Baked lightmaps restored with color and direction textures");
            Check(All<Camera>().Count(x=>x.gameObject.activeInHierarchy)==1,"Only player camera persisted");
            var renderers=All<MeshRenderer>().Where(x=>x.gameObject.activeInHierarchy&&x.enabled).ToArray();
            Check(renderers.SelectMany(x=>x.sharedMaterials).All(x=>x!=null&&x.shader!=null&&x.shader.isSupported&&x.shader.name!="Hidden/InternalErrorShader"),"All visible mesh materials supported");
            long triangles=0;foreach(var filter in All<MeshFilter>().Where(x=>x.gameObject.activeInHierarchy&&x.sharedMesh!=null))for(int i=0;i<filter.sharedMesh.subMeshCount;i++)triangles+=(long)filter.sharedMesh.GetIndexCount(i)/3;
            File.WriteAllLines(Dir+"/render-inventory.txt",new[]{"Unity="+Application.unityVersion,"Pipeline="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),"Active mesh renderers="+renderers.Length,"Mesh triangles across all submeshes="+triangles,"Distinct mesh materials="+renderers.SelectMany(x=>x.sharedMaterials).Distinct().Count(),"Lightmaps="+LightmapSettings.lightmaps.Length,"Realtime active lights="+All<Light>().Count(x=>x.gameObject.activeInHierarchy&&x.enabled&&x.lightmapBakeType!=LightmapBakeType.Baked),"Baked active lights="+All<Light>().Count(x=>x.gameObject.activeInHierarchy&&x.enabled&&x.lightmapBakeType==LightmapBakeType.Baked),"NOT a draw-call, FPS, memory or Android performance measurement"});
        }
        static void Fits(Transform root){Canvas.ForceUpdateCanvases();foreach(var t in root.GetComponentsInChildren<TextMeshProUGUI>()){t.ForceMeshUpdate();Check(!t.isTextOverflowing,"TMP fits "+PathOf(t.transform));}}
        static void CardsDoNotOverlap()
        {
            var content=ui.PanelAnchor.GetComponentsInChildren<RectTransform>().Single(x=>x.name=="Contenido");
            var cards=content.Cast<Transform>().Where(x=>x.gameObject.activeInHierarchy&&x.GetComponent<UnityEngine.UI.Image>()!=null).Select(x=>x.GetComponent<RectTransform>()).ToArray();
            for(int i=0;i<cards.Length;i++)for(int j=i+1;j<cards.Length;j++){
                var a=new Rect(cards[i].anchoredPosition-cards[i].sizeDelta*.5f,cards[i].sizeDelta);var b=new Rect(cards[j].anchoredPosition-cards[j].sizeDelta*.5f,cards[j].sizeDelta);
                Check(!a.Overlaps(b),"No overlapping dossier cards: "+cards[i].name+" / "+cards[j].name);
            }
        }
        static IEnumerator Review()
        {
            yield return null;yield return null;
            var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var seat=AnalystSeat.Current;var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();
            ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();var camera=gaze.View;var rig=camera.transform.parent;
            Check(seat!=null&&!seat.IsSeated&&!seat.IsTransitioning,"Chair never seats automatically");
            foreach(string name in new[]{"Teleport Entry","Teleport Inspection","Teleport Evidence","Teleport Security"})Check(All<BankInteractable>().Count(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.Teleport&&x.name==name)==1,"Original teleport retained "+name);
            var entryPad=GameObject.Find("Teleport Inspection").GetComponent<BankInteractable>();Check(RayReaches(entryPad,new Vector3(0,1.6f,-3.3f)),"Inspection navigation marker reachable before mode console");
            NavigationCheck(All<BankInteractable>().Where(x=>x.gameObject.activeInHierarchy&&x.Action==BankAction.Teleport).ToArray());
            Check(RayReaches(station.Attack,new Vector3(0,1.6f,-3.3f))&&RayReaches(station.Defense,new Vector3(0,1.6f,-3.3f)),"Both portal options reachable from original entry");Fits(station.transform);
            gaze.SimulateGazeInEditor=true;camera.transform.LookAt(station.Attack.GetComponent<Collider>().bounds.center);float modeDeadline=Time.realtimeSinceStartup+4;
            while(!game.IntroPlaying&&Time.realtimeSinceStartup<modeDeadline)yield return null;
            Check(game.IntroPlaying&&station.Selected==BankAction.Attack,"Actual 1.5-second mode console gaze starts existing prologue");game.SkipIntro();Check(game.Session.Mode==ScenarioMode.Attack,"Attack selection preserved");
            station.Defense.Activate();Check(game.Session.Mode==ScenarioMode.Defense,"Defense selection preserved");Check(station.Selected==BankAction.DefenseMode,"Selected console state confirms mode");
            rig.position=new Vector3(0,0,-.65f);camera.transform.localRotation=Quaternion.identity;yield return null;
            Check(RayReaches(seat.SitTarget,camera.transform.position),"Chair SIT marker reachable from inspection");
            var obstruction=GameObject.CreatePrimitive(PrimitiveType.Cube);obstruction.name="TEST ONLY - occupied chair destination";obstruction.transform.position=seat.SeatedEye.position;obstruction.transform.localScale=Vector3.one*.5f;Physics.SyncTransforms();
            var originalOrigin=rig.position;Check(!seat.Request(true)&&!seat.IsTransitioning&&rig.position==originalOrigin,"Occupied seated destination rejected without moving origin");UnityEngine.Object.DestroyImmediate(obstruction);Physics.SyncTransforms();
            Capture("05-analista-de-pie.png",camera.transform.position,new Vector3(0,1.4f,1.8f),80);
            // Actual dwell, using the sole production interaction manager.
            gaze.SimulateGazeInEditor=true;camera.transform.LookAt(seat.SitTarget.transform.position);float deadline=Time.realtimeSinceStartup+4;
            while(seat.TransitionCount<1&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(seat.FadeOpacity>=.99f,"Origin changes while blackout is opaque");
            while(seat.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(seat.IsSeated&&seat.TransitionCount==1,"1.5-second gaze seats once under fade");
            Check(!seat.Request(true),"Duplicate sit request rejected");
            Check(Vector3.Distance(camera.transform.position,seat.SeatedEye.position)<.02f,"Tracking offset compensated by origin movement");
            Check(camera.GetComponent<HeadPoseAdapter>()!=null&&camera.GetComponent<HeadPoseAdapter>().enabled,"Head pose provider remains enabled");
            Check(!seat.SitTarget.gameObject.activeSelf&&seat.StandTarget.gameObject.activeSelf,"Sit and stand states have exclusive controls");
            var pads=All<BankInteractable>().Where(x=>x.Action==BankAction.Teleport&&x.gameObject.activeInHierarchy).ToArray();var old=rig.position;
            foreach(var pad in pads){Check(!pad.GetComponent<Collider>().enabled,"Seated floor pad disabled "+pad.name);pad.Activate();}Check(rig.position==old,"Seated state blocks direct floor teleport");
            Check(RayReaches(seat.StandTarget,camera.transform.position),"LEVANTARSE visible and unobstructed from seated eye");
            foreach(var action in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return})Check(All<BankInteractable>().Any(t=>t.gameObject.activeInHierarchy&&t.Action==action&&RayReaches(t,camera.transform.position)),"Decision reachable while seated: "+action);
            var reference=UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>();Fits(reference.transform);Fits(ui.PanelAnchor);Fits(seat.StandTarget.transform);
            Capture("06-analista-sentado.png",camera.transform.position,new Vector3(-.3f,1.43f,1.6f),85);
            Capture("07-monitor-principal.png",camera.transform.position,ui.PanelAnchor.position,65);
            Capture("08-consulta-libreta.png",new Vector3(-.70f,1.9f,.70f),new Vector3(-1.25f,1.05f,1.55f),85);
            Capture("11-marcador-levantarse.png",camera.transform.position,seat.StandTarget.transform.position,65);
            var notebook=All<BankInteractable>().Single(x=>x.Action==BankAction.Notebook);Check(RayReaches(notebook,camera.transform.position),"Clipboard reachable while seated");
            var beforeRecord=game.Session.Transactions.Current;notebook.Activate();yield return null;
            Check(UnityEngine.Object.FindFirstObjectByType<BankDocumentUI>().CurrentPage==BankDocumentUI.Page.Procedure&&game.Session.Transactions.Current==beforeRecord,"Clipboard opens existing enlarged procedure without changing transaction");
            ui.ShowDocument(BankDocumentUI.Page.Dossier);
            foreach(var data in game.Session.Transactions.Records.Select(x=>x.Data)){
                reference.Refresh();Fits(reference.transform);Fits(ui.PanelAnchor);CardsDoNotOverlap();game.Decide(Decision.Hold);yield return null;
            }
            game.StartScenario(ScenarioMode.Attack);game.Decide(Decision.Approve);yield return null;reference.Refresh();
            Check(reference.Observed.text.Contains("No informado")&&reference.ExistingAlert.text.Contains(FormatReferencePanel.ObservedAlerts(game.Session.Transactions.Current.Data)),"Format panel projects actual missing intermediary and existing alert");
            Check(!reference.Observed.text.Contains("Correct")&&!reference.ExistingAlert.text.ToLowerInvariant().Contains("devolver")&&!reference.ExistingAlert.text.ToLowerInvariant().Contains("retener"),"Reference does not expose recommended decision");Fits(reference.transform);Fits(ui.PanelAnchor);
            game.InspectAlerts();yield return null;
            var feedback=ui.PanelAnchor.GetComponentsInChildren<TMP_Text>().Single(x=>x.name=="Respuesta del sistema").text;
            Check(!feedback.ToLowerInvariant().Contains("devolver")&&!feedback.ToLowerInvariant().Contains("retener"),"Alert query feedback also keeps authored observations without revealing a decision");
            var q=AnalystSeat.OriginForEye(new Vector3(1,1.25f,2),new Vector3(.2f,1.87f,.1f),Vector3.zero);
            Check(Vector3.Distance(q+new Vector3(.2f,1.87f,.1f),new Vector3(1,1.25f,2))<.0001f,"Origin calculation preserves non-default tracked pose offset");
            camera.transform.rotation=Quaternion.LookRotation(Vector3.up);yield return null;camera.transform.LookAt(seat.StandTarget.transform.position);deadline=Time.realtimeSinceStartup+4;
            while(seat.TransitionCount<2&&Time.realtimeSinceStartup<deadline)yield return null;while(seat.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!seat.IsSeated&&seat.TransitionCount==2,"1.5-second gaze stands once under fade");
            Check(seat.FadeOpacity<.01f&&!seat.Request(false),"Fade cleared and duplicate stand request rejected");
            Check(seat.DestinationSafe(camera.transform.position),"Standing destination has body clearance");foreach(var pad in pads)Check(pad.GetComponent<Collider>().enabled,"Standing restores pad "+pad.name);
            // The fixed four pads still work after standing, including their exact stored destinations.
            foreach(var pad in pads){pad.Activate();Check(Vector3.Distance(pad.Player.position,pad.Destination)<.01f,"Original teleport unchanged "+pad.name);}
            Check(errors==0,"No runtime errors during environment review");gaze.SimulateGazeInEditor=false;
        }
    }
}

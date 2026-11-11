using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Swift.Interaction;

namespace Swift.EditorTools
{
    // Runs only on an explicit local file command. The review camera is never saved.
    [InitializeOnLoad]
    public static class OfficeDetailPolish
    {
        const string Main="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Trial="Assets/_Project/Scenes/Review/BancoCorresponsal_DetailPolish_v3_20261110.unity";
        const string Queue="Artifacts/OfficeDetailPolish/command.txt";
        const string Evidence="Artifacts/OfficeDetailPolish";
        const string Gallery="Docs/Previews/OfficeDetailPolish";

        struct View
        {
            public string Name; public Vector3 Eye,Look;
            public View(string name,Vector3 eye,Vector3 look){Name=name;Eye=eye;Look=look;}
        }

        static readonly View[] Views={
            new View("01-office.png",new Vector3(8.8f,2.2f,-6.9f),new Vector3(0,1.3f,3.6f)),
            new View("02-cubicles.png",new Vector3(-3.6f,1.65f,-3.1f),new Vector3(-6.6f,1.05f,0)),
            new View("03-documentation.png",new Vector3(-1.5f,1.65f,1),new Vector3(-4.2f,1.2f,4.3f)),
            new View("04-supervision.png",new Vector3(2.7f,1.6f,4.3f),new Vector3(7.4f,1.4f,8.2f)),
            new View("05-close-turn.png",new Vector3(6.55f,1.65f,6.55f),new Vector3(6.55f,1.30f,8.55f)),
            new View("06-reference-shelf.png",new Vector3(-2.75f,1.65f,4.95f),new Vector3(-4.2f,1.35f,6.75f)),
            new View("07-evidence.png",new Vector3(-4.4f,1.55f,2.2f),new Vector3(-4.35f,1.0f,4.2f)),
            new View("08-close-turn-near.png",new Vector3(6f,1.55f,7.85f),new Vector3(6f,1.25f,8.54f))
        };

        static OfficeDetailPolish(){EditorApplication.update+=Tick;}

        static void Tick()
        {
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||!File.Exists(Queue))return;
            string command=File.ReadAllText(Queue).Trim();File.Delete(Queue);
            try{
                Check(SceneManager.sceneCount==1&&!SceneManager.GetActiveScene().isDirty,"Single saved clean scene required");
                Check(command=="inspect"||command=="open-main"||command=="trial"||command=="inspect-trial"||command=="integrate"||command=="capture","Known office detail command");
                Directory.CreateDirectory(Evidence);
                if(command=="open-main"){
                    Check(string.Equals(Hash(Main),File.ReadAllText(Evidence+"/baseline-scene.sha256").Trim(),StringComparison.OrdinalIgnoreCase),"Main still matches recovery backup");
                    EditorSceneManager.OpenScene(Main,OpenSceneMode.Single);
                }else if(command=="inspect"){
                    Check(SceneManager.GetActiveScene().path==Main,"Baseline uses main scene");
                    string expected=File.ReadAllText(Evidence+"/baseline-scene.sha256").Trim();
                    Check(string.Equals(Hash(Main),expected,StringComparison.OrdinalIgnoreCase),"Main matches recovery backup");
                    Inventory("inventory-before.txt");Capture("Before");
                }else if(command=="trial"){
                    Check(SceneManager.GetActiveScene().path==Main,"Review starts from saved main");
                    Check(string.Equals(Hash(Main),File.ReadAllText(Evidence+"/baseline-scene.sha256").Trim(),StringComparison.OrdinalIgnoreCase),"Main still matches backup");
                    Check(File.Exists(Evidence+"/backup-path.txt")&&!File.Exists(Trial),"Backup exists and review scene is new");
                    Check(AssetDatabase.CopyAsset(Main,Trial),"AssetDatabase made an isolated scene with a fresh GUID");
                    var scene=EditorSceneManager.OpenScene(Trial,OpenSceneMode.Single);
                    ApplyVisuals();Check(EditorSceneManager.SaveScene(scene),"Save trial through Unity API");
                    File.WriteAllText(Evidence+"/trial-scene.sha256",Hash(Trial));Capture("TrialV3");
                    Check(string.Equals(Hash(Main),File.ReadAllText(Evidence+"/baseline-scene.sha256").Trim(),StringComparison.OrdinalIgnoreCase),"Main remained byte-identical during trial");
                }else if(command=="inspect-trial"){
                    Check(SceneManager.GetActiveScene().path==Trial,"Inspect current trial");Inventory("inventory-trial.txt");
                }else if(command=="integrate"){
                    Check(SceneManager.GetActiveScene().path==Trial,"Integrate from inspected trial");
                    Check(File.ReadAllText(Evidence+"/trial-result.txt").StartsWith("PASS"),"Trial passed");
                    Check(Hash(Trial)==File.ReadAllText(Evidence+"/trial-scene.sha256"),"Reviewed trial unchanged");
                    Check(string.Equals(Hash(Main),File.ReadAllText(Evidence+"/baseline-scene.sha256").Trim(),StringComparison.OrdinalIgnoreCase),"Main still matches backup before integration");
                    var scene=EditorSceneManager.OpenScene(Main,OpenSceneMode.Single);
                    ApplyVisuals();Check(EditorSceneManager.SaveScene(scene),"Save integrated main through Unity API");
                    Capture("After");File.WriteAllText(Evidence+"/integrated-scene.sha256",Hash(Main));
                }else{
                    Check(SceneManager.GetActiveScene().path==Main,"Capture current main only");Capture("After");
                }
                File.WriteAllText(Evidence+"/"+command+"-result.txt","PASS "+command+" "+DateTime.UtcNow.ToString("O")+" Unity="+Application.unityVersion+" EDITOR ONLY\n");
            }catch(Exception ex){Directory.CreateDirectory(Evidence);File.WriteAllText(Evidence+"/"+command+"-result.txt","FAIL "+command+" "+ex+"\n");Debug.LogError(ex);}
        }

        static void Check(bool yes,string message){if(!yes)throw new InvalidOperationException(message);}
        static string Hash(string file){using(var sha=SHA256.Create())using(var stream=File.OpenRead(file))return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");}
        static string PathOf(Transform t){string p=t.name;while(t.parent!=null){t=t.parent;p=t.name+"/"+p;}return p;}
        static Bounds MeshBounds(GameObject go){var renderers=go.GetComponentsInChildren<MeshRenderer>(true);if(renderers.Length==0)return new Bounds(go.transform.position,Vector3.zero);var b=renderers[0].bounds;foreach(var r in renderers.Skip(1))b.Encapsulate(r.bounds);return b;}

        static Transform Active(string name)
        {
            var matches=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(t=>t.name==name&&t.gameObject.activeInHierarchy).ToArray();
            Check(matches.Length==1,"One active object named "+name);return matches[0];
        }

        static void OnSurface(Transform item,Vector3 point,float surfaceY)
        {
            Undo.RecordObject(item,"Support decorative books on a real surface");
            item.position=new Vector3(point.x,item.position.y,point.z);
            item.position+=Vector3.up*(surfaceY-MeshBounds(item.gameObject).min.y);
            if(PrefabUtility.IsPartOfPrefabInstance(item))PrefabUtility.RecordPrefabInstancePropertyModifications(item);
            Check(Mathf.Abs(MeshBounds(item.gameObject).min.y-surfaceY)<.006f,"Books touch their supporting surface: "+PathOf(item));
        }

        static Image Accent(Transform parent,string name,Vector2 position,Vector2 size,Color color,int sibling=1)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));Undo.RegisterCreatedObjectUndo(go,"Institutional sign detail");
            go.transform.SetParent(parent,false);var rect=go.GetComponent<RectTransform>();rect.anchoredPosition=position;rect.sizeDelta=size;
            var image=go.GetComponent<Image>();image.color=color;image.raycastTarget=false;go.transform.SetSiblingIndex(sibling);return image;
        }

        static void ApplyVisuals()
        {
            var plaqueNames=new[]{"Cubicle identifier","Evidence zone label","Bank title","Supervision identity","Welcome","Model credits"};
            Color navy=new Color(.035f,.105f,.145f,1),cream=new Color(.91f,.94f,.91f,1),cyan=new Color(.26f,.76f,.80f,1);
            var all=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            int recolored=0;
            foreach(var sign in all.Where(t=>t.gameObject.activeInHierarchy&&plaqueNames.Contains(t.name))){
                if(sign.name=="Supervision identity"){
                    var panel=sign.GetComponent<RectTransform>();Undo.RecordObject(panel,"Compact supervision wayfinding");panel.sizeDelta=new Vector2(2000,460);
                }
                foreach(var image in sign.GetComponentsInChildren<Image>(true)){
                    Undo.RecordObject(image,"Dark institutional sign surface");image.color=navy;recolored++;
                    if(sign.name=="Supervision identity")image.rectTransform.sizeDelta=new Vector2(2000,460);
                }
                foreach(var text in sign.GetComponentsInChildren<TMP_Text>(true)){
                    Undo.RecordObject(text,"High contrast sign text");text.color=sign.name=="Cubicle identifier"?cyan:cream;
                    if(sign.name=="Evidence zone label"){text.fontSize=49;text.fontStyle=FontStyles.Bold;}
                    if(sign.name=="Supervision identity"){
                        text.text="SUPERVISIÓN\nCIERRE DE TURNO";text.fontSize=95;text.fontStyle=FontStyles.Bold;
                        text.rectTransform.sizeDelta=new Vector2(1860,420);text.alignment=TextAlignmentOptions.Center;
                    }
                }
                if(sign.name=="Evidence zone label"||sign.name=="Supervision identity"){
                    var rect=sign.GetComponent<RectTransform>();Check(rect!=null,"Plaque has RectTransform: "+sign.name);
                    Accent(sign,"Cyan institutional rule",new Vector2(-rect.sizeDelta.x*.5f+15,0),new Vector2(10,rect.sizeDelta.y*.70f),cyan);
                }
            }
            Check(recolored>=8,"Existing light wall and cubicle plaques refinished");
            var supervisionSign=Active("Supervision identity");Undo.RecordObject(supervisionSign,"Wayfinding at the supervision entrance");
            supervisionSign.position=new Vector3(7.1f,2.62f,7.53f);
            ImproveClosure(navy,cream,cyan);
            SupportBooks();
            var activeEnd=UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Where(x=>x.Action==BankAction.End).ToArray();
            Check(activeEnd.Length==1&&activeEnd[0].GetComponent<BoxCollider>()!=null,"One existing close-turn action and collider remain");
            Check(UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Count(l=>l.lightmapBakeType!=LightmapBakeType.Baked)==2,"No realtime lights added");
            File.WriteAllText(Evidence+"/applied-summary.txt","RecoloredImages="+recolored+"\nEndCollider="+activeEnd[0].GetComponent<BoxCollider>().bounds+"\n");
        }

        static void ImproveClosure(Color navy,Color cream,Color cyan)
        {
            var screen=Active("Closure terminal screen");Undo.RecordObject(screen,"Larger supervision terminal face");
            screen.localScale=new Vector3(1.34f,.80f,.025f);
            var housing=Active("Supervisor terminal housing");Undo.RecordObject(housing,"Housing supports larger close-turn face");
            housing.localScale=new Vector3(1.37f,1.10f,1f);
            if(PrefabUtility.IsPartOfPrefabInstance(housing))PrefabUtility.RecordPrefabInstancePropertyModifications(housing);
            var end=UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Single(x=>x.Action==BankAction.End);
            var collider=end.GetComponent<BoxCollider>();Check(collider!=null,"Existing End collider present");
            Undo.RecordObject(collider,"Larger close-turn gaze target");collider.size=new Vector3(1.29f,.74f,.12f);

            var label=Active("End label");Undo.RecordObject(label,"Uncrowded close-turn canvas");
            label.localScale=Vector3.one*.002f;
            var root=label.GetComponent<RectTransform>();Undo.RecordObject(root,"Larger close-turn canvas area");root.sizeDelta=new Vector2(630,340);
            var background=label.GetComponentsInChildren<Image>(true).First();Undo.RecordObject(background,"Dark terminal surface");
            background.color=navy;background.rectTransform.sizeDelta=new Vector2(630,340);background.raycastTarget=false;
            var header=label.GetComponentsInChildren<TextMeshProUGUI>(true).First();Undo.RecordObject(header,"Short closure heading");
            header.text="SWIFT / SUPERVISIÓN";header.color=cyan;header.fontSize=31;header.fontStyle=FontStyles.Bold;
            header.alignment=TextAlignmentOptions.Center;header.enableAutoSizing=false;header.raycastTarget=false;
            header.rectTransform.anchoredPosition=new Vector2(0,113);header.rectTransform.sizeDelta=new Vector2(565,50);
            Accent(label,"Action well",new Vector2(0,8),new Vector2(570,110),new Color(.035f,.24f,.29f,1));
            Accent(label,"Action underline",new Vector2(0,-49),new Vector2(550,4),cyan);
            var action=UnityEngine.Object.Instantiate(header,label);Undo.RegisterCreatedObjectUndo(action.gameObject,"Readable close-turn action");
            action.name="Closure action";action.text="CERRAR TURNO";action.fontSize=57;action.color=cream;
            action.rectTransform.anchoredPosition=new Vector2(0,9);action.rectTransform.sizeDelta=new Vector2(575,90);
            var note=UnityEngine.Object.Instantiate(header,label);Undo.RegisterCreatedObjectUndo(note.gameObject,"Closure instruction");
            note.name="Closure condition";note.text="Después de los 6 expedientes";note.fontSize=29;note.fontStyle=FontStyles.Normal;note.color=cream;
            note.rectTransform.anchoredPosition=new Vector2(0,-110);note.rectTransform.sizeDelta=new Vector2(595,62);
            foreach(var text in label.GetComponentsInChildren<TMP_Text>(true)){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Closure text fits: "+text.name);}
            var edge=Active("Closure amber status edge");Undo.RecordObject(edge,"Status line matches larger terminal");
            edge.localScale=new Vector3(1.16f,.018f,.012f);edge.position=new Vector3(edge.position.x,.79f,edge.position.z);
        }

        static void ProbeLitBooks(Transform root)
        {
            foreach(var r in root.GetComponentsInChildren<MeshRenderer>(true)){
                Undo.RecordObject(r,"Decorative books use local light probes after moving");
                r.lightmapIndex=-1;r.lightProbeUsage=LightProbeUsage.BlendProbes;
                GameObjectUtility.SetStaticEditorFlags(r.gameObject,StaticEditorFlags.BatchingStatic);
                if(PrefabUtility.IsPartOfPrefabInstance(r))PrefabUtility.RecordPrefabInstancePropertyModifications(r);
            }
        }

        static void SupportBooks()
        {
            var cubicles=Active("02_SecondaryCubicles");
            var third=cubicles.Find("Cubicle 3");Check(third!=null,"Third cubicle exists");
            var thirdDesk=third.Find("Shared desk");Check(thirdDesk!=null,"Third desk exists");var thirdBounds=MeshBounds(thirdDesk.gameObject);
            var original=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Single(t=>t.name=="Reference books"&&t.parent!=null&&t.parent.name=="06_Secondary_Furniture");
            Undo.SetTransformParent(original,third,"Move unsupported books to a real workstation");
            OnSurface(original,new Vector3(thirdBounds.center.x+.78f,0,thirdBounds.center.z),thirdBounds.max.y);ProbeLitBooks(original);

            var secondBook=cubicles.Find("Cubicle 2/Reference books");var secondDesk=cubicles.Find("Cubicle 2/Shared desk");
            Check(secondBook!=null&&secondDesk!=null,"Second cubicle book and desk exist");
            OnSurface(secondBook,secondBook.position,MeshBounds(secondDesk.gameObject).max.y);ProbeLitBooks(secondBook);

            var visitor=Active("Visitor table");float visitorTop=MeshBounds(visitor.gameObject).max.y;
            var visitorBook=cubicles.Find("Cubicle 4/Reference books");
            Check(visitorBook!=null,"Visitor books currently parented under cubicle four");
            Undo.SetTransformParent(visitorBook,visitor,"Visitor books belong to their actual table");
            OnSurface(visitorBook,visitor.position,visitorTop);ProbeLitBooks(visitorBook);

            var supervisorBooks=Active("Supervisor books");var supervisorTable=Active("Supervisor reference table");
            OnSurface(supervisorBooks,supervisorBooks.position,MeshBounds(supervisorTable.gameObject).max.y);

            var shelf=Active("Document reference shelf");var shelfBounds=MeshBounds(shelf.gameObject);
            var existing=Active("Procedure reference volumes");
            var levels=new List<float>();
            foreach(var filter in shelf.GetComponentsInChildren<MeshFilter>()){
                var mesh=filter.sharedMesh;if(mesh==null)continue;var vertices=mesh.vertices;var triangles=mesh.triangles;
                for(int i=0;i<triangles.Length;i+=3){
                    var a=filter.transform.TransformPoint(vertices[triangles[i]]);var b=filter.transform.TransformPoint(vertices[triangles[i+1]]);var c=filter.transform.TransformPoint(vertices[triangles[i+2]]);
                    var normal=Vector3.Cross(b-a,c-a);if(normal.sqrMagnitude>.000004f&&Vector3.Dot(normal.normalized,Vector3.up)>.99f&&a.y>.12f&&a.y<1.55f)levels.Add(a.y);
                }
            }
            var distinct=levels.GroupBy(y=>Mathf.RoundToInt(y*100)).Select(g=>g.Average()).OrderBy(y=>y).ToArray();
            File.WriteAllLines(Evidence+"/shelf-levels.txt",distinct.Select(y=>y.ToString("F4")));
            Check(distinct.Any(y=>y>.15f&&y<.5f)&&distinct.Any(y=>y>1.0f&&y<1.5f),"Real lower and upper support shelves identified");
            float lower=distinct.Where(y=>y>.15f&&y<.5f).OrderBy(y=>Mathf.Abs(y-.25f)).First();
            float upper=distinct.Where(y=>y>1.0f&&y<1.5f).OrderBy(y=>Mathf.Abs(y-1.2f)).First();
            OnSurface(existing,existing.position,distinct.OrderBy(y=>Mathf.Abs(y-.715f)).First());ProbeLitBooks(existing);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/Prefabs/Kenney_Books.prefab");Check(prefab!=null,"Existing licensed Kenney books prefab");
            var originalRenderers=existing.GetComponentsInChildren<MeshRenderer>();
            foreach(var setup in new[]{("Archive volumes lower",lower,0f),("Archive volumes upper left",upper,-.18f),("Archive volumes upper right",upper,.18f)}){
                var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab);Undo.RegisterCreatedObjectUndo(go,"Populate existing document shelf");
                go.name=setup.Item1;Undo.SetTransformParent(go.transform,shelf,"Books stored in document shelf");
                go.transform.rotation=Quaternion.Euler(0,270,0);
                OnSurface(go.transform,new Vector3(shelfBounds.center.x,0,shelfBounds.center.z+setup.Item3),setup.Item2);
                var renderers=go.GetComponentsInChildren<MeshRenderer>();Check(renderers.Length==originalRenderers.Length,"Reused books keep expected renderer layout");
                for(int i=0;i<renderers.Length;i++){Undo.RecordObject(renderers[i],"Reuse institutional book materials");renderers[i].sharedMaterials=originalRenderers[i].sharedMaterials;}
                foreach(var c in go.GetComponentsInChildren<Collider>())Undo.DestroyObjectImmediate(c);
                ProbeLitBooks(go.transform);
            }
        }

        static void Inventory(string filename)
        {
            var all=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            string[] terms={"Reference books","Archive volumes","Procedure reference volumes","Supervisor books","Visitor table","Document reference shelf","End label","Closure terminal","Supervisor terminal","Supervisor desk","Evidence zone label","Supervision identity","Interactive Archive","Shared desk","Evidence table","Interactive Printer","Fictional transfer documents"};
            var lines=new List<string>{"SceneSHA256="+Hash(Main),"Unity="+Application.unityVersion,"--- TARGETS ---"};
            foreach(var t in all.Where(t=>terms.Any(term=>t.name.Contains(term))).OrderBy(t=>PathOf(t))){
                var b=MeshBounds(t.gameObject);
                lines.Add(PathOf(t)+" pos="+t.position+" rot="+t.eulerAngles+" bounds="+b+" localScale="+t.localScale+" active="+t.gameObject.activeInHierarchy);
                var c=t.GetComponent<Collider>();if(c!=null)lines.Add("  collider="+c.bounds);
                var rect=t.GetComponent<RectTransform>();if(rect!=null)lines.Add("  rect="+rect.sizeDelta+" lossyScale="+t.lossyScale);
                foreach(var tmp in t.GetComponentsInChildren<TMP_Text>(true))lines.Add("  TMP "+tmp.name+" size="+tmp.fontSize+" rect="+tmp.rectTransform.sizeDelta+" text="+tmp.text.Replace("\n"," / "));
                foreach(var im in t.GetComponentsInChildren<Image>(true))lines.Add("  Image "+im.name+" color="+im.color);
            }
            lines.Add("--- SCENE COUNTS ---");
            var meshes=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
            lines.Add("Renderers="+meshes.Length+" Materials="+meshes.SelectMany(m=>m.sharedMaterials).Where(m=>m!=null).Distinct().Count());
            lines.Add("Triangles="+meshes.Sum(r=>r.GetComponent<MeshFilter>()?.sharedMesh?.triangles.Length/3??0));
            File.WriteAllLines(Evidence+"/"+filename,lines);
        }

        static void Capture(string stage)
        {
            Check(SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null,"GPU required for real Unity captures");
            string dir=Gallery+"/"+stage;Directory.CreateDirectory(dir);
            var receipt=new List<string>{"Unity="+Application.unityVersion,"Scene="+Main,"SceneSHA256="+Hash(Main),"Pipeline="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),"GPU="+SystemInfo.graphicsDeviceName,"Editor URP, monoscopic, not Android or binocular"};
            foreach(var view in Views){string file=dir+"/"+view.Name;Render(view,file);receipt.Add(view.Name+" eye="+view.Eye+" look="+view.Look+" sha256="+Hash(file));}
            File.WriteAllLines(dir+"/capture-receipt.txt",receipt);
        }

        static void Render(View view,string file)
        {
            var prior=RenderTexture.active;
            var go=new GameObject("Office detail review camera - temporary"){hideFlags=HideFlags.HideAndDontSave};
            RenderTexture target=null;Texture2D image=null;
            try{
                var camera=go.AddComponent<Camera>();camera.enabled=false;camera.nearClipPlane=.05f;camera.farClipPlane=60;camera.clearFlags=CameraClearFlags.SolidColor;
                camera.backgroundColor=new Color(.08f,.1f,.12f);camera.aspect=1600f/900;camera.fieldOfView=60;
                camera.transform.position=view.Eye;camera.transform.LookAt(view.Look);
                var urp=camera.GetUniversalAdditionalCameraData();urp.renderPostProcessing=false;urp.renderShadows=true;
                target=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32);Check(target.Create(),"Capture RenderTexture created");
                var request=new UniversalRenderPipeline.SingleCameraRequest{destination=target};Check(RenderPipeline.SupportsRenderRequest(camera,request),"URP render request supported");
                RenderPipeline.SubmitRenderRequest(camera,request);RenderTexture.active=target;
                image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(file,image.EncodeToPNG());
            }finally{
                RenderTexture.active=prior;if(image!=null)UnityEngine.Object.DestroyImmediate(image);
                if(target!=null){target.Release();UnityEngine.Object.DestroyImmediate(target);}UnityEngine.Object.DestroyImmediate(go);
            }
        }
    }
}

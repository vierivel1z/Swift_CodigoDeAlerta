using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Swift.Game;

namespace Swift.EditorTools
{
    // Explicit local command queue. No network, renderer configuration, or office regeneration.
    [InitializeOnLoad]
    public static class ArtIntegrationReview
    {
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Queue="Artifacts/ArtIntegrationReview/command.txt";
        const string Pending="SWIFT_ART_REVIEW";
        const string AssetsOut="Assets/_Project/Art/Integrated";
        static IEnumerator routine;
        static int errors;
        static string active;
        static bool ownsPlay;
        public static string Output {
            get {
                const string receipt="Artifacts/ArtIntegrationReview/output-path.txt";
                if(!File.Exists(receipt)) File.WriteAllText(receipt,"Docs/Previews/ArtIntegrationReview/"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));
                string folder=File.ReadAllText(receipt).Trim();Directory.CreateDirectory(folder);return folder;
            }
        }
        static ArtIntegrationReview() { EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged; }
        static void Tick()
        {
            if(routine!=null) { try {if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return; }
            if(!File.Exists(Queue)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
            active=File.ReadAllText(Queue).Trim();File.Delete(Queue);
            try {
                if(active=="snapshot") {
                    var scene=SceneManager.GetActiveScene();
                    string path=AssetsOut+"/Snapshots/Memory-"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ")+".unity";
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    Check(EditorSceneManager.SaveScene(scene,path,true),"Unsaved memory state copied without overwriting the active scene");
                    AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
                    string backup=File.ReadAllText("Artifacts/ArtIntegrationReview/backup-path.txt").Trim();
                    File.Copy(path,Path.Combine(backup,Path.GetFileName(path)),false);
                    File.Copy(path+".meta",Path.Combine(backup,Path.GetFileName(path)+".meta"),false);
                    File.WriteAllText("Artifacts/ArtIntegrationReview/memory-snapshot-path.txt",path);
                    Finish(null);return;
                }
                if(EditorApplication.isPlayingOrWillChangePlaymode || Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Playing or unsaved scene: preserved; review cancelled");
                EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
                if(active=="wall") { InstallWall();Finish(null);return; }
                if(active=="wallfix") { FixWall();Finish(null);return; }
                if(active=="prologue") InstallPrologue();
                else if(active!="flow"&&active!="office"&&active!="smoke")throw new Exception("Unknown art command");
                SessionState.SetString(Pending,active);
                EditorApplication.EnterPlaymode();
            }catch(Exception e){Finish(e);}
        }
        static void PlayChanged(PlayModeStateChange state)
        {
            if(state!=PlayModeStateChange.EnteredPlayMode)return;
            active=SessionState.GetString(Pending,"");if(active=="")return;SessionState.EraseString(Pending);
            ownsPlay=true;
            errors=0;Application.logMessageReceived+=Log;routine=active=="prologue"?ReviewPrologue():RunProbe(active);
        }
        static void Log(string value,string stack,LogType type) { if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++; }
        static void Finish(Exception e)
        {
            routine=null;Application.logMessageReceived-=Log;
            string result=e==null&&errors==0?"PASS "+active+" "+DateTime.UtcNow.ToString("O")+" Editor only; Android NOT TESTED":"FAIL "+active+" "+(e?.ToString()??"Runtime errors="+errors);
            File.WriteAllText("Artifacts/ArtIntegrationReview/"+(active??"unknown")+"-result.txt",result);Debug.Log(result);
            if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();
            ownsPlay=false;
        }
        static void Check(bool value,string label) { if(!value)throw new Exception(label);File.AppendAllText("Artifacts/ArtIntegrationReview/checks.txt","PASS "+label+"\n"); }
        static void InstallWall()
        {
            Check(File.Exists("Artifacts/ArtIntegrationReview/backup-path.txt"),"External backup receipt exists before scene modification");
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Project/Art/WallGraphics/ART_BankWallGraphic.png");
            Check(texture!=null,"Institutional texture imported");
            if(GameObject.Find("ART - Bank Wall Graphic")!=null)throw new Exception("Wall artwork already installed; preserved, no duplicate");
            Directory.CreateDirectory(AssetsOut);AssetDatabase.Refresh();
            var shader=Shader.Find("Universal Render Pipeline/Lit");Check(shader!=null,"Existing URP Lit shader available");
            var picture=CreateMaterial("MAT_BankWallGraphic",shader,Color.white);picture.SetTexture("_BaseMap",texture);
            var frame=CreateMaterial("MAT_BankWallFrame",shader,new Color(.035f,.09f,.12f));
            var parent=new GameObject("ART - Bank Wall Graphic");Undo.RegisterCreatedObjectUndo(parent,"Install institutional art");
            parent.transform.SetPositionAndRotation(new Vector3(-4.84f,1.95f,-1.4f),Quaternion.Euler(0,-90,0));
            float width=2.2f,height=width*texture.height/texture.width;
            var back=GameObject.CreatePrimitive(PrimitiveType.Cube);back.name="Matte navy frame";back.transform.SetParent(parent.transform,false);
            back.transform.localScale=new Vector3(width+.06f,height+.06f,.035f);back.GetComponent<Renderer>().sharedMaterial=frame;
            var face=GameObject.CreatePrimitive(PrimitiveType.Quad);face.name="Institutional print - original aspect";face.transform.SetParent(parent.transform,false);
            face.transform.localPosition=new Vector3(0,0,-.025f);face.transform.localScale=new Vector3(width,height,1);face.GetComponent<Renderer>().sharedMaterial=picture;
            foreach(var collider in parent.GetComponentsInChildren<Collider>())UnityEngine.Object.DestroyImmediate(collider);
            foreach(var renderer in parent.GetComponentsInChildren<Renderer>()) {renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=true;renderer.lightProbeUsage=LightProbeUsage.BlendProbes;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;}
            Check(parent.GetComponentsInChildren<Collider>().Length==0,"Decoration adds no blocking colliders");
            Check(Mathf.Abs(face.transform.localScale.x/face.transform.localScale.y-(float)texture.width/texture.height)<.001f,"Original wall image aspect retained");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
            Capture(new SceneReviewCapture.View{file="01-grafica-pared.png",position=new Vector3(-1.2f,1.9f,-3.5f),target=parent.transform.position,fieldOfView=65});
            CaptureOfficeViews();
        }
        static Material CreateMaterial(string name,Shader shader,Color color)
        {
            string path=AssetsOut+"/"+name+".mat";if(File.Exists(path))throw new Exception("Existing material preserved: "+path);
            var material=new Material(shader){name=name};material.SetColor("_BaseColor",color);material.SetFloat("_Metallic",0);material.SetFloat("_Smoothness",.08f);material.SetColor("_EmissionColor",Color.black);material.DisableKeyword("_EMISSION");
            AssetDatabase.CreateAsset(material,path);return material;
        }
        static void FixWall()
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath("Assets/_Project/Art/WallGraphics/ART_BankWallGraphic.png");
            importer.npotScale=TextureImporterNPOTScale.None;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
            var parent=GameObject.Find("ART - Bank Wall Graphic");Check(parent!=null,"Existing single institutional art found");
            parent.transform.position=new Vector3(-4.84f,1.95f,.6f);
            float height=2.2f*941/1672;
            parent.transform.Find("Institutional print - original aspect").localScale=new Vector3(2.2f,height,1);
            parent.transform.Find("Matte navy frame").localScale=new Vector3(2.26f,height+.06f,.035f);
            Check(texture.width==1672&&texture.height==941,"Wall importer preserves original NPOT dimensions 1672x941");
            Check(parent.GetComponentsInChildren<Collider>().Length==0,"Wall art remains nonblocking");
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            Capture(new SceneReviewCapture.View{file="01-grafica-pared.png",position=new Vector3(-1.2f,1.9f,-2.5f),target=parent.transform.position,fieldOfView=65});
            CaptureOfficeViews();
        }
        static void InstallPrologue()
        {
            Check(GameObject.Find("ART - Bank Wall Graphic")!=null,"Wall stage installed before prologue");
            const string reference="Assets/_Project/Art/Prologue/Reference/";
            var paths=new[]{reference+"prologo_01_impresora_origen.png",reference+"prologo_02_impresora_detenida.png",AssetsOut+"/Prologue_BancoReal.png"};
            if(!File.Exists(paths[2]))Capture(new SceneReviewCapture.View{file="Prologue_BancoReal.png",position=new Vector3(1.9f,2.2f,-.9f),target=new Vector3(0,1.1f,1.6f)},paths[2]);
            AssetDatabase.ImportAsset(paths[2],ImportAssetOptions.ForceSynchronousImport);
            var textures=new Texture2D[3];
            for(int i=0;i<3;i++) {
                var importer=(TextureImporter)AssetImporter.GetAtPath(paths[i]);Check(importer!=null,"Prologue source importer "+i);
                importer.textureType=TextureImporterType.Default;importer.npotScale=TextureImporterNPOTScale.None;importer.wrapMode=TextureWrapMode.Clamp;
                importer.SaveAndReimport();textures[i]=AssetDatabase.LoadAssetAtPath<Texture2D>(paths[i]);Check(textures[i]!=null,"Prologue texture available "+i);
            }
            var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();
            var intro=game.GetComponent<IntroSequence>();if(intro==null)intro=Undo.AddComponent<IntroSequence>(game.gameObject);
            Undo.RecordObject(intro,"Assign illustrated prologue");intro.Illustrations=textures;
            // Pixel bounds measured from the individual 1920x1080 sources; discard padding, not content.
            intro.IllustrationUV=new[]{new Rect(0,373f/1080,1,335f/1080),new Rect(0,372f/1080,1,333f/1080),new Rect(0,0,1,1)};
            Check(Mathf.Approximately(intro.ShotSeconds,4),"Existing 4-second shot timing preserved");
            EditorUtility.SetDirty(intro);EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        }
        static IEnumerator ReviewPrologue()
        {
            File.AppendAllText("Artifacts/ArtIntegrationReview/checks.txt","\nPROLOGUE RUN "+DateTime.UtcNow.ToString("O")+"\n");
            yield return null;yield return null;
            var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();var camera=Camera.main;
            var intro=game.GetComponent<IntroSequence>();Check(intro!=null,"Serialized illustrated IntroSequence exists");
            // Refresh our derived third illustration in Play Mode so it includes the current native document UI.
            string third=AssetsOut+"/Prologue_BancoReal.png";
            Capture(new SceneReviewCapture.View{file="Prologue_BancoReal.png",position=new Vector3(1.9f,2.2f,-.9f),target=new Vector3(0,1.1f,1.6f)},third);
            AssetDatabase.ImportAsset(third,ImportAssetOptions.ForceSynchronousImport);
            intro.Illustrations[2]=AssetDatabase.LoadAssetAtPath<Texture2D>(third);
            yield return null;
            var position=camera.transform.position;var rotation=camera.transform.rotation;
            int starts=0;Action changed=()=>starts++;game.Changed+=changed;
            game.EnterScenario(Swift.Bank.ScenarioMode.Attack);yield return null;
            Check(game.IntroPlaying&&game.Session==null,"Attack portal defers session until prologue completes");
            CheckIntroText();
            var skip=UnityEngine.Object.FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).Single(x=>x.Action==Swift.Interaction.BankAction.SkipIntro);
            Physics.SyncTransforms();var direction=skip.GetComponent<Collider>().bounds.center-position;
            Check(Physics.Raycast(position,direction.normalized,out var hit,direction.magnitude+.05f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<Swift.Interaction.BankInteractable>()==skip,"Skip control reachable from actual player camera");
            skip.Activate();yield return null;
            Check(!game.IntroPlaying&&game.Session.Mode==Swift.Bank.ScenarioMode.Attack&&starts==1,"Skip starts selected attack mode exactly once");
            game.SkipIntro();Check(starts==1,"Repeated Skip cannot start another session");
            starts=0;var previousSession=game.Session;
            intro.Play(()=>game.StartScenario(Swift.Bank.ScenarioMode.Defense));
            float began=Time.realtimeSinceStartup,momentBegan=began;int previousMoment=-1;
            var captured=new System.Collections.Generic.HashSet<int>();
            while(intro.IsPlaying&&Time.realtimeSinceStartup-began<16) {
                int moment=intro.CurrentMoment;
                if(moment!=previousMoment) {previousMoment=moment;momentBegan=Time.realtimeSinceStartup;}
                if(moment>=0&&!captured.Contains(moment)&&Time.realtimeSinceStartup-momentBegan>.65f) {
                    Check(game.Session==previousSession&&starts==0,"No early or repeated session start at moment "+moment);
                    Check(intro.Illustration!=null&&intro.Illustration.texture!=null,"Illustrated moment loaded "+moment);
                    var image=intro.Illustration;float expected=image.texture.width*image.uvRect.width/(image.texture.height*image.uvRect.height);
                    Check(Mathf.Abs(image.rectTransform.rect.width/image.rectTransform.rect.height-expected)<.001f,"Illustration aspect preserved "+moment);
                    CheckIntroText();
                    Capture(new SceneReviewCapture.View{file=(moment==0?"02-prologo-impresora.png":moment==1?"03-prologo-detenida.png":"04-prologo-banco-real.png"),position=position,target=position+camera.transform.forward*2.5f,fieldOfView=60});
                    captured.Add(moment);
                }
                CheckCamera(camera,position,rotation);
                yield return null;
            }
            Check(captured.Count==3,"All three production moments captured");
            Check(!intro.IsPlaying&&intro.HasPlayed&&Time.realtimeSinceStartup-began>=11.8f,"Normal 12-second sequence ends automatically");
            Check(game.Session.Mode==Swift.Bank.ScenarioMode.Defense&&starts==1,"Automatic completion starts selected defense mode exactly once");
            game.Changed-=changed;
            Check(UnityEngine.Object.FindFirstObjectByType<Swift.UI.BankDocumentUI>()!=null,"Existing document presentation still installed");
            Capture(new SceneReviewCapture.View{file="05-oficina-partida.png",position=new Vector3(0,2.4f,-4.2f),target=new Vector3(0,1.3f,1),fieldOfView=85});
            Check(errors==0,"No runtime errors during illustrated prologue review");
        }
        static void CheckCamera(Camera camera,Vector3 position,Quaternion rotation)
        {
            if(Vector3.Distance(camera.transform.position,position)>.001f||Quaternion.Angle(camera.transform.rotation,rotation)>.1f)throw new Exception("Player camera moved during prologue review");
        }
        static void CheckIntroText()
        {
            Canvas.ForceUpdateCanvases();var stage=GameObject.Find("Prologo - Banco Andino (recreacion)");Check(stage!=null,"Stable prologue stage exists");
            foreach(var text in stage.GetComponentsInChildren<TMPro.TextMeshProUGUI>()) {text.ForceMeshUpdate();Check(!text.isTextOverflowing,"No prologue TMP overflow: "+text.text);}
        }
        static IEnumerator RunProbe(string command)
        {
            string resultPath=command=="flow"?"Artifacts/GameplayFlow/result.txt":command=="office"?"Artifacts/Office20261028/playmode-office.txt":"Artifacts/playmode-smoke.txt";
            if(File.Exists(resultPath))File.Delete(resultPath); // Previous outputs were copied to PreviousEvidence before this opt-in run.
            var target=new GameObject("Art review - existing "+command+" probe");
            if(command=="flow")target.AddComponent<Swift.Testing.GameplayFlowProbe>().ExitEditorOnComplete=false;
            else if(command=="office")target.AddComponent<Swift.Testing.BankOfficeProbe>().ExitEditorOnComplete=false;
            else target.AddComponent<Swift.Testing.PrototypeSmokeProbe>().ExitEditorOnComplete=false;
            float deadline=Time.realtimeSinceStartup+45;
            while(!File.Exists(resultPath)&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(File.Exists(resultPath),"Existing "+command+" probe completed");
            var result=File.ReadAllText(resultPath);File.WriteAllText("Artifacts/ArtIntegrationReview/"+command+"-probe.txt",result);
            Check(result.StartsWith("PASS"),result);
        }
        static void CaptureOfficeViews()
        {
            Capture(new SceneReviewCapture.View{file="06-oficina-general.png",position=new Vector3(0,2.65f,-4.5f),target=new Vector3(0,1.1f,1),fieldOfView=85});
            Capture(new SceneReviewCapture.View{file="07-escritorio.png",position=new Vector3(1.9f,2.2f,-.9f),target=new Vector3(0,1.1f,1.6f)});
            Capture(new SceneReviewCapture.View{file="08-computadora.png",position=new Vector3(-.15f,1.85f,-.3f),target=new Vector3(-.15f,1.4f,1.4f),fieldOfView=65});
            Capture(new SceneReviewCapture.View{file="09-distribucion-superior.png",position=new Vector3(0,12,0),target=Vector3.zero,orthographicSize=5.7f,hideCeiling=true});
        }
        static void Capture(SceneReviewCapture.View view,string path=null)
        {
            Canvas.ForceUpdateCanvases();Physics.SyncTransforms();
            path=path??Path.Combine(Output,view.file);
            typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{SceneManager.GetActiveScene(),view,path});
            File.AppendAllText(Path.Combine(Output,"capture-receipt.txt"),DateTime.UtcNow.ToString("O")+" "+view.file+" Unity="+Application.unityVersion+" Play="+EditorApplication.isPlaying+" eye="+view.position+" target="+view.target+" NOT_ANDROID\n");
        }
    }
}

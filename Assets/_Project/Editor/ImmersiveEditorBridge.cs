using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Swift.UI;
using Swift.DeskTools;
using Swift.Interaction;

namespace Swift.EditorTools
{
    // Local, explicit file commands only; no network endpoint and no scene work on import.
    [InitializeOnLoad]
    public static class ImmersiveEditorBridge
    {
        public const string Dir="Artifacts/ImmersiveGameplay";
        public const string Gallery="Docs/Previews/ImmersiveGameplay/20261110";
        public const string Main="Assets/_Project/Scenes/BancoCorresponsal.unity";
        public const string Trial="Assets/_Project/Scenes/Review/BancoCorresponsal_Immersive_20261110.unity";
        public const string ArtReview="Assets/_Project/Scenes/Review/BancoCorresponsal_ArtDirection_20261110.unity";
        public const string AssetRoot="Assets/_Project/DeskTools";
        const string Pending="SWIFT_IMMERSIVE_PLAY";
        public static string Running { get; private set; }
        static ImmersiveEditorBridge(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static void Tick()
        {
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            // Refresh is available during future iterations even when auto-refresh is disabled.
            string refresh=Dir+"/refresh.txt";
            if(File.Exists(refresh)&&!EditorApplication.isPlayingOrWillChangePlaymode){File.Delete(refresh);AssetDatabase.Refresh();return;}
            if(SessionState.GetBool("SWIFT_IMMERSIVE_BUILD_SWITCH",false)&&EditorUserBuildSettings.activeBuildTarget==BuildTarget.Android&&!EditorApplication.isPlayingOrWillChangePlaymode){
                SessionState.EraseBool("SWIFT_IMMERSIVE_BUILD_SWITCH");try{Build();}catch(Exception ex){Result("build",ex);}return;
            }
            var queue=Dir+"/command.txt";if(!File.Exists(queue)||EditorApplication.isPlayingOrWillChangePlaymode)return;
            Running=File.ReadAllText(queue).Trim();File.Delete(queue);
            try {
                if(Running=="status"){Status();Result(Running,null);return;}
                if(Running=="pilot-prepare"){Sim003PilotEditorSetup.Prepare();Result(Running,null);return;}
                if(Running=="art-baseline"){ArtDirectionPolish.Baseline();Result(Running,null);return;}
                if(Running=="art-material-variant"){ArtDirectionPolish.MaterialVariant();Result(Running,null);return;}
                if(Running=="art-architecture-variant"){ArtDirectionPolish.ArchitectureVariant();Result(Running,null);return;}
                if(Running=="art-lighting-variant"){ArtDirectionPolish.LightingVariant();Result(Running,null);return;}
                if(Running=="art-apply-main"){ArtDirectionPolish.ApplyMain();Result(Running,null);return;}
                if(Running=="art-open-variant"){
                    Check(SceneManager.GetActiveScene().path==ArtReview,"Only the saved art review may be reloaded");
                    Check(Hash(ArtReview)==File.ReadAllText("Artifacts/ArtDirectionPolish/AfterLighting-inventory.txt").Split('\n').First(x=>x.StartsWith("SceneSHA256=")).Trim().Substring(12),"Saved art review still matches its capture");
                    EditorSceneManager.OpenScene(ArtReview,OpenSceneMode.Single);Result(Running,null);return;
                }
                if(Running=="art-open-main"){
                    Check(SceneManager.GetActiveScene().path==Main,"Only the integrated main may be reloaded");
                    Check(Hash(Main)==File.ReadAllText("Artifacts/ArtDirectionPolish/main-after.sha256"),"Saved main still matches its final capture");
                    EditorSceneManager.OpenScene(Main,OpenSceneMode.Single);Result(Running,null);return;
                }
                if(Running=="restore-build-preloads"){
                    var preloads=PlayerSettings.GetPreloadedAssets();
                    File.WriteAllLines(Dir+"/preloads-inspection.txt",new[]{"Count="+preloads.Length}.Concat(preloads.Select(a=>a==null?"<null>":a.GetType().FullName+" path="+AssetDatabase.GetAssetPath(a)+" guid="+AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(a)))));
                    var expected=new[]{"052faaac586de48259a63d0c4782560b","7a2ee1bd3e7b6f841a8fc2bdda8370b1"};
                    Check(!EditorApplication.isPlayingOrWillChangePlaymode&&(preloads.Length==0||preloads.Length==2&&preloads.All(a=>a!=null&&expected.Contains(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(a))))),"Only known Input/XR build-generated preloads; initial list was empty");
                    File.WriteAllLines(Dir+"/build-generated-preloads.txt",preloads.Select(a=>AssetDatabase.GetAssetPath(a)));
                    PlayerSettings.SetPreloadedAssets(Array.Empty<UnityEngine.Object>());AssetDatabase.SaveAssets();
                    // Postprocessors already restore the in-memory empty list, but may leave their saved build-only entries on disk.
                    const string settings="ProjectSettings/ProjectSettings.asset";
                    string text=File.ReadAllText(settings).Replace("\r\n","\n");
                    const string generated="  preloadedAssets:\n  - {fileID: -944628639613478452, guid: 052faaac586de48259a63d0c4782560b, type: 3}\n  - {fileID: 6697253344292378036, guid: 7a2ee1bd3e7b6f841a8fc2bdda8370b1, type: 2}";
                    string restored=text.Replace(generated,"  preloadedAssets: []");
                    using(var sha=SHA256.Create())Check(BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(restored))).Replace("-","")=="1C8FF806FF1D84A59C0017F0C175E7C78680BA8CECF47935503FBEE307CD9F3D","Entire restored settings must equal initial backup; no unrelated edit overwritten");
                    File.WriteAllText(settings,restored,new System.Text.UTF8Encoding(false));
                    Result(Running,null);return;
                }
                if(Running=="capture-office"){SceneReviewCapture.CaptureBatch();Result(Running,null);return;}
                if(Running=="save-main"){Check(SceneManager.GetActiveScene().path==Main,"Only integrated main can be saved");Check(Hash(Main)==File.ReadAllText(Dir+"/main-integrated-sha.txt"),"Saved main still matches reviewed integration");Status();Undo.FlushUndoRecordObjects();EditorSceneManager.SaveScene(SceneManager.GetActiveScene());File.WriteAllText(Dir+"/main-integrated-sha.txt",Hash(Main));Result(Running,null);return;}
                if(Running=="save-trial"){Check(SceneManager.GetActiveScene().path==Trial,"Only owned review scene can be saved");Check(Hash(Main)==File.ReadAllText(Dir+"/main-before-sha.txt"),"Main still preserved");EditorSceneManager.SaveScene(SceneManager.GetActiveScene());Status();Result(Running,null);return;}
                if(Running=="prepare"){Prepare();Result(Running,null);return;}
                if(Running=="enable-phone"||Running=="enable-objects"){EnableStage(Running);Result(Running,null);return;}
                if(Running=="apply"){Apply();Result(Running,null);return;}
                if(Running=="build"){
                    if(EditorUserBuildSettings.activeBuildTarget!=BuildTarget.Android){
                        SessionState.SetBool("SWIFT_IMMERSIVE_BUILD_SWITCH",true);
                        if(!EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.Android,BuildTarget.Android))throw new Exception("Android target switch rejected.");
                    }else Build();return;
                }
                if(!new[]{"baseline","clipboard","phone","objects","regression","stationpc","closeui","tutorial","flow","office","smoke","welcome-before","welcome-after","physical-before","physical-after","pilot-attack","pilot-defense","pilot-attack-print"}.Contains(Running))throw new Exception("Unknown immersive command.");
                Check(SceneManager.sceneCount==1&&!SceneManager.GetActiveScene().isDirty,"Single clean scene; no unsaved work discarded");
                if(Running=="baseline")Check(SceneManager.GetActiveScene().path==Main,"Baseline uses saved main");
                else Check(SceneManager.GetActiveScene().path==Trial||SceneManager.GetActiveScene().path==Main||SceneManager.GetActiveScene().path==ArtReview||SceneManager.GetActiveScene().path==Sim003PilotEditorSetup.ReviewScene,"Expected bank or isolated trial");
                SessionState.SetString(Pending,Running);EditorApplication.EnterPlaymode();
            }catch(Exception ex){Result(Running,ex);}
        }
        static void PlayChanged(PlayModeStateChange state)
        {
            if(state!=PlayModeStateChange.EnteredPlayMode)return;
            var run=SessionState.GetString(Pending,"");if(run=="")return;SessionState.EraseString(Pending);Running=run;
            try {
                var type=typeof(Swift.Game.GameManager).Assembly.GetType("Swift.Testing.ImmersivePlayProbe");
                if(type==null)throw new Exception("ImmersivePlayProbe has not compiled.");
                var probe=new GameObject("Immersive verification - transient").AddComponent(type);
                type.GetField("Mode").SetValue(probe,run);
                EditorApplication.ExecuteMenuItem("Window/General/Game");
            }catch(Exception ex){Result(run,ex);EditorApplication.ExitPlaymode();}
        }
        static void Check(bool value,string label){if(!value)throw new Exception(label);File.AppendAllText(Dir+"/editor-checks.txt","PASS "+label+"\n");}
        public static void Result(string run,Exception error)
        {
            Directory.CreateDirectory(Dir);var value=(error==null?"PASS ":"FAIL ")+run+" "+DateTime.UtcNow.ToString("O")+" Unity="+Application.unityVersion+" EDITOR ONLY; ANDROID DEVICE NOT TESTED\n"+error;
            File.WriteAllText(Dir+"/"+run+"-result.txt",value);File.AppendAllText(Dir+"/result-history.txt",value+"\n");
            if(error==null)Debug.Log(value);else Debug.LogError(value);
        }
        static void Status()
        {
            File.WriteAllLines(Dir+"/editor-status.txt",new[]{
                "Unity="+Application.unityVersion,"Target="+EditorUserBuildSettings.activeBuildTarget,
                "Playing="+EditorApplication.isPlaying,"SceneCount="+SceneManager.sceneCount,"Scene="+SceneManager.GetActiveScene().path,
                "Dirty="+SceneManager.GetActiveScene().isDirty,"CompilationAssemblies="+UnityEditor.Compilation.CompilationPipeline.GetAssemblies().Length });
            File.WriteAllLines(Dir+"/dirty-components.txt",UnityEngine.Object.FindObjectsByType<Component>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>EditorUtility.IsDirty(c)).Select(c=>c.name+" / "+c.GetType().FullName));
            var consoleType=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("UnityEditor.LogEntries")??a.GetType("UnityEditorInternal.LogEntries")).FirstOrDefault(t=>t!=null);
            var counts=consoleType?.GetMethod("GetCounts",System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);
            if(counts!=null){var args=new object[]{0,0,0};counts.Invoke(null,args);File.AppendAllText(Dir+"/editor-status.txt","\nConsoleErrors="+args[0]+" ConsoleWarnings="+args[1]+" ConsoleLogs="+args[2]+" (history preserved)\n");}
            var transforms=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            File.WriteAllLines(Dir+"/object-inventory.txt",transforms.Where(t=>t.name.Contains("clipboard")||t.name.Contains("Printer")||t.name.Contains("documents")||t.name=="chairDesk")
                .Select(t=>t.name+" pos="+t.position+" rot="+t.eulerAngles+" colliders="+t.GetComponentsInChildren<Collider>().Length));
        }
        static void Prepare()
        {
            Check(SceneManager.GetActiveScene().path==Main&&!SceneManager.GetActiveScene().isDirty,"Saved main preserved before trial copy");
            Check(File.Exists(Dir+"/backup-path.txt"),"Recovery backup recorded");
            Check(!File.Exists(Trial),"Never overwrite an existing trial scene");
            File.WriteAllText(Dir+"/main-before-sha.txt",Hash(Main));
            Directory.CreateDirectory(Path.GetDirectoryName(Trial));Directory.CreateDirectory(AssetRoot+"/Prefabs");Directory.CreateDirectory(AssetRoot+"/Prototypes");
            AssetDatabase.Refresh();
            Check(AssetDatabase.CopyAsset(Main,Trial),"Isolated scene copied through AssetDatabase with new GUID");
            EditorSceneManager.OpenScene(Trial,OpenSceneMode.Single);
            var owner=UnityEngine.Object.FindFirstObjectByType<UIManager>();
            var font=owner.Font!=null?owner.Font:Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");Check(font!=null,"Existing TMP font reused");
            var panel=new GameObject("Desk inspection",typeof(RectTransform),typeof(Canvas),typeof(DeskInspectionView));
            panel.GetComponent<DeskInspectionView>().Build(font,owner.StationFrame,owner.StationFill);
            var inspection=PrefabUtility.SaveAsPrefabAsset(panel,AssetRoot+"/Prefabs/DeskInspectionPanel.prefab");UnityEngine.Object.DestroyImmediate(panel);
            var phone=CreatePhone(owner,font);
            var phonePrefab=PrefabUtility.SaveAsPrefabAsset(phone,AssetRoot+"/Prefabs/VerificationPhone.prefab");UnityEngine.Object.DestroyImmediate(phone);
            var clipboard=GameObject.Find("Analyst review clipboard");Check(clipboard!=null,"Existing clipboard geometry reused");
            var copy=UnityEngine.Object.Instantiate(clipboard);copy.name="VerificationClipboard";
            foreach(var action in copy.GetComponentsInChildren<BankInteractable>(true))action.Manager=null;
            PrefabUtility.SaveAsPrefabAsset(copy,AssetRoot+"/Prefabs/VerificationClipboard.prefab");UnityEngine.Object.DestroyImmediate(copy);
            var catalog=ScriptableObject.CreateInstance<VerificationCallCatalog>();
            Check(catalog.Validate(owner.Manager.Scenario.Transactions)==null,"Prototype call validates against current scenario");
            AssetDatabase.CreateAsset(catalog,AssetRoot+"/Prototypes/SIM003_CallPrototype.asset");
            var profile=ScriptableObject.CreateInstance<DeskToolsProfile>();profile.ClipboardEnabled=true;profile.AllowPrototypeCalls=true;
            profile.Calls=catalog;profile.PhonePrefab=phonePrefab;profile.InspectionPrefab=inspection;profile.Reference=ReferencePresentation.ClipboardTrial;
            AssetDatabase.CreateAsset(profile,AssetRoot+"/DeskTools_Trial.asset");
            Undo.RecordObject(owner,"Opt-in isolated desk tools");owner.DeskTools=profile;EditorUtility.SetDirty(owner);
            Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Only isolated trial saved");
            AssetDatabase.SaveAssetIfDirty(profile);Check(Hash(Main)==File.ReadAllText(Dir+"/main-before-sha.txt"),"Main byte content unchanged after staging");
            Status();
        }
        static Material Material(string name)
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/"+name+".mat");
            Check(material!=null,"Existing material reused: "+name);return material;
        }
        static void Cube(Transform parent,string name,Vector3 point,Vector3 size,Material material)
        {
            var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);cube.name=name;cube.transform.SetParent(parent,false);cube.transform.localPosition=point;cube.transform.localScale=size;
            cube.GetComponent<MeshRenderer>().sharedMaterial=material;cube.GetComponent<MeshRenderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            UnityEngine.Object.DestroyImmediate(cube.GetComponent<Collider>());
        }
        static GameObject CreatePhone(UIManager owner,TMPro.TMP_FontAsset font)
        {
            var go=new GameObject("Verification phone - fictional prototype");var plastic=Material("Plastic_EquipmentMatte");var keys=Material("Plastic_Keycaps");var metal=Material("Metal_AluminiumTrim");
            Cube(go.transform,"Phone base",new Vector3(0,.035f,0),new Vector3(.36f,.07f,.27f),plastic);
            Cube(go.transform,"Handset bridge",new Vector3(0,.12f,.075f),new Vector3(.28f,.04f,.055f),metal);
            foreach(float x in new[]{-.14f,.14f})Cube(go.transform,"Handset earpiece",new Vector3(x,.105f,.075f),new Vector3(.075f,.065f,.10f),plastic);
            for(int r=0;r<4;r++)for(int c=0;c<3;c++)Cube(go.transform,"Key "+r+"-"+c,new Vector3((c-1)*.043f,.077f,-.025f-r*.036f),new Vector3(.030f,.014f,.023f),keys);
            Cube(go.transform,"Status indicator",new Vector3(.135f,.08f,-.10f),new Vector3(.026f,.012f,.018f),metal);
            var box=go.AddComponent<BoxCollider>();box.size=new Vector3(.39f,.20f,.30f);box.center=new Vector3(0,.085f,0);
            var bank=go.AddComponent<BankInteractable>();bank.Action=BankAction.DeskTool;
            go.AddComponent<DeskActionTarget>().Command=DeskCommand.Phone;
            var canvas=new GameObject("Phone status / mounted",typeof(RectTransform),typeof(Canvas));canvas.transform.SetParent(go.transform,false);
            canvas.transform.localPosition=new Vector3(0,.22f,.10f);canvas.transform.localScale=Vector3.one*.001f;canvas.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;canvas.GetComponent<RectTransform>().sizeDelta=new Vector2(420,98);
            var background=new GameObject("Status housing",typeof(RectTransform),typeof(UnityEngine.UI.Image));background.transform.SetParent(canvas.transform,false);background.GetComponent<RectTransform>().sizeDelta=new Vector2(420,98);background.GetComponent<UnityEngine.UI.Image>().color=new Color(.02f,.12f,.16f);background.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
            var text=owner.Text(background.transform,"Status",Vector2.zero,new Vector2(400,78),25);text.text="CONTRASTE / SIM-003";text.font=font;text.alignment=TMPro.TextAlignmentOptions.Center;
            // Three shared-material meshes instead of one renderer for every key/cube.
            Directory.CreateDirectory(AssetRoot+"/Geometry");AssetDatabase.Refresh();
            var pieces=go.GetComponentsInChildren<MeshFilter>();
            int meshIndex=0;
            foreach(var group in pieces.GroupBy(x=>x.GetComponent<MeshRenderer>().sharedMaterial)) {
                var mesh=new Mesh{name="Institutional phone "+meshIndex};
                mesh.CombineMeshes(group.Select(x=>new CombineInstance{mesh=x.sharedMesh,transform=go.transform.worldToLocalMatrix*x.transform.localToWorldMatrix}).ToArray(),true,true);
                AssetDatabase.CreateAsset(mesh,AssetRoot+"/Geometry/Phone_"+meshIndex+".asset");
                var part=new GameObject(mesh.name,typeof(MeshFilter),typeof(MeshRenderer));part.transform.SetParent(go.transform,false);
                part.GetComponent<MeshFilter>().sharedMesh=mesh;part.GetComponent<MeshRenderer>().sharedMaterial=group.Key;
                part.GetComponent<MeshRenderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;meshIndex++;
            }
            foreach(var piece in pieces)UnityEngine.Object.DestroyImmediate(piece.gameObject);
            return go;
        }
        static void EnableStage(string stage)
        {
            Check(SceneManager.GetActiveScene().path==Trial&&!SceneManager.GetActiveScene().isDirty,"Only clean isolated trial enabled");
            var profile=UnityEngine.Object.FindFirstObjectByType<UIManager>().DeskTools;Check(profile!=null,"Trial profile exists");
            Check(File.Exists(Dir+"/clipboard-result.txt")&&File.ReadAllText(Dir+"/clipboard-result.txt").StartsWith("PASS"),"Clipboard validated first");
            if(stage=="enable-phone")profile.PhoneEnabled=true;
            else {Check(File.ReadAllText(Dir+"/phone-result.txt").StartsWith("PASS"),"Phone validated first");profile.SeatPolishEnabled=true;profile.PrinterFeedbackEnabled=true;profile.DocumentInspectionEnabled=true;}
            EditorUtility.SetDirty(profile);AssetDatabase.SaveAssetIfDirty(profile);
        }
        static void Apply()
        {
            foreach(var test in new[]{"clipboard","phone","objects","regression","stationpc","tutorial","flow","office","smoke"})Check(File.ReadAllText(Dir+"/"+test+"-result.txt").StartsWith("PASS"),"Integration gate: "+test);
            Check(!SceneManager.GetActiveScene().isDirty,"No unsaved trial edits discarded");
            Check(Hash(Main)==File.ReadAllText(Dir+"/main-before-sha.txt"),"Main not modified by another agent since staging");
            var profile=UnityEngine.Object.FindFirstObjectByType<UIManager>().DeskTools;EditorSceneManager.OpenScene(Main,OpenSceneMode.Single);
            var owner=UnityEngine.Object.FindFirstObjectByType<UIManager>();Undo.RecordObject(owner,"Integrate validated desk tools opt-in");
            owner.DeskTools=profile;EditorUtility.SetDirty(owner);
            Undo.FlushUndoRecordObjects();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Main integration saved through Unity API; office retained");
            File.WriteAllText(Dir+"/main-integrated-sha.txt",Hash(Main));
        }
        static void Build()
        {
            Check(SceneManager.GetActiveScene().path==Main&&!SceneManager.GetActiveScene().isDirty,"Build current saved main");
            Check(File.ReadAllText(Dir+"/regression-result.txt").StartsWith("PASS"),"Regression passed before build");
            Check(EditorUserBuildSettings.activeBuildTarget==BuildTarget.Android,"Existing Android support active");
            var directory=Dir+"/Build-20261110-"+DateTime.UtcNow.ToString("HHmmss");Directory.CreateDirectory(directory);
            string apk=directory+"/SWIFT-Immersive-v1-20261110.apk";Check(!File.Exists(apk),"New APK path");
            Application.LogCallback log=(message,stack,type)=>File.AppendAllText(directory+"/unity-build.log",type+": "+message+"\n"+(type==LogType.Exception?stack+"\n":""));
            Application.logMessageReceived+=log;BuildReport report;
            try{report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Main},locationPathName=apk,target=BuildTarget.Android,options=BuildOptions.Development|BuildOptions.DetailedBuildReport});}
            finally{Application.logMessageReceived-=log;}
            File.WriteAllLines(directory+"/build-report.txt",new[]{"Result="+report.summary.result,"Errors="+report.summary.totalErrors,"Warnings="+report.summary.totalWarnings,"Bytes="+report.summary.totalSize,"Duration="+report.summary.totalTime,"Unity="+Application.unityVersion,"AppVersion="+PlayerSettings.bundleVersion,"ArtifactVersion=Immersive-v1-20261110","InitialScene="+Main,"DeviceTest=NOT_RUN"});
            Check(report.summary.result==BuildResult.Succeeded&&report.summary.totalErrors==0,"Android build succeeded");
            File.WriteAllText(directory+"/SHA256.txt",Hash(apk)+"  "+Path.GetFileName(apk));File.WriteAllText(Dir+"/apk-path.txt",Path.GetFullPath(apk));Result("build",null);
        }
        public static string Hash(string path){using(var sha=SHA256.Create())using(var stream=File.OpenRead(path))return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");}
    }
}

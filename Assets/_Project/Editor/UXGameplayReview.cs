using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Swift.UI;
using Swift.Audio;
using Swift.Game;
using Swift.Bank;
using Swift.Interaction;
namespace Swift.EditorTools
{
    [InitializeOnLoad]
    public static class UXGameplayReview
    {
        const string Queue="Artifacts/UXGameplayReview/command.txt",Pending="SWIFT_UX_REVIEW";
        const string ScenePath="Assets/_Project/Scenes/BancoCorresponsal.unity";
        static IEnumerator routine;static string command;static int errors;static bool ownsPlay;
        static UXGameplayReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=PlayChanged;}
        static string Output {
            get {const string file="Artifacts/UXGameplayReview/output-path.txt";if(!File.Exists(file))File.WriteAllText(file,"Docs/Previews/UXGameplayReview/"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ"));var path=File.ReadAllText(file).Trim();Directory.CreateDirectory(path);return path;}
        }
        static void Tick()
        {
            if(routine!=null){try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
            if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||!File.Exists(Queue))return;
            try {
                command=File.ReadAllText(Queue).Trim();File.Delete(Queue);
                if(command=="save") {
                    Check(SceneManager.sceneCount==1&&SceneManager.GetActiveScene().path==ScenePath,"Only bank scene open for authorized save");
                    Check(File.Exists("Artifacts/UXGameplayReview/backup-path.txt"),"Backup exists before saving");
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
                    var dependencies=AssetDatabase.GetDependencies(ScenePath,true);
                    File.WriteAllLines("Artifacts/UXGameplayReview/scene-dependencies.txt",dependencies);
                    Check(dependencies.Where(x=>x.StartsWith("Assets/")).All(File.Exists),"All native scene asset dependencies exist on disk");
                    var roots=SceneManager.GetActiveScene().GetRootGameObjects();
                    Check(roots.SelectMany(x=>x.GetComponentsInChildren<MonoBehaviour>(true)).All(x=>x!=null),"No missing MonoBehaviour scripts in saved bank scene");
                    Check(LightmapSettings.lightmaps.Length>0&&LightmapSettings.lightmaps.All(x=>x.lightmapColor!=null),"Saved bank lighting and lightmap textures available");
                    Check(roots.SelectMany(x=>x.GetComponentsInChildren<Camera>(true)).Any(x=>x.CompareTag("MainCamera")),"Player camera preserved in saved bank");
                    Finish(null);return;
                }
                if(Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Unsaved scene preserved; save/review it before tests");
                PrepareAudio();
                if(command=="prepare"){Finish(null);return;}
                if(command!="review"&&command!="flow"&&command!="office"&&command!="smoke")throw new Exception("Unknown UX review command");
                EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);SessionState.SetString(Pending,command);EditorApplication.EnterPlaymode();
            }catch(IOException){/* A non-atomic writer may still own the queue; retry on next update. */}
            catch(Exception e){Finish(e);}
        }
        static void PrepareAudio()
        {
            const string path="Assets/_Project/Resources/Audio/SWIFT_AudioCues.asset";
            if(File.Exists(path))return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));AssetDatabase.Refresh();
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AudioCueLibrary>(),path);AssetDatabase.SaveAssets();
        }
        static void PlayChanged(PlayModeStateChange state)
        {
            if(state!=PlayModeStateChange.EnteredPlayMode)return;
            command=SessionState.GetString(Pending,"");if(command=="")return;SessionState.EraseString(Pending);
            ownsPlay=true;errors=0;Application.logMessageReceived+=Log;routine=command=="review"?Review():Probe(command);
        }
        static void Log(string value,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;}
        static void Finish(Exception error)
        {
            routine=null;Application.logMessageReceived-=Log;
            string result=error==null&&errors==0?"PASS "+command+" "+DateTime.UtcNow.ToString("O")+" Editor only; Android NOT TESTED":"FAIL "+command+" "+(error?.ToString()??"runtime errors="+errors);
            File.WriteAllText("Artifacts/UXGameplayReview/"+command+"-result.txt",result);Debug.Log(result);
            if(ownsPlay&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();ownsPlay=false;
        }
        static void Check(bool value,string label){if(!value)throw new Exception(label);File.AppendAllText("Artifacts/UXGameplayReview/checks.txt","PASS "+label+"\n");}
        static IEnumerator Review()
        {
            File.AppendAllText("Artifacts/UXGameplayReview/checks.txt","\nRUN "+DateTime.UtcNow.ToString("O")+"\n");
            yield return null;yield return null;
            var game=UnityEngine.Object.FindFirstObjectByType<GameManager>();var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();
            var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var visual=gaze.GetComponent<GazeProgressVisual>();
            var tutorial=UnityEngine.Object.FindFirstObjectByType<VRQuickTutorial>();var audio=FeedbackAudio.Current;
            Check(audio!=null&&audio.Library!=null,"Audio library and independent category buses loaded");
            Check(game.Session==null&&tutorial!=null,"Optional tutorial begins outside all banking sessions");
            Check(!BankDocumentUI.InputHelp(true).Contains("PC:"),"Android help contains no PC controls");
            int audioCount=audio.EmittedCount;for(int i=0;i<10;i++)game.Notify("Elige un modo o practica el tutorial.");yield return null;
            Check(audio.EmittedCount==audioCount,"Ten message refreshes produce no audio spam");
            Capture("01-menu-tutorial.png");CheckText(ui.PanelAnchor.GetComponentInChildren<Canvas>().transform);
            var camera=gaze.View.transform;var oldPosition=camera.position;var oldRotation=camera.rotation;bool oldSimulation=gaze.SimulateGazeInEditor;
            gaze.SimulateGazeInEditor=true;camera.position=new Vector3(0,1.6f,-.65f);
            var start=UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Single(x=>x.Action==BankAction.TutorialStart);
            camera.LookAt(start.transform.position);float deadline=Time.realtimeSinceStartup+5;
            while(gaze.FocusedTarget!=start&&Time.realtimeSinceStartup<deadline)yield return null;
            while(gaze.DwellProgress<.65f&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!tutorial.IsRunning&&!gaze.SelectionLatched,"No tutorial activation before 1.5-second dwell");
            Capture("02-mirada-progreso.png");
            while(!tutorial.IsRunning&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(tutorial.IsRunning,"Actual gaze opens optional tutorial");
            Check(Mathf.Approximately(gaze.DwellSeconds,1.5f),"Production dwell remains 1.5 seconds");
            Check(gaze.Reticle.GetComponent<Renderer>().sharedMaterial.GetColor("_BaseColor")==Color.white,"Central point is white with separate dark outline");
            CheckText(gaze.Reticle);
            int bankChanges=0;Action changed=()=>bankChanges++;game.Changed+=changed;
            for(int step=0;step<5;step++) {
                // Look away between uses of the same primary target; test real latch behavior.
                camera.rotation=Quaternion.LookRotation(Vector3.up);float away=Time.realtimeSinceStartup+.12f;
                while(Time.realtimeSinceStartup<away)yield return null;
                var target=step==4?tutorial.DecisionTargets[0]:tutorial.PrimaryTarget;
                camera.LookAt(target.transform.position);Physics.SyncTransforms();
                var ray=target.GetComponent<Collider>().bounds.center-camera.position;
                Check(Physics.Raycast(camera.position,ray.normalized,out var hit,ray.magnitude+.05f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<BankInteractable>()==target,"Tutorial target reachable at step "+step);
                CheckText(GameObject.Find("Tutorial VR - practica sin puntuacion").transform);
                if(step==4)Capture("05-tutorial-decision.png",camera.position,camera.position+camera.forward*2);
                deadline=Time.realtimeSinceStartup+4;
                while(tutorial.StageIndex==step&&Time.realtimeSinceStartup<deadline)yield return null;
                Check(tutorial.StageIndex==step+1,"Actual gaze advances tutorial step "+step);
                if(step==0) {
                    Check(visual.ConfirmationVisible,"Activation exposes brief confirmation");Capture("03-mirada-confirmacion.png",camera.position,camera.position+camera.forward*2);
                    float hold=Time.realtimeSinceStartup+.45f;while(Time.realtimeSinceStartup<hold)yield return null;
                    Check(tutorial.StageIndex==1&&gaze.SelectionLatched&&visual.State==GazeProgressVisual.VisualState.Rearm,"Held gaze cannot advance twice and requests looking away");
                    Capture("04-mirada-rearme.png",camera.position,camera.position+camera.forward*2);
                    UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==BankAction.Attack).Activate();
                    Check(game.Session==null&&!game.IntroPlaying,"Tutorial modal does not start a banking mode");
                }
            }
            Check(tutorial.DidComplete&&tutorial.PracticeChoice=="Aprobar"&&game.Session==null&&bankChanges==0,"Tutorial decision never changes GameManager or results");
            Capture("06-tutorial-completo.png",camera.position,camera.position+camera.forward*2);
            tutorial.SkipTarget.Activate();yield return null;
            Check(!tutorial.IsRunning&&ui.PanelAnchor.GetComponentInChildren<Canvas>()!=null,"Tutorial restores original banking monitor");
            camera.rotation=oldRotation;tutorial.Begin();yield return null;tutorial.SkipTarget.Activate();yield return null;
            Check(tutorial.WasSkipped&&game.Session==null&&bankChanges==0,"Tutorial can be skipped without creating or scoring a turn");
            game.Changed-=changed;camera.SetPositionAndRotation(oldPosition,oldRotation);gaze.SimulateGazeInEditor=oldSimulation;
            game.StartScenario(ScenarioMode.Attack);yield return null;
            Capture("07-expediente-ux.png");CheckText(ui.PanelAnchor.GetComponentInChildren<Canvas>().transform);
            var emitted=new System.Collections.Generic.List<AudioCue>();Action<AudioCue> cue=x=>emitted.Add(x);audio.CueEmitted+=cue;
            int n=audio.EmittedCount;game.Notify("Actualizacion visual sin cambio de estado.");ui.RefreshPresentation();
            Check(audio.EmittedCount==n,"Bank UI refresh remains silent");
            game.RegisterEvidence();game.RegisterEvidence();
            Check(emitted.Count(x=>x==AudioCue.Evidence)==1,"Evidence cue fires once for a new entry, not duplicate evidence");
            game.Decide(Decision.Approve);
            Check(emitted.Count(x=>x==AudioCue.NewAlert)==1,"New observed alert emits one cue on transaction change");
            for(int i=0;i<5;i++)game.InspectAlerts();
            Check(emitted.Count(x=>x==AudioCue.NewAlert)==1,"Repeated alert queries do not repeat new-alert sound");
            foreach(var choice in new[]{Decision.Return,Decision.Hold,Decision.Hold,Decision.Hold,Decision.Hold})game.Decide(choice);
            Check(game.Session.Ended&&game.Session.Correct==6&&game.Session.SimulatedLoss==0,"Existing coherent sequence remains 6/6 and loss 0");
            Check(emitted.Count(x=>x==AudioCue.TurnEnd)==1,"Turn-end cue fires exactly once");
            audio.CueEmitted-=cue;
            game.StartScenario(ScenarioMode.Attack);for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            game.StartScenario(ScenarioMode.Defense);game.ApplyDefense();for(int i=0;i<6;i++)game.Decide(Decision.Approve);yield return null;
            Check(game.LastAttackLoss==965000&&game.LastDefenseLoss==0,"Existing attack/defense calculations preserved");
            CheckText(ui.PanelAnchor.GetComponentInChildren<Canvas>().transform);Capture("08-resultados-ux.png");
            Check(errors==0,"No runtime errors in UX review");
        }
        static void CheckText(Transform root)
        {Canvas.ForceUpdateCanvases();foreach(var text in root.GetComponentsInChildren<TMPro.TextMeshProUGUI>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"TMP fits: "+text.transform.parent.name+"/"+text.name);}}
        static IEnumerator Probe(string name)
        {
            string path=name=="flow"?"Artifacts/GameplayFlow/result.txt":name=="office"?"Artifacts/Office20261028/playmode-office.txt":"Artifacts/playmode-smoke.txt";
            if(File.Exists(path))File.Delete(path);var target=new GameObject("UX review - existing "+name+" probe");
            if(name=="flow")target.AddComponent<Swift.Testing.GameplayFlowProbe>().ExitEditorOnComplete=false;
            else if(name=="office")target.AddComponent<Swift.Testing.BankOfficeProbe>().ExitEditorOnComplete=false;
            else target.AddComponent<Swift.Testing.PrototypeSmokeProbe>().ExitEditorOnComplete=false;
            float deadline=Time.realtimeSinceStartup+45;while(!File.Exists(path)&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(File.Exists(path),"Existing "+name+" probe completed");string result=File.ReadAllText(path);File.WriteAllText("Artifacts/UXGameplayReview/"+name+"-probe.txt",result);Check(result.StartsWith("PASS"),result);
        }
        static void Capture(string name,Vector3? eye=null,Vector3? target=null)
        {
            Canvas.ForceUpdateCanvases();Physics.SyncTransforms();var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();
            var view=new SceneReviewCapture.View{file=name,position=eye??new Vector3(0,1.6f,-.65f),target=target??ui.PanelAnchor.position,fieldOfView=60};
            typeof(SceneReviewCapture).GetMethod("Render",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{SceneManager.GetActiveScene(),view,Path.Combine(Output,name)});
            File.AppendAllText(Path.Combine(Output,"capture-receipt.txt"),DateTime.UtcNow.ToString("O")+" "+name+" Unity="+Application.unityVersion+" Play="+EditorApplication.isPlaying+" NOT_ANDROID\n");
        }
    }
}

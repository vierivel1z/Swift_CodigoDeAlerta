using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Swift.Bank;
using Swift.Game;

namespace Swift.EditorTools
{
    // Opt-in local review queue. Never saves the scene or reconfigures rendering/XR.
    [InitializeOnLoad]
    public static class DocumentUIReview
    {
        const string Queue = "Artifacts/DocumentUIReview/command.txt";
        const string Pending = "SWIFT_DOCUMENT_REVIEW_COMMAND";
        static IEnumerator routine;
        static int errors;
        static string output;
        static DocumentUIReview()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += PlayChanged;
        }
        static string Output
        {
            get
            {
                const string receipt = "Artifacts/DocumentUIReview/output-path.txt";
                if (output == null)
                {
                    output = File.Exists(receipt) ? File.ReadAllText(receipt).Trim()
                        : "Docs/Previews/DocumentUIReview/" + DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
                    Directory.CreateDirectory(output);
                    File.WriteAllText(receipt, output);
                }
                return output;
            }
        }
        static void Tick()
        {
            if (routine != null)
            {
                try { if (!routine.MoveNext()) Finish(null); }
                catch (Exception e) { Finish(e); }
                return;
            }
            if (!File.Exists(Queue) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string command = File.ReadAllText(Queue).Trim(); File.Delete(Queue);
            try
            {
                if (command != "before" && command != "stage1" && command != "all") throw new Exception("Unknown review command");
                if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play before review");
                if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty))
                    throw new Exception("Unsaved scene preserved; review cancelled");
                EditorSceneManager.OpenScene(PrototypeRelease.BankScene, OpenSceneMode.Single);
                SessionState.SetString(Pending, command);
                EditorApplication.EnterPlaymode();
            }
            catch (Exception e) { Finish(e); }
        }
        static void PlayChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode) return;
            string command = SessionState.GetString(Pending, "");
            if (command == "") return;
            SessionState.EraseString(Pending);
            errors = 0; Application.logMessageReceived += Log;
            routine = Run(command);
        }
        static void Log(string message, string stack, LogType type)
        { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors++; }
        static void Finish(Exception error)
        {
            routine = null; Application.logMessageReceived -= Log;
            string result = error == null && errors == 0 ? "PASS: Editor review completed. Android NOT TESTED."
                : "FAIL: " + (error?.ToString() ?? "runtime errors=" + errors);
            Directory.CreateDirectory("Artifacts/DocumentUIReview");
            File.WriteAllText("Artifacts/DocumentUIReview/result.txt", result);
            Debug.Log(result);
            if (EditorApplication.isPlaying) EditorApplication.ExitPlaymode();
        }
        static void Check(bool pass, string message)
        { if (!pass) throw new Exception(message); File.AppendAllText("Artifacts/DocumentUIReview/checks.txt", "PASS " + message + "\n"); }
        static IEnumerator Run(string command)
        {
            File.AppendAllText("Artifacts/DocumentUIReview/checks.txt","\nRUN "+command+" "+DateTime.UtcNow.ToString("O")+"\n");
            yield return null; yield return null;
            var game = UnityEngine.Object.FindFirstObjectByType<GameManager>();
            Check(game != null, "GameManager exists in saved scene");
            game.StartScenario(ScenarioMode.Attack);
            yield return null; yield return null;
            Capture(command == "before" ? "01-expediente-anterior.png" : "02-expediente-mejorado.png");
            if (command == "before") yield break;
            var ui = UnityEngine.Object.FindFirstObjectByType<Swift.UI.UIManager>();
            var documents = UnityEngine.Object.FindFirstObjectByType<Swift.UI.BankDocumentUI>();
            Check(documents != null, "Native document presentation installed");
            Check(game.Session.Transactions.Current.Data.Id == "SIM-001", "Presentation preserves active transaction");
            CheckText(); CheckRays();
            var registry=ui.GetComponentsInChildren<Swift.Interaction.BankInteractable>(true).FirstOrDefault(x=>x.Action==Swift.Interaction.BankAction.Registry);
            // The Canvas belongs to PanelAnchor, not necessarily the UIManager transform.
            if(registry==null) registry=UnityEngine.Object.FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==Swift.Interaction.BankAction.Registry);
            registry.Activate(); yield return null;
            Check(documents.CurrentPage==Swift.UI.BankDocumentUI.Page.Registry,"Registry interaction opens native registry page");
            CheckText(); Capture("03-consulta-registros.png");
            Check(game.Message.Contains(game.Session.Transactions.Current.Data.RegisteredName),"Registry retains existing query result");
            game.Decide(Decision.Approve); game.Decide(Decision.Return); registry.Activate(); yield return null;
            CheckText(); Check(game.Message.Contains("Fundacion Horizonte de Papel"),"Mismatched name uses existing registered name");
            game.Decide(Decision.Hold); registry.Activate(); yield return null;
            CheckText(); Check(game.Message.Contains("no encontrada"),"Unknown account retains existing no-record result");
            Check(VisibleText().Contains("No disponible"),"Unknown registry does not disclose an invented holder");
            game.StartScenario(ScenarioMode.Attack); yield return null;
            CheckText();
            if(command=="stage1") yield break;
            ui.ShowDocument(Swift.UI.BankDocumentUI.Page.Procedure); yield return null;
            CheckText(); CheckRays(); Capture("05-procedimiento.png");
            // Every shipped transaction, all three inspection views, existing decisions.
            var sequence=new[]{Decision.Approve,Decision.Return,Decision.Hold,Decision.Hold,Decision.Hold,Decision.Hold};
            for(int i=0;i<sequence.Length;i++) {
                ui.ShowDocument(Swift.UI.BankDocumentUI.Page.Dossier); yield return null; CheckText();
                registry.Activate(); yield return null; CheckText();
                game.InspectAlerts(); ui.ShowDocument(Swift.UI.BankDocumentUI.Page.Alerts);yield return null;CheckText();
                if(i==5) {
                    Capture("04-panel-alertas.png");
                    Check(VisibleText().Contains("no confirma fraude"),"Partial alert match remains a review observation");
                }
                var action=sequence[i]==Decision.Approve?Swift.Interaction.BankAction.Approve:sequence[i]==Decision.Hold?Swift.Interaction.BankAction.Hold:Swift.Interaction.BankAction.Return;
                var target=UnityEngine.Object.FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==action);
                target.Activate();yield return null;
            }
            Check(game.Session.Ended&&game.Session.Correct==6&&game.Session.SimulatedLoss==0,"Approve / Return / Hold controls retain 6/6 result");
            CheckText(); CheckRays();
            game.StartScenario(ScenarioMode.Attack);
            for(int i=0;i<6;i++) game.Decide(Decision.Approve);
            Check(game.LastAttackLoss==965000,"Unchanged attack calculation: USD 965000");
            game.StartScenario(ScenarioMode.Defense); game.ApplyDefense();
            for(int i=0;i<6;i++) game.Decide(Decision.Approve);
            yield return null;
            Check(game.LastDefenseLoss==0&&game.Session.Ended,"Unchanged reinforced defense calculation: USD 0");
            CheckText(); CheckRays(); Capture("06-resultados.png");
            ui.ShowDocument(Swift.UI.BankDocumentUI.Page.History); yield return null; CheckText(); Capture("07-contexto-creditos.png");
            game.StartScenario(ScenarioMode.Attack);yield return null;
            var gaze=UnityEngine.Object.FindFirstObjectByType<Swift.Interaction.VRInteractionManager>();
            Check(Mathf.Approximately(gaze.DwellSeconds,1.5f),"Saved gaze activation remains 1.5 seconds");
            var camera=gaze.View.transform; var oldPosition=camera.position;var oldRotation=camera.rotation;bool oldSimulation=gaze.SimulateGazeInEditor;
            var procedure=UnityEngine.Object.FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==Swift.Interaction.BankAction.Procedure);
            gaze.SimulateGazeInEditor=true;camera.position=new Vector3(0,1.6f,-.65f);camera.LookAt(procedure.transform.position);
            float deadline=Time.realtimeSinceStartup+4;
            while(gaze.FocusedTarget!=procedure&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(gaze.FocusedTarget==procedure,"Actual camera gaze reaches procedure tab");
            while(gaze.DwellProgress<.80f&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(!gaze.SelectionLatched&&documents.CurrentPage==Swift.UI.BankDocumentUI.Page.Dossier,"No activation before 1.5 seconds");
            Check(gaze.GetComponent<Swift.UI.GazeProgressVisual>().DisplayedProgress>.5f,"Visible gaze progress follows actual timer");
            Capture("08-progreso-mirada-editor.png");
            while(!gaze.SelectionLatched&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(gaze.SelectionLatched&&documents.CurrentPage==Swift.UI.BankDocumentUI.Page.Procedure,"Gaze activates at 1.5 second threshold");
            ui.ShowDocument(Swift.UI.BankDocumentUI.Page.Dossier);
            float until=Time.realtimeSinceStartup+.4f;while(Time.realtimeSinceStartup<until)yield return null;
            Check(documents.CurrentPage==Swift.UI.BankDocumentUI.Page.Dossier,"Gaze latches once while target remains focused");
            camera.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
            Check(gaze.FocusedTarget==null&&gaze.DwellProgress==0&&!gaze.SelectionLatched,"Leaving target clears gaze selection and progress");
            camera.SetPositionAndRotation(oldPosition,oldRotation);gaze.SimulateGazeInEditor=oldSimulation;
            ui.ShowDocument(Swift.UI.BankDocumentUI.Page.Dossier);yield return null;
            // Preserve intro's normal duration; explicitly distinguish skip from automatic ending.
            var intro=game.GetComponent<IntroSequence>()??game.gameObject.AddComponent<IntroSequence>();
            bool skipped=false;intro.Play(()=>skipped=true);yield return null;game.SkipIntro();yield return null;
            Check(skipped&&!intro.IsPlaying,"Existing prologue skip still completes");
            bool automatic=false;float began=Time.realtimeSinceStartup;intro.Play(()=>automatic=true);
            while(!automatic&&Time.realtimeSinceStartup-began<16)yield return null;
            Check(automatic&&Time.realtimeSinceStartup-began>=11.8f,"Existing prologue ends automatically at normal 12-second duration");
            var teleports=UnityEngine.Object.FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).Where(x=>x.Action==Swift.Interaction.BankAction.Teleport).ToArray();
            Check(teleports.Length==4,"Four existing teleport destinations retained");
            foreach(var target in teleports) {
                var position=target.Player.position;target.Activate();
                Check(Vector3.Distance(target.Player.position,target.Destination)<.001f,"Existing teleport reaches "+target.name);
                target.Player.position=position;
            }
            Check(errors==0,"No runtime Console errors during document review");
        }
        static string VisibleText() => string.Join("\n",UnityEngine.Object.FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsSortMode.None).Where(x=>x.isActiveAndEnabled).Select(x=>x.text));
        static void CheckText()
        {
            Canvas.ForceUpdateCanvases();
            var ui=UnityEngine.Object.FindFirstObjectByType<Swift.UI.BankDocumentUI>();
            var canvas=UnityEngine.Object.FindFirstObjectByType<Swift.UI.UIManager>().PanelAnchor.GetComponentInChildren<Canvas>();
            foreach(var text in canvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>()) {
                text.ForceMeshUpdate();
                Check(!text.isTextOverflowing,"No TMP overflow: "+text.transform.parent.name+"/"+text.name);
            }
            var content=canvas.transform.Find("Document UI - Propuesta C/Marco/Contenido");
            var blocks=content.Cast<Transform>().Where(x=>x.gameObject.activeInHierarchy).Select(x=>x.GetComponent<RectTransform>()).ToArray();
            for(int i=0;i<blocks.Length;i++)for(int j=i+1;j<blocks.Length;j++) {
                var a=new Rect(blocks[i].anchoredPosition-blocks[i].sizeDelta/2,blocks[i].sizeDelta);
                var b=new Rect(blocks[j].anchoredPosition-blocks[j].sizeDelta/2,blocks[j].sizeDelta);
                Check(!a.Overlaps(b),"No overlapping blocks: "+blocks[i].name+" / "+blocks[j].name);
            }
        }
        static void CheckRays()
        {
            Physics.SyncTransforms();
            var eye=new Vector3(0,1.6f,-.65f);
            var canvas=UnityEngine.Object.FindFirstObjectByType<Swift.UI.UIManager>().PanelAnchor.GetComponentInChildren<Canvas>();
            foreach(var target in canvas.GetComponentsInChildren<Swift.Interaction.BankInteractable>()) {
                if(!target.GetComponent<Collider>().enabled)continue;
                var vector=target.transform.position-eye;
                Check(Physics.Raycast(eye,vector.normalized,out var hit,vector.magnitude+.1f,~0,QueryTriggerInteraction.Ignore)
                    && hit.collider.GetComponentInParent<Swift.Interaction.BankInteractable>()==target,"Inspection ray reaches "+target.Action);
            }
            File.AppendAllText("Artifacts/DocumentUIReview/checks.txt","INFO Panel width angular="+(2*Mathf.Atan(.825f/1.4675f)*Mathf.Rad2Deg)+" deg; height="+(2*Mathf.Atan(.6f/1.4675f)*Mathf.Rad2Deg)+" deg; Android validation pending\n");
        }
        public static void Capture(string name)
        {
            Canvas.ForceUpdateCanvases(); Physics.SyncTransforms();
            var ui = UnityEngine.Object.FindFirstObjectByType<Swift.UI.UIManager>();
            var view = new SceneReviewCapture.View {
                file = name, position = new Vector3(0, 1.6f, -.65f),
                target = ui.PanelAnchor.position, fieldOfView = 60
            };
            typeof(SceneReviewCapture).GetMethod("Render", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { SceneManager.GetActiveScene(), view, Path.Combine(Output, name) });
            File.AppendAllText(Path.Combine(Output, "capture-receipt.txt"), DateTime.UtcNow.ToString("O")
                + " " + name + " Unity=" + Application.unityVersion + " quality="
                + QualitySettings.names[QualitySettings.GetQualityLevel()] + " eye=" + view.position
                + " target=" + view.target + " NOT_ANDROID\n");
        }
    }
}

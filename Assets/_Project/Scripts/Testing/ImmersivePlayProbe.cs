#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Swift.Bank;
using Swift.Game;
using Swift.UI;
using Swift.VR;
using Swift.Audio;
using Swift.Interaction;
using Swift.DeskTools;

namespace Swift.Testing
{
    public sealed class ImmersivePlayProbe : MonoBehaviour
    {
        public string Mode;
        const string EvidenceDirectory="Artifacts/ImmersiveGameplay", GalleryDirectory="Docs/Previews/ImmersiveGameplay/20261110";
        static void FinishReceipt(string mode,Exception error){var value=(error==null?"PASS ":"FAIL ")+mode+" "+DateTime.UtcNow.ToString("O")+" Unity="+Application.unityVersion+" EDITOR ONLY; ANDROID DEVICE NOT TESTED\n"+error;File.WriteAllText(EvidenceDirectory+"/"+mode+"-result.txt",value);File.AppendAllText(EvidenceDirectory+"/result-history.txt",value+"\n");}
        GameManager game;UIManager ui;VRInteractionManager gaze;AnalystSeat seat;OperationsStation station;DeskInteractionCoordinator tools;
        int errors;string output;Mouse mouse,previousMouse;
        void Start(){output=EvidenceDirectory+"/"+Mode+"-"+DateTime.UtcNow.ToString("HHmmss");Directory.CreateDirectory(output);Application.logMessageReceived+=Log;StartCoroutine(Safe(Tests()));}
        void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert){errors++;File.AppendAllText(output+"/console.txt",type+": "+message+"\n"+trace+"\n");}}
        IEnumerator Safe(IEnumerator root)
        {
            var stack=new Stack<IEnumerator>();stack.Push(root);Exception error=null;
            while(stack.Count>0) {
                object next=null;bool advanced=false;
                try{advanced=stack.Peek().MoveNext();if(advanced)next=stack.Peek().Current;}
                catch(Exception ex){error=ex;}
                if(error!=null)break;
                if(!advanced){stack.Pop();continue;}
                if(next is IEnumerator nested){stack.Push(nested);continue;}
                yield return next;
            }
            if(error==null&&errors>0)error=new Exception("Runtime errors="+errors);
            Application.logMessageReceived-=Log;RestoreMouse();
            File.WriteAllText(output+"/summary.txt",(error==null?"PASS ":"FAIL ")+Mode+" errors="+errors+"\n"+error);
            FinishReceipt(Mode,error);EditorApplication.ExitPlaymode();
        }
        void Check(bool value,string label){File.AppendAllText(output+"/checks.txt",(value?"PASS ":"FAIL ")+label+"\n");if(!value)throw new Exception(label);}
        IEnumerator Delay(float seconds){float until=Time.realtimeSinceStartup+seconds;while(Time.realtimeSinceStartup<until)yield return null;}
        IEnumerator Away(){gaze.View.transform.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;}
        bool Reaches(BankInteractable target) {
            Physics.SyncTransforms();var box=target.GetComponent<Collider>();if(box==null)return false;var delta=box.bounds.center-gaze.View.transform.position;
            bool result=Physics.Raycast(gaze.View.transform.position,delta.normalized,out var hit,delta.magnitude+.05f,~0,QueryTriggerInteraction.Ignore)&&hit.collider.GetComponentInParent<BankInteractable>()==target;
            if(!result)File.AppendAllText(output+"/ray-debug.txt",target.name+" eye="+gaze.View.transform.position+" target="+box.bounds.center+" hit="+hit.collider?.name+"\n");
            return result;
        }
        IEnumerator Select(BankInteractable target)
        {
            yield return Away();
            string targetName=target!=null?target.name:"<missing target>";
            Check(target!=null&&target.gameObject.activeInHierarchy&&target.CanActivate,"Target enabled: "+targetName);
            Check(Reaches(target),"Actual seated physics ray reaches "+targetName);
            int activations=0;Action<BankInteractable> counter=x=>{if(x==target)activations++;};gaze.Activated+=counter;
            gaze.View.transform.LookAt(target.GetComponent<Collider>().bounds.center);float deadline=Time.realtimeSinceStartup+6;
            while(activations==0&&Time.realtimeSinceStartup<deadline)yield return null;
            gaze.Activated-=counter;Check(activations==1,"One real 1.5-second dwell: "+targetName);yield return null;
        }
        BankInteractable Target(DeskActionTarget target)=>target.GetComponent<BankInteractable>();
        BankInteractable Bank(BankAction action)=>ui.PanelAnchor.GetComponentsInChildren<BankInteractable>(true).First(x=>x.Action==action&&x.gameObject.activeInHierarchy);
        IEnumerator Setup(bool startSession=true)
        {
            yield return null;yield return null;yield return null;
            game=FindFirstObjectByType<GameManager>();ui=FindFirstObjectByType<UIManager>();gaze=FindFirstObjectByType<VRInteractionManager>();seat=AnalystSeat.Current;station=ui.GetComponent<OperationsStation>();tools=DeskInteractionCoordinator.Current;
            foreach(var input in FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))input.enabled=false;
            gaze.SetEditorGazeSimulation(true);
            seat.Rig.position+=seat.StandingFloor.position+Vector3.up*1.6f-gaze.View.transform.position;
            yield return Away();
            if(startSession)game.StartScenario(ScenarioMode.Attack);
            yield return Select(station.EntryTarget);while(seat.IsTransitioning)yield return null;yield return null;
            Check(seat.IsSeated&&station.DetailedVisible&&station.SecondaryVisible,"Real seat enables both existing monitors");
            Check(gaze.View.GetComponent<HeadPoseAdapter>().enabled,"Tracking provider remains enabled");
        }
        IEnumerator Tests()
        {
            if(new[]{"flow","office","smoke"}.Contains(Mode)){yield return ExistingProbe();yield break;}
            yield return Setup(Mode!="tutorial"&&!Mode.StartsWith("welcome-",StringComparison.Ordinal));
            if(Mode=="welcome-before"||Mode=="welcome-after"){
                Check(game.Session==null,"Welcome keeps the banking session unopened");
                var tutorial=Bank(BankAction.TutorialStart);
                Check(tutorial.CanActivate&&Reaches(tutorial),"Welcome tutorial is reachable from the real seat");
                if(Mode=="welcome-after"){
                    var panel=GameObject.Find("Terminal disponible");
                    Check(panel!=null,"Welcome card exists");
                    foreach(var label in panel.GetComponentsInChildren<TMP_Text>()){
                        label.ForceMeshUpdate();
                        Check(!label.isTextOverflowing,"Welcome label fits: "+label.name);
                    }
                    foreach(var action in new[]{BankAction.Evidence,BankAction.Defense,BankAction.End})
                        Check(!ui.PanelAnchor.GetComponentsInChildren<BankInteractable>(true).Any(x=>x.Action==action&&x.gameObject.activeInHierarchy),"Unavailable bank action is hidden: "+action);
                }
                Render("welcome",ui.PanelAnchor.position);
                yield break;
            }
            if(Mode=="tutorial"){yield return Tutorial();yield break;}
            if(Mode=="baseline") {
                Check(tools==null,"Baseline has no new tools enabled");
                Render("before-main",ui.PanelAnchor.position);Render("before-clipboard",GameObject.Find("Analyst review clipboard").transform.position);
                yield return GameView("before-gameview");yield break;
            }
            Check(tools!=null&&tools.View!=null,"Opt-in tool coordinator and native TMP view loaded");
            if(Mode=="pilot-attack"||Mode=="pilot-defense"||Mode=="pilot-attack-print"){
                yield return Sim003PilotFlow(Mode=="pilot-defense",Mode=="pilot-attack-print");yield break;}
            if(Mode=="physical-before"){yield return PhysicalBefore();yield break;}
            if(Mode=="physical-after"){yield return PhysicalAfter();yield break;}
            if(Mode=="clipboard"){if(tools.PhysicalClipboard!=null)yield return PhysicalAfter();else yield return Clipboard();yield break;}
            if(Mode=="phone"){yield return Phone();yield break;}
            if(Mode=="stationpc"){yield return PC();yield break;}
            if(Mode=="closeui"){yield return CloseUI();yield break;}
            if(Mode=="objects"){yield return Objects();yield break;}
            if(Mode=="regression"){yield return Regression();yield break;}
            throw new Exception("Unhandled verification mode: "+Mode);
        }
        IEnumerator Tutorial()
        {
            if(seat.IsSeated){Check(seat.Request(false),"Tutorial returns safely to standing");while(seat.IsTransitioning)yield return null;yield return null;}
            var tutorial=FindFirstObjectByType<VRQuickTutorial>();gaze.View.transform.LookAt(ui.PanelAnchor.position);
            int audio=Swift.Audio.FeedbackAudio.Current.EmittedCount;
            for(int i=0;i<10;i++)game.Notify("Elige un modo o practica el tutorial.");
            Check(Swift.Audio.FeedbackAudio.Current.EmittedCount==audio,"Refresh does not spam audio");
            tutorial.Begin();Check(tutorial.IsRunning&&game.Session==null,"Existing tutorial starts outside banking session");
            for(int step=0;step<5;step++){
                yield return Select(step==4?tutorial.DecisionTargets[0]:tutorial.PrimaryTarget);
                Check(tutorial.StageIndex==step+1,"Real dwell advances tutorial step "+step);
                yield return Delay(.3f);Check(tutorial.StageIndex==step+1,"Held gaze cannot repeat tutorial step");
            }
            Render("tutorial-complete",tutorial.PrimaryTarget.transform.position);yield return GameView("tutorial-gameview");
            yield return Select(tutorial.PrimaryTarget);Check(!tutorial.IsRunning&&tutorial.DidComplete&&game.Session==null,"Tutorial completes without bank results");
            gaze.View.transform.LookAt(ui.PanelAnchor.position);tutorial.Begin();Check(tutorial.IsRunning,"Tutorial can be opened again");
            yield return Select(tutorial.SkipTarget);Check(tutorial.WasSkipped&&!tutorial.IsRunning&&game.Session==null,"Tutorial skip preserves absence of banking session");
            Check(!BankDocumentUI.InputHelp(true).Contains("PC:"),"Android instructions omit PC controls");
        }
        IEnumerator Clipboard()
        {
            var row=game.Session.Transactions.Current;var board=tools.ClipboardTarget.transform;var position=board.position;var rotation=board.rotation;
            Check(!tools.InspectionOpen,"Clipboard closed initially");yield return Away();gaze.View.transform.LookAt(tools.ClipboardTarget.GetComponent<Collider>().bounds.center);
            yield return null;Check(gaze.DwellProgress<.15f,"Clipboard starts near zero");Render("clipboard-00",board.position);
            float deadline=Time.realtimeSinceStartup+5;while(gaze.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(gaze.FocusedTarget==tools.ClipboardTarget&&gaze.DwellProgress<.62f&&!tools.InspectionOpen,"0.75 seconds corresponds to about 50 percent");Render("clipboard-50",board.position);
            yield return Away();Check(gaze.DwellProgress==0&&!tools.InspectionOpen,"Look-away resets actual charge");
            gaze.View.transform.LookAt(tools.ClipboardTarget.GetComponent<Collider>().bounds.center);deadline=Time.realtimeSinceStartup+6;
            while(!tools.InspectionOpen&&Time.realtimeSinceStartup<deadline)yield return null;
            Check(tools.ActiveTool=="Clipboard","Completed fixation opens physical clipboard inspection");Render("clipboard-100",board.position);
            Check(!tools.View.transform.IsChildOf(gaze.View.transform),"Inspection is world anchored, not rigidly attached to head");
            Check(Vector3.Distance(gaze.View.transform.position,tools.View.transform.position)>1,"Readable inspection is not against the eyes");
            Render("clipboard-open",tools.View.transform.position);TextFits();
            var corners=new Vector3[4];tools.View.GetComponent<RectTransform>().GetWorldCorners(corners);
            Check(corners.All(p=>{var viewport=gaze.View.WorldToViewportPoint(p);return viewport.z>0&&viewport.x>=0&&viewport.x<=1&&viewport.y>=0&&viewport.y<=1;}),"Entire inspection fits actual gaze after opening, including header");
            yield return GameView("clipboard-gameview");
            Check(!Bank(BankAction.Approve).CanActivate,"Hidden banking actions blocked by modal guard");
            Bank(BankAction.Approve).Activate();Check(game.Session.Transactions.Current==row,"Direct Activate cannot bypass inspection guard");
            yield return Select(Target(tools.View.Next));Check(tools.Page==1,"Next selects actual contextual sheet");
            Check(tools.View.Body.text.Contains(row.Data.Beneficiary)&&tools.View.Body.text.Contains(row.Data.Account),"Sheet uses existing transaction data");
            yield return Select(Target(tools.View.Primary));var notes=tools.Journal.For(row.Data.Id);Check(notes.ManualChecks[1],"Manual check toggled once");
            yield return Delay(1.8f);Check(notes.ManualChecks[1]&&tools.Page==1,"Held fixation cannot toggle twice or advance to another action");
            Render("clipboard-context",tools.View.transform.position);TextFits();
            int attempts=0;while(tools.Page!=2&&attempts++<6)yield return Select(Target(tools.View.Next));
            Check(tools.Page==2,"Long content paginates before moving to observation sheet");
            Check(!tools.View.Body.text.ToUpperInvariant().Contains("RETENER")&&!tools.View.Body.text.ToUpperInvariant().Contains("DEVOLVER"),"Observed information does not reveal recommended decision");
            Render("clipboard-sources",tools.View.transform.position);
            yield return Select(Target(tools.View.Close));
            Check(!tools.InspectionOpen&&station.DetailedVisible&&station.SecondaryVisible,"Closing preserves both monitors");
            Check(board.position==position&&Quaternion.Angle(board.rotation,rotation)<.01f,"Physical object remains at original desk pose");
            Check(game.Session.Transactions.Current==row&&game.Session.Correct==0&&game.Session.Evidence.Entries.Count==0,"Inspection and manual marks do not decide or register evidence");
            game.Decide(Decision.Approve);Check(!tools.Journal.For("SIM-002").ManualChecks.Any(x=>x),"Next record inherits no checks");
            game.StartScenario(ScenarioMode.Attack);Check(!tools.Journal.For("SIM-001").ManualChecks.Any(x=>x),"Restart resets manual checklist");
            yield return Select(tools.ClipboardTarget);
            string original=row.Data.Beneficiary;var active=game.Session.Transactions.Current.Data;string old=active.Beneficiary;
            try{
                active.Beneficiary=new string('W',180);tools.CloseInspection();yield return Select(tools.ClipboardTarget);yield return Select(Target(tools.View.Next));
                tools.View.Body.ForceMeshUpdate();Check(tools.View.Body.textInfo.pageCount>1&&tools.View.Body.fontSize>=32,"Long data has real pages without shrinking type");TextFits();
            }finally{active.Beneficiary=old;}
            tools.CloseInspection();Render("after-main",ui.PanelAnchor.position);
        }
        IEnumerator PhysicalBefore()
        {
            Check(tools.PhysicalClipboard==null,"Before capture uses existing enlarged accessibility view");
            Render("desk-before",tools.ClipboardTarget.transform.position);
            yield return Select(tools.ClipboardTarget);
            Check(tools.ActiveTool=="Clipboard","Existing clipboard opens");
            Render("reading-before",tools.View.transform.position);
            tools.CloseInspection();
        }
        IEnumerator Sim003PilotFlow(bool defense,bool attackPrintWorks)
        {
            var pilot=Sim003Pilot.Current;
            Check(pilot!=null,"Isolated SIM-003 pilot enabled in review scene");
            if(defense)game.StartScenario(ScenarioMode.Defense);
            game.Decide(Decision.Approve);game.Decide(Decision.Return);
            var row=game.Session.Transactions.Current;
            Check(row.Data.Id=="SIM-003"&&pilot.ActiveForCurrent,"Normal six-record turn reaches SIM-003");
            Check(!pilot.SearchedAccount&&pilot.RecordedFinding==Sim003Pilot.Finding.None,"No result or finding is prefilled");
            var visible=ui.PanelAnchor.GetComponentsInChildren<TMP_Text>().Where(x=>x.gameObject.activeInHierarchy).Select(x=>x.text).ToArray();
            Check(!visible.Any(x=>x.Trim()==row.Data.RegisteredName||x.Contains("Titular: "+row.Data.RegisteredName)),"Initial visible UI does not expose registered holder");
            Check(!visible.Any(x=>x.Contains("Beneficiario incorrecto")||x.Contains("Diferencia detectada")||x.Contains("Fraude simulado")),"No prewritten diagnosis on initial UI");
            Render("pilot-dossier",ui.PanelAnchor.position);
            yield return Select(Bank(BankAction.Registry));
            Check(!pilot.SearchedAccount&&ui.DocumentPage==BankDocumentUI.Page.Registry,"Registry initially awaits player query");
            visible=ui.PanelAnchor.GetComponentsInChildren<TMP_Text>().Where(x=>x.gameObject.activeInHierarchy).Select(x=>x.text).ToArray();
            Check(!visible.Any(x=>x.Trim()==row.Data.RegisteredName||x.Contains("Titular: "+row.Data.RegisteredName)),"Opening REGISTRO alone reveals no holder");
            Render("pilot-registry-before",ui.PanelAnchor.position);
            yield return Select(Bank(BankAction.PilotSelectAccount));
            Check(pilot.SelectedAccountNumber==row.Data.Account&&!pilot.SearchedAccount,"Player selected account from actual dossier data");
            yield return Select(Bank(BankAction.PilotSearch));
            Check(pilot.SearchedAccount&&pilot.RegisteredName==row.Data.RegisteredName,"BUSCAR retrieves authored registered holder");
            Check(pilot.RecordedFinding==Sim003Pilot.Finding.None,"Search alone does not record a finding");
            Render("pilot-registry-result",ui.PanelAnchor.position);
            yield return Select(Bank(BankAction.PilotMatches));
            Check(pilot.RecordedFinding==Sim003Pilot.Finding.Matches,"Player can enter a potentially wrong observation");
            yield return Select(Bank(BankAction.PilotDiffers));
            Check(pilot.RecordedFinding==Sim003Pilot.Finding.Differs,"Player can correct observation before decision");
            Check(pilot.Printer!=null&&pilot.Printer.PrintedCopies==0&&pilot.Printer.SheetTarget==null,"Printer contains no holder before explicit request");
            yield return Select(Bank(BankAction.Evidence));
            if(defense||attackPrintWorks){
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Printing||pilot.Printer.State==Sim003PrinterPresentation.PrintState.Tray,"Enabled printer starts controlled print activity");
                Render("pilot-printing",pilot.Printer.transform.position);
                yield return Delay(1f);
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Tray&&pilot.Printer.PrintedCopies==1,"One physical sheet reaches tray");
                Check(pilot.Printer.SheetText.text.Contains(row.Data.RegisteredName)&&pilot.Printer.PrintedAccount==row.Data.Account,"Printed data matches actual registry result");
                yield return Select(Bank(BankAction.Evidence));
                Check(pilot.Printer.PrintedCopies==1,"Repeated request creates no duplicate");
                yield return Select(station.ExitTarget);while(seat.IsTransitioning)yield return null;
                var evidencePad=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==BankAction.Teleport&&x.name=="Teleport Evidence");
                evidencePad.Activate();yield return null;
                Render("pilot-print-tray",pilot.Printer.SheetTarget.transform.position);
                yield return Select(pilot.Printer.SheetTarget);yield return Delay(.7f);
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Inspecting,"Physical sheet lifts by shared dwell");
                Render("pilot-print-inspect",pilot.Printer.SheetTarget.transform.position);
                gaze.View.transform.LookAt(pilot.Printer.SheetTarget.transform.position);
                yield return GameView("pilot-print-inspect");
                yield return Select(pilot.Printer.ReturnTarget);yield return Delay(.7f);
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Tray,"Physical sheet returns to tray");
                yield return Select(pilot.Printer.SheetTarget);yield return Delay(.7f);
                yield return Select(pilot.Printer.ExpandTarget);yield return Delay(.7f);
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Tray&&ui.DocumentPage==BankDocumentUI.Page.PilotPrintExpanded,"Enlarged printout remains available without another copy");
                var entryPad=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).First(x=>x.Action==BankAction.Teleport&&x.name=="Teleport Inspection");
                entryPad.Activate();yield return null;
                yield return Select(station.EntryTarget);while(seat.IsTransitioning)yield return null;
                Check(seat.IsSeated&&pilot.SearchedAccount,"Digital consultation survives optional printer visit");
                Render("pilot-print-expanded",ui.PanelAnchor.position);
            }else{
                Check(pilot.Printer.State==Sim003PrinterPresentation.PrintState.Failed&&pilot.Printer.PrintedCopies==0,"ATAQUE controlled failure makes no physical paper");
                Check(pilot.RegisteredName==row.Data.RegisteredName,"Print failure preserves digital query");
                Render("pilot-print-failed",ui.PanelAnchor.position);
            }
            yield return Select(tools.ClipboardTarget);yield return Delay(.8f);
            Check(tools.PhysicalClipboard!=null&&tools.PhysicalClipboard.State==PhysicalClipboardPresentation.PoseState.Inspecting,"Real wooden clipboard remains usable in pilot");
            Render("pilot-clipboard",tools.ClipboardTarget.transform.position);
            Render("pilot-clipboard-sheet-1",tools.ClipboardTarget.transform.position);
            gaze.View.transform.LookAt(tools.ClipboardTarget.transform.position);
            yield return GameView("pilot-clipboard-sheet-1");
            yield return Select(Target(tools.View.Next));yield return Delay(.11f);
            Check(tools.PhysicalClipboard.PageTurnActive,"Page change has a brief visual transition without moving controls");
            Render("pilot-clipboard-transition",tools.ClipboardTarget.transform.position);
            int sheetTurns=0;while(tools.Page!=1&&sheetTurns++<8)yield return Select(Target(tools.View.Next));
            Check(tools.Page==1,"Second physical sheet reachable by gaze");
            Render("pilot-clipboard-sheet-2",tools.ClipboardTarget.transform.position);
            sheetTurns=0;while(tools.Page!=2&&sheetTurns++<8)yield return Select(Target(tools.View.Next));
            Check(tools.Page==2,"Third physical sheet reachable by gaze");
            Render("pilot-clipboard-sheet-3",tools.ClipboardTarget.transform.position);
            yield return Select(Target(tools.View.Close));yield return Delay(.8f);
            Check(!tools.InspectionOpen,"Clipboard returns without erasing finding");
            if(defense){
                Check(tools.PhoneAvailable,"DEFENSA offers contextual SIM-003 phone");
                yield return Select(tools.PhoneTarget);
                yield return Select(Target(tools.View.Primary));yield return Delay(.8f);
                yield return Select(Target(tools.View.Primary));
                while(tools.Call.State==VerificationState.Connected)yield return Select(Target(tools.View.Primary));
                if(tools.Call.State==VerificationState.Reviewing)yield return Select(Target(tools.View.Primary));
                Check(tools.Journal.For("SIM-003").PhoneEvidence!=null,"Phone consultation registered as separate source");
                yield return Select(Target(tools.View.Close));
                yield return Select(Bank(BankAction.Defense));
                Check(game.Session.Defense.Active,"Reinforced controls activated separately from phone");
            } else Check(!tools.PhoneAvailable,"ATAQUE does not offer defense phone");
            yield return Select(Bank(BankAction.Approve));
            Check(pilot.PendingDecision==Decision.Approve&&ui.DocumentPage==BankDocumentUI.Page.PilotConfirm,"Decision requires explicit confirmation");
            Render("pilot-confirmation",ui.PanelAnchor.position);
            yield return Select(Bank(BankAction.PilotCancel));
            Check(!pilot.PendingDecision.HasValue&&row.Status==TransactionStatus.Pending,"Cancel preserves pending order and finding");
            yield return Select(Bank(BankAction.Approve));
            yield return Select(Bank(BankAction.PilotConfirm));
            Check(pilot.ReviewPending&&pilot.ReviewedRecord==row&&pilot.FindingSupported&&pilot.ExperimentalScore==5,"Post-decision review scores only supported finding for incorrect approval");
            Check(row.Status==(defense?TransactionStatus.Held:TransactionStatus.Approved),"Existing ATTACK/DEFENSE effective state preserved");
            Check(row.DefenseOverride==defense&&game.Session.SimulatedLoss==(defense?0:row.Data.Amount),"Control intervention and fictitious loss remain domain-owned");
            Render("pilot-review",ui.PanelAnchor.position);
            yield return Select(Bank(BankAction.PilotContinue));
            Check(!pilot.ReviewPending&&game.Session.Transactions.Current.Data.Id=="SIM-004","Normal six-record flow resumes after review");
            Check(!pilot.ActiveForCurrent&&ui.DocumentPage==BankDocumentUI.Page.Dossier,"Other records use original presentation");
            game.Decide(Decision.Hold);game.Decide(Decision.Hold);game.Decide(Decision.Hold);
            Check(game.Session.Ended&&game.Session.Correct==5&&game.Session.SimulatedLoss==(defense?0:row.Data.Amount),"All six records retain official outcome and comparative loss");
            Render("pilot-full-results",ui.PanelAnchor.position);
            if(!defense&&!attackPrintWorks){
                game.StartScenario(ScenarioMode.Attack);game.Decide(Decision.Approve);game.Decide(Decision.Return);
                yield return Select(Bank(BankAction.PilotSelectAccount));
                yield return Select(Bank(BankAction.PilotSearch));
                yield return Select(Bank(BankAction.PilotDiffers));
                yield return Select(Bank(BankAction.Hold));
                yield return Select(Bank(BankAction.PilotConfirm));
                Check(pilot.ExperimentalScore==15&&pilot.ReviewedRecord.Status==TransactionStatus.Held,"Coherent decision plus supported finding earns isolated 15/15");
            }
        }
        IEnumerator PhysicalAfter()
        {
            var physical=tools.PhysicalClipboard;
            Check(physical!=null&&physical.State==PhysicalClipboardPresentation.PoseState.Resting,"Physical variant active and resting");
            var board=tools.ClipboardTarget.transform;
            var start=board.position;var turn=board.rotation;
            var row=game.Session.Transactions.Current;
            Render("desk-after",board.position);
            gaze.View.transform.LookAt(tools.ClipboardTarget.GetComponent<Collider>().bounds.center);
            yield return null;yield return null;
            var hint=board.GetComponentsInChildren<TextMeshPro>(true).FirstOrDefault(x=>x.name.StartsWith("TOMAR PORTAPAPELES"));
            Check(hint!=null&&hint.gameObject.activeInHierarchy,"Focus shows TOMAR PORTAPAPELES on the real board");
            Render("focused-after",board.position);
            yield return Select(tools.ClipboardTarget);
            Check(physical.State==PhysicalClipboardPresentation.PoseState.Lifting||physical.State==PhysicalClipboardPresentation.PoseState.Inspecting,"Real dwell lifts the complete board");
            yield return Delay(.15f);
            Check(board.position.y>start.y+.05f,"Animation first clears the desktop vertically");
            yield return Delay(.8f);
            Check(physical.State==PhysicalClipboardPresentation.PoseState.Inspecting&&Vector3.Distance(board.position,start)>.25f,"Board arrives at stable reading pose");
            Check(tools.View==physical.PaperView&&tools.View.transform.IsChildOf(board),"Dynamic TMP sits on physical paper");
            Check(Vector3.Distance(board.position,gaze.View.transform.position)>.45f,"Reading position keeps separation from eyes");
            Check(!tools.ClipboardTarget.GetComponent<Collider>().enabled,"Board collider does not block paper controls");
            Render("reading-after",board.position);
            Check(!Bank(BankAction.Approve).CanActivate,"Bank actions remain blocked during inspection");
            int turns=0;while(tools.Page!=1&&turns++<6)yield return Select(Target(tools.View.Next));
            Check(tools.Page==1&&tools.View.Body.text.Contains(row.Data.Beneficiary),"Page navigation reuses dossier data");
            Render("dossier-after",board.position);
            yield return Select(Target(tools.View.Primary));
            Check(tools.Journal.For(row.Data.Id).ManualChecks[1],"Manual mark stays in existing journal");
            yield return Select(Target(tools.View.Expand));
            Check(tools.View==tools.ExpandedView&&tools.ActiveTool=="Clipboard","Gaze opens enlarged accessibility view without losing page");
            Check(tools.Page==1&&tools.View.Body.text.Contains(row.Data.Account),"Enlarged view reuses the same dossier and sheet");
            Render("expanded-after",tools.View.transform.position);
            yield return Select(Target(tools.View.Close));
            yield return Delay(.8f);
            Check(physical.State==PhysicalClipboardPresentation.PoseState.Resting&&Vector3.Distance(board.position,start)<.001f&&Quaternion.Angle(board.rotation,turn)<.01f,"Board returns to exact desk pose");
            Check(tools.Journal.For(row.Data.Id).ManualChecks[1]&&game.Session.Transactions.Current==row,"Return preserves mark and dossier");
            Render("returned-after",board.position);
            yield return Select(tools.ClipboardTarget);yield return Delay(.8f);
            Check(tools.Journal.For(row.Data.Id).ManualChecks[1],"Reopening retains review mark");
            yield return Select(Target(tools.View.Close));yield return Delay(.8f);
            Check(physical.State==PhysicalClipboardPresentation.PoseState.Resting,"DEJAR returns the physical board");
            yield return Select(tools.ClipboardTarget);yield return Delay(.8f);
            Check(seat.Request(false),"Exit station requested");
            while(seat.IsTransitioning)yield return null;
            yield return null;
            Check(physical.State==PhysicalClipboardPresentation.PoseState.Resting&&Vector3.Distance(board.position,start)<.001f,"Exit restores clipboard immediately");
            yield return Select(station.EntryTarget);while(seat.IsTransitioning)yield return null;
            Check(seat.IsSeated&&physical.State==PhysicalClipboardPresentation.PoseState.Resting,"Station reentry leaves board ready");
        }
        IEnumerator Defense003()
        {
            tools.CloseInspection();game.StartScenario(ScenarioMode.Defense);
            yield return Select(Bank(BankAction.Approve));yield return Select(Bank(BankAction.Return));
            Check(game.Session.Transactions.Current.Data.Id=="SIM-003","Authored bank flow reaches SIM-003");
        }
        IEnumerator Phone()
        {
            Check(!tools.PhoneAvailable,"Phone unavailable in ATTACK");
            if(tools.PhoneTarget!=null) {yield return Away();gaze.View.transform.LookAt(tools.PhoneTarget.GetComponent<Collider>().bounds.center);yield return Delay(.9f);Check(gaze.DwellProgress==0&&!tools.InspectionOpen,"Ineligible phone accumulates no hidden dwell");}
            yield return Defense003();Check(tools.PhoneAvailable&&!game.Session.Defense.Active,"Phone eligibility independent of automatic controls");
            yield return Select(tools.PhoneTarget);Check(tools.Call.State==VerificationState.Ready,"Phone starts with explicit request");
            var corners=new Vector3[4];tools.View.GetComponent<RectTransform>().GetWorldCorners(corners);
            File.WriteAllLines(output+"/phone-viewport.txt",corners.Select(p=>gaze.View.WorldToViewportPoint(p).ToString()));
            Check(corners.All(p=>{var v=gaze.View.WorldToViewportPoint(p);return v.z>0&&v.x>=0&&v.x<=1&&v.y>=0&&v.y<=1;}),"Phone inspection appears within actual opening gaze");
            int printerConfirmations=tools.PrinterFeedback?.ConfirmationCount??0;
            yield return Select(Target(tools.View.Primary));yield return Delay(.8f);
            Check(tools.Call.State==VerificationState.Ringing,"Requested call produces pending signal");Render("phone-ringing",tools.View.transform.position);
            yield return Delay(1.8f);Check(tools.Call.State==VerificationState.Ringing,"Held gaze on changed primary label does not auto-answer");
            yield return Select(Target(tools.View.Primary));Check(tools.Call.State==VerificationState.Connected,"Actual dwell answers call");
            Check(tools.View.Notice.text.Contains("PRUEBA"),"Unapproved dialogue clearly marked prototype");
            yield return Select(Target(tools.View.Primary));Check(tools.View.Body.text.Contains("Fundacion Horizonte de Papel"),"Transcript presents exact registry name");
            Check(game.Session.Transactions.Current.Data.Beneficiary=="Fundacion Horizonte de Papell","Player still compares different original order value");
            Render("phone-dialogue",tools.View.transform.position);TextFits();yield return GameView("phone-gameview");
            yield return Select(Target(tools.View.Primary));Check(tools.Call.State==VerificationState.Reviewing,"Last intervention enables explicit registration");
            Check(!tools.View.Body.text.ToUpperInvariant().Contains("RETENER")&&!tools.View.Body.text.ToUpperInvariant().Contains("APROBAR")&&!tools.View.Body.text.ToUpperInvariant().Contains("DEVOLVER"),"No decision instruction in dialogue");
            var session=game.Session;var row=session.Transactions.Current;int before=session.Evidence.Entries.Count;
            yield return Select(Target(tools.View.Primary));Check(session.Evidence.Entries.Count==before+1&&tools.Call.State==VerificationState.Recorded,"One explicit evidence entry committed");
            yield return Delay(1.8f);tools.Handle(DeskCommand.RegisterCall);
            Check(session.Evidence.Entries.Count==before+1&&session.Evidence.Entries.Last().StartsWith("SIM-003: "),"Duplicate command cannot duplicate or misattach evidence");
            Check(session.Correct==2&&row.Status==TransactionStatus.Pending&&!session.Defense.Active,"Phone changes neither decision, score nor control state");
            Check((tools.PrinterFeedback?.ConfirmationCount??0)==printerConfirmations,"Phone evidence never falsely confirms the printer");
            Render("phone-recorded",tools.View.transform.position);yield return Select(Target(tools.View.Close));Check(!tools.InspectionOpen,"Call can finish without deciding");
            yield return Select(tools.PhoneTarget);Check(tools.Call.State==VerificationState.Recorded,"Reopening preserves recorded call without another entry");tools.CloseInspection();
            yield return Defense003();yield return Select(tools.PhoneTarget);tools.Handle(DeskCommand.RequestCall);yield return Delay(.8f);tools.Handle(DeskCommand.AnswerCall);tools.Handle(DeskCommand.Next);tools.Handle(DeskCommand.Next);
            var stale=tools.Call;game.Decide(Decision.Hold);
            Check(!tools.InspectionOpen&&!stale.CanRegister(game.Session)&&game.Session.Transactions.Current.Data.Id=="SIM-004","Changing record cancels stale inspection safely");
            Check(game.Session.Evidence.Entries.Count==0,"Stale SIM-003 call produces no SIM-004 evidence");
            game.StartScenario(ScenarioMode.Defense);Check(!tools.PhoneAvailable&&tools.Call==null,"New session starts clean and phone unavailable for SIM-001");
        }
        IEnumerator Objects()
        {
            Check(tools.Profile.PrinterFeedbackEnabled&&tools.Profile.SeatPolishEnabled,"Object and seat polish enabled after phone gate");
            if(tools.DocumentTarget!=null) {
                yield return Select(tools.DocumentTarget);Check(tools.ActiveTool=="Document","Physical transfer document opens read-only inspection");
                Render("document-inspection",tools.View.transform.position);yield return Select(Target(tools.View.Close));
            }else Check(false,"Existing document prop found for inspection");
            var printer=tools.PrinterFeedback;Check(printer!=null,"Existing printer has local feedback");
            int confirmations=printer.ConfirmationCount;int evidence=game.Session.Evidence.Entries.Count;
            yield return Select(printer.Target);Check(printer.ConfirmationCount==confirmations+1&&game.Session.Evidence.Entries.Count==evidence+1,"Printer confirms one existing evidence entry");
            yield return Select(printer.Target);Check(printer.ConfirmationCount==confirmations+1&&game.Session.Evidence.Entries.Count==evidence+1,"Repeated printer action never duplicates evidence/confirmation");
            Render("printer-feedback",printer.transform.position);
            var row=game.Session.Transactions.Current;var heading=gaze.View.transform.rotation;
            yield return Select(tools.ClipboardTarget);yield return Select(station.ExitTarget);while(seat.IsTransitioning)yield return null;yield return null;
            Check(!tools.InspectionOpen&&!seat.IsSeated&&!station.DetailedVisible&&station.Desktop.gameObject.activeInHierarchy,"SALIR closes tools and restores idle PC");
            Check(game.Session.Transactions.Current==row,"Leaving preserves current transaction");
            Check(FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Where(x=>x.Action==BankAction.Teleport).All(x=>x.GetComponent<Collider>().enabled),"Allowed teleport targets restored on exit");
            Render("station-exited",ui.PanelAnchor.position);
            yield return Select(station.EntryTarget);while(seat.IsTransitioning)yield return null;yield return null;
            Check(Vector3.Distance(gaze.View.transform.position,seat.SeatedEye.position)<.02f&&gaze.View.GetComponent<HeadPoseAdapter>().enabled,"Re-entry uses original tracked eye/seat algorithm");
            Check(game.Session.Transactions.Current==row&&station.SecondaryVisible,"Re-entry keeps bank progress and secondary monitor");
            var presentation=seat.GetComponent<SeatPresentationFeedback>();
            Check(presentation!=null&&presentation.ResponseCount>=2,"Chair visual feedback responds once per transition");
            Render("station-reentered",ui.PanelAnchor.position);
        }
        IEnumerator PC()
        {
            yield return Select(tools.ClipboardTarget);yield return Away();
            if(tools.PhysicalClipboard!=null)yield return Delay(.8f);
            gaze.SetEditorGazeSimulation(false);previousMouse=Mouse.current;mouse=InputSystem.AddDevice<Mouse>("SWIFT immersive PC verification");
            var target=Target(tools.View.Primary);var notes=tools.Journal.For(game.Session.Transactions.Current.Data.Id);int group=tools.Page;
            gaze.View.transform.LookAt(target.GetComponent<Collider>().bounds.center);
            float end=Time.realtimeSinceStartup+.4f;while(Time.realtimeSinceStartup<end){Pointer(target,false);yield return null;}
            Check(gaze.FocusedTarget==target&&gaze.DwellProgress==0,"PC hover has no hidden charge");
            end=Time.realtimeSinceStartup+4;while(gaze.DwellProgress<.49f&&Time.realtimeSinceStartup<end){Pointer(target,true);yield return null;}
            Check(gaze.DwellProgress>.45f&&gaze.DwellProgress<.65f&&!notes.ManualChecks[group],"PC held pointer reaches actual half dwell");
            Pointer(target,false);yield return null;yield return null;Check(gaze.DwellProgress==0&&!notes.ManualChecks[group],"PC release resets progress uniformly");
            end=Time.realtimeSinceStartup+5;while(!notes.ManualChecks[group]&&Time.realtimeSinceStartup<end){Pointer(target,true);yield return null;}
            Check(notes.ManualChecks[group],"Fresh PC hold toggles once");end=Time.realtimeSinceStartup+1.8f;
            while(Time.realtimeSinceStartup<end){Pointer(target,true);yield return null;}
            Check(notes.ManualChecks[group],"Continued PC hold never triggers second toggle");
            RestoreMouse();gaze.SetEditorGazeSimulation(true);tools.CloseInspection();
        }
        IEnumerator CloseUI()
        {
            var close=Bank(BankAction.End);
            var rect=close.GetComponent<RectTransform>();
            var label=close.GetComponentInChildren<TextMeshProUGUI>();
            Check(Mathf.Abs(rect.sizeDelta.x-390)<.1f&&Mathf.Abs(rect.sizeDelta.y-60)<.1f,"Close-turn action has enlarged target");
            Check(label!=null&&label.text=="CERRAR TURNO"&&label.fontSize>=28,"Close-turn label has enlarged readable type");
            label.ForceMeshUpdate();Check(!label.isTextOverflowing,"Close-turn label stays inside its target");
            Check(Reaches(close),"Actual seated ray reaches close-turn action");
            Render("close-turn-monitor",ui.PanelAnchor.position);
            var session=game.Session;
            yield return Select(close);
            Check(!session.Ended&&game.Message.Contains("Resuelve"),"Close-turn keeps the existing all-decisions prerequisite");
            for(int i=0;i<session.Transactions.Records.Count;i++)game.Decide(Decision.Approve);
            Check(session.Ended,"Existing final-decision flow closes the turn once");
            Render("close-turn-results",ui.PanelAnchor.position);
        }
        void Pointer(BankInteractable target,bool held)
        {
            mouse.MakeCurrent();var point=gaze.View.WorldToScreenPoint(target.GetComponent<Collider>().bounds.center);
            InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(point.x,point.y),buttons=(ushort)(held?1:0)});InputSystem.Update();
        }
        void RestoreMouse(){if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);mouse=null;if(previousMouse!=null&&previousMouse.added)previousMouse.MakeCurrent();}
        IEnumerator ExistingProbe()
        {
            yield return null;yield return null;
            foreach(var adapter in FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;
            var runner=(IEnumerator)AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("Swift.EditorTools.UXGameplayReview")).First(t=>t!=null).GetMethod("Probe",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{Mode});
            yield return runner;
            File.Copy("Artifacts/UXGameplayReview/"+Mode+"-probe.txt",output+"/"+Mode+"-probe.txt",true);
            Check(File.ReadAllText(output+"/"+Mode+"-probe.txt").StartsWith("PASS"),"Existing "+Mode+" probe passes from fresh Play session");
        }
        IEnumerator Regression()
        {
            foreach(var test in new[]{"flow","office","smoke"})Check(File.ReadAllText(EvidenceDirectory+"/"+test+"-result.txt").StartsWith("PASS"),"Fresh existing probe gate: "+test);
            game=FindFirstObjectByType<GameManager>();tools=DeskInteractionCoordinator.Current;
            tools?.CloseInspection();game.StartScenario(ScenarioMode.Attack);for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            Check(game.LastAttackLoss==965000&&game.Session.Correct==1,"All-approve ATTACK unchanged: loss 965000, score 1/6");
            game.StartScenario(ScenarioMode.Defense);game.ApplyDefense();for(int i=0;i<6;i++)game.Decide(Decision.Approve);
            Check(game.LastDefenseLoss==0&&game.Session.Correct==1,"Automatic DEFENSE unchanged: loss zero, score 1/6");
            yield return null;Render("results-choice-and-control",ui.PanelAnchor.position);
            var debrief=ui.PanelAnchor.GetComponentsInChildren<DeskActionTarget>(true).First(x=>x.Command==DeskCommand.Debrief);
            if(!seat.IsSeated){Check(seat.Request(true),"Review seat entry accepted");while(seat.IsTransitioning)yield return null;}
            yield return Select(Target(debrief));Check(tools.ActiveTool=="Debrief","Educational review reachable through actual dwell");
            int turns=0;while(tools.Page!=1&&turns++<8)yield return Select(Target(tools.View.Next));
            Check(tools.Page==1&&tools.View.Body.text.Contains("Aprobar")&&tools.View.Body.text.Contains("Devuelta"),"Review distinguishes original decision and effective format return");
            Render("educational-review",tools.View.transform.position);TextFits();tools.CloseInspection();
            Check(FindObjectsByType<Camera>(FindObjectsSortMode.None).Length==1,"No temporary capture camera survives");
        }
        void TextFits()
        {
            Canvas.ForceUpdateCanvases();
            foreach(var text in tools.View.GetComponentsInChildren<TMP_Text>()) {
                text.ForceMeshUpdate();
                Check(!text.isTextOverflowing||text.overflowMode==TextOverflowModes.Page,"TMP bounded or genuinely paginated: "+text.name);
                if(text!=tools.View.Body)Check(text.textInfo.pageCount<=1,"No hidden page in static label "+text.name);
            }
        }
        void Render(string name,Vector3 look)
        {
            string folder=Mode.StartsWith("pilot-")?"Docs/Previews/SIM003Pilot/"+Mode:
                Mode.StartsWith("physical-")?"Docs/Previews/PhysicalClipboard/"+Mode:
                Mode=="closeui"?"Docs/Previews/OfficeDetailPolish/Runtime":
                Mode=="welcome-before"?"Docs/Previews/WelcomeTypography/Before":
                Mode=="welcome-after"?"Docs/Previews/WelcomeTypography/After":GalleryDirectory+"/"+Mode;Directory.CreateDirectory(folder);
            var go=new GameObject("Temporary immersive capture camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(gaze.View);camera.enabled=false;
            camera.transform.position=gaze.View.transform.position;camera.transform.LookAt(look);camera.aspect=16f/9;
            var target=new RenderTexture(1600,900,24);var previous=RenderTexture.active;Texture2D image=null;
            try {
                target.Create();Canvas.ForceUpdateCanvases();
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();
                File.WriteAllBytes(folder+"/"+name+".png",image.EncodeToPNG());
                File.AppendAllText(folder+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" Unity="+Application.unityVersion+" scene="+UnityEngine.SceneManagement.SceneManager.GetActiveScene().path+" eye="+camera.transform.position+" forward="+camera.transform.forward+" dwell="+gaze.DwellProgress+" play=true source=URP_EDITOR_CAMERA NOT_ANDROID\n");
            }finally{RenderTexture.active=previous;if(image!=null)DestroyImmediate(image);target.Release();DestroyImmediate(target);DestroyImmediate(go);}
        }
        IEnumerator GameView(string name)
        {
            var folder=Mode.StartsWith("pilot-")?"Docs/Previews/SIM003Pilot/"+Mode:GalleryDirectory+"/"+Mode;
            Directory.CreateDirectory(folder);var path=folder+"/"+name+"-gameview.png";
            ScreenCapture.CaptureScreenshot(path);
            // ScreenCapture writes at frame end; keep the tested gaze stable until rendering completes.
            yield return new WaitForEndOfFrame();yield return null;
            float end=Time.realtimeSinceStartup+3;while(!File.Exists(path)&&Time.realtimeSinceStartup<end)yield return null;
            File.AppendAllText(folder+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" GameView="+File.Exists(path)+" NOT_ANDROID\n");
        }
        void OnDestroy(){Application.logMessageReceived-=Log;RestoreMouse();}
    }
}

#endif

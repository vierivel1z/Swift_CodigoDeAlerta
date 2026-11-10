using UnityEngine;
using Swift.Game;
using Swift.Bank;
namespace Swift.Interaction
{
    public enum BankAction { Inspect, Registry, Alerts, Evidence, Defense, End, Attack, DefenseMode, Approve, Hold, Return, Teleport, SkipIntro, ViewDossier, Procedure, History, TutorialStart, TutorialNext, TutorialSkip, TutorialDecision, Sit, Stand, FormatReference, Notebook, MoreObservations, BackToDocument, ObservationNext, ObservationPrevious, DeskTool }
    public sealed class BankInteractable : MonoBehaviour
    {
        public GameManager Manager;
        public BankAction Action;
        public Transform Player;
        public Vector3 Destination;
        public Swift.UI.VRQuickTutorial Tutorial;
        public int TutorialToken, TutorialChoice;
        public bool CanActivate => Swift.DeskTools.DeskInteractionCoordinator.Allows(this);
        public void Activate()
        {
            var audio=Swift.Audio.FeedbackAudio.Current;
            var seat=Swift.VR.AnalystSeat.Current;
            if(!CanActivate){audio?.Emit(Swift.Audio.AudioCue.Unavailable);return;}
            if(Action==BankAction.Stand)Swift.DeskTools.DeskInteractionCoordinator.Current?.CloseInspection();
            if(Action==BankAction.Sit||Action==BankAction.Stand){bool accepted=seat!=null&&seat.Request(Action==BankAction.Sit);audio?.Emit(accepted?Swift.Audio.AudioCue.Confirm:Swift.Audio.AudioCue.Unavailable);return;}
            if(seat!=null&&(seat.IsTransitioning||(seat.IsSeated&&Action==BankAction.Teleport)))return;
            if(Action==BankAction.TutorialStart) {
                var tutorial=FindFirstObjectByType<Swift.UI.VRQuickTutorial>();bool running=tutorial!=null&&tutorial.IsRunning;tutorial?.Begin();
                audio?.Emit(!running&&tutorial!=null&&tutorial.IsRunning?Swift.Audio.AudioCue.Confirm:Swift.Audio.AudioCue.Unavailable);return;
            }
            if(Action==BankAction.TutorialSkip) {Tutorial?.Close();audio?.Emit(Swift.Audio.AudioCue.Confirm);return;}
            if(Action==BankAction.TutorialNext||Action==BankAction.TutorialDecision) {Tutorial?.Select(TutorialToken,TutorialChoice);audio?.Emit(Swift.Audio.AudioCue.Confirm);return;}
            if(Swift.UI.VRQuickTutorial.Active!=null) {audio?.Emit(Swift.Audio.AudioCue.Unavailable);return;}
            if (Manager == null && Action == BankAction.Teleport) Manager = FindFirstObjectByType<GameManager>();
            if (Action == BankAction.SkipIntro) {bool playing=Manager!=null&&Manager.IntroPlaying;Manager?.SkipIntro();audio?.Emit(playing?Swift.Audio.AudioCue.Confirm:Swift.Audio.AudioCue.Unavailable);return;}
            if (Manager != null && Manager.IntroPlaying) {audio?.Emit(Swift.Audio.AudioCue.Unavailable);return;}
            if (Action == BankAction.Teleport) {if(Player!=null)Player.position=Destination;audio?.Emit(Player!=null?Swift.Audio.AudioCue.Teleport:Swift.Audio.AudioCue.Unavailable);return;}
            if (Manager == null) return;
            if(Swift.DeskTools.DeskInteractionCoordinator.Current?.TryHandle(this)==true)return;
            var before=Swift.Audio.FeedbackAudio.Capture(Manager);audio?.SetActionOrigin(transform.position);
            switch (Action) {
                case BankAction.Attack: if(FindFirstObjectByType<Swift.UI.ModeStationVisual>()?.PresentSelection(Action)!=true) Manager.EnterScenario(ScenarioMode.Attack); break;
                case BankAction.DefenseMode: if(FindFirstObjectByType<Swift.UI.ModeStationVisual>()?.PresentSelection(Action)!=true) Manager.EnterScenario(ScenarioMode.Defense); break;
                case BankAction.MoreObservations: FindFirstObjectByType<Swift.UI.BankDocumentUI>()?.ExpandObservations(); break;
                case BankAction.BackToDocument: FindFirstObjectByType<Swift.UI.BankDocumentUI>()?.Back(); break;
                case BankAction.ObservationNext: FindFirstObjectByType<Swift.UI.BankDocumentUI>()?.StepObservations(1); break;
                case BankAction.ObservationPrevious: FindFirstObjectByType<Swift.UI.BankDocumentUI>()?.StepObservations(-1); break;
                case BankAction.FormatReference: FindFirstObjectByType<Swift.UI.FormatReferencePanel>()?.Refresh(); break;
                case BankAction.Notebook: Show(Swift.UI.BankDocumentUI.Page.Procedure); break;
                case BankAction.Approve: Manager.Decide(Decision.Approve); break;
                case BankAction.Hold: Manager.Decide(Decision.Hold); break;
                case BankAction.Return: Manager.Decide(Decision.Return); break;
                case BankAction.Registry: Manager.InspectRegistry(); Show(Swift.UI.BankDocumentUI.Page.Registry); break;
                case BankAction.Alerts: Manager.InspectAlerts(); Show(Swift.UI.BankDocumentUI.Page.Alerts); break;
                case BankAction.ViewDossier: Show(Swift.UI.BankDocumentUI.Page.Dossier); break;
                case BankAction.Procedure: Show(Swift.UI.BankDocumentUI.Page.Procedure); break;
                case BankAction.History: Show(Swift.UI.BankDocumentUI.Page.History); break;
                case BankAction.Evidence: Manager.RegisterEvidence(); Swift.DeskTools.DeskInteractionCoordinator.Current?.PrinterEvidenceCommitted(before.Evidence); break;
                case BankAction.Defense: Manager.ApplyDefense(); break;
                case BankAction.End: Manager.EndScenario(); break;
                default: Manager.Notify("Consulta el expediente central y compara el registro y las alertas."); Show(Swift.UI.BankDocumentUI.Page.Dossier); break;
            }
            Swift.DeskTools.DeskInteractionCoordinator.Current?.ObserveQuery(Action);
            audio?.AfterAction(Action,before);
        }
        void Show(Swift.UI.BankDocumentUI.Page page) { FindFirstObjectByType<Swift.UI.UIManager>()?.ShowDocument(page); }
    }
}

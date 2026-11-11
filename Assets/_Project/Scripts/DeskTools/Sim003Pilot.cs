using System;
using System.Linq;
using UnityEngine;
using Swift.Bank;
using Swift.Game;
using Swift.Interaction;
using Swift.UI;

namespace Swift.DeskTools
{
    // Opt-in presentation experiment for one authored record. BankingDomain is unchanged.
    public sealed class Sim003Pilot : MonoBehaviour
    {
        public enum Finding { None, Matches, Differs, Unverified }
        public static Sim003Pilot Current { get; private set; }
        public UIManager Owner { get; private set; }
        public bool SelectedAccount { get; private set; }
        public bool SearchedAccount { get; private set; }
        public string SelectedAccountNumber { get; private set; }
        public Finding RecordedFinding { get; private set; }
        public Decision? PendingDecision { get; private set; }
        public bool ReviewPending { get; private set; }
        public TransactionRecord ReviewedRecord { get; private set; }
        public Decision? ChosenDecision { get; private set; }
        public bool FindingSupported { get; private set; }
        public int ExperimentalScore { get; private set; }
        public bool PrintedEvidence { get; private set; }
        public Sim003PrinterPresentation Printer { get; private set; }
        public string Hint { get; private set; }
        GameSession observedSession;
        BankDocumentUI.Page returnPage;

        public bool ActiveForCurrent => Owner?.Manager?.Session==observedSession &&
            Owner.Manager.Session?.Transactions.Current?.Data.Id=="SIM-003" && !Owner.Manager.Session.Ended;
        public bool ShowingPilot => ActiveForCurrent || ReviewPending;
        public string RegisteredName => SearchedAccount && ActiveForCurrent
            ? Owner.Manager.Session.Transactions.Current.Data.RegisteredName : null;
        public bool WasPhoneUsed => DeskInteractionCoordinator.Current?.Journal.For("SIM-003")?.PhoneEvidence!=null;

        public void Initialize(UIManager owner)
        {
            Owner=owner;Current=this;observedSession=owner.Manager.Session;
            if(owner.DeskTools.Sim003PrinterEnabled){
                var target=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None)
                    .FirstOrDefault(x=>x.Action==BankAction.Evidence&&x.name=="Interactive Printer");
                if(target!=null){Printer=target.gameObject.AddComponent<Sim003PrinterPresentation>();Printer.Initialize(owner,this,target.transform);}
            }
            owner.Manager.Changed+=Observe;
            owner.Manager.DecisionCommitted+=Committed;
        }
        void Observe()
        {
            var session=Owner.Manager.Session;
            if(ReferenceEquals(session,observedSession))return;
            observedSession=session;SelectedAccount=false;SearchedAccount=false;SelectedAccountNumber=null;
            RecordedFinding=Finding.None;PendingDecision=null;ReviewPending=false;ReviewedRecord=null;
            ChosenDecision=null;FindingSupported=false;ExperimentalScore=0;PrintedEvidence=false;Hint=null;
            Printer?.Clear();
        }
        void Changed(){Owner.RefreshPresentation();}
        public bool Allows(BankInteractable target)
        {
            if(Printer!=null&&(Printer.State==Sim003PrinterPresentation.PrintState.Lifting||Printer.State==Sim003PrinterPresentation.PrintState.Inspecting))
                return target.Action==BankAction.Stand||target.Action==BankAction.PilotReturnPrint||target.Action==BankAction.PilotPrintExpand;
            if(!ReviewPending&&!PendingDecision.HasValue)return true;
            if(target.Action==BankAction.Stand)return true;
            if(ReviewPending)return target.Action==BankAction.PilotContinue;
            return target.Action==BankAction.PilotConfirm||target.Action==BankAction.PilotCancel;
        }
        public bool TryHandle(BankInteractable target)
        {
            var action=target.Action;
            if(Printer!=null&&(Printer.State==Sim003PrinterPresentation.PrintState.Lifting||Printer.State==Sim003PrinterPresentation.PrintState.Inspecting)){
                if(action==BankAction.PilotReturnPrint){Printer.Return();return true;}
                if(action==BankAction.PilotPrintExpand){Printer.Return();Owner.ShowDocument(BankDocumentUI.Page.PilotPrintExpanded);return true;}
                return false;
            }
            if(ReviewPending){if(action==BankAction.PilotContinue){Continue();return true;}return false;}
            if(PendingDecision.HasValue){
                if(action==BankAction.PilotConfirm){Confirm();return true;}
                if(action==BankAction.PilotCancel){Cancel();return true;}
                return false;
            }
            if(!ActiveForCurrent)return false;
            switch(action){
                case BankAction.Registry:ShowRegistry();return true;
                case BankAction.Alerts:Owner.ShowDocument(BankDocumentUI.Page.Alerts);return true;
                case BankAction.Evidence:
                    if(!SearchedAccount){Hint="Consulta REGISTRO antes de solicitar la constancia.";Owner.ShowDocument(BankDocumentUI.Page.Registry);Changed();return true;}
                    if(Printer==null){Hint="Impresora no disponible en esta variante.";Changed();return true;}
                    var requested=Printer.Request(Owner.Manager.Session.Transactions.Current.Data,
                        Owner.Manager.Session.Mode==ScenarioMode.Attack&&Owner.DeskTools.Sim003PrinterFailsInAttack);
                    Hint=Printer.State==Sim003PrinterPresentation.PrintState.Failed?
                        "Fallo de impresión. La consulta digital sigue disponible; no indica fraude.":
                        requested?"Constancia solicitada. Examina la hoja en la bandeja de la impresora.":"Ya hay una constancia disponible; no se generó otra copia.";
                    if(requested&&Printer.State!=Sim003PrinterPresentation.PrintState.Failed){PrintedEvidence=true;DeskInteractionCoordinator.Current?.Journal.Consult("SIM-003","Constancia impresa");}
                    Changed();return true;
                case BankAction.PilotInspectPrint:
                    if(Printer!=null&&Printer.State==Sim003PrinterPresentation.PrintState.Tray){
                        Printer.Inspect(FindFirstObjectByType<VRInteractionManager>().View.transform);return true;}
                    return true;
                case BankAction.PilotSelectAccount:
                    SelectedAccountNumber=Owner.Manager.Session.Transactions.Current.Data.Account;
                    SelectedAccount=true;SearchedAccount=false;RecordedFinding=Finding.None;Hint="Cuenta seleccionada. Busca para consultar el titular.";
                    Owner.ShowDocument(BankDocumentUI.Page.Registry);Changed();return true;
                case BankAction.PilotSearch:
                    if(SelectedAccount){SearchedAccount=true;Hint="Consulta realizada. Compara los nombres y registra tu observación.";
                        DeskInteractionCoordinator.Current?.Journal.Consult("SIM-003","Registro de cuentas");}
                    else Hint="Selecciona primero la cuenta del expediente.";
                    Changed();return true;
                case BankAction.PilotMatches:Record(Finding.Matches);return true;
                case BankAction.PilotDiffers:Record(Finding.Differs);return true;
                case BankAction.PilotUnverified:Record(Finding.Unverified);return true;
                case BankAction.Approve:Begin(Decision.Approve);return true;
                case BankAction.Hold:Begin(Decision.Hold);return true;
                case BankAction.Return:Begin(Decision.Return);return true;
            }
            return false;
        }
        void ShowRegistry(){Hint=SearchedAccount?"Resultado de la consulta disponible.":"Selecciona la cuenta y activa BUSCAR.";Owner.ShowDocument(BankDocumentUI.Page.Registry);}
        void Record(Finding finding)
        {
            if(!SearchedAccount){Hint="Realiza la consulta antes de registrar un hallazgo.";Changed();return;}
            RecordedFinding=finding;Hint="Hallazgo guardado. Puedes corregirlo antes de decidir.";Changed();
        }
        void Begin(Decision decision)
        {
            returnPage=Owner.DocumentPage;PendingDecision=decision;Hint=null;
            Owner.ShowDocument(BankDocumentUI.Page.PilotConfirm);
        }
        void Cancel(){PendingDecision=null;Owner.ShowDocument(returnPage==BankDocumentUI.Page.PilotConfirm?BankDocumentUI.Page.Dossier:returnPage);}
        void Confirm()
        {
            if(!PendingDecision.HasValue)return;
            var decision=PendingDecision.Value;PendingDecision=null;
            Owner.Manager.Decide(decision);
        }
        void Committed(GameSession session,TransactionRecord row,Decision decision)
        {
            if(!ReferenceEquals(session,observedSession)||row?.Data.Id!="SIM-003")return;
            ReviewedRecord=row;ChosenDecision=decision;ReviewPending=true;
            FindingSupported=SearchedAccount&&RecordedFinding==Finding.Differs&&
                !string.Equals(row.Data.Beneficiary,row.Data.RegisteredName,StringComparison.Ordinal);
            ExperimentalScore=(row.CorrectDecision==true?10:0)+(FindingSupported?5:0);
            Printer?.Clear();
            Owner.ShowDocument(BankDocumentUI.Page.PilotReview);
        }
        void Continue(){ReviewPending=false;Hint=null;Owner.ShowDocument(BankDocumentUI.Page.Dossier);}
        public string FindingText => RecordedFinding==Finding.Matches?"Coincide":RecordedFinding==Finding.Differs?"No coincide":
            RecordedFinding==Finding.Unverified?"No pude comprobarlo":"Sin hallazgo registrado";
        void OnDestroy(){
            if(Owner?.Manager!=null){Owner.Manager.Changed-=Observe;Owner.Manager.DecisionCommitted-=Committed;}
            if(Current==this)Current=null;
        }
    }
}

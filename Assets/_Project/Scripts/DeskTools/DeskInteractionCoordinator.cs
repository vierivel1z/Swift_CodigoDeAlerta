using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Swift.Bank;
using Swift.Game;
using Swift.Interaction;
using Swift.UI;
using Swift.VR;
using Swift.Audio;

namespace Swift.DeskTools
{
    public sealed class DeskInteractionCoordinator : MonoBehaviour
    {
        public static DeskInteractionCoordinator Current { get; private set; }
        public UIManager Owner { get; private set; }
        public GameManager Manager => Owner!=null?Owner.Manager:null;
        public DeskToolsProfile Profile { get; private set; }
        public DeskSessionJournal Journal { get; } = new DeskSessionJournal();
        public DeskInspectionView View { get; private set; }
        public DeskInspectionView ExpandedView { get; private set; }
        public PhysicalClipboardPresentation PhysicalClipboard { get; private set; }
        public VerificationConversation Call { get; private set; }
        public BankInteractable ClipboardTarget { get; private set; }
        public BankInteractable PhoneTarget { get; private set; }
        public BankInteractable DocumentTarget { get; private set; }
        public DeskObjectFeedback PrinterFeedback { get; private set; }
        public string ActiveTool { get; private set; }
        public int Page { get; private set; }
        public bool InspectionOpen => !string.IsNullOrEmpty(ActiveTool);
        AnalystSeat seat;VRInteractionManager gaze;
        GameSession observedSession;TransactionRecord observedRecord;
        int evidenceCount,bodyPage=1,phoneLine;
        VerificationScript availableCall;
        GameObject phoneObject;Coroutine ringRoutine;
        readonly List<UnityEngine.Object> attachments=new List<UnityEngine.Object>();
        readonly List<DeskObjectFeedback> feedback=new List<DeskObjectFeedback>();
        TMPro.TMP_Text phoneStatus,printerStatus;

        public void Initialize(UIManager owner)
        {
            Owner=owner;Profile=owner.DeskTools;Current=this;seat=AnalystSeat.Current;gaze=FindFirstObjectByType<VRInteractionManager>();
            var panel=Profile.InspectionPrefab!=null?Instantiate(Profile.InspectionPrefab,transform):new GameObject("Desk inspection",typeof(RectTransform),typeof(Canvas),typeof(DeskInspectionView));
            panel.transform.SetParent(transform,false);View=panel.GetComponent<DeskInspectionView>();View.Build(owner.Font,owner.StationFrame,owner.StationFill);View.Bind(this);panel.SetActive(false);ExpandedView=View;
            ClipboardTarget=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).FirstOrDefault(x=>x.Action==BankAction.Notebook);
            if(Profile.ClipboardEnabled&&ClipboardTarget!=null) {
                Attach(ClipboardTarget,DeskCommand.Clipboard);
                if(Profile.PhysicalClipboardEnabled) {
                    var paper=Profile.InspectionPrefab!=null?Instantiate(Profile.InspectionPrefab,ClipboardTarget.transform):new GameObject("Texto sobre papel",typeof(RectTransform),typeof(Canvas),typeof(DeskInspectionView));
                    var paperView=paper.GetComponent<DeskInspectionView>();paperView.Build(owner.Font,owner.StationFrame,owner.StationFill);
                    paperView.ConfigureForPhysicalPaper();paperView.Bind(this);
                    if(Profile.PolishedClipboardVisual)paperView.PolishPhysicalControls();
                    PhysicalClipboard=ClipboardTarget.gameObject.AddComponent<PhysicalClipboardPresentation>();
                    PhysicalClipboard.Initialize(ClipboardTarget.transform,paperView,owner.Font,Profile.PolishedClipboardVisual);
                    attachments.Add(PhysicalClipboard);attachments.Add(paper);
                }
            }
            var document=GameObject.Find("Fictional transfer documents");
            if(Profile.DocumentInspectionEnabled&&document!=null) {
                DocumentTarget=document.GetComponent<BankInteractable>();
                if(DocumentTarget==null){DocumentTarget=document.AddComponent<BankInteractable>();DocumentTarget.Action=BankAction.DeskTool;attachments.Add(DocumentTarget);}
                DocumentTarget.Manager=Manager;
                if(DocumentTarget.GetComponent<Collider>()==null) {
                    var renderers=document.GetComponentsInChildren<MeshRenderer>();
                    if(renderers.Length>0) {
                        var bounds=renderers[0].bounds;foreach(var renderer in renderers.Skip(1))bounds.Encapsulate(renderer.bounds);
                        var collider=document.AddComponent<BoxCollider>();collider.center=document.transform.InverseTransformPoint(bounds.center);
                        collider.size=new Vector3(bounds.size.x/document.transform.lossyScale.x,bounds.size.y/document.transform.lossyScale.y,bounds.size.z/document.transform.lossyScale.z)+Vector3.one*.015f;
                        attachments.Add(collider);
                    }
                }
                Attach(DocumentTarget,DeskCommand.Document);
            }
            if(Profile.PhoneEnabled&&Profile.PhonePrefab!=null) {
                phoneObject=Instantiate(Profile.PhonePrefab,transform);
                phoneObject.transform.position=new Vector3(1.32f,.80f,1.55f);
                phoneObject.transform.rotation=Quaternion.identity;
                PhoneTarget=phoneObject.GetComponent<BankInteractable>();PhoneTarget.Manager=Manager;
                phoneObject.GetComponent<DeskActionTarget>().Owner=this;
                Attach(PhoneTarget,DeskCommand.Phone);
                phoneStatus=phoneObject.GetComponentsInChildren<TMPro.TMP_Text>(true).FirstOrDefault();
            }
            // The desk documents also carry the legacy Evidence action, so renderer order
            // must never decide which physical object receives printer confirmation.
            var printer=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None)
                .FirstOrDefault(x=>x.Action==BankAction.Evidence&&x.name=="Interactive Printer"&&x.GetComponentsInChildren<MeshRenderer>().Length>0);
            if(Profile.PrinterFeedbackEnabled&&printer!=null) {
                PrinterFeedback=printer.gameObject.AddComponent<DeskObjectFeedback>();PrinterFeedback.Target=printer;attachments.Add(PrinterFeedback);
                if(printer.GetComponent<DocumentFocus>()==null){var focus=printer.gameObject.AddComponent<DocumentFocus>();focus.Target=printer;attachments.Add(focus);}
                printerStatus=StatusPlate(printer.transform,"EVIDENCIA / simulación actual",new Vector3(0,.50f,-.26f),.54f);
            }
            if(Profile.SeatPolishEnabled&&seat!=null) {
                var visual=seat.transform.Find("chairDesk");
                // Never animate a collider or tracking anchor as presentation.
                if(visual!=null&&visual.GetComponentsInChildren<Collider>().Length==0) {
                    var presentation=seat.gameObject.AddComponent<SeatPresentationFeedback>();presentation.Seat=seat;presentation.ChairVisual=visual;attachments.Add(presentation);
                }
            }
            Manager.Changed+=Observe;Manager.DecisionCommitted+=OnDecision;Observe();
        }
        void Attach(BankInteractable bank,DeskCommand command)
        {
            var target=bank.GetComponent<DeskActionTarget>();
            if(target==null){target=bank.gameObject.AddComponent<DeskActionTarget>();attachments.Add(target);}
            target.Command=command;target.Owner=this;
            if(bank.GetComponent<DocumentFocus>()==null){var f=bank.gameObject.AddComponent<DocumentFocus>();f.Target=bank;attachments.Add(f);}
            var visual=bank.GetComponent<DeskObjectFeedback>();
            if(visual==null){visual=bank.gameObject.AddComponent<DeskObjectFeedback>();visual.Target=bank;attachments.Add(visual);}
            feedback.Add(visual);
        }
        TMPro.TMP_Text StatusPlate(Transform parent,string value,Vector3 local,float width)
        {
            var go=new GameObject("Estado de evidencia / soporte local",typeof(RectTransform),typeof(Canvas));
            go.transform.SetParent(parent,false);go.transform.localPosition=local;go.transform.localScale=Vector3.one*.001f;
            go.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;go.GetComponent<RectTransform>().sizeDelta=new Vector2(width*1000,145);
            var surface=new GameObject("Contenedor",typeof(RectTransform),typeof(UnityEngine.UI.Image));surface.transform.SetParent(go.transform,false);surface.GetComponent<RectTransform>().sizeDelta=new Vector2(width*1000,145);
            surface.GetComponent<UnityEngine.UI.Image>().color=new Color(.02f,.12f,.15f);surface.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;
            var text=Owner.Text(surface.transform,"Estado",Vector2.zero,new Vector2(width*1000-28,124),27);text.text=value;text.richText=false;text.alignment=TMPro.TextAlignmentOptions.Center;
            attachments.Add(go);return text;
        }
        public static bool Allows(BankInteractable target)
        {
            var self=Current;
            if(self==null||!self.isActiveAndEnabled||self.Manager==null||target.Manager!=self.Manager)return true;
            if(target.Action==BankAction.Stand)return true; // Exiting cancels inspection before the seat transition.
            var desk=target.GetComponent<DeskActionTarget>();
            if(self.InspectionOpen) return desk!=null&&desk.Owner==self&&desk.transform.IsChildOf(self.View.transform)&&self.CanHandle(desk.Command);
            if(desk!=null&&desk.Owner==self)return self.CanHandle(desk.Command);
            if(target.Action==BankAction.Notebook&&self.Profile.ClipboardEnabled)return self.CanWork;
            return true;
        }
        public bool TryHandle(BankInteractable target)
        {
            var desk=target.GetComponent<DeskActionTarget>();
            if(desk!=null&&desk.Owner==this){Handle(desk.Command);return true;}
            if(target.Action==BankAction.Notebook&&Profile.ClipboardEnabled){Handle(DeskCommand.Clipboard);return true;}
            return false;
        }
        bool SeatedAndReady => seat!=null&&seat.IsSeated&&!seat.IsTransitioning&&Manager!=null&&!Manager.IntroPlaying&&VRQuickTutorial.Active==null;
        public bool CanWork => SeatedAndReady&&Manager.Session?.Transactions.Current!=null&&Manager.Session.Ended==false;
        public bool PhoneAvailable => CanWork&&Profile.PhoneEnabled&&Manager.Session.Mode==ScenarioMode.Defense&&
            Manager.Session.Transactions.Current.Data.Id=="SIM-003"&&availableCall!=null&&(!availableCall.PrototypeContent||Profile.AllowPrototypeCalls);
        public bool CanHandle(DeskCommand command)
        {
            if(command==DeskCommand.Close||command==DeskCommand.EndCall)return InspectionOpen;
            if(!SeatedAndReady)return false;
            if(command==DeskCommand.Debrief)return Manager.Session?.Ended==true;
            if(ActiveTool=="Debrief")return command==DeskCommand.Previous||command==DeskCommand.Next;
            if(!CanWork)return false;
            switch(command) {
                case DeskCommand.Clipboard:return Profile.ClipboardEnabled&&!InspectionOpen;
                case DeskCommand.Document:return Profile.DocumentInspectionEnabled&&!InspectionOpen;
                case DeskCommand.Phone:return PhoneAvailable&&!InspectionOpen;
                case DeskCommand.RequestCall:return ActiveTool=="Phone"&&Call?.State==VerificationState.Ready;
                case DeskCommand.AnswerCall:return ActiveTool=="Phone"&&Call?.State==VerificationState.Ringing;
                case DeskCommand.RegisterCall:return ActiveTool=="Phone"&&Call!=null&&Call.CanRegister(Manager.Session);
                case DeskCommand.Mark:return ActiveTool=="Clipboard";
                case DeskCommand.Expand:return ActiveTool=="Clipboard"&&PhysicalClipboard!=null&&View==PhysicalClipboard.PaperView&&PhysicalClipboard.State==PhysicalClipboardPresentation.PoseState.Inspecting;
                case DeskCommand.Next:case DeskCommand.Previous:return InspectionOpen&&(ActiveTool!="Phone"||Call?.State==VerificationState.Connected||Call?.State==VerificationState.Reviewing||Call?.State==VerificationState.Recorded);
            }
            return false;
        }
        public void Handle(DeskCommand command)
        {
            if(!CanHandle(command))return;
            switch(command) {
                case DeskCommand.Clipboard:Open("Clipboard");Journal.Consult(observedRecord.Data.Id,"Portapapeles");RefreshView();break;
                case DeskCommand.Document:Open("Document");Journal.Consult(observedRecord.Data.Id,"Documento");RefreshView();break;
                case DeskCommand.Debrief:Open("Debrief");RefreshView();break;
                case DeskCommand.Phone:
                    var script=Profile.Calls.For(observedRecord.Data.Id);
                    if(script.Validate(Manager.Session.Transactions.Records.Select(x=>x.Data))!=null)return;
                    if(Call==null||!Call.ContextMatches(Manager.Session)||Call.State==VerificationState.Cancelled)Call=new VerificationConversation(Manager.Session,script);
                    phoneLine=Call.State==VerificationState.Recorded?Call.Script.Lines.Length-1:0;Open("Phone");RefreshView();break;
                case DeskCommand.Previous:Step(-1);break;
                case DeskCommand.Next:Step(1);break;
                case DeskCommand.Mark:
                    if(Journal.Toggle(Manager.Session,observedRecord.Data.Id,Page)){FeedbackAudio.Current?.Emit(AudioCue.Confirm);RefreshView();}break;
                case DeskCommand.Expand:SwitchToExpandedClipboard();break;
                case DeskCommand.RequestCall:
                    if(Call.Request(Manager.Session)){RefreshView();ringRoutine=StartCoroutine(RingAfterRequest());}break;
                case DeskCommand.AnswerCall:
                    if(Call.Answer(Manager.Session)){FeedbackAudio.Current?.StopCue(AudioCue.PhoneRing);FeedbackAudio.Current?.Emit(AudioCue.PhoneAnswer,PhoneTarget.transform.position);Journal.Consult(observedRecord.Data.Id,"Teléfono");RefreshView();}break;
                case DeskCommand.RegisterCall:RegisterCall();break;
                case DeskCommand.Close:case DeskCommand.EndCall:CloseInspection();break;
            }
        }
        IEnumerator RingAfterRequest()
        {
            yield return new WaitForSecondsRealtime(.65f);
            ringRoutine=null;
            if(Call!=null&&Call.Ring(Manager.Session)){
                FeedbackAudio.Current?.Emit(AudioCue.PhoneRing,PhoneTarget.transform.position);RefreshView();RefreshPhoneState();
            }
        }
        void Open(string tool)
        {
            View=tool=="Clipboard"&&PhysicalClipboard!=null?PhysicalClipboard.PaperView:ExpandedView;
            ActiveTool=tool;Page=0;bodyPage=1;View.Body.pageToDisplay=1;
            if(tool=="Clipboard"&&PhysicalClipboard!=null){
                PhysicalClipboard.Lift(seat.Eye);
                FeedbackAudio.Current?.Emit(AudioCue.Dossier);
                return;
            }
            // World anchor relative to the station, never a child of the tracked camera.
            var eye=seat.Eye.position;var forward=Vector3.ProjectOnPlane(Owner.PanelAnchor.forward,Vector3.up).normalized;
            var right=Vector3.Cross(Vector3.up,forward);
            bool rightTool=tool=="Phone"||tool=="Document";
            var center=rightTool?eye+forward*.90f+right*1.05f-Vector3.up*.08f:eye+forward*.70f-right*1.30f-Vector3.up*.10f;
            View.transform.SetPositionAndRotation(center,Quaternion.LookRotation(center-eye,Vector3.up));
            View.transform.localScale=Vector3.one*(rightTool?.00095f:.00105f);View.gameObject.SetActive(true);
            FeedbackAudio.Current?.Emit(AudioCue.Dossier);
        }
        void SwitchToExpandedClipboard()
        {
            View.gameObject.SetActive(false);
            PhysicalClipboard.Return();
            View=ExpandedView;bodyPage=1;View.Body.pageToDisplay=1;
            var eye=seat.Eye.position;
            var forward=Vector3.ProjectOnPlane(Owner.PanelAnchor.forward,Vector3.up).normalized;
            var right=Vector3.Cross(Vector3.up,forward);
            var center=eye+forward*.70f-right*1.30f-Vector3.up*.10f;
            View.transform.SetPositionAndRotation(center,Quaternion.LookRotation(center-eye,Vector3.up));
            View.transform.localScale=Vector3.one*.00105f;
            View.gameObject.SetActive(true);
            RefreshView();
        }
        public void CloseInspection()
        {
            if(!InspectionOpen)return;
            if(ringRoutine!=null){StopCoroutine(ringRoutine);ringRoutine=null;}
            if(ActiveTool=="Phone"){Call?.Cancel();FeedbackAudio.Current?.StopCue(AudioCue.PhoneRing);FeedbackAudio.Current?.Emit(AudioCue.PhoneEnd,PhoneTarget!=null?PhoneTarget.transform.position:(Vector3?)null);}
            bool wasPhysical=ActiveTool=="Clipboard"&&PhysicalClipboard!=null;
            ActiveTool=null;if(View!=null)View.gameObject.SetActive(false);
            if(wasPhysical)PhysicalClipboard.Return();
            View=ExpandedView;RefreshPhoneState();
        }
        void Step(int direction)
        {
            View.Body.ForceMeshUpdate();
            if(direction>0&&bodyPage<View.Body.textInfo.pageCount){bodyPage++;View.Body.pageToDisplay=bodyPage;FeedbackAudio.Current?.Emit(AudioCue.PageTurn);UpdatePageNumber();if(ActiveTool=="Clipboard")PhysicalClipboard?.AnimatePageTurn();return;}
            if(direction<0&&bodyPage>1){bodyPage--;View.Body.pageToDisplay=bodyPage;FeedbackAudio.Current?.Emit(AudioCue.PageTurn);UpdatePageNumber();if(ActiveTool=="Clipboard")PhysicalClipboard?.AnimatePageTurn();return;}
            bodyPage=1;View.Body.pageToDisplay=1;
            if(ActiveTool=="Phone") {
                phoneLine=Mathf.Clamp(phoneLine+direction,0,Call.Script.Lines.Length-1);
                if(phoneLine==Call.Script.Lines.Length-1&&Call.State==VerificationState.Connected)Call.FinishReading(Manager.Session);
            } else {
                int pages=ActiveTool=="Debrief"?Manager.Session.Transactions.Records.Count:ActiveTool=="Clipboard"?3:1;
                Page=(Page+direction+pages)%pages;
            }
            FeedbackAudio.Current?.Emit(AudioCue.PageTurn);RefreshView();if(ActiveTool=="Clipboard")PhysicalClipboard?.AnimatePageTurn();
        }
        void RegisterCall()
        {
            var live=Manager.Session;
            if(Call==null||!Call.CanRegister(live)||Journal.For(Call.Record.Data.Id).PhoneEvidence!=null)return;
            var expected=Call.Record.Data.Id+": "+Call.EvidenceText.Trim();
            Manager.RegisterEvidence(Call.EvidenceText);
            if(Call.ContextMatches(Manager.Session)&&Manager.Session.Evidence.Entries.Contains(expected)&&Call.MarkRecorded(Manager.Session)) {
                Journal.RecordPhone(Call,Manager.Session);RefreshView();RefreshPhoneState();
                PhoneTarget?.GetComponent<DeskObjectFeedback>()?.Confirm();
            }
        }
        public void PrinterEvidenceCommitted(int previousCount)
        {
            int count=Manager.Session?.Evidence.Entries.Count??0;
            if(!Profile.PrinterFeedbackEnabled||count<=previousCount)return;
            PrinterFeedback?.Confirm();
            if(printerStatus!=null)printerStatus.text="EVIDENCIA REGISTRADA\n"+count+" / simulación actual";
            FeedbackAudio.Current?.Emit(AudioCue.Printer,PrinterFeedback!=null?PrinterFeedback.transform.position:(Vector3?)null);
        }
        public void ShowPrinterStatus(string value){if(printerStatus!=null)printerStatus.text=value;}
        public void ObserveQuery(BankAction action)
        {
            if(observedRecord==null)return;
            string source=action==BankAction.Registry?"Registro":action==BankAction.Alerts?"Alertas":action==BankAction.Procedure?"Procedimiento":null;
            if(source!=null)Journal.Consult(observedRecord.Data.Id,source);
        }
        void Observe()
        {
            var live=Manager.Session;var row=live?.Transactions.Current;bool changed=!ReferenceEquals(live,observedSession)||!ReferenceEquals(row,observedRecord);
            if(changed){CloseInspection();Call=null;}
            if(!ReferenceEquals(live,observedSession)) {
                foreach(var f in feedback)if(f!=null)f.ResetConfirmation();
                PrinterFeedback?.ResetConfirmation();if(printerStatus!=null)printerStatus.text="EVIDENCIA / simulación actual";Journal.Bind(live);evidenceCount=live?.Evidence.Entries.Count??0;
            }
            observedSession=live;observedRecord=row;
            availableCall=Profile.Calls!=null&&row!=null?Profile.Calls.For(row.Data.Id):null;
            int count=live?.Evidence.Entries.Count??0;
            evidenceCount=count;RefreshPhoneState();
        }
        void OnDecision(GameSession session,TransactionRecord row,Decision decision){Journal.Bind(session);Journal.DecisionCommitted(session,row,decision);}
        void LateUpdate()
        {
            if(InspectionOpen&&(!seat.IsSeated||seat.IsTransitioning||Manager.IntroPlaying||VRQuickTutorial.Active!=null))CloseInspection();
            if(PhysicalClipboard!=null&&(!seat.IsSeated||seat.IsTransitioning||Manager.IntroPlaying||VRQuickTutorial.Active!=null))PhysicalClipboard.RestoreImmediately();
            if(phoneStatus!=null){bool ready=PhoneAvailable;if(phoneStatus.gameObject.activeSelf!=ready)phoneStatus.gameObject.SetActive(ready);}
        }
        void RefreshPhoneState()
        {
            if(phoneStatus==null)return;
            phoneStatus.text=Call?.State==VerificationState.Ringing?"LLAMADA PENDIENTE":Call?.State==VerificationState.Recorded?"CONSULTA REGISTRADA":"CONTRASTE / SIM-003";
        }
        void RefreshView()
        {
            if(!InspectionOpen)return;
            string title="",body="",notice="",heading="",context="";
            View.Previous.GetComponent<Collider>().enabled=true;View.Next.GetComponent<Collider>().enabled=true;
            if(ActiveTool=="Clipboard") {
                var d=observedRecord.Data;var notes=Journal.For(d.Id);heading="SWIFT / PORTAPAPELES";context=d.Id+" / referencia de revisión";
                if(Page==0){title="FORMATO Y PROCEDIMIENTO";body=Sim003Pilot.Current?.ActiveForCurrent==true?
                    "1. Compara beneficiario y titular registrado.\n2. Verifica la cuenta indicada.\n3. Contrasta lo consultado.\n4. Registra cualquier discrepancia.\n5. No apruebes sin aclarar inconsistencias relevantes.":
                    "1. Lee emisor, beneficiario y cuenta.\n2. Comprueba monto e intermediario.\n3. Contrasta registro y alertas.\n4. Documenta y toma tu decisión.";}
                if(Page==1){title="DATOS DEL EXPEDIENTE";body="Emisor: "+Value(d.OriginBank)+"\nBeneficiario: "+Value(d.Beneficiary)+"\nCuenta: "+Value(d.Account)+"\nMonto: USD "+d.Amount.ToString("N0")+"\nIntermediario: "+Value(d.IntermediaryBank)+(string.IsNullOrEmpty(d.Date)?"":"\nFecha: "+d.Date);}
                if(Page==2){title="OBSERVACIONES Y FUENTES";body=(Sim003Pilot.Current?.ActiveForCurrent==true?
                    "Hallazgo elegido: "+Sim003Pilot.Current.FindingText:"Observaciones: "+FormatReferencePanel.ObservedAlerts(d))+"\nFuentes abiertas: "+(notes.Sources.Count==0?"ninguna":string.Join(", ",notes.Sources.OrderBy(x=>x)))+"\nConsulta telefónica registrada: "+(notes.PhoneEvidence==null?"no":"sí");}
                notice="Marca manual: "+(notes.ManualChecks[Page]?"REVISADO":"sin marcar")+". No valida datos ni confirma fraude.";
                View.ConfigurePrimary(DeskCommand.Mark,PhysicalClipboard!=null&&View==PhysicalClipboard.PaperView?(notes.ManualChecks[Page]?"QUITAR":"MARCAR"):(notes.ManualChecks[Page]?"QUITAR MARCA":"MARCAR REVISIÓN"),true);
            } else if(ActiveTool=="Document") {
                var d=observedRecord.Data;heading="SWIFT / DOCUMENTO";context=d.Id+" / extracto ficticio";
                title="ORDEN PARA CONTRASTAR";body="Beneficiario: "+Value(d.Beneficiary)+"\nCuenta: "+Value(d.Account)+"\nMonto: USD "+d.Amount.ToString("N0")+"\nIntermediario: "+Value(d.IntermediaryBank);
                notice="La decisión se realiza en el monitor. La inspección no modifica la operación.";View.ConfigurePrimary(DeskCommand.Mark,"SOLO CONSULTA",false);
            } else if(ActiveTool=="Phone") {
                heading="SWIFT / VERIFICACIÓN";context=Call.Record.Data.Id+" / "+Call.Script.Speaker;
                notice=Call.Script.PrototypeContent?"GUION DE PRUEBA / pendiente de aprobación. No confirma autorización.":"Consulta ficticia. Contrasta las fuentes antes de decidir.";
                switch(Call.State) {
                    case VerificationState.Ready:
                        title="CONSULTA DISPONIBLE";body="Contrasta el nombre asociado a la cuenta.\nSolicita una llamada simulada a la fuente indicada.\nNo activa los controles automáticos.";
                        View.ConfigurePrimary(DeskCommand.RequestCall,"SOLICITAR",true);break;
                    case VerificationState.Requested:
                        title="SOLICITUD EN CURSO";body="Se prepara una devolución de llamada simulada.\nPuedes cerrar sin modificar el expediente.";View.ConfigurePrimary(DeskCommand.AnswerCall,"ESPERANDO",false);break;
                    case VerificationState.Ringing:
                        title="LLAMADA PENDIENTE";body="Mesa de registros / fuente ficticia.\nResponde para consultar la información.";View.ConfigurePrimary(DeskCommand.AnswerCall,"RESPONDER",true);break;
                    default:
                        title=Call.State==VerificationState.Recorded?"CONSULTA REGISTRADA":"TRANSCRIPCIÓN";
                        body=Call.Script.Outcome==VerificationOutcome.NoAnswer?"No se obtuvo respuesta de la fuente.\nLa falta de respuesta no confirma fraude ni autorización.":VerificationScript.Render(Call.Script.Lines[phoneLine],Call.Record.Data);
                        if(Call.State==VerificationState.Reviewing||Call.State==VerificationState.Recorded)
                            View.ConfigurePrimary(DeskCommand.RegisterCall,Call.State==VerificationState.Recorded?"REGISTRADA":"REGISTRAR",Call.State!=VerificationState.Recorded);
                        else View.ConfigurePrimary(DeskCommand.Next,"CONTINUAR",true);
                        break;
                }
                bool reading=Call.State==VerificationState.Connected||Call.State==VerificationState.Reviewing||Call.State==VerificationState.Recorded;
                // Eligibility comes from CanHandle; never remove a collider underneath a completed fixation.
                View.Previous.GetComponent<UnityEngine.UI.Image>().color=reading?new Color(.035f,.19f,.24f):new Color(.075f,.11f,.13f);
                View.Next.GetComponent<UnityEngine.UI.Image>().color=reading?new Color(.035f,.19f,.24f):new Color(.075f,.11f,.13f);
            } else if(ActiveTool=="Debrief") {
                var row=Manager.Session.Transactions.Records[Page];var notes=Journal.For(row.Data.Id);
                heading="SWIFT / REVISIÓN DEL TURNO";context=row.Data.Id+" / explicación posterior";
                title="DECISIÓN Y CONSECUENCIA";
                body="Elegiste: "+(notes.ChosenDecision.HasValue?Choice(notes.ChosenDecision.Value):"No registrado")+"\nResultado final: "+Status(row.Status)+"\nControl defensivo: "+(row.DefenseOverride?"intervino":"no intervino")+
                    "\nFuentes: "+(notes.Sources.Count==0?"ninguna":string.Join(", ",notes.Sources.OrderBy(x=>x)))+"  ·  Llamada: "+(notes.PhoneEvidence==null?"no":"sí")+
                    "\nConsulta: "+(notes.PhoneEvidence??"Sin consulta registrada.");
                notice=row.Data.Anomalies==Anomaly.None?"No había discrepancias en las comprobaciones simuladas.":(row.Data.Anomalies&Anomaly.MissingIntermediary)!=0?"La orden no incluía intermediario. El formato y el riesgo requieren revisiones distintas.":"Las observaciones requieren revisión; una coincidencia no demuestra fraude.";
                View.ConfigurePrimary(DeskCommand.Mark,"SOLO REVISIÓN",false);
            }
            View.Present(heading,context,title,body,notice,"");
            View.Body.pageToDisplay=bodyPage;Canvas.ForceUpdateCanvases();View.Body.ForceMeshUpdate();UpdatePageNumber();
        }
        void UpdatePageNumber()
        {
            int count=ActiveTool=="Clipboard"?3:ActiveTool=="Debrief"?Manager.Session.Transactions.Records.Count:1;
            View.PageLabel.text=ActiveTool=="Phone"?"Consulta: "+CallStatus(Call.State)+" / lectura "+(phoneLine+1)+"/"+Call.Script.Lines.Length:"Hoja "+(Page+1)+" / "+count;
            if(View.Body.textInfo.pageCount>1)View.PageLabel.text+=" · texto "+bodyPage+"/"+View.Body.textInfo.pageCount;
        }
        public string DecisionSummary(TransactionRecord row)
        {
            var notes=Journal.For(row.Data.Id);
            return row.Data.Id+": "+(notes.ChosenDecision.HasValue?Choice(notes.ChosenDecision.Value)+" -> ":"")+Status(row.Status)+(row.DefenseOverride?" (control)":"");
        }
        static string CallStatus(VerificationState state)=>state==VerificationState.Ready?"disponible":state==VerificationState.Requested?"solicitada":state==VerificationState.Ringing?"pendiente":state==VerificationState.Connected?"en curso":state==VerificationState.Reviewing?"para registrar":state==VerificationState.Recorded?"registrada":"cancelada";
        static string Value(string value)=>string.IsNullOrWhiteSpace(value)?"No informado":value;
        static string Choice(Decision decision)=>decision==Decision.Approve?"Aprobar":decision==Decision.Hold?"Retener":"Devolver";
        static string Status(TransactionStatus status)=>status==TransactionStatus.Approved?"Aprobada":status==TransactionStatus.Held?"Retenida":status==TransactionStatus.Returned?"Devuelta":"Pendiente";
        void OnApplicationPause(bool paused){if(paused){CloseInspection();PhysicalClipboard?.RestoreImmediately();}}
        void OnDisable(){CloseInspection();PhysicalClipboard?.RestoreImmediately();if(Current==this)Current=null;}
        void OnDestroy()
        {
            if(Manager!=null){Manager.Changed-=Observe;Manager.DecisionCommitted-=OnDecision;}
            foreach(var attachment in attachments)if(attachment!=null)Destroy(attachment);
            if(phoneObject!=null)Destroy(phoneObject);
        }
    }
}

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Swift.Bank;
using Swift.Interaction;
namespace Swift.UI {
 // Native monitor presentation. GameSession remains the only source of banking data and scores.
 public sealed class BankDocumentUI:MonoBehaviour {
 public enum Page { Dossier,Registry,Alerts,Procedure,History,Observations,PilotConfirm,PilotReview,PilotPrintExpanded }
 public Page CurrentPage{get;private set;}
 UIManager owner;Transform root,content;Image messageWell;TextMeshProUGUI title,metadata,message,footer;
 readonly TextMeshProUGUI[] tabs=new TextMeshProUGUI[4];readonly Image[] decisions=new Image[3];
 readonly GameObject[] utilityActions=new GameObject[3];
 GameSession session;TransactionRecord record;string registryResult;Page previousPage;int observationPage=1;
 bool snapshotValid,cachedEnded,cachedDefenseActive;GameSession cachedSession;TransactionRecord cachedRecord;
 Page cachedPage;string cachedMessage;int cachedObservationPage,cachedEvidenceCount,cachedCorrect;
 long cachedLoss;long? cachedAttackLoss,cachedDefenseLoss;
 public int RebuildCount{get;private set;}
 static readonly Color Navy=new Color(.018f,.05f,.078f),Cyan=new Color(.12f,.81f,.87f),Paper=new Color(.94f,.92f,.86f),Ink=new Color(.025f,.09f,.12f),Muted=new Color(.23f,.35f,.39f);
 public void Initialize(UIManager ui,Transform canvas){
 owner=ui;root=canvas;((RectTransform)root).sizeDelta=new Vector2(1600,900);root.localScale=Vector3.one*.0013f;
 foreach(var name in new[]{"Expediente","Feedback","Resumen","OC Chrome","Fondo de lectura"}){var old=root.Find(name);if(old!=null)old.gameObject.SetActive(false);}
 var surface=Box(root,"Document UI - Operations Final",Vector2.zero,new Vector2(1600,900),Navy).transform;Frame(surface,new Vector2(1600,900),Cyan);
 Box(surface,"Cabecera institucional",new Vector2(0,397),new Vector2(1472,86),new Color(.025f,.10f,.14f));
 Box(surface,"Header rule",new Vector2(0,343),new Vector2(1472,2),new Color(.12f,.49f,.57f));
 title=Label(surface,"Encabezado",new Vector2(-312,397),new Vector2(800,70),36,Color.white);title.fontStyle=FontStyles.Bold;title.richText=true;
 metadata=Label(surface,"Estado de operacion",new Vector2(420,397),new Vector2(560,65),25,Cyan);
 var actions=new[]{BankAction.ViewDossier,BankAction.Registry,BankAction.Alerts,BankAction.Procedure};var labels=new[]{"EXPEDIENTE","REGISTRO","ALERTAS","PROCEDIMIENTO"};
 for(int i=0;i<4;i++){tabs[i]=Action(surface,labels[i],new Vector2(-561+374*i,300),new Vector2(350,72),actions[i],new Color(.035f,.17f,.22f),30);Glyph(tabs[i].transform.parent,i,new Vector2(-139,5),30,Color.white);tabs[i].margin=new Vector4(56,0,6,7);tabs[i].textWrappingMode=TextWrappingModes.NoWrap;tabs[i].enableAutoSizing=true;tabs[i].fontSizeMin=26;tabs[i].fontSizeMax=30;}
 content=new GameObject("Contenido",typeof(RectTransform)).transform;content.SetParent(surface,false);
 messageWell=Box(surface,"Canal de respuesta",new Vector2(0,-252),new Vector2(1472,40),new Color(.024f,.075f,.11f));
 message=Label(surface,"Respuesta del sistema",new Vector2(0,-252),new Vector2(1460,40),24,Color.white);message.overflowMode=TextOverflowModes.Ellipsis;
 footer=Label(surface,"Resumen de turno",new Vector2(0,0),new Vector2(1,1),20,Cyan);footer.gameObject.SetActive(false);
 int index=0;foreach(var action in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return}){
 var target=root.GetComponentsInChildren<BankInteractable>(true).First(x=>x.Action==action);
 var tint=index==0?new Color(.015f,.35f,.28f):index==1?new Color(.48f,.26f,.025f):new Color(.025f,.27f,.45f);
 var bg=Box(surface,"Decision "+index,new Vector2(-505+505*index,-334),new Vector2(462,92),tint);decisions[index]=bg;Frame(bg.transform,new Vector2(462,92),index==1?new Color(1,.68f,.23f):Cyan);
 target.transform.SetParent(bg.transform,false);var rt=target.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(462,92);rt.anchoredPosition=Vector2.zero;rt.localPosition=new Vector3(0,0,-2);
 target.GetComponent<BoxCollider>().size=new Vector3(462,92,12);var label=target.GetComponent<TextMeshProUGUI>();label.fontSize=40;label.fontStyle=FontStyles.Bold;label.margin=new Vector4(65,0,0,8);label.color=Color.white;
 Glyph(bg.transform,4+index,new Vector2(-170,6),48,Color.white);BindFocus(target,bg,new Vector2(462,92));target.GetComponent<DocumentFocus>().Accent=index==0?new Color(.18f,.88f,.70f):index==1?new Color(1,.67f,.22f):new Color(.20f,.72f,1);index++;
 }
 utilityActions[0]=Action(surface,"EVIDENCIA",new Vector2(-600,-414),new Vector2(320,60),BankAction.Evidence,new Color(.035f,.16f,.20f),24).transform.parent.gameObject;
 utilityActions[1]=Action(surface,"CONTROLES",new Vector2(-245,-414),new Vector2(320,60),BankAction.Defense,new Color(.035f,.16f,.20f),24).transform.parent.gameObject;
 utilityActions[2]=Action(surface,"CERRAR TURNO",new Vector2(135,-414),new Vector2(390,60),BankAction.End,new Color(.045f,.22f,.25f),28).transform.parent.gameObject;
 Action(surface,"SALIR DE ESTACI\u00d3N",new Vector2(545,-414),new Vector2(400,60),BankAction.Stand,new Color(.04f,.12f,.17f),23);
 }
 public void Show(Page page){CurrentPage=page;if(page==Page.Registry&&Swift.DeskTools.Sim003Pilot.Current?.ActiveForCurrent!=true)registryResult=owner.Manager.Message;Refresh();}
 public void ExpandObservations(){previousPage=CurrentPage;observationPage=1;Show(Page.Observations);}
 public void Back(){Show(previousPage==Page.Observations?Page.Dossier:previousPage);}
 public void StepObservations(int direction){observationPage=Mathf.Max(1,observationPage+direction);Refresh();}
 public void Invalidate(){snapshotValid=false;}
 public void Refresh(){
 var game=owner.Manager;var pilot=Swift.DeskTools.Sim003Pilot.Current;
 if(game.Session!=session||game.Session?.Transactions.Current!=record){session=game.Session;record=session?.Transactions.Current;CurrentPage=pilot!=null&&pilot.ReviewPending?Page.PilotReview:Page.Dossier;registryResult=null;}
 bool pilotCurrent=pilot!=null&&pilot.ActiveForCurrent,pilotReview=pilot!=null&&pilot.ReviewPending,pilotConfirm=pilotCurrent&&pilot.PendingDecision.HasValue;
 bool ended=session?.Ended==true,defenseActive=session?.Defense.Active==true;
 int evidenceCount=session?.Evidence.Entries.Count??0,correct=session?.Correct??0;
 long loss=session?.SimulatedLoss??0;
 if(snapshotValid&&ReferenceEquals(session,cachedSession)&&ReferenceEquals(record,cachedRecord)&&CurrentPage==cachedPage&&
    string.Equals(game.Message,cachedMessage,System.StringComparison.Ordinal)&&observationPage==cachedObservationPage&&
    ended==cachedEnded&&defenseActive==cachedDefenseActive&&evidenceCount==cachedEvidenceCount&&correct==cachedCorrect&&
    loss==cachedLoss&&game.LastAttackLoss==cachedAttackLoss&&game.LastDefenseLoss==cachedDefenseLoss)return;
 cachedSession=session;cachedRecord=record;cachedPage=CurrentPage;cachedMessage=game.Message;cachedObservationPage=observationPage;
 cachedEnded=ended;cachedDefenseActive=defenseActive;cachedEvidenceCount=evidenceCount;cachedCorrect=correct;
 cachedLoss=loss;cachedAttackLoss=game.LastAttackLoss;cachedDefenseLoss=game.LastDefenseLoss;snapshotValid=true;RebuildCount++;
 for(int i=content.childCount-1;i>=0;i--){var child=content.GetChild(i);child.gameObject.SetActive(false);child.SetParent(null);Destroy(child.gameObject);}
 bool finished=session?.Ended==true;title.text=finished?"<i>SWIFT</i> / RESULTADOS DEL TURNO":"<i>SWIFT</i> / CENTRO DE OPERACIONES";
 metadata.text=session==null?"ESTACI\u00d3N DE AN\u00c1LISIS / DATOS FICTICIOS":$"{(session.Mode==ScenarioMode.Attack?"ATAQUE":"DEFENSA")}  /  {(record==null?"TURNO FINALIZADO":record.Data.Id)}\n{(record==null?"RESULTADOS":Status(record.Status))}  \u00b7  {session.Transactions.Records.Count(x=>x.Status!=TransactionStatus.Pending)}/{session.Transactions.Records.Count} resueltos";
 tabs[0].text=finished?"RESULTADOS":"EXPEDIENTE";
 for(int i=0;i<4;i++){bool available=!pilotReview&&!pilotConfirm&&((i!=1&&i!=2)||(record!=null&&!finished));tabs[i].color=!available?new Color(.46f,.57f,.61f):(int)CurrentPage==i?Cyan:Color.white;tabs[i].transform.parent.GetComponent<BoxCollider>().enabled=available;tabs[i].transform.parent.GetComponent<DocumentFocus>().SetNormalColor((int)CurrentPage==i?new Color(.025f,.31f,.36f):new Color(.035f,.12f,.17f));}
 foreach(var bg in decisions)bg.gameObject.SetActive(record!=null&&!finished&&!pilotReview&&!pilotConfirm);
 foreach(var action in utilityActions)action.SetActive(session!=null&&!pilotReview&&!pilotConfirm);
 var evidenceLabel=utilityActions[0].GetComponentInChildren<TMP_Text>(true);
 if(evidenceLabel!=null){evidenceLabel.text=pilotCurrent?"IMPRIMIR CONSTANCIA":"EVIDENCIA";evidenceLabel.fontSize=pilotCurrent?21:24;}
 message.gameObject.SetActive(record!=null&&!finished&&!pilotReview&&!pilotConfirm);messageWell.gameObject.SetActive(record!=null&&!finished&&!pilotReview&&!pilotConfirm);
 message.text=pilotCurrent?(pilot.Hint??"Consulta las fuentes y registra tu observación antes de decidir."):FormatReferencePanel.ObservationText(game.Message);
 if(pilotReview){PilotReview(pilot);return;}if(pilotConfirm){PilotConfirmation(pilot);return;}
 if(pilotCurrent&&CurrentPage==Page.PilotPrintExpanded){PilotPrintExpanded(pilot);return;}
 if(CurrentPage==Page.Procedure){if(pilotCurrent)PilotProcedure();else Procedure();return;}if(CurrentPage==Page.History){History();return;}if(finished){Results();return;}if(record==null){Welcome();return;}
 if(CurrentPage==Page.Registry)Registry();else if(CurrentPage==Page.Alerts)Alerts();else if(CurrentPage==Page.Observations)Observations();else Dossier();
 }
 void Dossier(){
 // Presentation only: identical data, text bounds, font sizes and interactive targets.
 var d=record.Data;var sheet=new Color(.78f,.83f,.84f);var fieldPaper=new Color(.87f,.90f,.90f);var rule=new Color(.42f,.57f,.60f);var captionInk=new Color(.09f,.24f,.28f);
 Box(content,"Sombra documental",new Vector2(-181,4),new Vector2(1104,462),new Color(0,.015f,.025f,.40f));
 var document=Box(content,"Documento gris papel",new Vector2(-186,10),new Vector2(1100,460),sheet).transform;Frame(document,new Vector2(1100,460),new Color(.25f,.58f,.66f));
 var header=Box(document,"Banda documental petroleo",new Vector2(0,174),new Vector2(1064,100),new Color(.045f,.16f,.20f));
 var gradient=(PortalPanelGraphic)header;gradient.Bottom=new Color(.75f,.84f,.87f);gradient.SetVerticesDirty();
 Basic(document,"Acento institucional",new Vector2(-516,174),new Vector2(4,58),new Color(.29f,.70f,.72f));
 // Inset wells precede all labels in the canvas, so they cannot occlude data.
 void Well(string name,Vector2 at,Vector2 size){Box(document,name+" borde",at,size,rule);var well=Box(document,name,at,size-new Vector2(2,2),fieldPaper);((PortalPanelGraphic)well).CornerCut=12;}
 Well("Grupo emisor",new Vector2(0,72),new Vector2(1040,96));
 Well("Grupo beneficiario",new Vector2(-270,-32),new Vector2(500,104));
 Well("Grupo cuenta",new Vector2(270,-32),new Vector2(500,104));
 Well("Grupo intermediario",new Vector2(0,-138),new Vector2(1040,96));
 var heading=Label(document,"Clase documental",new Vector2(-155,184),new Vector2(650,60),31,new Color(.88f,.94f,.94f));heading.text="DATOS DE LA OPERACI\u00d3N";heading.fontStyle=FontStyles.Bold;
 var amount=Box(document,"Monto destacado",new Vector2(330,180),new Vector2(340,90),new Color(.075f,.25f,.29f)).transform;Frame(amount,new Vector2(340,90),new Color(.36f,.65f,.67f));
 Label(amount,"Monto etiqueta",new Vector2(0,22),new Vector2(304,28),21,new Color(.65f,.84f,.84f)).text="MONTO / USD";
 var number=Label(amount,"Monto valor",new Vector2(0,-16),new Vector2(304,44),35,new Color(.96f,.86f,.63f));number.text=$"{d.Amount:N0}";number.fontStyle=FontStyles.Bold;Fit(number,30,35);
 Glyph(document,8,new Vector2(-482,71),42,Ink);
 void Field(string name,string value,Vector2 labelAt,Vector2 valueAt,float width,float font=32){var caption=Label(document,name,labelAt,new Vector2(width,30),24,captionInk);caption.text=name;caption.fontStyle=FontStyles.Bold;var text=Label(document,"Valor "+name,valueAt,new Vector2(width,53),font,Ink);text.text=string.IsNullOrEmpty(value)?"NO INFORMADO":value;Fit(text,28,font);PreviewIfNeeded(text,28);}
 Field("BANCO EMISOR",d.OriginBank,new Vector2(30,104),new Vector2(30,56),900,33);
 Field("BENEFICIARIO",d.Beneficiary,new Vector2(-270,-7),new Vector2(-270,-53),460);
 Field("CUENTA",d.Account,new Vector2(270,-7),new Vector2(270,-53),460,31);
 Field("BANCO INTERMEDIARIO",d.IntermediaryBank,new Vector2(0,-107),new Vector2(0,-151),1000,31);
 if(!string.IsNullOrEmpty(d.Date)){var date=Label(document,"Valor FECHA",new Vector2(0,-207),new Vector2(1000,40),25,Muted);date.text="FECHA / "+d.Date;}
 var observations=Box(content,"Observaciones",new Vector2(576,10),new Vector2(320,460),new Color(.03f,.105f,.145f)).transform;Frame(observations,new Vector2(320,460),new Color(.12f,.49f,.57f));
 var title=Label(observations,"Titulo observaciones",new Vector2(0,185),new Vector2(272,50),27,Color.white);title.text="OBSERVACIONES";title.fontStyle=FontStyles.Bold;
 var paper=Box(observations,"Notas",new Vector2(0,-3),new Vector2(272,260),fieldPaper).transform;Frame(paper,new Vector2(272,260),rule);
 Basic(paper,"Identificador de observaciones",new Vector2(-130,0),new Vector2(4,212),new Color(.65f,.47f,.22f));
 bool pilot=Swift.DeskTools.Sim003Pilot.Current?.ActiveForCurrent==true;
 var notes=Label(paper,"Resumen observaciones",Vector2.zero,new Vector2(228,212),28,Ink);
 notes.text=pilot?"Consulta REGISTRO para contrastar la cuenta. Registra lo que observaste.":FormatReferencePanel.ObservedAlerts(d);
 Fit(notes,26,28);notes.overflowMode=TextOverflowModes.Ellipsis;
 Action(observations,pilot?"ELEGIR CUENTA":"VER DETALLE",new Vector2(0,-178),new Vector2(260,64),pilot?BankAction.PilotSelectAccount:BankAction.MoreObservations,new Color(.06f,.24f,.30f),pilot?23:26);
 }
 void Registry(){if(Swift.DeskTools.Sim003Pilot.Current?.ActiveForCurrent==true){PilotRegistry(Swift.DeskTools.Sim003Pilot.Current);return;}var d=record.Data;bool absent=(d.Anomalies&Anomaly.UnknownAccount)!=0;
 Field("CUENTA CONSULTADA",d.Account,0,150);Field("BENEFICIARIO DEL EXPEDIENTE",d.Beneficiary,1,150);Field("TITULAR DEL REGISTRO",absent?"No disponible":d.RegisteredName,0,40);Field("ESTADO DEL REGISTRO",absent?"Cuenta no encontrada":"Consta en registro ficticio",1,40);
 Card("RESULTADO DE LA CONSULTA",registryResult??"Usa REGISTRO para realizar la consulta existente.",new Vector2(0,-122),new Vector2(1480,166),30);}
 void Alerts(){
 if(Swift.DeskTools.Sim003Pilot.Current?.ActiveForCurrent==true){Card("ALERTAS / FUENTE DE APOYO","La revisión de la cuenta se realiza en REGISTRO. Una señal aislada no confirma fraude.",new Vector2(0,35),new Vector2(1480,220),32);return;}
 Label(content,"Cabecera campo",new Vector2(-600,212),new Vector2(250,32),24,Cyan).text="CAMPO";
 Label(content,"Cabecera observacion",new Vector2(0,212),new Vector2(900,32),24,Cyan).text="OBSERVACI\u00d3N EXISTENTE";
 Label(content,"Cabecera estado",new Vector2(620,212),new Vector2(200,32),24,Cyan).text="ESTADO";
 var flags=new[]{Anomaly.MissingIntermediary,Anomaly.NameMismatch,Anomaly.UnknownAccount,Anomaly.UnusualAmount,Anomaly.AlertMatch};var names=new[]{"Intermediario","Beneficiario","Cuenta","Monto","Alertas"};var lines=FormatReferencePanel.ObservedAlerts(record.Data).Split('\n');int row=0;int count=Mathf.Max(1,flags.Count(flag=>(record.Data.Anomalies&flag)!=0));float rowHeight=Mathf.Min(260,(360-10*(count-1))/count);
 for(int i=0;i<flags.Length;i++)if((record.Data.Anomalies&flags[i])!=0){AlertRow(row,names[i],lines[row],i==0?"Formato":"Revisar",rowHeight);row++;}if(row==0)AlertRow(0,"Expediente",lines[0],"Sin discrep.",rowHeight);
 Label(content,"Nota educativa",new Vector2(0,-211),new Vector2(1470,32),24,Cyan).text="Una coincidencia requiere contexto y revisi\u00f3n; no confirma fraude.";
 }
 void AlertRow(int row,string field,string observation,string status,float height){var panel=Box(content,"Alerta "+row,new Vector2(0,185-height/2-row*(height+10)),new Vector2(1480,height),Paper).transform;
 Label(panel,"Campo",new Vector2(-600,0),new Vector2(250,height-12),height>100?30:25,Ink).text=field;var t=Label(panel,"Observacion",new Vector2(0,0),new Vector2(900,height-12),height>100?32:27,Ink);t.text=observation;Fit(t,25,height>100?32:27);Label(panel,"Estado",new Vector2(620,0),new Vector2(200,height-12),height>100?30:25,new Color(.48f,.27f,.025f)).text=status;}
 void Procedure(){for(int i=0;i<4;i++)Card(owner.ReviewStepTitles[i],owner.ReviewStepTexts[i],new Vector2(i%2==0?-380:380,i<2?110:-100),new Vector2(720,185),31);}
 void PilotProcedure(){
     Card("01 / RECIBE","Lee emisor, beneficiario, cuenta, monto, intermediario y fecha.",new Vector2(-380,110),new Vector2(720,185),31);
     Card("02 / CONSULTA","Selecciona la cuenta y busca el titular en REGISTRO.",new Vector2(380,110),new Vector2(720,185),31);
     Card("03 / DOCUMENTA","Compara lo consultado y registra tu observación; puedes corregirla.",new Vector2(-380,-100),new Vector2(720,185),31);
     Card("04 / DECIDE","Elige una acción y confirma tras revisar tu hallazgo.",new Vector2(380,-100),new Vector2(720,185),31);
 }
 void Observations(){
 if(Swift.DeskTools.Sim003Pilot.Current?.ActiveForCurrent==true){Card("OBSERVACIONES","Sin diagnóstico automático en el piloto. Consulta la cuenta y registra tu propio hallazgo.",new Vector2(0,35),new Vector2(1480,220),31);Action(content,"VOLVER",new Vector2(0,-205),new Vector2(400,70),BankAction.BackToDocument,Navy,27);return;}
 var panel=Box(content,"Observaciones ampliadas",new Vector2(0,5),new Vector2(1480,450),Paper).transform;Frame(panel,new Vector2(1480,450),Cyan);
 Label(panel,"Titulo",new Vector2(0,180),new Vector2(1400,44),30,Ink).text="OBSERVACIONES / DATOS COMPLETOS";
 var d=record.Data;var text=Label(panel,"Detalle paginado",new Vector2(0,5),new Vector2(1390,280),30,Ink);text.text=$"Emisor: {d.OriginBank}\nBeneficiario: {d.Beneficiary}\nCuenta: {d.Account}  \u00b7  USD {d.Amount:N0}\nIntermediario: {d.IntermediaryBank}\n"+FormatReferencePanel.ObservedAlerts(d)+(FormatReferencePanel.ObservationText(owner.Manager.Message)==FormatReferencePanel.ObservedAlerts(d)?"":"\n"+FormatReferencePanel.ObservationText(owner.Manager.Message));text.overflowMode=TextOverflowModes.Page;text.ForceMeshUpdate();observationPage=Mathf.Clamp(observationPage,1,Mathf.Max(1,text.textInfo.pageCount));text.pageToDisplay=observationPage;
 Action(panel,"VOLVER",new Vector2(-475,-177),new Vector2(410,58),BankAction.BackToDocument,new Color(.035f,.19f,.24f),25);
 if(text.textInfo.pageCount>1){Action(panel,"ANTERIOR",new Vector2(0,-177),new Vector2(330,58),BankAction.ObservationPrevious,Navy,24);Action(panel,"SIGUIENTE",new Vector2(420,-177),new Vector2(330,58),BankAction.ObservationNext,Navy,24);}
 }
 public static string InputHelp(bool android)=>android?"Mira un control durante 1,5 segundos.\nAparta la mirada para volver a activarlo.":"PC: mant\u00e9n pulsado sobre un control durante 1,5 s.\nBot\u00f3n derecho para mirar / WASD. VR: selecci\u00f3n por mirada.";
 void Welcome(){
     var well=Box(content,"Terminal disponible",Vector2.zero,new Vector2(1472,472),new Color(.03f,.12f,.16f)).transform;
     Frame(well,new Vector2(1472,472),new Color(.18f,.53f,.58f));
     Basic(well,"Acento institucional",new Vector2(-695,0),new Vector2(6,390),new Color(.22f,.75f,.78f));
     var eyebrow=Label(well,"Estado",new Vector2(0,157),new Vector2(1260,48),28,new Color(.48f,.83f,.85f));
     eyebrow.text="ESTACIÓN DISPONIBLE";eyebrow.alignment=TextAlignmentOptions.Center;eyebrow.fontStyle=FontStyles.Bold;eyebrow.characterSpacing=2;
     var heading=Label(well,"Acción principal",new Vector2(0,73),new Vector2(1300,78),46,new Color(.91f,.94f,.92f));
     heading.text="ELIGE ATAQUE O DEFENSA";heading.alignment=TextAlignmentOptions.Center;heading.fontStyle=FontStyles.Bold;
     var instruction=Label(well,"Ubicación del portal",new Vector2(0,7),new Vector2(1300,56),31,new Color(.78f,.86f,.86f));
     instruction.text="Usa el portal de operaciones frente a esta estación.";instruction.alignment=TextAlignmentOptions.Center;
     Basic(well,"Separador",new Vector2(0,-36),new Vector2(1110,2),new Color(.17f,.43f,.48f));
     var help=Label(well,"Ayuda de mirada",new Vector2(0,-84),new Vector2(1300,60),29,new Color(.51f,.81f,.83f));
     help.text=Application.platform==RuntimePlatform.Android
         ?"Mira una tarjeta durante 1,5 s para seleccionarla."
         :"PC: mantén clic 1,5 s; botón derecho para mirar.";
     help.alignment=TextAlignmentOptions.Center;
     Action(well,"PRACTICAR MIRADA / TUTORIAL VR",new Vector2(0,-171),new Vector2(910,82),BankAction.TutorialStart,new Color(.025f,.32f,.30f),32);
     var status=Box(content,"Barra de estado en espera",new Vector2(0,-307),new Vector2(1472,70),new Color(.024f,.075f,.11f)).transform;
     Label(status,"Terminal lista",new Vector2(-500,0),new Vector2(410,50),27,new Color(.50f,.83f,.85f)).text="TERMINAL LISTA";
     var next=Label(status,"Paso siguiente",new Vector2(260,0),new Vector2(930,50),27,new Color(.75f,.84f,.84f));
     next.text="SELECCIONA UN MODO EN EL PORTAL";next.alignment=TextAlignmentOptions.Right;
 }
        void Results()
        {
            Card("DECISIONES COHERENTES",$"{session.Correct} / {session.Transactions.Records.Count}",new Vector2(-455,180),new Vector2(420,88),31);
            Card("PERDIDA FICTICIA",$"USD {session.SimulatedLoss:N0}",new Vector2(0,180),new Vector2(420,88),31);
            Card("EVIDENCIAS REGISTRADAS",session.Evidence.Entries.Count.ToString(),new Vector2(455,180),new Vector2(420,88),31);
            Card("COMPARACION / ATAQUE",owner.Manager.LastAttackLoss.HasValue?"USD "+owner.Manager.LastAttackLoss.Value.ToString("N0"):"Pendiente",new Vector2(-345,90),new Vector2(650,82),29);
            Card("COMPARACION / DEFENSA",owner.Manager.LastDefenseLoss.HasValue?"USD "+owner.Manager.LastDefenseLoss.Value.ToString("N0"):"Pendiente",new Vector2(345,90),new Vector2(650,82),29);
            var comparisonNote=Label(content,"Limite de la comparacion",new Vector2(0,244),new Vector2(1400,34),26,Cyan);
            comparisonNote.alignment=TextAlignmentOptions.Center;
            comparisonNote.text=session.Mode==ScenarioMode.Attack
                ?"ATAQUE / Sin revisión automática: la decisión define el estado final."
                :session.Defense.Active
                    ?"DEFENSA / Control activo: puede corregir aprobaciones con anomalías."
                    :"DEFENSA / Control sin activar: no hay corrección automática.";
            var records=session.Transactions.Records;
            var journal=Swift.DeskTools.DeskInteractionCoordinator.Current?.Journal;
            for(int i=0;i<records.Count;i++) {
                var r=records[i];
                // Read the already committed choice and effective state; never recompute a bank result.
                var notes=journal?.For(r.Data.Id);
                string choice=notes?.ChosenDecision==Decision.Approve?"APROBAR":notes?.ChosenDecision==Decision.Hold?"RETENER":notes?.ChosenDecision==Decision.Return?"DEVOLVER":"NO REGISTRADA";
                var panel=Box(content,"Decision "+r.Data.Id,new Vector2(i%2==0?-345:345,-18-(i/2)*76),new Vector2(650,68),new Color(.84f,.88f,.86f)).transform;
                Frame(panel,new Vector2(650,68),r.DefenseOverride?new Color(.79f,.48f,.14f):new Color(.25f,.58f,.66f));
                Label(panel,"Accion elegida",new Vector2(-112,14),new Vector2(402,29),24,Ink).text=r.Data.Id+"  /  ELEGISTE "+choice;
                var effective=Label(panel,"Estado final",new Vector2(217,14),new Vector2(172,29),25,r.Status==TransactionStatus.Approved?new Color(.03f,.35f,.26f):r.Status==TransactionStatus.Returned?new Color(.03f,.25f,.50f):new Color(.48f,.27f,.025f));
                effective.text=Status(r.Status).ToUpperInvariant();effective.fontStyle=FontStyles.Bold;
                Label(panel,"Fuentes y control",new Vector2(0,-18),new Vector2(620,26),21,Muted).text="Fuentes: "+(notes?.Sources.Count??0)+"  ·  Llamada: "+(notes?.PhoneEvidence==null?"no":"si")+"  ·  Control: "+(r.DefenseOverride?"intervino":"no intervino");
            }
            if(Swift.DeskTools.DeskInteractionCoordinator.Current!=null) {
                var review=Action(content,"REVISAR DECISIONES Y FUENTES",new Vector2(0,-276),new Vector2(1330,74),BankAction.DeskTool,new Color(.04f,.22f,.26f),30);
                var command=review.transform.parent.gameObject.AddComponent<Swift.DeskTools.DeskActionTarget>();command.Owner=Swift.DeskTools.DeskInteractionCoordinator.Current;command.Command=Swift.DeskTools.DeskCommand.Debrief;
            } else Card("EXPLICACION EDUCATIVA","Las seis ordenes jugables son ficticias.",new Vector2(0,-276),new Vector2(1330,74),25);
            Action(content,"CONTEXTO HISTORICO Y CREDITOS",new Vector2(0,-358),new Vector2(1330,52),BankAction.History,new Color(.045f,.19f,.24f),23);
        }
        void History()
        {
            var history=owner.Manager.Scenario!=null?owner.Manager.Scenario.HistoricalContext:TrainingData.HistoricalSummary;
            Card("CONTEXTO HISTORICO / LAS ORDENES JUGABLES SON FICTICIAS",history,new Vector2(0,19),new Vector2(1330,326),25);
            Card("CREDITOS", "Equipo 05 - A Toda Olla. IA: Codex. Modelos: Kenney, Quaternius y Poly (ver creditos incluidos). Cardboard: Apache-2.0. Liberation Sans: OFL-1.1.",new Vector2(0,-250),new Vector2(1330,180),25);
        }

 void PilotRegistry(Swift.DeskTools.Sim003Pilot pilot){
     var d=record.Data;
     Card("CUENTA EN LA ORDEN",d.Account,new Vector2(-380,165),new Vector2(720,100),32);
     Action(content,pilot.SelectedAccount?"CUENTA SELECCIONADA":"ELEGIR CUENTA",new Vector2(380,165),new Vector2(700,82),BankAction.PilotSelectAccount,new Color(.035f,.21f,.26f),28);
     Action(content,"BUSCAR / CONSULTAR",new Vector2(0,55),new Vector2(700,82),BankAction.PilotSearch,new Color(.025f,.31f,.30f),31);
     Card("TITULAR EN EL REGISTRO",pilot.SearchedAccount?d.RegisteredName:"SIN CONSULTAR",new Vector2(0,-48),new Vector2(1480,100),32);
     if(pilot.SearchedAccount){
         Label(content,"Registrar observación",new Vector2(0,-126),new Vector2(1450,38),26,Cyan).text="BENEFICIARIO / FUENTE: REGISTRO DE CUENTAS / ELIGE TU HALLAZGO";
         Action(content,"COINCIDE",new Vector2(-495,-192),new Vector2(450,78),BankAction.PilotMatches,new Color(.04f,.20f,.25f),29);
         Action(content,"NO COINCIDE",new Vector2(0,-192),new Vector2(450,78),BankAction.PilotDiffers,new Color(.04f,.20f,.25f),29);
         Action(content,"NO PUDE COMPROBAR",new Vector2(495,-192),new Vector2(450,78),BankAction.PilotUnverified,new Color(.04f,.20f,.25f),26);
     }
 }
 void PilotConfirmation(Swift.DeskTools.Sim003Pilot pilot){
     string choice=pilot.PendingDecision==Decision.Approve?"APROBAR":pilot.PendingDecision==Decision.Hold?"RETENER":"DEVOLVER";
     Card("CONFIRMAR DECISIÓN / SIM-003",choice,new Vector2(0,150),new Vector2(1480,115),37);
     Card("HALLAZGO DEL JUGADOR",pilot.FindingText,new Vector2(0,10),new Vector2(1480,125),32);
     Label(content,"Sin evaluación previa",new Vector2(0,-105),new Vector2(1400,55),29,Cyan).text="Puedes cancelar y continuar investigando. La evaluación aparece después de decidir.";
     Action(content,"CANCELAR",new Vector2(-375,-205),new Vector2(670,90),BankAction.PilotCancel,new Color(.055f,.17f,.22f),31);
     Action(content,"CONFIRMAR",new Vector2(375,-205),new Vector2(670,90),BankAction.PilotConfirm,new Color(.025f,.33f,.30f),31);
 }
 void PilotPrintExpanded(Swift.DeskTools.Sim003Pilot pilot){
     var printer=pilot.Printer;
     if(printer==null||printer.PrintedCopies==0){Card("CONSTANCIA","No hay documento impreso.",new Vector2(0,30),new Vector2(1480,220),31);return;}
     Card("CONSTANCIA DE CONSULTA DE REGISTRO",printer.PrintedCase,new Vector2(0,175),new Vector2(1480,100),32);
     Card("CUENTA CONSULTADA",printer.PrintedAccount,new Vector2(-380,35),new Vector2(720,120),32);
     Card("TITULAR EN REGISTRO",printer.PrintedHolder,new Vector2(380,35),new Vector2(720,120),32);
     Card("ALCANCE","Consulta ficticia. La constancia no confirma autorización ni recomienda una decisión.",new Vector2(0,-120),new Vector2(1480,110),27);
     Action(content,"VOLVER A REGISTRO",new Vector2(0,-235),new Vector2(700,70),BankAction.Registry,new Color(.04f,.22f,.27f),28);
 }
 void PilotReview(Swift.DeskTools.Sim003Pilot pilot){
     var row=pilot.ReviewedRecord;if(row==null)return;
     var notes=Swift.DeskTools.DeskInteractionCoordinator.Current?.Journal.For(row.Data.Id);
     var sources=notes==null||notes.Sources.Count==0?"Ninguna":string.Join(", ",notes.Sources.OrderBy(x=>x));
     string choice=pilot.ChosenDecision==Decision.Approve?"APROBAR":pilot.ChosenDecision==Decision.Hold?"RETENER":"DEVOLVER";
     long loss=row.Data.SimulatedFraud&&row.Status==TransactionStatus.Approved?row.Data.Amount:0;
     Label(content,"Revisión del piloto",new Vector2(0,245),new Vector2(1460,45),33,Cyan).text="REVISIÓN EDUCATIVA / "+row.Data.Id+"  ·  PUNTUACIÓN EXPERIMENTAL "+pilot.ExperimentalScore+"/15";
     Card("DECISIÓN ORIGINAL",choice,new Vector2(-380,165),new Vector2(720,84),29);
     Card("ESTADO EFECTIVO",Status(row.Status),new Vector2(380,165),new Vector2(720,84),29);
     Card("HALLAZGO REGISTRADO",pilot.FindingText,new Vector2(-380,75),new Vector2(720,84),29);
     Card("HALLAZGO SUSTENTADO",pilot.FindingSupported?"Sí / contraste de nombres":"No",new Vector2(380,75),new Vector2(720,84),29);
     Card("FUENTES CONSULTADAS",sources,new Vector2(-380,-15),new Vector2(720,84),27);
     Card("TELÉFONO",pilot.WasPhoneUsed?"Consulta registrada":"Sin consulta registrada",new Vector2(380,-15),new Vector2(720,84),27);
     Card("CONTROLES",row.DefenseOverride?"Intervino tras tu elección":"Sin intervención",new Vector2(-380,-105),new Vector2(720,84),27);
     Card("PÉRDIDA FICTICIA DEL CASO","USD "+loss.ToString("N0"),new Vector2(380,-105),new Vector2(720,84),27);
     string explanation=pilot.ChosenDecision==Decision.Hold?"La retención detuvo la orden para revisión; el turno no simula una resolución posterior.":
         pilot.ChosenDecision==Decision.Approve?(row.DefenseOverride?"Elegiste aprobar; CONTROLES retuvo la operación. La llamada no provocó esa intervención.":"La aprobación dejó avanzar una orden con nombres distintos en la simulación."):
         "La devolución no corresponde al criterio educativo actual de esta discrepancia.";
     Card("EXPLICACIÓN EDUCATIVA",explanation,new Vector2(0,-212),new Vector2(1480,102),25);
     Action(content,"CONTINUAR CON EL TURNO",new Vector2(0,-337),new Vector2(780,74),BankAction.PilotContinue,new Color(.035f,.25f,.29f),29);
 }
 void Field(string label,string value,int column,float y)=>Card(label,value,new Vector2(column==0?-380:380,y),new Vector2(720,95),30);
 void Card(string label,string value,Vector2 position,Vector2 size,float font){var card=Box(content,label,position,size,Paper).transform;Frame(card,size,new Color(.25f,.58f,.66f));var heading=Label(card,"Etiqueta",new Vector2(0,size.y/2-22),new Vector2(size.x-42,30),22,Muted);heading.text=label;heading.fontStyle=FontStyles.Bold;var text=Label(card,"Valor",new Vector2(0,-18),new Vector2(size.x-42,size.y-49),font,Ink);text.text=string.IsNullOrEmpty(value)?"No disponible":value;Fit(text,22,font);text.overflowMode=TextOverflowModes.Ellipsis;}
 TextMeshProUGUI Action(Transform parent,string label,Vector2 position,Vector2 size,BankAction action,Color tint,float font){var bg=Box(parent,label,position,size,tint);var text=Label(bg.transform,"Texto",Vector2.zero,size-new Vector2(16,12),font,Color.white);text.text=label;text.alignment=TextAlignmentOptions.Center;text.fontStyle=FontStyles.Bold;Frame(bg.transform,size,Cyan);var collider=bg.gameObject.AddComponent<BoxCollider>();collider.size=new Vector3(size.x,size.y,12);var target=bg.gameObject.AddComponent<BankInteractable>();target.Manager=owner.Manager;target.Action=action;BindFocus(target,bg,size);return text;}
 void BindFocus(BankInteractable target,Image background,Vector2 size){var focus=target.GetComponent<DocumentFocus>()??target.gameObject.AddComponent<DocumentFocus>();focus.Target=target;focus.Background=background;focus.Border=background.transform.Find("Rim")?.GetComponent<Image>();focus.Progress=Basic(background.transform,"Dwell real",new Vector2(0,-size.y/2+5),new Vector2(size.x-26,6),Cyan);focus.Progress.sprite=owner.StationFill;focus.Progress.type=Image.Type.Filled;focus.Progress.fillMethod=Image.FillMethod.Horizontal;focus.Progress.fillOrigin=0;focus.Progress.fillAmount=0;}
 void Frame(Transform parent,Vector2 size,Color tint){if(owner.StationFrame==null)return;var rim=Basic(parent,"Rim",Vector2.zero,size,tint);rim.sprite=owner.StationFrame;rim.type=Image.Type.Sliced;rim.fillCenter=false;rim.pixelsPerUnitMultiplier=2;}
 Image Box(Transform parent,string name,Vector2 position,Vector2 size,Color tint){var obj=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(PortalPanelGraphic));obj.transform.SetParent(parent,false);var rt=obj.GetComponent<RectTransform>();rt.sizeDelta=size;rt.anchoredPosition=position;var image=obj.GetComponent<PortalPanelGraphic>();image.Top=Color.white;image.Bottom=Color.white;image.color=tint;image.CornerCut=18;image.raycastTarget=false;return image;}
 // Basic Image required for sliced borders/fills; custom polygon mesh would ignore Image.fillAmount.
 Image Basic(Transform parent,string name,Vector2 point,Vector2 size,Color tint){var obj=new GameObject(name,typeof(RectTransform),typeof(Image));obj.transform.SetParent(parent,false);var r=obj.GetComponent<RectTransform>();r.sizeDelta=size;r.anchoredPosition=point;var image=obj.GetComponent<Image>();image.color=tint;image.raycastTarget=false;return image;}
 TextMeshProUGUI Label(Transform parent,string name,Vector2 point,Vector2 size,float font,Color tint){var t=owner.Text(parent,name,point,size,font);t.color=tint;t.alignment=TextAlignmentOptions.MidlineLeft;t.overflowMode=TextOverflowModes.Ellipsis;t.textWrappingMode=TextWrappingModes.Normal;t.richText=false;return t;}
 static void Fit(TMP_Text t,float min,float max){t.enableAutoSizing=true;t.fontSizeMin=min;t.fontSizeMax=max;}
 static void PreviewIfNeeded(TMP_Text text,float minimum){
     text.ForceMeshUpdate();if(!text.isTextOverflowing)return;
     string full=text.text;text.enableAutoSizing=false;text.fontSize=minimum;
     int low=0,high=full.Length;
     while(low<high){int mid=(low+high+1)/2;text.text=full.Substring(0,mid)+"…";text.ForceMeshUpdate();if(text.isTextOverflowing)high=mid-1;else low=mid;}
     text.text=full.Substring(0,low).TrimEnd()+"…";text.ForceMeshUpdate();
 }
 void Glyph(Transform parent,int symbol,Vector2 point,float size,Color tint){var obj=new GameObject("Icon "+symbol,typeof(RectTransform),typeof(CanvasRenderer),typeof(StationIconGraphic));obj.transform.SetParent(parent,false);var r=obj.GetComponent<RectTransform>();r.sizeDelta=Vector2.one*size;r.anchoredPosition=point;var graphic=obj.GetComponent<StationIconGraphic>();graphic.Symbol=symbol;graphic.color=tint;graphic.raycastTarget=false;}
 static string Status(TransactionStatus s)=>s==TransactionStatus.Approved?"Aprobada":s==TransactionStatus.Held?"Retenida":s==TransactionStatus.Returned?"Devuelta":"Pendiente";
 }
}

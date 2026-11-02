using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Swift.Bank;
using Swift.Interaction;

namespace Swift.UI
{
    // Presentation only. Values and decisions continue to belong to GameManager/GameSession.
    public sealed class BankDocumentUI : MonoBehaviour
    {
        public enum Page { Dossier, Registry, Alerts, Procedure, History }
        public Page CurrentPage { get; private set; }
        UIManager owner;
        Transform root, content;
        TextMeshProUGUI title, metadata, message, footer;
        readonly TextMeshProUGUI[] tabs = new TextMeshProUGUI[4];
        readonly Image[] decisions = new Image[3];
        GameSession session;
        TransactionRecord record;
        string registryResult;
        static readonly Color Navy = new Color(.025f,.065f,.10f);
        static readonly Color Cyan = new Color(.12f,.74f,.81f);
        static readonly Color Paper = new Color(.91f,.93f,.90f);
        static readonly Color Ink = new Color(.035f,.095f,.13f);
        static readonly Color Muted = new Color(.22f,.34f,.37f);
        public void Initialize(UIManager ui, Transform canvas)
        {
            owner = ui; root = canvas;
            foreach (var name in new[] {"Expediente", "Feedback", "Resumen", "OC Chrome"})
            { var old = root.Find(name); if (old != null) old.gameObject.SetActive(false); }
            var layer = new GameObject("Document UI - Propuesta C", typeof(RectTransform));
            layer.transform.SetParent(root, false);
            var surface = Box(layer.transform, "Marco", Vector2.zero, new Vector2(1100,800), Navy).transform;
            Box(surface,"Acento",new Vector2(0,397),new Vector2(1100,6),Cyan);
            title = Label(surface,"Encabezado",new Vector2(0,350),new Vector2(1030,54),32,Color.white);
            metadata = Label(surface,"Estado de operacion",new Vector2(0,292),new Vector2(1030,36),23,Cyan);
            var actions = new[]{BankAction.ViewDossier,BankAction.Registry,BankAction.Alerts,BankAction.Procedure};
            var labels = new[]{"EXPEDIENTE","REGISTRO","ALERTAS","PROCEDIMIENTO"};
            for(int i=0;i<4;i++) tabs[i]=Action(surface, labels[i],new Vector2(-390+260*i,236),new Vector2(246,62),actions[i],new Color(.045f,.19f,.24f),23);
            content = new GameObject("Contenido",typeof(RectTransform)).transform; content.SetParent(surface,false);
            message = Label(surface,"Respuesta del sistema",new Vector2(0,-321),new Vector2(1030,70),23,Color.white);
            footer = Label(surface,"Resumen de turno",new Vector2(0,-377),new Vector2(1030,32),21,Cyan);
            int index=0;
            foreach (var action in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return})
            {
                var button = root.GetComponentsInChildren<BankInteractable>(true).First(x=>x.Action==action);
                var rt=button.GetComponent<RectTransform>(); rt.anchoredPosition=new Vector2(-350+350*index,-244); rt.sizeDelta=new Vector2(310,80);
                button.GetComponent<BoxCollider>().size=new Vector3(310,80,12);
                var text=button.GetComponent<TextMeshProUGUI>(); text.fontSize=31; text.color=Color.white;
                var old=root.Find("OC Button "+index); if(old!=null) old.gameObject.SetActive(false);
                var color=index==0?new Color(.025f,.35f,.32f):index==1?new Color(.48f,.28f,.045f):new Color(.035f,.24f,.43f);
                decisions[index]=Box(surface,"Decision "+index,rt.anchoredPosition,new Vector2(322,88),color);
                // Existing decision colliders and labels remain independent and in front of the presentation.
                button.transform.SetAsLastSibling(); index++;
            }
            index=0;
            foreach(var action in new[]{BankAction.Approve,BankAction.Hold,BankAction.Return}) {
                var target=root.GetComponentsInChildren<BankInteractable>(true).First(x=>x.Action==action);
                var focus=target.gameObject.AddComponent<DocumentFocus>();focus.Target=target;focus.Background=decisions[index++];
            }
            var gaze=FindFirstObjectByType<VRInteractionManager>();
            if(gaze!=null && gaze.Reticle!=null && gaze.GetComponent<GazeProgressVisual>()==null) gaze.gameObject.AddComponent<GazeProgressVisual>();
        }
        public void Show(Page page)
        {
            CurrentPage=page;
            if(page==Page.Registry) registryResult=owner.Manager.Message;
            Refresh();
        }
        public void Refresh()
        {
            var game=owner.Manager;
            if(game.Session!=session || game.Session?.Transactions.Current!=record)
            { session=game.Session; record=session?.Transactions.Current; CurrentPage=Page.Dossier; registryResult=null; }
            for(int i=content.childCount-1;i>=0;i--) { var child=content.GetChild(i); child.gameObject.SetActive(false); child.SetParent(null); Destroy(child.gameObject); }
            bool finished=session?.Ended==true;
            title.text=finished?"SWIFT  /  RESULTADOS DEL TURNO":"SWIFT  /  CENTRO DE OPERACIONES";
            metadata.text=session==null?"BANCO CORRESPONSAL  /  SIMULACION EDUCATIVA":
                $"MODO {(session.Mode==ScenarioMode.Attack?"ATAQUE":"DEFENSA")}  /  {(record==null?"TURNO FINALIZADO":record.Data.Id+"  /  "+Status(record.Status).ToUpperInvariant())}";
            tabs[0].text=finished?"RESULTADOS":"EXPEDIENTE";
            for(int i=0;i<tabs.Length;i++) {
                bool selected=(int)CurrentPage==i;
                bool available=(i!=1&&i!=2)||(record!=null&&!finished);
                tabs[i].color=!available?new Color(.45f,.53f,.55f):selected?Cyan:Color.white;
                tabs[i].transform.parent.GetComponent<BoxCollider>().enabled=available;
            }
            foreach(var bg in decisions) bg.gameObject.SetActive(record!=null&&!finished);
            message.gameObject.SetActive(!finished);
            message.text=game.Message;
            footer.text=session==null?"MIRADA: 1,5 s  /  TODOS LOS DATOS SON FICTICIOS":
                $"Resueltos: {session.Transactions.Records.Count(x=>x.Status!=TransactionStatus.Pending)}/{session.Transactions.Records.Count}   |   Evidencias: {session.Evidence.Entries.Count}   |   Controles: {(session.Defense.Active?"ACTIVOS":"sin activar")}";
            if(CurrentPage==Page.Procedure) { Procedure(); return; }
            if(CurrentPage==Page.History) { History(); return; }
            if(finished) { Results(); return; }
            if(record==null) { Welcome(); return; }
            if(CurrentPage==Page.Registry) Registry();
            else if(CurrentPage==Page.Alerts) Alerts();
            else Dossier();
        }
        void Dossier()
        {
            var d=record.Data;
            Field("BANCO EMISOR",d.OriginBank,0,128);
            Field("MONTO",$"USD {d.Amount:N0}",1,128);
            Field("BENEFICIARIO",d.Beneficiary,0,48);
            Field("CUENTA",d.Account,1,48);
            Field("BANCO INTERMEDIARIO",string.IsNullOrEmpty(d.IntermediaryBank)?"AUSENTE":d.IntermediaryBank,0,-32);
            if(!string.IsNullOrEmpty(d.Date)) Field("FECHA",d.Date,1,-32);
            Card("OBSERVACIONES DISPONIBLES",TrainingData.Alerts(d),new Vector2(0,-128),new Vector2(1030,108),24);
        }
        public static string InputHelp(bool android) => android
            ? "Mira un control durante 1,5 segundos.\nTras seleccionar, aparta la mirada para volver a activarlo."
            : "PC: clic / boton derecho para mirar / WASD.\nVR: mira durante 1,5 s; aparta la mirada para rearmar.";
        void Welcome()
        {
            Card("BIENVENIDO / TUTORIAL OPCIONAL SIN PUNTUACION",
                "Elige ATAQUE o DEFENSA en los portales.\n"+InputHelp(Application.platform==RuntimePlatform.Android),
                new Vector2(0,55),new Vector2(1030,240),26);
            Action(content,"PRACTICAR MIRADA / TUTORIAL VR",new Vector2(0,-124),new Vector2(1030,78),BankAction.TutorialStart,new Color(.025f,.35f,.32f),27);
        }
        void Registry()
        {
            var d=record.Data;
            bool absent=(d.Anomalies&Anomaly.UnknownAccount)!=0;
            Field("CUENTA CONSULTADA",d.Account,0,128);
            Field("BENEFICIARIO DEL EXPEDIENTE",d.Beneficiary,1,128);
            Field("TITULAR EN EL REGISTRO",absent?"No disponible":d.RegisteredName,0,38);
            Field("ESTADO DEL REGISTRO",absent?"Cuenta no encontrada":"Consta en registro ficticio",1,38);
            Card("RESULTADO DE LA CONSULTA",registryResult??"Usa REGISTRO para realizar la consulta existente.",new Vector2(0,-92),new Vector2(1030,172),26);
        }
        void Alerts()
        {
            var columns=new[]{-405f,0f,435f};
            Label(content,"Campo",new Vector2(columns[0],165),new Vector2(190,30),22,Cyan).text="CAMPO";
            Label(content,"Observacion",new Vector2(columns[1],165),new Vector2(590,30),22,Cyan).text="OBSERVACION";
            Label(content,"Estado",new Vector2(columns[2],165),new Vector2(140,30),22,Cyan).text="ESTADO";
            var flags=new[]{Anomaly.MissingIntermediary,Anomaly.NameMismatch,Anomaly.UnknownAccount,Anomaly.UnusualAmount,Anomaly.AlertMatch};
            var names=new[]{"Intermediario","Beneficiario","Cuenta","Monto","Alertas"};
            var lines=TrainingData.Alerts(record.Data).Split('\n');
            bool spacious=lines.Length<=3;
            int row=0;
            for(int i=0;i<flags.Length;i++) if((record.Data.Anomalies&flags[i])!=0) {
                AlertRow(row,names[i],lines[row],i==0?"Formato":"Revisar",spacious);row++;
            }
            if(row==0) AlertRow(0,"Expediente",lines[0],"Sin discrep.",true);
            Label(content,"Nota educativa",new Vector2(0,-171),new Vector2(1030,30),22,Cyan).text="Coincidencia parcial: requiere revision; no confirma fraude.";
        }
        void AlertRow(int row,string field,string observation,string status,bool spacious)
        {
            float height=spacious?88:56, stride=height+(spacious?8:4);
            var panel=Box(content,"Observacion "+row,new Vector2(0,145-height/2-row*stride),new Vector2(1030,height),Paper).transform;
            Label(panel,"Campo",new Vector2(-405,0),new Vector2(190,height-8),spacious?26:23,Ink).text=field;
            Label(panel,"Observacion",Vector2.zero,new Vector2(590,height-8),spacious?27:22,Ink).text=observation;
            Label(panel,"Estado",new Vector2(435,0),new Vector2(140,height-8),spacious?25:22,Ink).text=status;
        }
        void Procedure()
        {
            for(int i=0;i<4;i++) Card(owner.ReviewStepTitles[i],owner.ReviewStepTexts[i],new Vector2(0,139-i*90),new Vector2(1030,82),24);
        }
        void Results()
        {
            Card("DECISIONES COHERENTES",$"{session.Correct} / {session.Transactions.Records.Count}",new Vector2(-350,135),new Vector2(330,88),31);
            Card("PERDIDA FICTICIA",$"USD {session.SimulatedLoss:N0}",new Vector2(0,135),new Vector2(330,88),31);
            Card("EVIDENCIAS",session.Evidence.Entries.Count.ToString(),new Vector2(350,135),new Vector2(330,88),31);
            Card("COMPARACION / ATAQUE",owner.Manager.LastAttackLoss.HasValue?"USD "+owner.Manager.LastAttackLoss.Value.ToString("N0"):"Pendiente",new Vector2(-265,37),new Vector2(500,88),29);
            Card("COMPARACION / DEFENSA",owner.Manager.LastDefenseLoss.HasValue?"USD "+owner.Manager.LastDefenseLoss.Value.ToString("N0"):"Pendiente",new Vector2(265,37),new Vector2(500,88),29);
            var records=session.Transactions.Records;
            for(int i=0;i<records.Count;i++) {
                var r=records[i];
                // The shipped scenario contains six records; no scoring is recomputed here.
                var panel=Box(content,"Decision "+r.Data.Id,new Vector2(i%2==0?-265:265,-65-(i/2)*55),new Vector2(500,48),Paper).transform;
                Label(panel,"Decision",Vector2.zero,new Vector2(472,44),23,Ink).text=r.Data.Id+"  /  "+Status(r.Status)+(r.DefenseOverride?" (control)":"");
            }
            Card("EXPLICACION EDUCATIVA","Defensa = contrafactual; no cambia los hechos historicos.\nLas seis ordenes jugables son ficticias.",new Vector2(0,-260),new Vector2(1030,102),25);
            Action(content,"CONTEXTO HISTORICO Y CREDITOS",new Vector2(0,-338),new Vector2(1030,44),BankAction.History,new Color(.045f,.19f,.24f),23);
        }
        void History()
        {
            var history=owner.Manager.Scenario!=null?owner.Manager.Scenario.HistoricalContext:TrainingData.HistoricalSummary;
            Card("CONTEXTO HISTORICO / LAS ORDENES JUGABLES SON FICTICIAS",history,new Vector2(0,19),new Vector2(1030,326),25);
            Card("CREDITOS", "Equipo 05 - A Toda Olla. IA: Codex. Modelos: Kenney, Quaternius y Poly (ver creditos incluidos). Cardboard: Apache-2.0. Liberation Sans: OFL-1.1.",new Vector2(0,-250),new Vector2(1030,180),25);
        }
        void Notice(string label,string value) => Card(label,value,new Vector2(0,0),new Vector2(1030,364),27);
        void Field(string label,string value,int column,float y) => Card(label,value,new Vector2(column==0?-265:265,y),new Vector2(500,72),label=="MONTO"?30:25);
        void Card(string label,string value,Vector2 position,Vector2 size,float font)
        {
            var card=Box(content,label,position,size,Paper).transform;
            Box(card,"Acento",new Vector2(-size.x/2+2,0),new Vector2(4,size.y),Cyan);
            var heading=Label(card,"Etiqueta",new Vector2(0,size.y/2-16),new Vector2(size.x-32,24),20,Muted); heading.text=label;
            heading.fontStyle=FontStyles.Bold;
            var text=Label(card,"Valor",new Vector2(0,-12),new Vector2(size.x-32,size.y-36),font,Ink); text.text=string.IsNullOrEmpty(value)?"No disponible":value;
        }
        TextMeshProUGUI Action(Transform parent,string label,Vector2 position,Vector2 size,BankAction action,Color color,float font)
        {
            var bg=Box(parent,label,position,size,color);
            var text=Label(bg.transform,"Texto",Vector2.zero,size-new Vector2(12,8),font,Color.white);text.text=label;text.alignment=TextAlignmentOptions.Center;
            var box=bg.gameObject.AddComponent<BoxCollider>();box.size=new Vector3(size.x,size.y,12);
            var target=bg.gameObject.AddComponent<BankInteractable>(); target.Manager=owner.Manager;target.Action=action;
            var focus=bg.gameObject.AddComponent<DocumentFocus>();focus.Target=target;focus.Background=bg;
            return text;
        }
        Image Box(Transform parent,string name,Vector2 position,Vector2 size,Color color) => OperationsCenterStyle.Panel(parent,name,position,size,color);
        TextMeshProUGUI Label(Transform parent,string name,Vector2 position,Vector2 size,float font,Color color)
        {
            var text=owner.Text(parent,name,position,size,font);text.color=color;text.alignment=TextAlignmentOptions.MidlineLeft;
            text.overflowMode=TextOverflowModes.Overflow;text.textWrappingMode=TextWrappingModes.Normal;text.richText=false;
            return text;
        }
        static string Status(TransactionStatus status) => status==TransactionStatus.Approved?"Aprobada":status==TransactionStatus.Held?"Retenida":status==TransactionStatus.Returned?"Devuelta":"Pendiente";
    }
}

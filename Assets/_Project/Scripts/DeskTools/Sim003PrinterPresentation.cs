using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Swift.Bank;
using Swift.Interaction;
using Swift.UI;

namespace Swift.DeskTools
{
    // Optional local printout; data is supplied only after an explicit registry search.
    public sealed class Sim003PrinterPresentation : MonoBehaviour
    {
        public enum PrintState { Ready, Printing, Failed, Tray, Lifting, Inspecting, Returning }
        public PrintState State { get; private set; } = PrintState.Ready;
        public int PrintedCopies { get; private set; }
        public BankInteractable SheetTarget { get; private set; }
        public BankInteractable ReturnTarget { get; private set; }
        public BankInteractable ExpandTarget { get; private set; }
        public TMP_Text SheetText { get; private set; }
        public string PrintedAccount { get; private set; }
        public string PrintedHolder { get; private set; }
        public string PrintedCase { get; private set; }
        UIManager owner;Sim003Pilot pilot;Transform printer,sheet;
        Material paperMaterial;
        Collider sheetCollider;
        Coroutine motion;
        Vector3 trayPosition;Quaternion trayRotation;

        public void Initialize(UIManager ui,Sim003Pilot controller,Transform physicalPrinter)
        {
            owner=ui;pilot=controller;printer=physicalPrinter;
        }
        public bool Request(TransactionData data,bool fail)
        {
            if(data==null||data.Id!="SIM-003"||State!=PrintState.Ready&&State!=PrintState.Failed)return false;
            if(fail){State=PrintState.Failed;Status("FALLO DE IMPRESIÓN / consulta digital disponible");return true;}
            if(PrintedCopies>0)return false;
            PrintedCase=data.Id;PrintedAccount=data.Account;PrintedHolder=data.RegisteredName;
            CreateSheet(data);PrintedCopies=1;State=PrintState.Printing;
            Status("IMPRIMIENDO CONSTANCIA / " + data.Id);
            motion=StartCoroutine(PrintOut());return true;
        }
        void Status(string value)
        {
            DeskInteractionCoordinator.Current?.ShowPrinterStatus(value);
        }
        void CreateSheet(TransactionData data)
        {
            var root=new GameObject("Constancia de registro / "+data.Id);
            root.transform.SetParent(printer,false);sheet=root.transform;
            trayPosition=new Vector3(0,.70f,-.42f);trayRotation=Quaternion.Euler(90,0,0);
            sheet.localPosition=new Vector3(0,.70f,-.10f);sheet.localRotation=trayRotation;
            var basePaper=GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePaper.name="Hoja física marfil";basePaper.transform.SetParent(sheet,false);
            basePaper.transform.localScale=new Vector3(.32f,.40f,.004f);
            paperMaterial=new Material(Shader.Find("Universal Render Pipeline/Lit"));
            paperMaterial.SetColor("_BaseColor",new Color(.90f,.91f,.85f));
            paperMaterial.SetFloat("_Smoothness",.04f);
            basePaper.GetComponent<Renderer>().sharedMaterial=paperMaterial;
            Destroy(basePaper.GetComponent<Collider>());
            var box=root.AddComponent<BoxCollider>();box.size=new Vector3(.32f,.40f,.016f);sheetCollider=box;
            SheetTarget=root.AddComponent<BankInteractable>();SheetTarget.Action=BankAction.PilotInspectPrint;SheetTarget.Manager=owner.Manager;
            var canvasObject=new GameObject("Texto dinámico de la constancia",typeof(RectTransform),typeof(Canvas));
            canvasObject.transform.SetParent(sheet,false);
            canvasObject.transform.localPosition=new Vector3(0,0,-.0045f);
            canvasObject.transform.localScale=Vector3.one*.001f;
            var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.sortingOrder=32;
            canvasObject.GetComponent<RectTransform>().sizeDelta=new Vector2(320,400);
            var text=owner.Text(canvasObject.transform,"Constancia TMP",new Vector2(0,30),new Vector2(282,280),19);
            text.color=new Color(.04f,.13f,.16f);text.alignment=TextAlignmentOptions.TopLeft;
            text.overflowMode=TextOverflowModes.Page;text.fontStyle=FontStyles.Normal;
            text.text="SWIFT / CONSTANCIA DE REGISTRO\n\nEXPEDIENTE  "+data.Id+
                "\nCUENTA CONSULTADA  "+data.Account+
                "\nTITULAR EN REGISTRO\n"+data.RegisteredName+
                "\n\nFuente: Registro de cuentas\nConsulta realizada en este turno.\nNo confirma autorización.";
            SheetText=text;
            var previous=DeskInspectionView.Button(canvasObject.transform,"DEJAR",new Vector2(-78,-171),new Vector2(135,43),DeskCommand.Close,owner.Font,owner.StationFrame,owner.StationFill);
            ReturnTarget=previous.GetComponent<BankInteractable>();ReturnTarget.Action=BankAction.PilotReturnPrint;ReturnTarget.Manager=owner.Manager;
            var expand=DeskInspectionView.Button(canvasObject.transform,"AMPLIAR",new Vector2(78,-171),new Vector2(135,43),DeskCommand.Close,owner.Font,owner.StationFrame,owner.StationFill);
            ExpandTarget=expand.GetComponent<BankInteractable>();ExpandTarget.Action=BankAction.PilotPrintExpand;ExpandTarget.Manager=owner.Manager;
            foreach(var button in new[]{previous,expand}){
                var label=button.GetComponentInChildren<TMP_Text>();label.fontSize=18;
                button.gameObject.SetActive(false);
            }
        }
        IEnumerator PrintOut()
        {
            var start=sheet.localPosition;var elapsed=0f;
            while(elapsed<.8f){elapsed+=Time.deltaTime;var t=Mathf.Clamp01(elapsed/.8f);sheet.localPosition=Vector3.Lerp(start,trayPosition,t*t*(3-2*t));yield return null;}
            sheet.localPosition=trayPosition;State=PrintState.Tray;
            Status("CONSTANCIA DISPONIBLE / "+PrintedCase);
            motion=null;
        }
        public void Inspect(Transform eye)
        {
            if(State!=PrintState.Tray||sheet==null)return;
            var from=sheet.position;var rotation=sheet.rotation;
            var forward=Vector3.ProjectOnPlane(eye.forward,Vector3.up).normalized;
            if(forward.sqrMagnitude<.01f)forward=eye.forward;
            var right=Vector3.Cross(Vector3.up,forward).normalized;
            var target=eye.position+forward*.62f-right*.26f-Vector3.up*.06f;
            var facing=Quaternion.LookRotation(target-eye.position,Vector3.up);
            State=PrintState.Lifting;sheetCollider.enabled=false;
            sheet.SetParent(null,true);
            motion=StartCoroutine(Move(from,rotation,target,facing,true));
        }
        public void Return()
        {
            if(sheet==null||State!=PrintState.Inspecting)return;
            State=PrintState.Returning;
            ReturnTarget.gameObject.SetActive(false);ExpandTarget.gameObject.SetActive(false);
            motion=StartCoroutine(Move(sheet.position,sheet.rotation,printer.TransformPoint(trayPosition),printer.rotation*trayRotation,false));
        }
        IEnumerator Move(Vector3 from,Quaternion fromRotation,Vector3 to,Quaternion toRotation,bool opening)
        {
            var up=from+Vector3.up*.15f;
            yield return Segment(from,fromRotation,up,fromRotation,.2f);
            yield return Segment(up,fromRotation,to,toRotation,.42f);
            sheet.SetPositionAndRotation(to,toRotation);
            if(opening){State=PrintState.Inspecting;ReturnTarget.gameObject.SetActive(true);ExpandTarget.gameObject.SetActive(true);}
            else{sheet.SetParent(printer,true);sheetCollider.enabled=true;State=PrintState.Tray;}
            motion=null;
        }
        IEnumerator Segment(Vector3 from,Quaternion a,Vector3 to,Quaternion b,float duration)
        {
            var elapsed=0f;while(elapsed<duration){elapsed+=Time.deltaTime;var t=Mathf.Clamp01(elapsed/duration);t=t*t*(3-2*t);
                sheet.SetPositionAndRotation(Vector3.Lerp(from,to,t),Quaternion.Slerp(a,b,t));yield return null;}
        }
        public void Clear()
        {
            if(motion!=null){StopCoroutine(motion);motion=null;}
            if(sheet!=null)Destroy(sheet.gameObject);
            sheet=null;SheetTarget=null;ReturnTarget=null;ExpandTarget=null;SheetText=null;
            State=PrintState.Ready;PrintedCopies=0;PrintedAccount=null;PrintedHolder=null;PrintedCase=null;
            Status("EVIDENCIA / simulación actual");
        }
        void OnDestroy(){if(paperMaterial!=null)Destroy(paperMaterial);if(sheet!=null)Destroy(sheet.gameObject);}
    }
}

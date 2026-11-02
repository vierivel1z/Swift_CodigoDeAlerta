using UnityEngine;
using TMPro;
using Swift.Interaction;
namespace Swift.UI
{
    // Optional sandbox of UI controls. Never creates a GameSession or calls Decide/Notify.
    public sealed class VRQuickTutorial : MonoBehaviour
    {
        public static VRQuickTutorial Active { get; private set; }
        public UIManager Owner;
        public bool IsRunning => Active==this;
        public int StageIndex { get; private set; }
        public bool DidComplete { get; private set; }
        public bool WasSkipped { get; private set; }
        public string PracticeChoice { get; private set; }
        public BankInteractable PrimaryTarget { get; private set; }
        public BankInteractable SkipTarget { get; private set; }
        public BankInteractable[] DecisionTargets { get; private set; }
        GameObject stage,bankCanvas;
        bool bankWasActive;
        TextMeshProUGUI title,description,primaryLabel;
        BankInteractable left,right;
        public void Begin()
        {
            if(IsRunning||Active!=null||Owner==null||Owner.Manager==null||Owner.Manager.Session!=null||Owner.Manager.IntroPlaying)return;
            var camera=Camera.main;if(camera==null)return;
            float distance=2;
            if(Physics.Raycast(camera.transform.position,camera.transform.forward,out var hit,2,~0,QueryTriggerInteraction.Ignore)) {
                if(hit.distance<.6f)return;
                distance=hit.distance-.12f;
            }
            Active=this;StageIndex=0;DidComplete=false;WasSkipped=false;PracticeChoice=null;
            bankCanvas=Owner.PanelAnchor.GetComponentInChildren<Canvas>().gameObject;bankWasActive=bankCanvas.activeSelf;bankCanvas.SetActive(false);
            stage=new GameObject("Tutorial VR - practica sin puntuacion");
            stage.transform.SetPositionAndRotation(camera.transform.position+camera.transform.forward*distance,Quaternion.LookRotation(camera.transform.forward,Vector3.up));
            var canvas=new GameObject("Controles de practica",typeof(RectTransform),typeof(Canvas));canvas.transform.SetParent(stage.transform,false);
            canvas.transform.localScale=Vector3.one*(.0009f*distance);canvas.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            canvas.GetComponent<RectTransform>().sizeDelta=new Vector2(1000,760);
            var root=OperationsCenterStyle.Panel(canvas.transform,"Fondo",Vector2.zero,new Vector2(1000,760),OperationsCenterStyle.Navy).transform;
            title=Text(root,"Paso",new Vector2(0,300),new Vector2(920,100),32);
            description=Text(root,"Instruccion",new Vector2(0,140),new Vector2(900,170),29);
            primaryLabel=Button(root,"OBJETIVO DE PRACTICA",new Vector2(0,-55),new Vector2(850,110),BankAction.TutorialNext,out var primary);PrimaryTarget=primary;
            Button(root,"APROBAR (PRACTICA)",new Vector2(-300,-55),new Vector2(280,110),BankAction.TutorialDecision,out left);left.TutorialChoice=0;
            Button(root,"DEVOLVER (PRACTICA)",new Vector2(300,-55),new Vector2(280,110),BankAction.TutorialDecision,out right);right.TutorialChoice=2;
            Button(root,"OMITIR TUTORIAL / VOLVER AL BANCO",new Vector2(0,-310),new Vector2(900,76),BankAction.TutorialSkip,out var skip);SkipTarget=skip;
            Text(root,"Aviso",new Vector2(0,-199),new Vector2(900,84),24).text="Practica de controles: no inicia modos, no evalua transferencias\ny no modifica puntuaciones ni resultados.";
            DecisionTargets=new[]{left,PrimaryTarget,right};RenderStep();
        }
        public void Select(int expectedStep,int choice)
        {
            if(!IsRunning||expectedStep!=StageIndex)return;
            if(StageIndex==4) {PracticeChoice=choice==0?"Aprobar":choice==2?"Devolver":"Retener";DidComplete=true;}
            if(StageIndex>=5) {Close();return;}
            StageIndex++;RenderStep();
        }
        void RenderStep()
        {
            var titles=new[]{"1 / 5  -  MIRA UN OBJETO","2 / 5  -  COMPLETA EL INDICADOR","3 / 5  -  SELECCIONA EL MONITOR","4 / 5  -  ABRE UN EXPEDIENTE","5 / 5  -  PRACTICA UNA DECISION","PRACTICA COMPLETADA"};
            var instructions=new[]{
                "Centra el punto sobre el objetivo.\nEl contorno indica que puedes seleccionarlo.",
                "Aparta la mirada y vuelve al objetivo.\nEspera 1,5 s hasta llenar el anillo. Se activa una sola vez.",
                "Aparta la mirada y mira el monitor de practica.\nCompleta el anillo para seleccionarlo.",
                "Selecciona ABRIR EXPEDIENTE.\nEs una demostracion visual; no contiene una transferencia.",
                "Elige cualquiera de los tres controles de practica.\nNo hay una decision correcta ni puntuacion en este tutorial.",
                "Ya conoces mirar, esperar, abrir y seleccionar.\nVuelve al banco y elige ATAQUE o DEFENSA en un portal."};
            title.text=titles[StageIndex];description.text=instructions[StageIndex];
            primaryLabel.text=StageIndex==0?"OBJETIVO DE PRACTICA":StageIndex==1?"MANTEN LA MIRADA 1,5 s":StageIndex==2?"MONITOR DE PRACTICA":StageIndex==3?"ABRIR EXPEDIENTE DE PRACTICA":StageIndex==4?"RETENER (PRACTICA)":"VOLVER AL BANCO";
            bool decision=StageIndex==4;left.gameObject.SetActive(decision);right.gameObject.SetActive(decision);
            PrimaryTarget.TutorialToken=StageIndex;PrimaryTarget.TutorialChoice=1;
            PrimaryTarget.Action=StageIndex>=5?BankAction.TutorialSkip:decision?BankAction.TutorialDecision:BankAction.TutorialNext;
            var rect=PrimaryTarget.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(decision?280:850,110);
            PrimaryTarget.GetComponent<BoxCollider>().size=new Vector3(rect.sizeDelta.x,110,10);
            primaryLabel.rectTransform.sizeDelta=rect.sizeDelta-new Vector2(16,10);
            foreach(var target in new[]{left,right})target.TutorialToken=StageIndex;
            // Reuse the same primary collider across steps: a held gaze cannot advance twice.
        }
        TextMeshProUGUI Button(Transform root,string label,Vector2 position,Vector2 size,BankAction action,out BankInteractable target)
        {
            var bg=OperationsCenterStyle.Panel(root,label,position,size,new Color(.025f,.32f,.34f));
            target=bg.gameObject.AddComponent<BankInteractable>();target.Action=action;target.Tutorial=this;
            bg.gameObject.AddComponent<BoxCollider>().size=new Vector3(size.x,size.y,10);
            var focus=bg.gameObject.AddComponent<DocumentFocus>();focus.Target=target;focus.Background=bg;
            var text=Text(bg.transform,"Texto",Vector2.zero,size-new Vector2(16,10),27);text.text=label;return text;
        }
        TextMeshProUGUI Text(Transform parent,string name,Vector2 position,Vector2 size,float font)
        {var text=Owner.Text(parent,name,position,size,font);text.alignment=TextAlignmentOptions.Center;text.richText=false;return text;}
        public void Close()
        {
            if(!IsRunning)return;WasSkipped=!DidComplete;Active=null;
            if(stage!=null){stage.SetActive(false);Destroy(stage);}if(bankCanvas!=null)bankCanvas.SetActive(bankWasActive);
        }
        void Update() {if(IsRunning&&(Owner.Manager.Session!=null||Owner.Manager.IntroPlaying))Close();}
        void OnDisable() {Close();}
    }
}

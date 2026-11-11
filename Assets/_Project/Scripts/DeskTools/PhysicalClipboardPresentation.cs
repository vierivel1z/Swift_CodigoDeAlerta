using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Swift.Interaction;

namespace Swift.DeskTools
{
    // Moves the existing wood/clip/paper object. Gaze timing remains in VRInteractionManager.
    public sealed class PhysicalClipboardPresentation : MonoBehaviour
    {
        public enum PoseState { Resting, Lifting, Inspecting, Returning }
        public PoseState State { get; private set; } = PoseState.Resting;
        public Vector3 RestPosition { get; private set; }
        public Quaternion RestRotation { get; private set; }
        public DeskInspectionView PaperView { get; private set; }
        Transform board;
        Collider boardCollider;
        Transform printedSteps;
        TextMeshPro takeHint;
        VRInteractionManager gaze;
        BankInteractable target;
        Coroutine motion;
        Coroutine pageMotion;
        Image pageWipe;
        readonly Dictionary<Renderer,Material> originalMaterials=new Dictionary<Renderer,Material>();
        readonly List<Material> visualMaterials=new List<Material>();
        readonly List<GameObject> visualLayers=new List<GameObject>();
        bool polished;
        public bool PageTurnActive => pageMotion!=null;

        public void Initialize(Transform physicalBoard, DeskInspectionView paperView,TMP_FontAsset font,bool polishVisual=false)
        {
            board=physicalBoard;PaperView=paperView;
            polished=polishVisual;
            RestPosition=board.position;RestRotation=board.rotation;
            boardCollider=board.GetComponent<Collider>();
            printedSteps=board.Find("Editable review steps/Steps");
            target=board.GetComponent<BankInteractable>();
            gaze=FindFirstObjectByType<VRInteractionManager>();
            var hint=new GameObject("TOMAR PORTAPAPELES / foco de mirada",typeof(TextMeshPro));
            hint.transform.SetParent(board,false);
            hint.transform.localPosition=new Vector3(-.145f,-.162f,-.028f);
            hint.transform.localScale=Vector3.one*.001f;
            takeHint=hint.GetComponent<TextMeshPro>();takeHint.font=font;takeHint.fontSize=22;
            takeHint.text="TOMAR PORTAPAPELES";takeHint.color=new Color(.02f,.25f,.29f);
            takeHint.rectTransform.sizeDelta=new Vector2(290,35);
            takeHint.gameObject.SetActive(false);
            PaperView.transform.SetParent(board,false);
            PaperView.transform.localPosition=new Vector3(0,0,-.026f);
            PaperView.transform.localRotation=Quaternion.identity;
            PaperView.transform.localScale=Vector3.one*.001f;
            PaperView.gameObject.SetActive(false);
            if(polished)CreatePhysicalFinish();
        }
        void CreatePhysicalFinish()
        {
            Tint(board.Find("Clipboard back"),new Color(.13f,.19f,.22f));
            Tint(board.Find("Metal clip"),new Color(.55f,.62f,.63f));
            Layer("Hoja inferior / borde visible",new Vector3(.008f,-.012f,-.0193f),new Vector3(.316f,.402f,.001f),new Color(.76f,.79f,.75f));
            Layer("Hoja superior / marfil",new Vector3(0,-.005f,-.0212f),new Vector3(.318f,.404f,.001f),new Color(.91f,.92f,.86f));
            Layer("Acento institucional en el margen",new Vector3(-.152f,.108f,-.0225f),new Vector3(.002f,.046f,.0006f),new Color(.22f,.48f,.52f));
            Layer("Pinza metálica frontal",new Vector3(0,.198f,-.027f),new Vector3(.17f,.026f,.005f),new Color(.55f,.62f,.65f));
            var wipe=new GameObject("Transición de hoja / solo visual",typeof(RectTransform),typeof(Image));
            wipe.transform.SetParent(PaperView.transform,false);
            pageWipe=wipe.GetComponent<Image>();pageWipe.raycastTarget=false;
            pageWipe.rectTransform.sizeDelta=new Vector2(54,290);
            pageWipe.color=new Color(.08f,.20f,.22f,0);
            wipe.SetActive(false);
        }
        void Tint(Transform part,Color color)
        {
            if(part==null)return;
            foreach(var renderer in part.GetComponentsInChildren<MeshRenderer>()){
                var original=renderer.sharedMaterial;if(original==null)continue;
                var variant=new Material(original);variant.name=original.name+" / clipboard pilot";
                if(variant.HasProperty("_BaseColor"))variant.SetColor("_BaseColor",color);
                if(variant.HasProperty("_Color"))variant.SetColor("_Color",color);
                if(variant.HasProperty("_Smoothness"))variant.SetFloat("_Smoothness",.12f);
                originalMaterials.Add(renderer,original);visualMaterials.Add(variant);renderer.sharedMaterial=variant;
            }
        }
        void Layer(string name,Vector3 position,Vector3 scale,Color color)
        {
            var layer=GameObject.CreatePrimitive(PrimitiveType.Cube);layer.name=name;
            layer.transform.SetParent(board,false);layer.transform.localPosition=position;
            layer.transform.localScale=scale;
            Destroy(layer.GetComponent<Collider>());
            var shader=Shader.Find("Universal Render Pipeline/Lit");
            var material=new Material(shader!=null?shader:Shader.Find("Standard"));material.name=name+" / runtime";
            if(material.HasProperty("_BaseColor"))material.SetColor("_BaseColor",color);
            if(material.HasProperty("_Color"))material.SetColor("_Color",color);
            if(material.HasProperty("_Smoothness"))material.SetFloat("_Smoothness",.03f);
            layer.GetComponent<Renderer>().sharedMaterial=material;
            visualMaterials.Add(material);visualLayers.Add(layer);
        }
        public void AnimatePageTurn()
        {
            if(!polished||State!=PoseState.Inspecting||pageWipe==null)return;
            if(pageMotion!=null)StopCoroutine(pageMotion);
            pageMotion=StartCoroutine(PageSweep());
        }
        IEnumerator PageSweep()
        {
            pageWipe.gameObject.SetActive(true);var elapsed=0f;
            while(elapsed<.30f){elapsed+=Time.unscaledDeltaTime;var t=Mathf.Clamp01(elapsed/.30f);
                pageWipe.rectTransform.anchoredPosition=new Vector2(Mathf.Lerp(155,-155,t),14);
                pageWipe.color=new Color(.08f,.20f,.22f,Mathf.Sin(t*Mathf.PI)*.25f);
                yield return null;}
            pageWipe.gameObject.SetActive(false);pageMotion=null;
        }
        public void Lift(Transform eye)
        {
            if(board==null||State!=PoseState.Resting)return;
            var forward=Vector3.ProjectOnPlane(eye.forward,Vector3.up).normalized;
            if(forward.sqrMagnitude<.01f)forward=eye.forward;
            var right=Vector3.Cross(Vector3.up,forward).normalized;
            var target=eye.position+forward*.64f-right*.34f-Vector3.up*.06f;
            var facing=Quaternion.LookRotation(target-eye.position,Vector3.up);
            State=PoseState.Lifting;
            takeHint.gameObject.SetActive(false);
            if(boardCollider!=null)boardCollider.enabled=false;
            if(printedSteps!=null)printedSteps.gameObject.SetActive(false);
            if(motion!=null)StopCoroutine(motion);
            motion=StartCoroutine(MoveTo(target,facing,true));
        }
        public void Return()
        {
            if(board==null||State==PoseState.Resting||State==PoseState.Returning)return;
            PaperView.gameObject.SetActive(false);
            State=PoseState.Returning;
            if(motion!=null)StopCoroutine(motion);
            motion=StartCoroutine(MoveTo(RestPosition,RestRotation,false));
        }
        public void RestoreImmediately()
        {
            if(board==null)return;
            if(motion!=null){StopCoroutine(motion);motion=null;}
            if(pageMotion!=null){StopCoroutine(pageMotion);pageMotion=null;}
            if(pageWipe!=null)pageWipe.gameObject.SetActive(false);
            PaperView.gameObject.SetActive(false);
            board.SetPositionAndRotation(RestPosition,RestRotation);
            if(boardCollider!=null)boardCollider.enabled=true;
            if(printedSteps!=null)printedSteps.gameObject.SetActive(true);
            State=PoseState.Resting;
        }
        void LateUpdate()
        {
            if(takeHint!=null&&gaze!=null&&target!=null)
                takeHint.gameObject.SetActive(State==PoseState.Resting&&gaze.FocusedTarget==target&&target.CanActivate);
        }
        IEnumerator MoveTo(Vector3 destination,Quaternion rotation,bool opening)
        {
            var start=board.position;var turn=board.rotation;
            // Lift clear of the desktop before tilting or crossing its edge.
            var clearance=start+Vector3.up*(opening?.24f:.18f);
            yield return Segment(start,turn,clearance,turn,.24f);
            yield return Segment(clearance,turn,destination,rotation,.46f);
            board.SetPositionAndRotation(destination,rotation);
            if(opening){State=PoseState.Inspecting;PaperView.gameObject.SetActive(true);}
            else{
                if(boardCollider!=null)boardCollider.enabled=true;
                if(printedSteps!=null)printedSteps.gameObject.SetActive(true);
                State=PoseState.Resting;
            }
            motion=null;
        }
        IEnumerator Segment(Vector3 from,Quaternion fromRotation,Vector3 to,Quaternion toRotation,float duration)
        {
            var elapsed=0f;
            while(elapsed<duration){
                elapsed+=Time.deltaTime;
                var t=Mathf.Clamp01(elapsed/duration);t=t*t*(3f-2f*t);
                board.SetPositionAndRotation(Vector3.Lerp(from,to,t),Quaternion.Slerp(fromRotation,toRotation,t));
                yield return null;
            }
        }
        void OnDisable(){RestoreImmediately();}
        void OnDestroy(){
            RestoreImmediately();
            foreach(var entry in originalMaterials)if(entry.Key!=null)entry.Key.sharedMaterial=entry.Value;
            foreach(var layer in visualLayers)if(layer!=null)Destroy(layer);
            foreach(var material in visualMaterials)if(material!=null)Destroy(material);
        }
    }
}

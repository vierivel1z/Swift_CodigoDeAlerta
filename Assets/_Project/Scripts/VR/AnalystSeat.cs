using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Swift.Interaction;

namespace Swift.VR
{
    // Moves only the tracking origin under an opaque fade; the head pose provider remains enabled.
    public sealed class AnalystSeat : MonoBehaviour
    {
        public static AnalystSeat Current { get; private set; }
        public Transform Rig, Eye, SeatedEye, StandingFloor;
        public BankInteractable SitTarget, StandTarget;
        public bool IsSeated { get; private set; }
        public bool IsTransitioning { get; private set; }
        public int TransitionCount { get; private set; }
        public float FadeOpacity => fade!=null?fade.color.a:0;
        public string LastFailure { get; private set; }
        Collider[] pads;
        bool[] padStates;
        Image fade;
        float standingEyeHeight;
        Vector3 originBeforeSit;
        void Awake() { Current=this; }
        void Start() { UpdateTargets(); }
        public static Vector3 OriginForEye(Vector3 desiredEye, Vector3 trackedEye, Vector3 origin)
            => desiredEye - (trackedEye-origin);
        public bool DestinationSafe(Vector3 desiredEye)
        {
            // Feet to head clearance; own chair/markers are intentionally excluded, floors below the feet are not hit.
            var foot = new Vector3(desiredEye.x, StandingFloor.position.y+.25f, desiredEye.z);
            return !Physics.OverlapCapsule(foot,desiredEye,.18f,~0,QueryTriggerInteraction.Ignore)
                .Any(c=>!c.transform.IsChildOf(transform) && !c.transform.IsChildOf(Rig));
        }
        public bool Request(bool sit)
        {
            if(IsTransitioning||sit==IsSeated||Rig==null||Eye==null||SeatedEye==null||StandingFloor==null
                ||Swift.UI.VRQuickTutorial.Active!=null||FindFirstObjectByType<Swift.Game.GameManager>()?.IntroPlaying==true)return false;
            if(sit) standingEyeHeight=Eye.position.y-StandingFloor.position.y;
            var desired=sit?SeatedEye.position:StandingFloor.position+Vector3.up*standingEyeHeight;
            Physics.SyncTransforms();
            if(!DestinationSafe(desired)){LastFailure="Destino ocupado: libera el espacio junto a la silla.";return false;}
            if(sit)originBeforeSit=Rig.position;
            IsTransitioning=true;StartCoroutine(Transition(sit,desired));return true;
        }
        IEnumerator Transition(bool sit,Vector3 desired)
        {
            EnsureFade();
            yield return Fade(0,1,.16f);
            Physics.SyncTransforms();
            if(DestinationSafe(desired)) {
                Rig.position=OriginForEye(desired,Eye.position,Rig.position);
                IsSeated=sit;TransitionCount++;LastFailure=null;
                if(sit) {
                    pads=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Where(x=>x.Action==BankAction.Teleport)
                        .SelectMany(x=>x.GetComponents<Collider>()).ToArray();
                    padStates=pads.Select(x=>x.enabled).ToArray();foreach(var p in pads)p.enabled=false;
                } else RestorePads();
                UpdateTargets();
            } else LastFailure="El destino se ocupo durante la transicion.";
            yield return null;
            yield return Fade(1,0,.20f);
            IsTransitioning=false;
        }
        void UpdateTargets()
        { if(SitTarget!=null)SitTarget.gameObject.SetActive(!IsSeated);if(StandTarget!=null)StandTarget.gameObject.SetActive(IsSeated); }
        void RestorePads()
        {if(pads!=null)for(int i=0;i<pads.Length;i++)if(pads[i]!=null)pads[i].enabled=padStates[i];pads=null;padStates=null;}
        void EnsureFade()
        {
            if(fade!=null)return;
            var canvas=new GameObject("Seat transition - binocular blackout",typeof(RectTransform),typeof(Canvas));
            canvas.transform.SetParent(Eye,false);canvas.transform.localPosition=new Vector3(0,0,Mathf.Max(.15f,Eye.GetComponent<Camera>().nearClipPlane+.03f));
            canvas.transform.localScale=Vector3.one*.001f;
            canvas.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;canvas.GetComponent<Canvas>().sortingOrder=32000;
            canvas.GetComponent<RectTransform>().sizeDelta=new Vector2(4000,4000);
            var panel=new GameObject("Blackout",typeof(RectTransform),typeof(Image));panel.transform.SetParent(canvas.transform,false);
            panel.GetComponent<RectTransform>().sizeDelta=new Vector2(4000,4000);fade=panel.GetComponent<Image>();fade.raycastTarget=false;
            fade.color=new Color(0,0,0,0);
        }
        IEnumerator Fade(float from,float to,float seconds)
        {float elapsed=0;while(elapsed<seconds){elapsed+=Time.unscaledDeltaTime;fade.color=new Color(0,0,0,Mathf.Lerp(from,to,Mathf.SmoothStep(0,1,elapsed/seconds)));yield return null;}fade.color=new Color(0,0,0,to);}
        void OnDisable()
        {
            StopAllCoroutines();RestorePads();
            if(IsSeated&&Rig!=null)Rig.position=originBeforeSit;
            IsSeated=false;IsTransitioning=false;if(fade!=null)Destroy(fade.transform.parent.gameObject);
            if(Current==this)Current=null;
        }
    }
}

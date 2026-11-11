using UnityEngine;
using Swift.Interaction;
namespace Swift.DeskTools
{
    // Shared-material-safe highlight: no material instances, extra lights or secondary raycast.
    public sealed class DeskObjectFeedback : MonoBehaviour
    {
        public BankInteractable Target;
        public int ConfirmationCount { get; private set; }
        public bool Confirmed { get; private set; }
        Renderer[] surfaces;Color[] baseColors;MaterialPropertyBlock block;VRInteractionManager gaze;
        float highlight;
        void Start() {
            gaze=FindFirstObjectByType<VRInteractionManager>();surfaces=GetComponentsInChildren<MeshRenderer>();
            baseColors=new Color[surfaces.Length];block=new MaterialPropertyBlock();
            for(int i=0;i<surfaces.Length;i++)baseColors[i]=surfaces[i].sharedMaterial!=null&&surfaces[i].sharedMaterial.HasProperty("_BaseColor")?surfaces[i].sharedMaterial.GetColor("_BaseColor"):Color.white;
        }
        public void Confirm(){Confirmed=true;ConfirmationCount++;}
        public void ResetConfirmation(){Confirmed=false;}
        void LateUpdate(){
            bool focus=gaze!=null&&gaze.FocusedTarget==Target&&Target!=null&&Target.CanActivate;
            float desired=focus?.23f:Confirmed?.08f:0f;
            float next=Mathf.Lerp(highlight,desired,1f-Mathf.Exp(-14f*Time.unscaledDeltaTime));
            if(Mathf.Abs(next-desired)<.002f)next=desired;
            if(Mathf.Abs(next-highlight)<.0005f)return;
            highlight=next;Apply(highlight);
        }
        void Apply(float strength) {
            if(surfaces==null)return;
            for(int i=0;i<surfaces.Length;i++) {
                var r=surfaces[i];if(r==null||r.sharedMaterial==null||!r.sharedMaterial.HasProperty("_BaseColor"))continue;
                r.GetPropertyBlock(block);block.SetColor("_BaseColor",Color.Lerp(baseColors[i],new Color(.28f,.85f,.87f),strength));r.SetPropertyBlock(block);
            }
        }
        void OnDisable(){highlight=0;if(surfaces!=null)foreach(var r in surfaces)if(r!=null)r.SetPropertyBlock(null);}
    }
}

using UnityEngine;
using Swift.Interaction;

namespace Swift.UI {
    // Presentation only: consumes the existing dwell/latch; never activates a target.
    public sealed class ArchitecturalFocusVisual : MonoBehaviour {
        public BankInteractable Target;
        public Renderer Surface;
        public bool HideWhenColliderDisabled;
        public Color Idle=new Color(.45f,.54f,.57f), Focus=new Color(.12f,.75f,.78f);
        VRInteractionManager gaze;MaterialPropertyBlock block;
        void Awake(){gaze=FindFirstObjectByType<VRInteractionManager>();block=new MaterialPropertyBlock();}
        void LateUpdate(){
            if(Surface==null||Target==null)return;
            if(block==null)block=new MaterialPropertyBlock();
            if(gaze==null)gaze=FindFirstObjectByType<VRInteractionManager>();
            var collider=Target.GetComponent<Collider>();
            Surface.enabled=!HideWhenColliderDisabled||(collider!=null&&collider.enabled);
            bool active=gaze!=null&&gaze.FocusedTarget==Target;
            var color=active?(gaze.SelectionLatched?new Color(1,.76f,.28f):Focus):Idle;
            Surface.GetPropertyBlock(block);block.SetColor("_BaseColor",color);Surface.SetPropertyBlock(block);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Swift.Interaction;
namespace Swift.UI
{
    // Pure visual feedback; activation stays in VRInteractionManager.
    public sealed class DocumentFocus : MonoBehaviour
    {
        public BankInteractable Target;
        public Image Background;
        VRInteractionManager gaze;
        Color normal;
        void Start() { gaze=FindFirstObjectByType<VRInteractionManager>(); if(Background!=null) normal=Background.color; }
        void LateUpdate() {
            if(Background==null)return;
            var color=gaze!=null&&gaze.SelectionLatched
                ?(Time.unscaledTime-gaze.LastActivationTime<.28f?new Color(.25f,1,.55f):new Color(1,.76f,.22f)):Color.white;
            Background.color=gaze!=null&&gaze.FocusedTarget==Target?Color.Lerp(normal,color,.24f):normal;
        }
    }
}

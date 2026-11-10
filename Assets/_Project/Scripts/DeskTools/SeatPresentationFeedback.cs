using UnityEngine;
using Swift.VR;
using Swift.Audio;
namespace Swift.DeskTools
{
    // Moves the chair's visual mesh only. Tracking origin, destination and colliders are untouched.
    public sealed class SeatPresentationFeedback : MonoBehaviour
    {
        public AnalystSeat Seat;
        public Transform ChairVisual;
        Vector3 rest;bool transition;int count;
        public int ResponseCount { get; private set; }
        void Start(){if(ChairVisual!=null)rest=ChairVisual.localPosition;count=Seat!=null?Seat.TransitionCount:0;}
        void LateUpdate(){
            if(Seat==null)return;
            if(Seat.IsTransitioning&&!transition){ResponseCount++;FeedbackAudio.Current?.Emit(AudioCue.Chair,Seat.transform.position);}
            transition=Seat.IsTransitioning;
            if(ChairVisual!=null)ChairVisual.localPosition=rest+Vector3.back*(Seat.IsTransitioning?.035f*Mathf.Sin(Seat.FadeOpacity*Mathf.PI):0);
            count=Seat.TransitionCount;
        }
        void OnDisable(){if(ChairVisual!=null)ChairVisual.localPosition=rest;}
    }
}

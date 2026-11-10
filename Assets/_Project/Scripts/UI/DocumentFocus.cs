using UnityEngine;
using UnityEngine.UI;
using Swift.Interaction;
namespace Swift.UI {
 // Presentation only: every bar reads the sole clock in VRInteractionManager.
 public sealed class DocumentFocus : MonoBehaviour {
 public BankInteractable Target; public Image Background,Border,Progress;
 public Color Accent=new Color(.22f,.94f,1);
 VRInteractionManager gaze;Color normal;
 public void SetNormalColor(Color value){normal=value;if(Background!=null)Background.color=value;}
 void Start(){gaze=FindFirstObjectByType<VRInteractionManager>();if(Background!=null)normal=Background.color;}
 void LateUpdate(){
 bool focus=gaze!=null&&gaze.FocusedTarget==Target;
 bool confirmed=focus&&gaze.SelectionLatched&&Time.unscaledTime-gaze.LastActivationTime<.28f;
 if(Background!=null)Background.color=focus?Color.Lerp(normal,confirmed?Color.white:new Color(.16f,.8f,.86f),confirmed?.32f:.16f):normal;
 if(Border!=null)Border.color=confirmed?Color.white:focus?Accent:new Color(Accent.r*.50f,Accent.g*.50f,Accent.b*.50f,.8f);
 if(Progress!=null){Progress.fillAmount=focus&&gaze.UsesDwellSelection?gaze.DwellProgress:0;Progress.color=confirmed?Color.white:Accent;}
 }
 }
}

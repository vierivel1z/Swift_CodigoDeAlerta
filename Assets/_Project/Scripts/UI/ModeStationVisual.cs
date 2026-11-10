using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Swift.Interaction;
using System.Collections;
namespace Swift.UI {
 // Presentation consumes the existing raycast/dwell; the coroutine only delays the transition AFTER selection.
 public sealed class ModeStationVisual:MonoBehaviour {
 public BankInteractable Attack,Defense;
 public Renderer AttackLamp,DefenseLamp;
 public TMP_Text AttackStatus,DefenseStatus;
 public Image AttackProgress,DefenseProgress,AttackBorder,DefenseBorder;
 public BankAction Selected{get;private set;}=(BankAction)(-1);
 public bool ConfirmationVisible{get;private set;}
 VRInteractionManager gaze; MaterialPropertyBlock block;
 Canvas modeCanvas; CanvasGroup portalGroup; bool hiddenForIntro; float originalAlpha;
 public Image AttackWash,DefenseWash,AttackModuleBorder,DefenseModuleBorder,AttackSelectionEdge,DefenseSelectionEdge;
 static readonly Color Amber=new Color(1,.65f,.23f), Cyan=new Color(.24f,.86f,.96f);
 public bool PresentSelection(BankAction action){
 if(AttackProgress==null||DefenseProgress==null)return false;
 if(ConfirmationVisible)return true;
 Confirm(action);StartCoroutine(Transition(action));return true;
 }
 public void Confirm(BankAction action){if(action==BankAction.Attack||action==BankAction.DefenseMode)Selected=action;}
 IEnumerator Transition(BankAction action){ConfirmationVisible=true;yield return new WaitForSecondsRealtime(.35f);ConfirmationVisible=false;
 var target=action==BankAction.Attack?Attack:Defense;
 if(target!=null&&target.Manager!=null)target.Manager.EnterScenario(action==BankAction.Attack?Swift.Bank.ScenarioMode.Attack:Swift.Bank.ScenarioMode.Defense);
 }
 void OnEnable(){ResetPresentation();}
 void OnDisable(){StopAllCoroutines();RestorePortal();ResetPresentation();}
 public void ResetPresentation(){ConfirmationVisible=false;Selected=(BankAction)(-1);if(AttackProgress!=null)AttackProgress.fillAmount=0;if(DefenseProgress!=null)DefenseProgress.fillAmount=0;}
 void Awake(){if(Attack!=null&&Attack.Manager==null)Attack.Manager=FindFirstObjectByType<Swift.Game.GameManager>();if(Defense!=null&&Defense.Manager==null)Defense.Manager=FindFirstObjectByType<Swift.Game.GameManager>();}
 void Start(){gaze=FindFirstObjectByType<VRInteractionManager>();block=new MaterialPropertyBlock();if(AttackProgress!=null)modeCanvas=AttackProgress.GetComponentInParent<Canvas>();if(modeCanvas!=null){portalGroup=modeCanvas.GetComponent<CanvasGroup>();if(portalGroup==null)portalGroup=modeCanvas.gameObject.AddComponent<CanvasGroup>();originalAlpha=portalGroup.alpha;}}
 void RestorePortal(){if(hiddenForIntro&&portalGroup!=null)portalGroup.alpha=originalAlpha;hiddenForIntro=false;}
 void LateUpdate(){
 if(gaze==null)return;
 bool intro=Attack!=null&&Attack.Manager!=null&&Attack.Manager.IntroPlaying;
 if(intro&&!hiddenForIntro&&portalGroup!=null){hiddenForIntro=true;portalGroup.alpha=originalAlpha*.48f;}
 else if(!intro&&hiddenForIntro)RestorePortal();
 bool af=gaze.FocusedTarget==Attack,df=gaze.FocusedTarget==Defense;
 if(!af&&!df&&!ConfirmationVisible)Selected=(BankAction)(-1);
 Draw(Attack,AttackProgress,AttackBorder,AttackStatus,AttackLamp,Amber,af);
 Draw(Defense,DefenseProgress,DefenseBorder,DefenseStatus,DefenseLamp,Cyan,df);
 }
 void Draw(BankInteractable target,Image progress,Image border,TMP_Text text,Renderer lamp,Color tint,bool focused){
 bool confirmed=ConfirmationVisible&&target!=null&&Selected==target.Action;
 bool disabled=target!=null&&target.Manager!=null&&target.Manager.IntroPlaying;
 // Exactly the value accumulated in VRInteractionManager.Update; no local elapsed time or smoothing timer.
 float value=focused&&(confirmed||!disabled&&gaze.UsesDwellSelection)?gaze.DwellProgress:0;
 if(progress!=null)progress.fillAmount=value;
 bool left=target==Attack;var wash=left?AttackWash:DefenseWash;var module=left?AttackModuleBorder:DefenseModuleBorder;var edge=left?AttackSelectionEdge:DefenseSelectionEdge;
 if(wash!=null)wash.color=new Color(tint.r,tint.g,tint.b,disabled?0:confirmed?.23f:focused?.025f+.055f*value:0);
 if(module!=null)module.color=confirmed?Color.Lerp(tint,Color.white,.55f):new Color(tint.r,tint.g,tint.b,focused?1:.38f);
 if(edge!=null){edge.fillAmount=value;edge.color=confirmed?Color.white:tint;}
 if(progress!=null)progress.color=confirmed?Color.Lerp(tint,Color.white,.35f):tint;
 if(border!=null)border.color=tint*(disabled?.3f:focused||confirmed?1:.55f);
 if(text!=null)text.text=confirmed?"MODO CONFIRMADO":disabled?"PR\u00d3LOGO EN CURSO":focused&&gaze.SelectionLatched?"APARTA LA MIRADA":focused?(gaze.IsGazeInput?"MANT\u00c9N LA MIRADA / 1,5 s":"MANT\u00c9N PULSADO / 1,5 s"):"MIRA PARA SELECCIONAR";
 if(lamp!=null&&block!=null){lamp.GetPropertyBlock(block);block.SetColor("_BaseColor",tint*(focused?1:.15f));lamp.SetPropertyBlock(block);}
 }
 }
}

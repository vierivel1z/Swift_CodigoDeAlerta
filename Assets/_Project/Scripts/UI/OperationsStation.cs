using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Swift.Interaction;
using Swift.VR;
namespace Swift.UI {
 // Workstation visibility follows the existing chair state; this component never starts a banking session.
 public sealed class OperationsStation:MonoBehaviour {
 public enum StationState { Standing,Focused,Entering,Seated,Leaving }
 public StationState State{get;private set;}
 public bool DetailedVisible=>detailed!=null&&detailed.gameObject.activeInHierarchy;
 public bool ContextVisible=>context!=null&&context.activeInHierarchy;
 public bool SecondaryVisible=>secondary!=null&&secondary.gameObject.activeInHierarchy;
 public BankInteractable EntryTarget{get;private set;}public BankInteractable ExitTarget{get;private set;}
 public Transform Desktop{get;private set;}public Image EntryProgress{get;private set;}
 bool wasTransitioning,entering;
 UIManager owner;AnalystSeat seat;Transform detailed,secondary,secondaryIdle;GameObject context;VRInteractionManager gaze;
 public void Initialize(UIManager ui,Transform bankCanvas){
 owner=ui;detailed=bankCanvas;seat=AnalystSeat.Current??FindFirstObjectByType<AnalystSeat>();gaze=FindFirstObjectByType<VRInteractionManager>();if(seat==null)return;
 if(seat.SitTarget!=null)seat.SitTarget.gameObject.SetActive(false);if(seat.StandTarget!=null)seat.StandTarget.gameObject.SetActive(false);
 var desktop=new GameObject("PC corporativa - reposo activo",typeof(RectTransform),typeof(Canvas));desktop.transform.SetParent(detailed.parent,false);desktop.transform.localPosition=detailed.localPosition;desktop.transform.localRotation=detailed.localRotation;desktop.transform.localScale=detailed.localScale;desktop.GetComponent<RectTransform>().sizeDelta=new Vector2(1600,900);desktop.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;Desktop=desktop.transform;
 var hit=desktop.AddComponent<BoxCollider>();hit.size=new Vector3(1600,900,12);EntryTarget=desktop.AddComponent<BankInteractable>();EntryTarget.Manager=owner.Manager;EntryTarget.Action=BankAction.Sit;seat.SitTarget=EntryTarget;
 BuildDesktop(Desktop);
 var module=Panel(Desktop,"Entrada contextual",new Vector2(0,-270),new Vector2(690,126),new Color(.025f,.20f,.25f));context=module.gameObject;
 Text(module.transform,"Entrar","ENTRAR A ESTACI\u00d3N",new Vector2(50,15),new Vector2(530,65),35,Color.white);Icon(module.transform,7,new Vector2(-275,17),55);
 var border=Frame(Desktop,new Vector2(1600,900));var track=Basic(module.transform,"Pista entrada",new Vector2(0,-40),new Vector2(620,13),new Color(.06f,.12f,.16f));EntryProgress=Basic(track.transform,"Dwell real de entrada",Vector2.zero,new Vector2(620,13),new Color(.16f,.85f,.92f));EntryProgress.sprite=owner.StationFill;EntryProgress.type=Image.Type.Filled;EntryProgress.fillMethod=Image.FillMethod.Horizontal;EntryProgress.fillOrigin=0;EntryProgress.fillAmount=0;
 var focus=desktop.AddComponent<DocumentFocus>();focus.Target=EntryTarget;focus.Background=module;focus.Border=border;focus.Progress=EntryProgress;
 ExitTarget=detailed.GetComponentsInChildren<BankInteractable>(true).Single(x=>x.Action==BankAction.Stand);seat.StandTarget=ExitTarget;
 var reference=FindFirstObjectByType<FormatReferencePanel>();if(reference!=null){var canvas=reference.GetComponentInChildren<Canvas>(true);if(canvas!=null){secondary=canvas.transform;StyleSecondary(reference);var idle=new GameObject("PC secundaria - consulta en espera",typeof(RectTransform),typeof(Canvas));idle.transform.SetParent(secondary.parent,false);idle.transform.localPosition=secondary.localPosition;idle.transform.localRotation=secondary.localRotation;var size=((RectTransform)secondary).rect.size;idle.transform.localScale=secondary.localScale;idle.GetComponent<RectTransform>().sizeDelta=size;idle.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;secondaryIdle=idle.transform;BuildSecondaryDesktop(secondaryIdle,size);}}
 RefreshVisibility();
 }

 void StyleSecondary(FormatReferencePanel panel){
 panel.CompactSupport=true;panel.RuleText="Emisor / Beneficiario\nCuenta / Monto / Intermediario";
 var canvas=panel.GetComponentInChildren<Canvas>(true);var surface=canvas.transform;var size=((RectTransform)surface).rect.size;
 var chrome=Panel(surface,"Support - structured chrome",Vector2.zero,size,new Color(.018f,.055f,.085f)).transform;chrome.SetAsLastSibling();var previousSurface=surface.Find("Surface");if(previousSurface!=null)previousSurface.gameObject.SetActive(false);Frame(chrome,size);
 var header=Panel(chrome,"Cabecera de apoyo",new Vector2(0,375),new Vector2(size.x-120,130),new Color(.028f,.115f,.16f)).transform;
 Text(header,"Identidad","SWIFT / APOYO DE REVISI\u00d3N",new Vector2(0,25),new Vector2(size.x-190,65),39,new Color(.84f,.95f,.96f)).fontStyle=FontStyles.Bold;
 panel.SupportRecord=Text(header,"Expediente actual","",new Vector2(0,-34),new Vector2(size.x-190,42),29,new Color(.23f,.81f,.86f));
 var oldHeader=surface.Find("Header");if(oldHeader!=null)oldHeader.gameObject.SetActive(false);
 void Card(TMP_Text body,string heading,float y,Color tint){
 var card=Panel(chrome,heading,new Vector2(0,y),new Vector2(size.x-120,190),new Color(.035f,.12f,.165f)).transform;Frame(card,new Vector2(size.x-120,190));
 Text(card,"Titulo",heading,new Vector2(0,64),new Vector2(size.x-200,40),30,tint).fontStyle=FontStyles.Bold;
 body.transform.SetParent(card,false);body.rectTransform.anchoredPosition=new Vector2(0,-23);body.rectTransform.sizeDelta=new Vector2(size.x-200,126);body.fontStyle=FontStyles.Normal;body.enableAutoSizing=true;body.fontSizeMin=34;body.fontSizeMax=40;body.fontSize=40;body.color=new Color(.91f,.94f,.94f);body.overflowMode=TextOverflowModes.Ellipsis;
 }
 Card(panel.Rules,"01 / FORMATO",190,new Color(.25f,.85f,.89f));Card(panel.Observed,"02 / CUENTA Y BENEFICIARIO",-20,new Color(.25f,.85f,.89f));Card(panel.ExistingAlert,"03 / OBSERVACIONES",-230,new Color(1,.72f,.34f));
 Text(chrome,"Detalle completo","Detalle completo en REGISTRO y ALERTAS",new Vector2(0,-365),new Vector2(size.x-150,55),28,new Color(.57f,.76f,.8f));panel.Refresh();
 }
 void LateUpdate(){RefreshVisibility();}
 void RefreshVisibility(){if(seat==null||Desktop==null)return;
 bool seated=seat.IsSeated;bool focused=!seated&&gaze!=null&&gaze.FocusedTarget==EntryTarget&&!owner.Manager.IntroPlaying;
 if(seat.IsTransitioning&&!wasTransitioning)entering=!seated;wasTransitioning=seat.IsTransitioning;
 State=seat.IsTransitioning?(entering?StationState.Entering:StationState.Leaving):seated?StationState.Seated:focused?StationState.Focused:StationState.Standing;
 detailed.gameObject.SetActive(seated);Desktop.gameObject.SetActive(!seated);context.SetActive(focused&&(!seat.IsTransitioning||entering));
 if(!focused)EntryProgress.fillAmount=0;
 if(secondary!=null)secondary.gameObject.SetActive(seated);if(secondaryIdle!=null)secondaryIdle.gameObject.SetActive(!seated);
 }
 void BuildSecondaryDesktop(Transform parent,Vector2 size){
 Panel(parent,"Pantalla secundaria luminosa",Vector2.zero,size,new Color(.022f,.075f,.115f));Frame(parent,size);
 var header=Panel(parent,"Cabecera secundaria",new Vector2(0,350),new Vector2(size.x-120,110),new Color(.035f,.12f,.17f)).transform;
 Text(header,"Identidad","SWIFT / APOYO",Vector2.zero,new Vector2(size.x-200,80),47,Color.white).fontStyle=FontStyles.Bold;
 var status=Panel(parent,"Consulta disponible",new Vector2(0,0),new Vector2(size.x-120,330),new Color(.035f,.12f,.17f)).transform;Frame(status,new Vector2(size.x-120,330));Icon(status,7,new Vector2(0,100),100);
 Text(status,"Espera","CONSULTA EN ESPERA",new Vector2(0,-10),new Vector2(size.x-200,85),44,Color.white).alignment=TextAlignmentOptions.Center;
 Text(status,"Apoyo","Apoyo de revisi\u00f3n",new Vector2(0,-105),new Vector2(size.x-200,70),36,new Color(.35f,.82f,.85f)).alignment=TextAlignmentOptions.Center;
 var bar=Panel(parent,"Barra secundaria",new Vector2(0,-395),new Vector2(size.x-120,65),new Color(.012f,.04f,.065f)).transform;
 Text(bar,"Sistema","Terminal lista",Vector2.zero,new Vector2(size.x-200,48),30,new Color(.55f,.82f,.85f));
 }
 void BuildDesktop(Transform parent){
 Panel(parent,"Pantalla luminosa institucional",Vector2.zero,new Vector2(1600,900),new Color(.025f,.075f,.115f));Frame(parent,new Vector2(1600,900));
 var header=Panel(parent,"Cabecera corporativa",new Vector2(0,345),new Vector2(1440,120),new Color(.035f,.115f,.16f)).transform;
 var brand=Text(header,"Marca corporativa","SWIFT / CENTRO DE OPERACIONES",Vector2.zero,new Vector2(1330,85),49,new Color(.9f,.96f,.97f));brand.alignment=TextAlignmentOptions.Center;brand.fontStyle=FontStyles.Bold;
 var ready=Panel(parent,"Estado de terminal",new Vector2(0,155),new Vector2(1440,205),new Color(.035f,.13f,.18f)).transform;
 var title=Text(ready,"Disponibilidad","ESTACI\u00d3N DISPONIBLE",new Vector2(0,38),new Vector2(1320,85),62,new Color(.22f,.85f,.88f));title.alignment=TextAlignmentOptions.Center;title.fontStyle=FontStyles.Bold;
 Text(ready,"Indicacion","Mira el monitor principal para entrar",new Vector2(0,-48),new Vector2(1320,65),37,new Color(.82f,.89f,.91f)).alignment=TextAlignmentOptions.Center;
 var labels=new[]{"Expediente","Registro","Alertas","Procedimiento"};for(int i=0;i<4;i++){
 var app=Panel(parent,"Aplicacion "+labels[i],new Vector2(-540+360*i,-90),new Vector2(330,210),new Color(.035f,.12f,.17f)).transform;Frame(app,new Vector2(330,210));Icon(app,i,new Vector2(0,38),76);
 var label=Text(app,"Acceso",labels[i],new Vector2(0,-55),new Vector2(290,65),35,new Color(.87f,.93f,.94f));label.alignment=TextAlignmentOptions.Center;label.fontStyle=FontStyles.Bold;
 }
 var bar=Panel(parent,"Barra de sistema",new Vector2(0,-410),new Vector2(1440,60),new Color(.015f,.04f,.065f)).transform;
 Text(bar,"Sistema","Terminal lista",Vector2.zero,new Vector2(1320,48),31,new Color(.5f,.8f,.83f));
 }
 Image Panel(Transform parent,string name,Vector2 point,Vector2 size,Color tint){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(PortalPanelGraphic));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=point;r.sizeDelta=size;var image=go.GetComponent<PortalPanelGraphic>();image.Top=new Color(1,1,1);image.Bottom=new Color(.62f,.78f,.85f);image.color=tint;image.CornerCut=22;image.raycastTarget=false;return image;}
 Image Basic(Transform parent,string name,Vector2 point,Vector2 size,Color tint){var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=point;r.sizeDelta=size;var image=go.GetComponent<Image>();image.color=tint;image.raycastTarget=false;return image;}
 Image Frame(Transform parent,Vector2 size){var image=Basic(parent,"Marco cian",Vector2.zero,size,new Color(.12f,.55f,.65f));image.sprite=owner.StationFrame;image.type=Image.Type.Sliced;image.fillCenter=false;image.pixelsPerUnitMultiplier=2;return image;}
 TMP_Text Text(Transform parent,string name,string value,Vector2 point,Vector2 size,float font,Color tint){var t=owner.Text(parent,name,point,size,font);t.text=value;t.color=tint;t.alignment=TextAlignmentOptions.MidlineLeft;t.textWrappingMode=TextWrappingModes.Normal;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
 void Icon(Transform parent,int symbol,Vector2 point,float size){var go=new GameObject("Acceso visual "+symbol,typeof(RectTransform),typeof(CanvasRenderer),typeof(StationIconGraphic));go.transform.SetParent(parent,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=point;r.sizeDelta=Vector2.one*size;var image=go.GetComponent<StationIconGraphic>();image.Symbol=symbol;image.color=new Color(.3f,.82f,.86f);image.raycastTarget=false;}
 }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Swift.Interaction;
using Swift.UI;
namespace Swift.DeskTools
{
    public sealed class DeskInspectionView : MonoBehaviour
    {
        public TMP_Text Heading, Context, SheetTitle, Body, Notice, PageLabel;
        public DeskActionTarget Previous,Primary,Next,Close;
        public Transform Root => transform;
        public void Build(TMP_FontAsset font,Sprite frame,Sprite fill)
        {
            if(Heading!=null){Body.fontSize=Mathf.Max(36,Body.fontSize);return;}
            gameObject.name="Inspección documental / mirada";
            var rt=GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1000,800);
            var canvas=GetComponent<Canvas>()??gameObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.sortingOrder=30;
            var background=Box(transform,"Marco institucional",Vector2.zero,new Vector2(1000,800),new Color(.018f,.052f,.075f),frame);
            Box(transform,"Cabecera",new Vector2(0,310),new Vector2(944,144),new Color(.035f,.15f,.19f),frame);
            Heading=Text(transform,"Herramienta",new Vector2(0,342),new Vector2(884,68),42,Color.white,font);
            Context=Text(transform,"Fuente y expediente",new Vector2(0,281),new Vector2(884,48),28,new Color(.40f,.87f,.9f),font);
            Box(transform,"Sombra de papel",new Vector2(3,18),new Vector2(940,424),new Color(.005f,.02f,.03f),frame);
            Box(transform,"Papel gris frío",new Vector2(0,24),new Vector2(934,424),new Color(.84f,.88f,.87f),frame);
            SheetTitle=Text(transform,"Título de hoja",new Vector2(0,189),new Vector2(864,58),34,new Color(.045f,.19f,.23f),font);
            SheetTitle.fontStyle=FontStyles.Bold;
            Body=Text(transform,"Contenido dinámico",new Vector2(0,36),new Vector2(864,242),36,new Color(.025f,.085f,.12f),font);
            Body.lineSpacing=6;
            Notice=Text(transform,"Límites de la consulta",new Vector2(0,-139),new Vector2(864,90),26,new Color(.16f,.28f,.31f),font);
            PageLabel=Text(transform,"Página y estado",new Vector2(0,-215),new Vector2(880,40),27,new Color(.61f,.86f,.88f),font);
            Previous=Button(transform,"ANTERIOR",new Vector2(-324,-276),new Vector2(288,78),DeskCommand.Previous,font,frame,fill);
            Primary=Button(transform,"MARCAR REVISIÓN",new Vector2(0,-276),new Vector2(328,78),DeskCommand.Mark,font,frame,fill);
            Next=Button(transform,"SIGUIENTE",new Vector2(324,-276),new Vector2(288,78),DeskCommand.Next,font,frame,fill);
            Close=Button(transform,"VOLVER A ESTACIÓN",new Vector2(0,-363),new Vector2(936,66),DeskCommand.Close,font,frame,fill);
            // Behind interactive colliders. Empty paper stops rays without activating anything.
            var blocker=gameObject.AddComponent<BoxCollider>();blocker.size=new Vector3(1000,800,4);blocker.center=new Vector3(0,0,15);
        }
        public void Bind(DeskInteractionCoordinator owner)
        {
            foreach(var action in GetComponentsInChildren<DeskActionTarget>(true)) {
                action.Owner=owner;var bank=action.GetComponent<BankInteractable>();bank.Manager=owner.Manager;
            }
        }
        public void Present(string heading,string context,string title,string body,string notice,string page)
        {
            Heading.text=heading;Context.text=context;SheetTitle.text=title;Body.text=body;Notice.text=notice;PageLabel.text=page;
        }
        public void ConfigurePrimary(DeskCommand command,string label,bool enabled)
        {
            Primary.Command=command;Primary.GetComponentInChildren<TMP_Text>().text=label;
            // Keep the collider identity stable even while unavailable: the shared dwell latch must survive a label/state change.
            Primary.GetComponent<Collider>().enabled=true;
            Primary.GetComponent<DocumentFocus>().SetNormalColor(enabled?new Color(.04f,.25f,.29f):new Color(.075f,.11f,.13f));
        }
        public static DeskActionTarget Button(Transform parent,string title,Vector2 at,Vector2 size,DeskCommand command,TMP_FontAsset font,Sprite frame,Sprite fill)
        {
            var background=Box(parent,title,at,size,new Color(.035f,.19f,.24f),frame);
            background.transform.localPosition+=Vector3.back*3;
            var label=Text(background.transform,"Texto",Vector2.zero,size-new Vector2(24,17),27,Color.white,font);label.text=title;label.alignment=TextAlignmentOptions.Center;label.fontStyle=FontStyles.Bold;
            var bank=background.gameObject.AddComponent<BankInteractable>();bank.Action=BankAction.DeskTool;
            var collider=background.gameObject.AddComponent<BoxCollider>();collider.size=new Vector3(size.x,size.y,12);
            var action=background.gameObject.AddComponent<DeskActionTarget>();action.Command=command;
            var progress=Box(background.transform,"Dwell real",new Vector2(0,-size.y/2+5),new Vector2(size.x-24,7),OperationsCenterStyle.Cyan,null);
            progress.sprite=fill;progress.type=Image.Type.Filled;progress.fillMethod=Image.FillMethod.Horizontal;progress.fillOrigin=0;progress.fillAmount=0;
            var focus=background.gameObject.AddComponent<DocumentFocus>();focus.Target=bank;focus.Background=background;focus.Progress=progress;
            return action;
        }
        static Image Box(Transform parent,string name,Vector2 point,Vector2 size,Color color,Sprite frame)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var rect=go.GetComponent<RectTransform>();rect.anchoredPosition=point;rect.sizeDelta=size;
            var image=go.GetComponent<Image>();image.color=color;image.raycastTarget=false;
            if(frame!=null){var rim=new GameObject("Borde",typeof(RectTransform),typeof(Image));rim.transform.SetParent(go.transform,false);rim.GetComponent<RectTransform>().sizeDelta=size;
                var edge=rim.GetComponent<Image>();edge.sprite=frame;edge.type=Image.Type.Sliced;edge.fillCenter=false;edge.color=new Color(.20f,.61f,.65f);edge.raycastTarget=false;}
            return image;
        }
        static TMP_Text Text(Transform parent,string name,Vector2 at,Vector2 size,float fontSize,Color color,TMP_FontAsset font)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
            var text=go.GetComponent<TMP_Text>();text.rectTransform.anchoredPosition=at;text.rectTransform.sizeDelta=size;text.font=font;text.fontSize=fontSize;
            text.color=color;text.richText=false;text.raycastTarget=false;text.textWrappingMode=TextWrappingModes.Normal;text.overflowMode=TextOverflowModes.Page;
            text.alignment=TextAlignmentOptions.MidlineLeft;return text;
        }
    }
}

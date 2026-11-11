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
        public DeskActionTarget Expand;
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
        // A second instance of the existing view is laid directly on the real paper.
        // The original landscape prefab remains intact for accessibility.
        public void ConfigureForPhysicalPaper()
        {
            var rt=GetComponent<RectTransform>();rt.sizeDelta=new Vector2(320,410);
            foreach(var name in new[]{"Marco institucional","Cabecera","Sombra de papel","Papel gris frÃ­o","Papel gris frío"}) {
                var decoration=transform.Find(name);if(decoration!=null)decoration.gameObject.SetActive(false);
            }
            Layout(Heading,new Vector2(0,174),new Vector2(282,26),20);
            Layout(Context,new Vector2(0,147),new Vector2(282,23),16);
            Layout(SheetTitle,new Vector2(0,112),new Vector2(282,52),20);
            Layout(Body,new Vector2(0,-2),new Vector2(282,204),19);
            Body.lineSpacing=1;Body.overflowMode=TextOverflowModes.Page;
            Layout(Notice,new Vector2(0,-116),new Vector2(282,36),15);
            Layout(PageLabel,new Vector2(-55,-145),new Vector2(174,23),16);
            Heading.color=new Color(.06f,.14f,.17f);Context.color=new Color(.04f,.28f,.32f);
            SheetTitle.color=new Color(.04f,.18f,.22f);Body.color=new Color(.03f,.09f,.12f);
            Notice.color=new Color(.13f,.21f,.23f);PageLabel.color=new Color(.04f,.24f,.28f);
            Place(Previous,new Vector2(-126,-183),new Vector2(59,43),"‹");
            Place(Primary,new Vector2(-42,-183),new Vector2(105,43),"MARCAR");
            Place(Next,new Vector2(42,-183),new Vector2(59,43),"›");
            Place(Close,new Vector2(122,-183),new Vector2(72,43),"DEJAR");
            Expand=Button(transform,"AMPLIAR",new Vector2(108,-146),new Vector2(90,29),DeskCommand.Expand,Heading.font,null,Previous.GetComponent<DocumentFocus>().Progress.sprite);
            Place(Expand,new Vector2(108,-146),new Vector2(90,29),"AMPLIAR");
            var blocker=GetComponent<BoxCollider>();if(blocker!=null){blocker.size=new Vector3(320,410,2);blocker.center=new Vector3(0,0,8);}
            var canvas=GetComponent<Canvas>();canvas.sortingOrder=31;
        }
        public void PolishPhysicalControls()
        {
            Layout(Body,new Vector2(0,3),new Vector2(282,190),19);
            Layout(Notice,new Vector2(0,-111),new Vector2(282,34),15);
            Layout(PageLabel,new Vector2(-54,-140),new Vector2(175,23),16);
            Place(Expand,new Vector2(109,-140),new Vector2(92,27),"AMPLIAR");
            Place(Previous,new Vector2(-78,-175),new Vector2(140,34),"ANTERIOR");
            Place(Next,new Vector2(78,-175),new Vector2(140,34),"SIGUIENTE");
            Place(Primary,new Vector2(-78,-214),new Vector2(140,34),"MARCAR");
            Place(Close,new Vector2(78,-214),new Vector2(140,34),"DEJAR");
            foreach(var action in new[]{Previous,Next,Primary,Close,Expand}){
                var background=action.GetComponent<Image>();background.color=new Color(.11f,.24f,.27f);
                var label=action.GetComponentInChildren<TMP_Text>();label.color=new Color(.92f,.94f,.89f);
            }
        }
        static void Layout(TMP_Text label,Vector2 at,Vector2 size,float fontSize)
        {label.rectTransform.anchoredPosition=at;label.rectTransform.sizeDelta=size;label.fontSize=fontSize;}
        static void Place(DeskActionTarget action,Vector2 at,Vector2 size,string label)
        {
            var rt=action.GetComponent<RectTransform>();rt.anchoredPosition=at;rt.sizeDelta=size;
            var box=action.GetComponent<BoxCollider>();box.size=new Vector3(size.x,size.y,12);
            var text=action.GetComponentInChildren<TMP_Text>();text.text=label;Layout(text,Vector2.zero,size-new Vector2(8,6),17);
            var progress=action.GetComponent<DocumentFocus>().Progress;
            progress.rectTransform.anchoredPosition=new Vector2(0,-size.y/2+4);
            progress.rectTransform.sizeDelta=new Vector2(size.x-10,5);
            foreach(var child in action.GetComponentsInChildren<Image>(true))
                if(child.name=="Borde")child.gameObject.SetActive(false);
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

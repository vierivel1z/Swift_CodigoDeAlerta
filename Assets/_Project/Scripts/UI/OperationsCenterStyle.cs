using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Swift.Interaction;
namespace Swift.UI
{
    // Rebuilt native UI: no flattened screens, generated alpha or stretched crops.
    public static class OperationsCenterStyle
    {
        public static readonly Color Navy = new Color(.025f,.065f,.10f,1);
        public static readonly Color Cyan = new Color(.12f,.74f,.81f,1);
        public static Image Panel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            var found = parent.Find(name);
            var obj = found != null ? found.gameObject : new GameObject(name,typeof(RectTransform),typeof(Image));
            if (found == null) obj.transform.SetParent(parent,false);
            var rect = obj.GetComponent<RectTransform>(); rect.anchoredPosition = position; rect.sizeDelta = size;
            var image = obj.GetComponent<Image>(); image.color = color; image.raycastTarget = false;
            return image;
        }
        public static void Apply(UIManager owner, TextMeshProUGUI dossier, TextMeshProUGUI feedback, TextMeshProUGUI summary, bool finished)
        {
            if (!owner.OperationsCenterStyle || dossier == null) return;
            var root = dossier.transform.parent;
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(1100,800);
            var backdrop = root.Find("Fondo de lectura");
            if (backdrop != null) { backdrop.GetComponent<Image>().color = Navy; backdrop.GetComponent<RectTransform>().sizeDelta = new Vector2(1100,800); }
            if (root.Find("OC Chrome") == null)
            {
                var chrome = new GameObject("OC Chrome",typeof(RectTransform)); chrome.transform.SetParent(root,false); chrome.transform.SetSiblingIndex(1);
                Panel(chrome.transform,"Top border",new Vector2(0,398),new Vector2(1100,4),Cyan);
                Panel(chrome.transform,"Bottom border",new Vector2(0,-398),new Vector2(1100,4),Cyan);
                Panel(chrome.transform,"Left border",new Vector2(-548,0),new Vector2(4,800),Cyan);
                Panel(chrome.transform,"Right border",new Vector2(548,0),new Vector2(4,800),Cyan);
                Panel(chrome.transform,"Header",new Vector2(0,350),new Vector2(1090,78),new Color(.035f,.15f,.21f));
                Panel(chrome.transform,"Document well",new Vector2(0,100),new Vector2(1040,400),new Color(.07f,.14f,.20f));
                Panel(chrome.transform,"Feedback well",new Vector2(0,-235),new Vector2(1040,80),new Color(.045f,.105f,.15f));
                var header=owner.Text(chrome.transform,"Centro de operaciones",new Vector2(0,350),new Vector2(1030,50),29);
                header.text="SWIFT  /  CENTRO DE OPERACIONES             EXPEDIENTES"; header.color=Cyan; header.alignment=TextAlignmentOptions.MidlineLeft;
            }
            dossier.rectTransform.anchoredPosition = new Vector2(0,finished?180:100);
            dossier.rectTransform.sizeDelta = new Vector2(990,finished?230:380); dossier.fontSize=28;
            feedback.rectTransform.anchoredPosition=new Vector2(0,-235); feedback.rectTransform.sizeDelta=new Vector2(990,75); feedback.fontSize=24;
            summary.rectTransform.anchoredPosition=new Vector2(0,finished?-150:-332);
            summary.rectTransform.sizeDelta=new Vector2(990,finished?400:100); summary.fontSize=finished?23:22;
            root.Find("OC Chrome/Document well").gameObject.SetActive(!finished);
            root.Find("OC Chrome/Feedback well").gameObject.SetActive(!finished);
            foreach (var target in root.GetComponentsInChildren<BankInteractable>(true))
            {
                int index=target.Action==BankAction.Approve?0:target.Action==BankAction.Hold?1:target.Action==BankAction.Return?2:-1;
                if(index<0) continue;
                var rt=target.GetComponent<RectTransform>(); rt.anchoredPosition=new Vector2(-350+350*index,-150); rt.sizeDelta=new Vector2(310,82);
                var text=target.GetComponent<TextMeshProUGUI>(); text.fontSize=30; text.color=Color.white;
                var box=target.GetComponent<BoxCollider>(); if(box!=null) box.size=new Vector3(310,82,12);
                var color=index==0?new Color(.025f,.35f,.32f):index==1?new Color(.48f,.28f,.045f):new Color(.035f,.24f,.43f);
                var bg=Panel(root,"OC Button "+index,rt.anchoredPosition,new Vector2(322,88),color);
                bg.transform.SetSiblingIndex(2); bg.gameObject.SetActive(target.gameObject.activeSelf);
            }
        }
    }
}

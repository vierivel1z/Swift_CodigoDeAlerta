using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Swift.Interaction;
using UnityEngine.UI;
namespace Swift.Game
{
    // Stable world-space presentation. The player's camera is never animated or reparented.
    public sealed class IntroSequence : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }
        public bool HasPlayed { get; private set; }
        public float ShotSeconds = 4f;
        public Texture2D[] Illustrations = new Texture2D[3];
        public Rect[] IllustrationUV = { new Rect(0,0,1,1), new Rect(0,0,1,1), new Rect(0,0,1,1) };
        public int CurrentMoment { get; private set; } = -1;
        public RawImage Illustration { get; private set; }
        GameObject stage;
        Action completed;
        public void Play(Action onComplete)
        {
            if (IsPlaying) return;
            var camera = Camera.main;
            if (camera == null) { HasPlayed = true; onComplete?.Invoke(); return; }
            completed = onComplete; IsPlaying = true;
            stage = new GameObject("Prologo - Banco Andino (recreacion)");
            // Fixed in world space: head tracking stays free in Cardboard.
            stage.transform.SetPositionAndRotation(camera.transform.position + camera.transform.forward * 2.5f,
                Quaternion.LookRotation(camera.transform.forward, Vector3.up));
            StartCoroutine(Sequence());
        }
        IEnumerator Sequence()
        {
            var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            var canvasObject = new GameObject("Storyboard", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(stage.transform, false);
            canvasObject.transform.localScale = Vector3.one * .002f;
            var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.WorldSpace;
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 800);
            var background = new GameObject("Fondo", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            background.transform.SetParent(canvasObject.transform, false);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(1200,800);
            background.GetComponent<UnityEngine.UI.Image>().color = new Color(.015f,.025f,.045f);
            background.GetComponent<Image>().raycastTarget=false;
            var title = Label(canvasObject.transform, font, "BANCO ANDINO - BANCO DE ORIGEN", 336, 34, 1060, 90);
            var caption = Label(canvasObject.transform, font, "", -219, 30, 1080, 106);
            var pictureFrame = new GameObject("Marco de ilustracion",typeof(RectTransform),typeof(Image));pictureFrame.transform.SetParent(canvasObject.transform,false);
            pictureFrame.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,45);pictureFrame.GetComponent<RectTransform>().sizeDelta=new Vector2(1140,394);
            pictureFrame.GetComponent<Image>().color=new Color(.035f,.09f,.13f);pictureFrame.GetComponent<Image>().raycastTarget=false;
            var picture=new GameObject("Ilustracion - proporcion original",typeof(RectTransform),typeof(RawImage));picture.transform.SetParent(pictureFrame.transform,false);
            Illustration=picture.GetComponent<RawImage>();Illustration.raycastTarget=false;
            var skipBackground=new GameObject("Control omitir",typeof(RectTransform),typeof(Image));skipBackground.transform.SetParent(canvasObject.transform,false);
            skipBackground.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-330);skipBackground.GetComponent<RectTransform>().sizeDelta=new Vector2(1060,76);
            skipBackground.GetComponent<Image>().color=new Color(.035f,.26f,.30f);skipBackground.GetComponent<Image>().raycastTarget=false;
            var skip = Label(skipBackground.transform, font, "OMITIR INTRODUCCION  /  MIRADA 1,5 s", 0, 31, 1030, 70);
            skip.gameObject.AddComponent<BoxCollider>().size = new Vector3(1030,70,10);
            var action = skip.gameObject.AddComponent<BankInteractable>();
            action.Manager = GetComponent<GameManager>(); action.Action = BankAction.SkipIntro;
            var captions = new[] {
                "RECREACION EDUCATIVA - 5 de febrero de 2016\nUn empleado del banco de origen revisa la impresora de mensajes.",
                "La impresora no responde. Los comprobantes no aparecen.\nEsta escena no representa el puesto del jugador.",
                "Ahora: Banco Corresponsal Internacional (ficticio).\nTu tarea sera contrastar expedientes, registrar evidencias y decidir."
            };
            for (int shot = 0; shot < captions.Length; shot++) {
                CurrentMoment=shot;
                caption.text = captions[shot];
                title.text = (shot+1)+" / 3  -  "+(shot == 2 ? "BANCO CORRESPONSAL INTERNACIONAL\nSWIFT: CODIGO DE ALERTA" : "BANCO ANDINO - BANCO DE ORIGEN\nRECREACION EDUCATIVA");
                var texture=Illustrations!=null && Illustrations.Length>shot?Illustrations[shot]:null;
                var uv=IllustrationUV!=null && IllustrationUV.Length>shot?IllustrationUV[shot]:new Rect(0,0,1,1);
                Illustration.texture=texture;Illustration.uvRect=uv;Illustration.enabled=texture!=null;
                if(texture!=null) {
                    float aspect=(texture.width*uv.width)/(texture.height*uv.height);
                    float width=Mathf.Min(1120,390*aspect);Illustration.rectTransform.sizeDelta=new Vector2(width,width/aspect);
                }
                float elapsed = 0;
                float duration=Mathf.Max(.1f,ShotSeconds);
                while (elapsed < duration) {
                    elapsed += Time.unscaledDeltaTime;
                    // Fade the artwork only; captions and skip control remain stable and readable.
                    float fade=Mathf.Min(.3f,duration*.25f);
                    float alpha=Mathf.SmoothStep(0,1,Mathf.Min(elapsed/fade,(duration-elapsed)/fade));
                    Illustration.color=new Color(1,1,1,alpha);
                    yield return null;
                }
            }
            Skip();
        }
        static TextMeshProUGUI Label(Transform parent, TMP_FontAsset font, string value, float y, float size,float width,float height)
        {
            var obj = new GameObject("Texto", typeof(RectTransform)); obj.transform.SetParent(parent,false);
            var rect = obj.GetComponent<RectTransform>(); rect.anchoredPosition = new Vector2(0,y); rect.sizeDelta = new Vector2(width,height);
            var text = obj.AddComponent<TextMeshProUGUI>(); text.font = font; text.fontSize = size;
            text.alignment = TextAlignmentOptions.Center; text.text = value; text.raycastTarget = false;
            return text;
        }
        public void Skip()
        {
            if (!IsPlaying) return;
            StopAllCoroutines(); Cleanup(); HasPlayed = true;
            var callback = completed; completed = null; callback?.Invoke();
        }
        void Cleanup() { IsPlaying = false; CurrentMoment=-1; Illustration=null; if(stage != null) { stage.SetActive(false); Destroy(stage); } }
        void OnDisable() { StopAllCoroutines(); completed = null; Cleanup(); }
    }
}

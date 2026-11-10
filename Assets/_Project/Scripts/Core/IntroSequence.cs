using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Swift.Interaction;
using UnityEngine.UI;
namespace Swift.Game
{
    // Near-field world-space presentation follows the viewer without moving the camera.
    public sealed class IntroSequence : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }
        public bool HasPlayed { get; private set; }
        public float ShotSeconds = 4f;
        public Texture2D[] Illustrations = new Texture2D[3];
        public Rect[] IllustrationUV = { new Rect(0,0,1,1), new Rect(0,0,1,1), new Rect(0,0,1,1) };
        public int CurrentMoment { get; private set; } = -1;
        public RawImage Illustration { get; private set; }
        public Transform PresentationRoot => stage != null ? stage.transform : null;
        public float ViewingDistance { get; private set; }
        GameObject stage;
        Camera viewer;
        Transform storyboard;
        Quaternion facing;
        bool recentering;
        readonly RaycastHit[] obstructionHits = new RaycastHit[32];
        const float PreferredDistance = 1.1f;
        const float UiScalePerMetre = .00095f;
        public Sprite ModalFrame;
        const float RecenterAngle = 14f;
        Action completed;
        public void Play(Action onComplete)
        {
            if (IsPlaying) return;
            var camera = Camera.main;
            if (camera == null) { HasPlayed = true; onComplete?.Invoke(); return; }
            completed = onComplete; IsPlaying = true;
            stage = new GameObject("Prologo - Banco Andino (recreacion)");
            var portal=FindFirstObjectByType<Swift.UI.ModeStationVisual>();
            if(portal!=null)stage.transform.SetParent(portal.transform,true);
            viewer = camera;
            facing = camera.transform.rotation;
            recentering = false;
            FollowViewer(false);
            StartCoroutine(Sequence());
        }
        void OnEnable() { Application.onBeforeRender += BeforeRender; }
        void LateUpdate() { FollowViewer(true); }
        void BeforeRender() { FollowViewer(false); }
        void FollowViewer(bool rotate)
        {
            if (!IsPlaying || stage == null || viewer == null) return;
            if (rotate) {
                float angle = Quaternion.Angle(facing, viewer.transform.rotation);
                if (angle > RecenterAngle) recentering = true;
                if (recentering) {
                    facing = Quaternion.Slerp(facing, viewer.transform.rotation, 1 - Mathf.Exp(-12 * Time.unscaledDeltaTime));
                    if (Quaternion.Angle(facing, viewer.transform.rotation) < .5f) recentering = false;
                }
            }
            // A small angular dead zone keeps the gaze-selectable skip button stable.
            // Translation and before-render placement follow the viewer, never the bank desk.
            float scale = Mathf.Min(UiScalePerMetre,
                Mathf.Tan(viewer.fieldOfView * .5f * Mathf.Deg2Rad) * Mathf.Max(.1f, viewer.aspect) * .88f / 680);
            var forward = facing * Vector3.forward;
            var right = facing * Vector3.right;
            var up = facing * Vector3.up;
            float distance = PreferredDistance;
            for (int i = 0; i < 5; i++) {
                float x = i == 0 ? 0 : (i % 2 == 0 ? 1 : -1) * 680 * scale;
                float y = i == 0 ? 0 : (i > 2 ? 1 : -1) * 350 * scale;
                var direction = forward + right * x + up * y;
                float length = direction.magnitude;
                int count = Physics.RaycastNonAlloc(viewer.transform.position, direction / length,
                    obstructionHits, PreferredDistance * length, ~0, QueryTriggerInteraction.Ignore);
                for (int hit = 0; hit < count; hit++) {
                    var collider = obstructionHits[hit].collider;
                    if (collider == null || collider.transform.IsChildOf(stage.transform) || collider.transform.IsChildOf(viewer.transform)) continue;
                    distance = Mathf.Min(distance, obstructionHits[hit].distance / length - .10f);
                }
            }
            ViewingDistance = Mathf.Clamp(distance, viewer.nearClipPlane + .06f, PreferredDistance);
            stage.transform.SetPositionAndRotation(viewer.transform.position + forward * ViewingDistance, facing);
            if (storyboard != null) storyboard.localScale = Vector3.one * (scale * ViewingDistance);
        }
        IEnumerator Sequence()
        {
            var font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            var canvasObject = new GameObject("Storyboard", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(stage.transform, false);
            storyboard = canvasObject.transform;
            FollowViewer(false);
            var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.WorldSpace;canvas.overrideSorting=true;canvas.sortingOrder=30;
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1360, 700);
            var background = new GameObject("Fondo", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            background.transform.SetParent(canvasObject.transform, false);
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(1360,700);
            background.GetComponent<UnityEngine.UI.Image>().color = new Color(.015f,.025f,.045f);
            background.GetComponent<Image>().raycastTarget=false;
            if(ModalFrame!=null){var frame=new GameObject("Marco institucional",typeof(RectTransform),typeof(Image));frame.transform.SetParent(canvasObject.transform,false);frame.GetComponent<RectTransform>().sizeDelta=new Vector2(1360,700);var rim=frame.GetComponent<Image>();rim.sprite=ModalFrame;rim.type=Image.Type.Sliced;rim.fillCenter=false;rim.color=new Color(.18f,.84f,.96f);rim.raycastTarget=false;}
            var title = Label(canvasObject.transform, font, "BANCO ANDINO - BANCO DE ORIGEN", 280, 42, 1280, 110);
            var caption = Label(canvasObject.transform, font, "", -100, 40, 1280, 110);
            var pictureFrame = new GameObject("Marco de ilustracion",typeof(RectTransform),typeof(Image));pictureFrame.transform.SetParent(canvasObject.transform,false);
            pictureFrame.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,100);pictureFrame.GetComponent<RectTransform>().sizeDelta=new Vector2(1280,240);
            pictureFrame.GetComponent<Image>().color=new Color(.035f,.09f,.13f);pictureFrame.GetComponent<Image>().raycastTarget=false;
            var picture=new GameObject("Ilustracion - proporcion original",typeof(RectTransform),typeof(RawImage));picture.transform.SetParent(pictureFrame.transform,false);
            Illustration=picture.GetComponent<RawImage>();Illustration.raycastTarget=false;
            var skipBackground=new GameObject("Control omitir",typeof(RectTransform),typeof(Image));skipBackground.transform.SetParent(canvasObject.transform,false);
            skipBackground.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-230);skipBackground.GetComponent<RectTransform>().sizeDelta=new Vector2(1260,100);
            skipBackground.GetComponent<Image>().color=new Color(.035f,.26f,.30f);skipBackground.GetComponent<Image>().raycastTarget=false;
            var skip = Label(skipBackground.transform, font, "OMITIR INTRODUCCION  /  MIRADA 1,5 s", 0, 38, 1240, 90);
            skip.gameObject.AddComponent<BoxCollider>().size = new Vector3(1240,90,10);
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
                    float width=Mathf.Min(1240,230*aspect);Illustration.rectTransform.sizeDelta=new Vector2(width,width/aspect);
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
            text.lineSpacing=4; text.alignment = TextAlignmentOptions.Center; text.text = value; text.raycastTarget = false;
            return text;
        }
        public void Skip()
        {
            if (!IsPlaying) return;
            StopAllCoroutines(); Cleanup(); HasPlayed = true;
            var callback = completed; completed = null; callback?.Invoke();
        }
        void Cleanup() { IsPlaying = false; CurrentMoment=-1; Illustration=null; storyboard=null; viewer=null; ViewingDistance=0; if(stage != null) { stage.SetActive(false); Destroy(stage); } }
        void OnDisable() { Application.onBeforeRender -= BeforeRender; StopAllCoroutines(); completed = null; Cleanup(); }
    }
}

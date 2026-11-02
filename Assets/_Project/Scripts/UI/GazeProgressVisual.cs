using UnityEngine;
using TMPro;
using Swift.Interaction;
namespace Swift.UI
{
    // Presentation only: meshes/canvas have no colliders; the interactor owns timing and activation.
    public sealed class GazeProgressVisual : MonoBehaviour
    {
        public enum VisualState { Idle, Focus, Filling, Confirmed, Rearm }
        public VisualState State { get; private set; }
        public float DisplayedProgress { get; private set; }
        public bool ConfirmationVisible => gaze != null && Time.unscaledTime-gaze.LastActivationTime < .28f;
        VRInteractionManager gaze;
        Transform stage;
        Renderer center;
        Material originalCenter, centerMaterial, activeMaterial, darkMaterial, whiteMaterial;
        Mesh arc;
        GameObject track, border, arcObject;
        TextMeshProUGUI hint;
        readonly Vector3[] arcVertices = new Vector3[130];
        readonly System.Collections.Generic.List<Mesh> meshes = new System.Collections.Generic.List<Mesh>();
        float previousProgress=-1;
        void Start()
        {
            gaze=GetComponent<VRInteractionManager>();
            if(gaze.Reticle==null || (center=gaze.Reticle.GetComponent<Renderer>())==null) { enabled=false; return; }
            originalCenter=center.sharedMaterial;
            centerMaterial=new Material(originalCenter);centerMaterial.SetColor("_BaseColor",Color.white);center.sharedMaterial=centerMaterial;
            activeMaterial=new Material(originalCenter);darkMaterial=new Material(originalCenter);whiteMaterial=new Material(originalCenter);
            darkMaterial.SetColor("_BaseColor",new Color(.008f,.012f,.018f));whiteMaterial.SetColor("_BaseColor",Color.white);
            stage=new GameObject("Mirada - contraste, progreso y rearme").transform;stage.SetParent(gaze.Reticle,false);
            Ring("Contorno del punto",darkMaterial,.42f,.73f,-.08f,1);
            track=Ring("Base de progreso",darkMaterial,.94f,1.32f,-.10f,1);
            border=Ring("Contraste exterior",whiteMaterial,1.32f,1.40f,-.10f,1);
            arcObject=Ring("Progreso",activeMaterial,1,1.25f,-.14f,0);arc=arcObject.GetComponent<MeshFilter>().sharedMesh;
            var canvas=new GameObject("Estado de seleccion",typeof(RectTransform),typeof(Canvas));canvas.transform.SetParent(stage,false);
            canvas.transform.localPosition=new Vector3(0,-3.8f,-.75f);canvas.transform.localScale=Vector3.one*.08f;
            canvas.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            var rt=canvas.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(360,50);
            hint=canvas.AddComponent<TextMeshProUGUI>();hint.font=Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            hint.fontSize=28;hint.alignment=TextAlignmentOptions.Center;hint.raycastTarget=false;hint.richText=false;
            hint.fontMaterial.SetColor("_OutlineColor",Color.black);hint.fontMaterial.SetFloat("_OutlineWidth",.2f);
            hint.fontMaterial.EnableKeyword("OUTLINE_ON");
        }
        GameObject Ring(string label,Material material,float inner,float outer,float depth,float progress)
        {
            var obj=new GameObject(label,typeof(MeshFilter),typeof(MeshRenderer));obj.transform.SetParent(stage,false);obj.transform.localPosition=new Vector3(0,0,depth);
            var mesh=new Mesh {name=label};meshes.Add(mesh);obj.GetComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=obj.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;renderer.receiveShadows=false;
            var vertices=new Vector3[130];var triangles=new int[384];
            for(int i=0;i<=64;i++) {
                float a=(90-360*progress*i/64)*Mathf.Deg2Rad;var v=new Vector3(Mathf.Cos(a),Mathf.Sin(a),0);vertices[2*i]=v*inner;vertices[2*i+1]=v*outer;
                if(i<64) {int j=i*6,k=i*2;triangles[j]=k;triangles[j+1]=k+1;triangles[j+2]=k+2;triangles[j+3]=k+1;triangles[j+4]=k+3;triangles[j+5]=k+2;}
            }
            mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateBounds();return obj;
        }
        void LateUpdate()
        {
            if(stage==null||gaze.View==null)return;
            stage.rotation=gaze.View.transform.rotation;
            bool focused=gaze.FocusedTarget!=null;
            State=ConfirmationVisible?VisualState.Confirmed:focused&&gaze.IsGazeInput&&gaze.SelectionLatched?VisualState.Rearm:
                focused?(gaze.IsGazeInput?VisualState.Filling:VisualState.Focus):VisualState.Idle;
            bool visible=State!=VisualState.Idle;
            track.SetActive(visible);border.SetActive(visible);arcObject.SetActive(visible&&State!=VisualState.Focus);hint.gameObject.SetActive(visible);
            DisplayedProgress=focused&&gaze.IsGazeInput?gaze.DwellProgress:0;
            float progress=State==VisualState.Confirmed||State==VisualState.Rearm?1:DisplayedProgress;
            var color=State==VisualState.Confirmed?new Color(.25f,1,.55f):State==VisualState.Rearm?new Color(1,.76f,.22f):new Color(.15f,.95f,1);
            activeMaterial.SetColor("_BaseColor",color);
            hint.color=State==VisualState.Rearm?new Color(1,.87f,.5f):Color.white;
            hint.text=State==VisualState.Confirmed?"SELECCION CONFIRMADA":State==VisualState.Rearm?"APARTA LA MIRADA":gaze.IsGazeInput?"MANTEN LA MIRADA":"CLIC PARA SELECCIONAR";
            if(visible&&!Mathf.Approximately(previousProgress,progress)) {
                for(int i=0;i<=64;i++) {float a=(90-360*progress*i/64)*Mathf.Deg2Rad;var v=new Vector3(Mathf.Cos(a),Mathf.Sin(a),0);arcVertices[2*i]=v;arcVertices[2*i+1]=v*1.25f;}
                arc.vertices=arcVertices;arc.RecalculateBounds();previousProgress=progress;
            }
        }
        void OnDestroy()
        {
            if(center!=null)center.sharedMaterial=originalCenter;
            if(stage!=null)Destroy(stage.gameObject);
            foreach(var mesh in meshes)Destroy(mesh);
            Destroy(centerMaterial);Destroy(activeMaterial);Destroy(darkMaterial);Destroy(whiteMaterial);
        }
    }
}

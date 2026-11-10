using UnityEngine;
using UnityEngine.UI;

namespace Swift.UI
{
    // Native UI mesh: a few coloured vertices, default UI shader, no texture or lights.
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("")]
    public sealed class PortalPanelGraphic : Image
    {
        protected override void OnEnable() { base.OnEnable(); useLegacyMeshGeneration=false; }
        public override Texture mainTexture => Texture2D.whiteTexture;
        public float CornerCut = 36;
        public Color Top = new Color(.035f, .10f, .15f);
        public Color Bottom = new Color(.014f, .032f, .055f);
        public bool Triangle;
        public bool CornerFacet;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = GetPixelAdjustedRect();
            float c = Mathf.Min(CornerCut, Mathf.Min(r.width, r.height) * .45f);
            Vector2[] points = CornerFacet ? new[] {new Vector2(r.xMin,r.yMin),new Vector2(r.xMax,r.yMax),new Vector2(r.xMin+c,r.yMax),new Vector2(r.xMin,r.yMax-c)} : Triangle
                ? new[] { new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMin) }
                : new[] { new Vector2(r.xMin+c,r.yMin), new Vector2(r.xMax-c,r.yMin),
                    new Vector2(r.xMax,r.yMin+c),new Vector2(r.xMax,r.yMax-c),
                    new Vector2(r.xMax-c,r.yMax),new Vector2(r.xMin+c,r.yMax),
                    new Vector2(r.xMin,r.yMax-c),new Vector2(r.xMin,r.yMin+c) };
            var centre=Vector2.zero;foreach(var point in points)centre+=point;centre/=points.Length;
            vh.AddVert(centre, Color.Lerp(Bottom, Top, .5f) * color, new Vector2(.5f,.5f));
            foreach (var p in points)
                vh.AddVert(p, Color.Lerp(Bottom, Top, Mathf.InverseLerp(r.yMin, r.yMax, p.y)) * color, new Vector2(.5f,.5f));
            for (int i = 0; i < points.Length; i++) vh.AddTriangle(0, i+1, (i+1)%points.Length+1);
        }
    }
}

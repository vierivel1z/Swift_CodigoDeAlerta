using UnityEngine;
using UnityEngine.UI;

namespace Swift.UI
{
    // Bounded vector icons, sharing the standard UI material and canvas batch.
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("")]
    public sealed class PortalIconGraphic : Image
    {
        protected override void OnEnable() { base.OnEnable(); useLegacyMeshGeneration=false; }
        public override Texture mainTexture => Texture2D.whiteTexture;
        public enum Symbol { Bank, Shield, Eye }
        public Symbol Icon;
        VertexHelper vertices;
        Rect bounds;
        Vector2 Point(float x, float y) => new Vector2(bounds.center.x+x*bounds.width, bounds.center.y+y*bounds.height);
        void Quad(float left,float bottom,float right,float top,Color tint)
        {
            int i=vertices.currentVertCount;
            vertices.AddVert(Point(left,bottom),tint,new Vector2(.5f,.5f));vertices.AddVert(Point(left,top),tint,new Vector2(.5f,.5f));
            vertices.AddVert(Point(right,top),tint,new Vector2(.5f,.5f));vertices.AddVert(Point(right,bottom),tint,new Vector2(.5f,.5f));
            vertices.AddTriangle(i,i+1,i+2);vertices.AddTriangle(i,i+2,i+3);
        }
        void Line(Vector2 a,Vector2 b,float width,Color tint)
        {
            a=Point(a.x,a.y);b=Point(b.x,b.y);
            Vector2 normal=new Vector2(-(b-a).y,(b-a).x).normalized*width;
            int i=vertices.currentVertCount;
            vertices.AddVert(a-normal,tint,new Vector2(.5f,.5f));vertices.AddVert(a+normal,tint,new Vector2(.5f,.5f));
            vertices.AddVert(b+normal,tint,new Vector2(.5f,.5f));vertices.AddVert(b-normal,tint,new Vector2(.5f,.5f));
            vertices.AddTriangle(i,i+1,i+2);vertices.AddTriangle(i,i+2,i+3);
        }
        void Path(Vector2[] points,float width,Color tint)
        {
            bool closed=points.Length>2&&points[0]==points[points.Length-1];int count=closed?points.Length-1:points.Length;int first=vertices.currentVertCount;
            for(int i=0;i<count;i++){
                var current=Point(points[i].x,points[i].y);var prior=Point(points[(i-1+count)%count].x,points[(i-1+count)%count].y);var next=Point(points[(i+1)%count].x,points[(i+1)%count].y);
                var before=(current-prior).normalized;var after=(next-current).normalized;if(!closed&&i==0)before=after;if(!closed&&i==count-1)after=before;
                var normal=new Vector2(-before.y,before.x)+new Vector2(-after.y,after.x);normal.Normalize();float divisor=Mathf.Max(.4f,Vector2.Dot(normal,new Vector2(-after.y,after.x)));var offset=normal*(width/divisor);
                vertices.AddVert(current-offset,tint,new Vector2(.5f,.5f));vertices.AddVert(current+offset,tint,new Vector2(.5f,.5f));
            }
            for(int i=0;i<(closed?count:count-1);i++){int a=first+i*2,b=first+((i+1)%count)*2;vertices.AddTriangle(a,a+1,b+1);vertices.AddTriangle(a,b+1,b);}
        }
        void Fill(Vector2[] polygon,Color tint){int i=vertices.currentVertCount;foreach(var p in polygon)vertices.AddVert(Point(p.x,p.y),tint,new Vector2(.5f,.5f));for(int k=1;k<polygon.Length-1;k++)vertices.AddTriangle(i,i+k,i+k+1);}
        void Disk(Vector2 centre,float rx,float ry,Color tint){var points=new Vector2[24];for(int k=0;k<24;k++){float a=k*Mathf.PI*2/24;points[k]=centre+new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry);}Fill(points,tint);}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();vertices=vh;bounds=GetPixelAdjustedRect();
            Color bright=Color.Lerp(color,Color.white,.24f),dark=new Color(.025f,.08f,.11f,color.a);
            float w=Mathf.Min(bounds.width,bounds.height)*.023f;
            if(Icon==Symbol.Bank){
                Fill(new[]{new Vector2(-.46f,.20f),new Vector2(0,.48f),new Vector2(.46f,.20f)},color);
                Fill(new[]{new Vector2(-.28f,.25f),new Vector2(0,.41f),new Vector2(.28f,.25f)},dark);
                Path(new[]{new Vector2(-.31f,.26f),new Vector2(0,.43f),new Vector2(.31f,.26f)},w*.55f,bright);
                Quad(-.42f,.14f,.42f,.19f,color);
                for(int k=-1;k<=1;k++){
                    float x=k*.27f;
                    Quad(x-.060f,-.30f,x+.060f,.12f,color);
                    Quad(x-.065f,.10f,x+.065f,.15f,bright);
                    Quad(x-.060f,-.34f,x+.060f,-.29f,color);
                    Quad(x-.047f,-.28f,x-.028f,.09f,bright);
                }
                Quad(-.43f,-.41f,.43f,-.35f,color);Quad(-.48f,-.49f,.48f,-.43f,color);
                Quad(-.46f,-.44f,.46f,-.425f,bright);
            }else if(Icon==Symbol.Shield){
                var outline=new[]{new Vector2(0,.47f),new Vector2(.41f,.30f),new Vector2(.37f,-.13f),new Vector2(.24f,-.32f),new Vector2(0,-.49f),new Vector2(-.24f,-.32f),new Vector2(-.37f,-.13f),new Vector2(-.41f,.30f),new Vector2(0,.47f)};
                Path(outline,w*1.7f,color);
                Line(new Vector2(-.40f,.30f),new Vector2(0,.46f),w*.55f,bright);
                var arch=new Vector2[17];for(int k=0;k<=16;k++){float a=k*Mathf.PI/16;arch[k]=new Vector2(Mathf.Cos(a)*.14f,.03f+Mathf.Sin(a)*.18f);}
                Path(arch,w*1.6f,color);
                Quad(-.19f,-.25f,.19f,.03f,color);Quad(-.16f,-.04f,-.14f,.015f,bright);
                Disk(new Vector2(0,-.09f),.038f,.042f,dark);Quad(-.020f,-.20f,.020f,-.09f,dark);
            }else{
                var upper=new Vector2[33];var lower=new Vector2[33];var iris=new Vector2[33];
                for(int k=0;k<=32;k++){float x=-.46f+.92f*k/32;float y=.33f*Mathf.Sin(k*Mathf.PI/32);upper[k]=new Vector2(x,y);lower[k]=new Vector2(x,-y);float a=k*Mathf.PI*2/32;iris[k]=new Vector2(Mathf.Cos(a)*.13f,Mathf.Sin(a)*.22f);}
                iris[32]=iris[0];Path(upper,w*2,color);Path(lower,w*2,color);Path(iris,w*2.4f,color);
            }
        }
    }
}

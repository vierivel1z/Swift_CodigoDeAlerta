using UnityEngine;
using UnityEngine.UI;
namespace Swift.UI {
 [ExecuteAlways,RequireComponent(typeof(CanvasRenderer))]
 public sealed class StationIconGraphic:Image {
 public int Symbol;VertexHelper mesh;Rect bounds;
 public override Texture mainTexture=>Texture2D.whiteTexture;
 protected override void OnEnable(){base.OnEnable();useLegacyMeshGeneration=false;}
 Vector2 P(float x,float y)=>bounds.center+new Vector2(x*bounds.width,y*bounds.height);
 void Quad(float x,float y,float w,float h){int i=mesh.currentVertCount;foreach(var p in new[]{P(x,y),P(x,y+h),P(x+w,y+h),P(x+w,y)})mesh.AddVert(p,color,new Vector2(.5f,.5f));mesh.AddTriangle(i,i+1,i+2);mesh.AddTriangle(i,i+2,i+3);}
 void Line(float x,float y,float a,float b,float width=.055f){var p=P(x,y);var q=P(a,b);var d=q-p;var n=new Vector2(-d.y,d.x).normalized*(bounds.width*width/2);int i=mesh.currentVertCount;foreach(var v in new[]{p-n,p+n,q+n,q-n})mesh.AddVert(v,color,new Vector2(.5f,.5f));mesh.AddTriangle(i,i+1,i+2);mesh.AddTriangle(i,i+2,i+3);}
 void Outline(float x,float y,float w,float h){Line(x,y,x+w,y);Line(x+w,y,x+w,y+h);Line(x+w,y+h,x,y+h);Line(x,y+h,x,y);}
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();mesh=vh;bounds=GetPixelAdjustedRect();
 switch(Symbol){
 case 0:Outline(-.43f,-.30f,.86f,.55f);Quad(-.38f,.25f,.35f,.12f);break;
 case 1:for(int i=0;i<3;i++){float y=-.3f+i*.24f;Outline(-.36f,y,.72f,.20f);}break;
 case 2:Line(-.44f,-.36f,0,.43f);Line(0,.43f,.44f,-.36f);Line(.44f,-.36f,-.44f,-.36f);Quad(-.035f,-.08f,.07f,.28f);Quad(-.035f,-.25f,.07f,.07f);break;
 case 3:Outline(-.43f,-.33f,.86f,.68f);Line(0,-.32f,0,.35f);Line(-.34f,.18f,-.08f,.18f);Line(.08f,.18f,.34f,.18f);break;
 case 4:Line(-.4f,0,-.12f,-.26f,.13f);Line(-.12f,-.26f,.43f,.35f,.13f);break;
 case 5:Quad(-.28f,-.34f,.17f,.68f);Quad(.11f,-.34f,.17f,.68f);break;
 case 6:Line(.43f,-.05f,-.36f,-.05f,.11f);Line(-.36f,-.05f,-.08f,.25f,.11f);Line(-.36f,-.05f,-.08f,-.34f,.11f);break;
 case 8:Line(-.43f,.19f,0,.44f,.09f);Line(0,.44f,.43f,.19f,.09f);Quad(-.42f,.1f,.84f,.08f);for(int k=-1;k<=1;k++)Quad(k*.27f-.05f,-.30f,.1f,.40f);Quad(-.46f,-.42f,.92f,.09f);break;
 case 9:for(int k=0;k<24;k++){float a=k*Mathf.PI*2/24,b=(k+1)*Mathf.PI*2/24;Line(Mathf.Cos(a)*.17f,.20f+Mathf.Sin(a)*.17f,Mathf.Cos(b)*.17f,.20f+Mathf.Sin(b)*.17f,.07f);}Quad(-.30f,-.4f,.6f,.32f);break;
 case 10:Outline(-.44f,-.28f,.88f,.56f);Quad(-.39f,.08f,.78f,.08f);Quad(-.31f,-.17f,.27f,.07f);break;
 case 11:Outline(-.4f,-.36f,.8f,.67f);Quad(-.4f,.13f,.8f,.08f);for(int i=0;i<3;i++)Quad(-.27f+i*.22f,-.12f,.11f,.11f);break;
 default:Outline(-.44f,-.26f,.88f,.63f);Line(0,-.26f,0,-.42f);Line(-.23f,-.42f,.23f,-.42f);break;
 }
 }
 }
}

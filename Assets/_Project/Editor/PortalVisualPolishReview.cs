using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Swift.Interaction;
using Swift.UI;
namespace Swift.EditorTools {
[InitializeOnLoad] public static class PortalVisualPolishReview {
 const string Dir="Artifacts/PortalVisualPolish", Pending="SWIFT_PORTAL_VISUAL_POLISH";
 static IEnumerator routine;static int frame=-1,errors;static string run;
 static PortalVisualPolishReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
 static void Tick(){
 if(routine!=null){if(frame==Time.frameCount)return;frame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
 var q=Dir+"/command.txt";if(File.Exists(q)&&EditorApplication.isPlaying&&!EditorApplication.isCompiling){EditorApplication.ExitPlaymode();return;}if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
 run=File.ReadAllText(q).Trim();File.Delete(q);try{
 if(SceneManager.GetActiveScene().path!="Assets/_Project/Scenes/BancoCorresponsal.unity")throw new Exception("Open bank scene preserved; unexpected scene rejected");
 if(run=="apply"){Apply();Finish(null);return;}if(run=="finish"){FinalizeVisual();Finish(null);return;}if(run=="inspect"){Inspect();Finish(null);return;}if(run=="before"){Capture("00-before.png");Finish(null);return;}
 SessionState.SetString(Pending,run);EditorApplication.EnterPlaymode();}catch(Exception e){Finish(e);}}
 static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode)return;run=SessionState.GetString(Pending,"");if(run=="")return;SessionState.EraseString(Pending);errors=0;frame=-1;Application.logMessageReceived+=Log;routine=Probe();}
 static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert){errors++;File.AppendAllText(Dir+"/console.txt",m+"\n"+s);}}
 static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;if(e==null&&errors>0)e=new Exception("Runtime errors="+errors);File.WriteAllText(Dir+"/"+run+"-result.txt",(e==null?"PASS ":"FAIL ")+run+" "+DateTime.UtcNow.ToString("O")+" "+e);if(EditorApplication.isPlaying)EditorApplication.ExitPlaymode();}
 static void Check(bool ok,string label){File.AppendAllText(Dir+"/"+run+"-checks.txt",(ok?"PASS ":"FAIL ")+label+"\n");if(!ok)throw new Exception(label);}
 const string Gallery="Docs/Previews/PortalVisualPolish/20261103";
 const string AssetFolder="Assets/_Project/OfficeVisual/PortalOperations";
 static readonly Color Cyan=new Color(.18f,.84f,.96f),Amber=new Color(1,.65f,.23f),Navy=new Color(.012f,.029f,.049f);
 static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 point){var g=new GameObject(name,typeof(RectTransform));Undo.RegisterCreatedObjectUndo(g,"Polish portal visual");var r=g.GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=size;r.anchoredPosition=point;return r;}
 static Image Image(string name,Transform parent,Vector2 size,Vector2 point,Color tint,Sprite sprite=null){var r=Rect(name,parent,size,point);var i=r.gameObject.AddComponent<Image>();i.color=tint;i.raycastTarget=false;i.sprite=sprite;if(sprite!=null)i.type=UnityEngine.UI.Image.Type.Sliced;return i;}
 static PortalPanelGraphic Panel(string name,Transform parent,Vector2 size,Vector2 point,Color top,Color bottom,float cut=40){var r=Rect(name,parent,size,point);r.gameObject.AddComponent<CanvasRenderer>();var g=r.gameObject.AddComponent<PortalPanelGraphic>();g.raycastTarget=false;g.Top=top;g.Bottom=bottom;g.CornerCut=cut;return g;}
 static TMPro.TMP_Text Text(string name,Transform parent,string content,Vector2 size,Vector2 point,float font,Color tint){var r=Rect(name,parent,size,point);var t=r.gameObject.AddComponent<TMPro.TextMeshProUGUI>();t.text=content;t.fontSize=font;t.color=tint;t.fontStyle=TMPro.FontStyles.Normal;t.alignment=TMPro.TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
 static void Move(RectTransform r,Vector2 size,Vector2 point){Undo.RecordObject(r,"Polish UI layout");r.sizeDelta=size;r.anchoredPosition=point;}
 static PortalIconGraphic Icon(string name,Transform parent,PortalIconGraphic.Symbol symbol,Vector2 size,Vector2 point,Color tint){var r=Rect(name,parent,size,point);r.gameObject.AddComponent<CanvasRenderer>();var g=r.gameObject.AddComponent<PortalIconGraphic>();g.raycastTarget=false;g.Icon=symbol;g.color=tint;return g;}
 static void Line(Transform parent,Vector2 a,Vector2 b,float width,Color tint){var image=Image("Inlay",parent,new Vector2(Vector2.Distance(a,b),width),(a+b)*.5f,tint);image.rectTransform.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg);}
 static Sprite Sprite(string name,bool capsule){
 int width=capsule?128:256,height=capsule?64:256;var tex=new Texture2D(width,height,TextureFormat.RGBA32,false);var colors=new Color32[width*height];
 for(int y=0;y<height;y++)for(int x=0;x<width;x++){int samples=0;for(int sy=0;sy<4;sy++)for(int sx=0;sx<4;sx++){
 float px=x+(sx+.5f)/4,py=y+(sy+.5f)/4;bool inside;
 if(capsule){float cx=Mathf.Clamp(px,32,width-32);inside=(new Vector2(px-cx,py-32)).sqrMagnitude<=32*32;}
 else{inside=Chamfer(px,py,width,height,48);bool inner=px>=6&&px<=width-6&&py>=6&&py<=height-6&&Chamfer(px-6,py-6,width-12,height-12,44.5f);inside=inside&&!inner;}
 if(inside)samples++;}colors[y*width+x]=new Color32(255,255,255,(byte)(255*samples/16));}
 tex.SetPixels32(colors);tex.Apply();var path=AssetFolder+"/"+name+".png";File.WriteAllBytes(path,tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spriteBorder=Vector4.one*(capsule?32:52);importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.maxTextureSize=256;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(path);
 }
 static bool Chamfer(float x,float y,float w,float h,float c)=>x+y>=c&&w-x+y>=c&&x+h-y>=c&&w-x+h-y>=c;
 static void Apply(){
 var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();Check(station!=null,"Original portal controller found");
 var root=station.transform;var canvas=root.Find("WorldSpaceCanvas");Check(canvas!=null,"Original WorldSpaceCanvas found");
 var backup=File.ReadAllText(Dir+"/backup-path.txt").Trim();Check(Directory.Exists(backup),"External backup exists");
 var recovery="Assets/_Recovery/PortalVisualPolish_Before_"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff")+".unity";
 Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),recovery,true),"Unsaved current Editor scene copied before visual changes");AssetDatabase.ImportAsset(recovery);File.Copy(recovery,backup+"/EditorBefore.unity");File.Copy(recovery+".meta",backup+"/EditorBefore.unity.meta");
 var attack=station.Attack;var defense=station.Defense;var ap=station.AttackProgress;var dp=station.DefenseProgress;var ac=attack.GetComponent<BoxCollider>();var dc=defense.GetComponent<BoxCollider>();var asize=ac.size;var dsize=dc.size;var apos=attack.transform.position;var dpos=defense.transform.position;
 Undo.IncrementCurrentGroup();Undo.RecordObject(station,"Preserve portal bindings");
 var borderSprite=Sprite("UI_ChamferFrame_256",false);var capsuleSprite=Sprite("UI_Capsule_128",true);
 foreach(Transform child in canvas){if(child.name=="AttackCard"||child.name=="DefenseCard")continue;Undo.RecordObject(child.gameObject,"Preserve old portal art");child.gameObject.SetActive(false);}
 var chrome=Rect("PremiumChrome",canvas,new Vector2(2800,1180),Vector2.zero);chrome.SetAsFirstSibling();
 var shadow=Panel("Panel depth",chrome,new Vector2(2836,1216),new Vector2(0,-8),new Color(.002f,.008f,.014f),Color.black,55);
 var glow=Image("Controlled edge halo",chrome,new Vector2(2820,1200),Vector2.zero,new Color(.12f,.85f,1,.12f),borderSprite);glow.pixelsPerUnitMultiplier=.5f;
 Image("Cyan outer frame",chrome,new Vector2(2800,1180),Vector2.zero,Cyan*.8f,borderSprite);
 Panel("Inset navy face",chrome,new Vector2(2780,1160),Vector2.zero,new Color(.018f,.06f,.09f),Navy,40);
 var rim=Image("Inner bevel",chrome,new Vector2(2762,1142),Vector2.zero,new Color(.22f,.6f,.7f,.22f),borderSprite);rim.pixelsPerUnitMultiplier=1.6f;
 var header=Panel("Institutional header",chrome,new Vector2(2690,170),new Vector2(0,490),new Color(.025f,.09f,.13f),new Color(.009f,.035f,.062f),24).transform;
 var brand=Text("SWIFT brand",header,"SWIFT <color=#35DDF5>/</color>",new Vector2(510,130),new Vector2(-1050,0),96,Color.white);brand.fontStyle=TMPro.FontStyles.Bold|TMPro.FontStyles.Italic;brand.alignment=TMPro.TextAlignmentOptions.Left;
 var title=Text("Portal title",header,"PORTAL DE OPERACIONES",new Vector2(1290,110),new Vector2(-120,0),66,Color.white);title.alignment=TMPro.TextAlignmentOptions.Left;
 Line(header,new Vector2(500,-30),new Vector2(1040,-30),5,Cyan);for(int k=0;k<3;k++)Line(header,new Vector2(1055+k*25,-31),new Vector2(1080+k*25,9),7,new Color(.16f,.58f,.70f));
 Image("Institution seal rim",header,new Vector2(154,134),new Vector2(1230,0),Cyan*.8f,borderSprite);Panel("Institution seal inset",header,new Vector2(138,118),new Vector2(1230,0),Navy,Navy,20);
 Icon("Institution seal",header,PortalIconGraphic.Symbol.Bank,new Vector2(100,88),new Vector2(1230,0),Cyan);
 Line(chrome,new Vector2(-1290,402),new Vector2(1290,402),3,new Color(.12f,.57f,.66f));
 for(int n=0;n<2;n++){
 bool left=n==0;var tint=left?Amber:Cyan;var card=canvas.Find(left?"AttackCard":"DefenseCard");var progress=left?ap:dp;var focus=left?station.AttackBorder:station.DefenseBorder;var status=left?station.AttackStatus:station.DefenseStatus;
 foreach(Transform child in card){if(child==focus.transform||child==status.transform||child==progress.transform||child.name=="Title"||child.name=="Description")continue;Undo.RecordObject(child.gameObject,"Preserve original card artwork");child.gameObject.SetActive(false);}
 Undo.RecordObject(focus,"Style existing focus image");focus.sprite=borderSprite;focus.type=UnityEngine.UI.Image.Type.Sliced;focus.pixelsPerUnitMultiplier=.75f;focus.raycastTarget=false;Move(focus.rectTransform,new Vector2(1280,920),Vector2.zero);
 var backing=Panel("Layered card background",card,new Vector2(1264,904),Vector2.zero,left?new Color(.115f,.088f,.055f):new Color(.025f,.12f,.17f),new Color(.009f,.025f,.043f),53);backing.transform.SetAsFirstSibling();
 var triangle=Panel("Corner accent",backing.transform,new Vector2(290,235),new Vector2(-420,326),new Color(tint.r,tint.g,tint.b,.40f),new Color(tint.r,tint.g,tint.b,.04f));triangle.Triangle=true;
 var diagonal=Panel("Corner secondary facet",backing.transform,new Vector2(310,235),new Vector2(-250,326),new Color(tint.r,tint.g,tint.b,.11f),new Color(tint.r,tint.g,tint.b,.015f));diagonal.Triangle=true;
 Line(backing.transform,new Vector2(-540,440),new Vector2(490,440),2,new Color(tint.r,tint.g,tint.b,.38f));
 Icon(left?"Bank icon premium":"Shield lock premium",card,left?PortalIconGraphic.Symbol.Bank:PortalIconGraphic.Symbol.Shield,left?new Vector2(370,260):new Vector2(300,280),new Vector2(0,310),tint);
 var hero=card.Find("Title").GetComponent<TMPro.TMP_Text>();Undo.RecordObject(hero,"Hierarchy of mode title");hero.fontSize=118;hero.fontStyle=TMPro.FontStyles.Bold;hero.color=tint;Move(hero.rectTransform,new Vector2(1160,160),new Vector2(0,105));
 var desc=card.Find("Description").GetComponent<TMPro.TMP_Text>();Undo.RecordObject(desc,"Description spacing");desc.fontSize=62;desc.lineSpacing=5;desc.color=new Color(.90f,.94f,.96f);Move(desc.rectTransform,new Vector2(1140,230),new Vector2(0,-90));
 var module=Panel("Gaze module",card,new Vector2(1160,220),new Vector2(0,-330),new Color(.035f,.09f,.13f),new Color(.015f,.039f,.060f),26).transform;
 var moduleRim=Image("Module rim",module,new Vector2(1160,220),Vector2.zero,new Color(tint.r,tint.g,tint.b,.5f),borderSprite);moduleRim.pixelsPerUnitMultiplier=2;
 Icon("Eye",module,PortalIconGraphic.Symbol.Eye,new Vector2(130,75),new Vector2(-450,50),tint);
 Undo.RecordObject(status,"Gaze prompt weight");status.transform.SetParent(module,false);status.fontSize=49;status.fontStyle=TMPro.FontStyles.Bold;status.color=tint;Move(status.rectTransform,new Vector2(900,75),new Vector2(80,50));
 Image("Track rim",module,new Vector2(1080,66),new Vector2(0,-47),new Color(tint.r,tint.g,tint.b,.4f),capsuleSprite);
 var mask=Image("Rounded track mask",module,new Vector2(1060,48),new Vector2(0,-47),new Color(.07f,.16f,.22f),capsuleSprite);mask.gameObject.AddComponent<Mask>().showMaskGraphic=true;
 Undo.RecordObject(progress,"Preserve live dwell bar");progress.transform.SetParent(mask.transform,false);Move(progress.rectTransform,new Vector2(1060,48),Vector2.zero);progress.fillAmount=0;progress.color=tint;
 Image("Track reflection",mask.transform,new Vector2(1020,3),new Vector2(0,18),new Color(1,1,1,.12f));
 Check(progress.type==UnityEngine.UI.Image.Type.Filled&&progress.fillMethod==UnityEngine.UI.Image.FillMethod.Horizontal,"Original filled horizontal dwell image preserved");
 }
 Check(station.Attack==attack&&station.Defense==defense&&station.AttackProgress==ap&&station.DefenseProgress==dp,"Original interaction and progress object identities preserved");
 Check(ac.size==asize&&dc.size==dsize&&attack.transform.position==apos&&defense.transform.position==dpos,"Physical targets unchanged");
 foreach(var g in canvas.GetComponentsInChildren<Graphic>(true)){
 if(g is PortalPanelGraphic||g is PortalIconGraphic){if(g.GetComponent<CanvasRenderer>()==null)Undo.AddComponent<CanvasRenderer>(g.gameObject);g.enabled=false;g.enabled=true;g.SetAllDirty();}
 var image=g as Image;if(image!=null&&image.sprite!=null&&image.sprite.name=="UI_ChamferFrame_256")image.fillCenter=false;
 }
 Canvas.ForceUpdateCanvases();foreach(var t in canvas.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();Check(!t.isTextOverflowing,"Styled TMP fits "+t.name);}
 PrefabUtility.SaveAsPrefabAsset(root.gameObject,AssetFolder+"/PortalOperations.prefab");EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("01-front-polished.png");
 }
 static void Inspect(){var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();Canvas.ForceUpdateCanvases();var lines=station.GetComponentsInChildren<Graphic>(true).Where(g=>g is PortalPanelGraphic||g is PortalIconGraphic).Select(g=>g.name+" active="+g.gameObject.activeInHierarchy+" enabled="+g.enabled+" size="+g.rectTransform.rect+" color="+g.color+" renderer="+(g.GetComponent<CanvasRenderer>()!=null)+" cull="+g.canvasRenderer.cull+" depth="+g.canvasRenderer.absoluteDepth+" canvas="+(g.canvas!=null)+" shader="+g.materialForRendering.shader.name+" nativeTint="+g.canvasRenderer.GetColor()+" texture="+(g.mainTexture!=null?g.mainTexture.name:"NULL")+" vertices="+g.canvasRenderer.GetMesh().vertexCount+" vertex0="+g.canvasRenderer.GetMesh().colors[0]);File.WriteAllLines(Dir+"/graphic-inspection.txt",lines);}
 static void FinalizeVisual(){
 var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();var canvas=station.transform.Find("WorldSpaceCanvas");
 var badge=canvas.Find("PremiumChrome/Institutional header/Institution seal rim");if(badge!=null)badge.GetComponent<Image>().pixelsPerUnitMultiplier=2;
 foreach(var cardName in new[]{"AttackCard","DefenseCard"}){var t=canvas.Find(cardName+"/Title").GetComponent<TMPro.TMP_Text>();Move(t.rectTransform,new Vector2(1160,160),new Vector2(0,105));}
 foreach(var g in canvas.GetComponentsInChildren<Graphic>(true)){
 if(g is PortalPanelGraphic||g is PortalIconGraphic){if(g.GetComponent<CanvasRenderer>()==null)Undo.AddComponent<CanvasRenderer>(g.gameObject);g.enabled=false;g.enabled=true;g.SetAllDirty();}
 var image=g as Image;if(image!=null&&image.sprite!=null&&image.sprite.name=="UI_ChamferFrame_256")image.fillCenter=false;
 }
 Canvas.ForceUpdateCanvases();foreach(var t in canvas.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();Check(!t.isTextOverflowing,"Styled TMP fits "+t.name+" preferred="+t.preferredHeight+" rect="+t.rectTransform.rect.height);}
 PrefabUtility.SaveAsPrefabAsset(station.gameObject,AssetFolder+"/PortalOperations.prefab");EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("01-front-polished.png");
 }
 static void Capture(string name,bool initial=false){
 Directory.CreateDirectory(Gallery);var source=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>().View;var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();
 var go=new GameObject("Temporary portal polish capture");var camera=go.AddComponent<Camera>();camera.CopyFrom(source);camera.enabled=false;camera.tag="Untagged";camera.aspect=16f/9;
 var face=station.transform.parent;camera.transform.position=initial?source.transform.position:face.position-face.forward*1.65f;camera.transform.LookAt(face.position);
 var rt=new RenderTexture(1600,900,24);var previous=RenderTexture.active;Texture2D image=null;
 try{Canvas.ForceUpdateCanvases();rt.Create();UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(Gallery+"/"+name,image.EncodeToPNG());File.AppendAllText(Gallery+"/receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" Play="+Application.isPlaying+" eye="+camera.transform.position+" FOV="+camera.fieldOfView+" drawCalls(Editor aggregate)="+UnityEditor.UnityStats.drawCalls+" triangles="+UnityEditor.UnityStats.triangles+"\n");}
 finally{RenderTexture.active=previous;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(go);}}
 static IEnumerator Probe(){
 if(run!="review"){
 PortalOperationsReview.SuppressCaptures=true;IEnumerator original;
 if(run=="gaze")original=(IEnumerator)typeof(PortalOperationsReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,null);
 else if(run=="pc")original=(IEnumerator)typeof(PortalUniformFillReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,null);
 else original=(IEnumerator)typeof(UXGameplayReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,new object[]{run});
 try{while(original.MoveNext())yield return null;}finally{PortalOperationsReview.SuppressCaptures=false;if(run=="pc"){
 var field=typeof(PortalUniformFillReview).GetField("mouse",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);var testMouse=field.GetValue(null) as Mouse;if(testMouse!=null&&testMouse.added)InputSystem.RemoveDevice(testMouse);field.SetValue(null,null);
 var prior=typeof(PortalUniformFillReview).GetField("previousMouse",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null) as Mouse;if(prior!=null&&prior.added)prior.MakeCurrent();}}
 yield break;}
 yield return null;yield return null;
 var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();foreach(var adapter in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;
 gaze.SetEditorGazeSimulation(true);var view=gaze.View.transform;view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 Capture("02-play-normal.png");Capture("07-real-initial-position.png",true);
 int count=0;gaze.Activated+=t=>{if(t==station.Attack||t==station.Defense)count++;};
 for(int n=0;n<2;n++){
 var target=n==0?station.Attack:station.Defense;var bar=n==0?station.AttackProgress:station.DefenseProgress;var other=n==0?station.DefenseProgress:station.AttackProgress;
 view.LookAt(target.GetComponent<Collider>().bounds.center);float until=Time.realtimeSinceStartup+5;
 while(gaze.DwellProgress<.1f&&Time.realtimeSinceStartup<until)yield return null;
 Check(gaze.FocusedTarget==target,"Original raycast reaches focused card");Capture(n==0?"03-attack-focused.png":"05-defense-focused.png");
 while(gaze.DwellProgress<.49f&&Time.realtimeSinceStartup<until)yield return null;
 Check(Mathf.Abs(gaze.DwellProgress-bar.fillAmount)<.001f&&other.fillAmount==0&&count==0,"New rounded bar follows real dwell, other mode stays empty");Capture(n==0?"04-attack-partial.png":"06-defense-partial.png");
 File.AppendAllText(Dir+"/samples.txt",target.Action+"="+bar.fillAmount+" real="+gaze.DwellProgress+"\n");view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;Check(bar.fillAmount==0,"Cancellation still resets styled bar");}
 foreach(var text in station.transform.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Live TMP fits "+text.name);}
 Check(Mathf.Approximately(gaze.DwellSeconds,1.5f),"Existing 1.5 second dwell unchanged");
 }
}}

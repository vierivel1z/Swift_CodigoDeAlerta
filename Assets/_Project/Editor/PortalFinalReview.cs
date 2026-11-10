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
[InitializeOnLoad] public static class PortalFinalReview {
 const string Dir="Artifacts/PortalFinal", Pending="SWIFT_PORTAL_FINAL";
 static IEnumerator routine;static int frame=-1,errors;static string run;
 static PortalFinalReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
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
 const string Gallery="Docs/Previews/PortalFinalReview/20261103";
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
 var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();var canvas=station.transform.Find("WorldSpaceCanvas");
 var backup=File.ReadAllText(Dir+"/backup-path.txt").Trim();
 var recovery="Assets/_Recovery/PortalFinal_Before_"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff")+".unity";
 Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),recovery,true),"Unsaved Editor scene preserved");AssetDatabase.ImportAsset(recovery);File.Copy(recovery,backup+"/EditorBefore.unity");File.Copy(recovery+".meta",backup+"/EditorBefore.unity.meta");
 Capture("00-before.png");Undo.RecordObject(station,"Final portal presentation");
 var frame=AssetDatabase.LoadAssetAtPath<Sprite>(AssetFolder+"/UI_ChamferFrame_256.png");
 for(int n=0;n<2;n++){
 bool left=n==0;var tint=left?Amber:Cyan;var card=canvas.Find(left?"AttackCard":"DefenseCard");var background=card.Find("Layered card background");
 var facet=background.Find("Corner accent").GetComponent<PortalPanelGraphic>();Undo.RecordObject(facet,"Single angular accent");facet.Triangle=false;facet.CornerFacet=true;facet.CornerCut=53;facet.Top=new Color(tint.r,tint.g,tint.b,.65f);facet.Bottom=new Color(tint.r,tint.g,tint.b,.08f);Move(facet.rectTransform,new Vector2(450,260),new Vector2(-398,314));facet.SetAllDirty();
 background.Find("Corner secondary facet").gameObject.SetActive(false);
 var icon=card.Find(left?"Bank icon premium":"Shield lock premium").GetComponent<PortalIconGraphic>();Move(icon.rectTransform,left?new Vector2(380,270):new Vector2(320,290),new Vector2(0,300));icon.SetAllDirty();
 var wash=Panel("Live focus wash",card,new Vector2(1250,890),Vector2.zero,Color.white,Color.white,53);wash.transform.SetSiblingIndex(1);wash.color=Color.clear;
 var module=card.Find("Gaze module/Module rim").GetComponent<Image>();
 var edge=Image("Live selection edge",card,new Vector2(1130,7),new Vector2(0,-443),tint);edge.type=UnityEngine.UI.Image.Type.Filled;edge.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;edge.fillOrigin=0;edge.fillAmount=0;
 var reflection=card.Find("Gaze module/Rounded track mask/Track reflection");if(reflection!=null)reflection.gameObject.SetActive(false);
 if(left){station.AttackWash=wash;station.AttackModuleBorder=module;station.AttackSelectionEdge=edge;}else{station.DefenseWash=wash;station.DefenseModuleBorder=module;station.DefenseSelectionEdge=edge;}
 }
 var intro=UnityEngine.Object.FindFirstObjectByType<Swift.Game.IntroSequence>();Undo.RecordObject(intro,"Readable modal frame");intro.ModalFrame=frame;
 PrefabUtility.SaveAsPrefabAsset(station.gameObject,AssetFolder+"/PortalOperations.prefab");EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("01-front-improved.png");
 }
 static void Inspect(){var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();Canvas.ForceUpdateCanvases();var lines=station.GetComponentsInChildren<Graphic>(true).Where(g=>g is PortalPanelGraphic||g is PortalIconGraphic).Select(g=>g.name+" active="+g.gameObject.activeInHierarchy+" enabled="+g.enabled+" size="+g.rectTransform.rect+" color="+g.color+" renderer="+(g.GetComponent<CanvasRenderer>()!=null)+" cull="+g.canvasRenderer.cull+" depth="+g.canvasRenderer.absoluteDepth+" canvas="+(g.canvas!=null)+" shader="+g.materialForRendering.shader.name+" nativeTint="+g.canvasRenderer.GetColor()+" texture="+(g.mainTexture!=null?g.mainTexture.name:"NULL")+" vertices="+g.canvasRenderer.GetMesh().vertexCount+" vertex0="+g.canvasRenderer.GetMesh().colors[0]);File.WriteAllLines(Dir+"/graphic-inspection.txt",lines);}
 static void FinalizeVisual(){
 var finalStation=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();
 finalStation.AttackSelectionEdge.sprite=finalStation.AttackProgress.sprite;finalStation.DefenseSelectionEdge.sprite=finalStation.DefenseProgress.sprite;
 finalStation.AttackSelectionEdge.fillAmount=0;finalStation.DefenseSelectionEdge.fillAmount=0;

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
 static void Capture(string name,bool initial=false,bool actual=false){
 Directory.CreateDirectory(Gallery);var source=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>().View;var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();
 var go=new GameObject("Temporary portal polish capture");var camera=go.AddComponent<Camera>();camera.CopyFrom(source);camera.enabled=false;camera.tag="Untagged";camera.aspect=16f/9;
 var face=station.transform.parent;camera.transform.position=initial?source.transform.position:face.position-face.forward*1.65f;camera.transform.LookAt(face.position);if(actual){camera.transform.SetPositionAndRotation(source.transform.position,source.transform.rotation);camera.aspect=source.aspect;}
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
 Check(Mathf.Abs((n==0?station.AttackSelectionEdge:station.DefenseSelectionEdge).fillAmount-gaze.DwellProgress)<.001f,"Secondary selection edge reads same dwell");
 Check(Mathf.Abs(gaze.DwellProgress-bar.fillAmount)<.001f&&other.fillAmount==0&&count==0,"New rounded bar follows real dwell, other mode stays empty");Capture(n==0?"04-attack-partial.png":"06-defense-partial.png");
 File.AppendAllText(Dir+"/samples.txt",target.Action+"="+bar.fillAmount+" real="+gaze.DwellProgress+"\n");view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;Check(bar.fillAmount==0,"Cancellation still resets styled bar");}
 foreach(var text in station.transform.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Live TMP fits "+text.name);}
 Check(Mathf.Approximately(gaze.DwellSeconds,1.5f),"Existing 1.5 second dwell unchanged");
 // Exercise production selection from the close portal inspection perspective.
 var face=station.transform.parent;view.position=face.position-face.forward*1.65f;view.LookAt(station.Attack.GetComponent<Collider>().bounds.center);float deadline=Time.realtimeSinceStartup+6;
 while(!station.ConfirmationVisible&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(station.ConfirmationVisible&&count==1,"Single activation and existing confirmation");Capture("08-confirmed.png");
 while(!station.Attack.Manager.IntroPlaying&&Time.realtimeSinceStartup<deadline)yield return null;
 for(int i=0;i<20;i++)yield return null;
 var intro=UnityEngine.Object.FindFirstObjectByType<Swift.Game.IntroSequence>();var main=station.AttackProgress.GetComponentInParent<Canvas>();
 Check(intro.IsPlaying&&main.gameObject.activeInHierarchy,"Prologue keeps main portal active");
 Check(main.GetComponent<CanvasGroup>().alpha>0&&main.GetComponent<CanvasGroup>().alpha<1,"Portal dimmed without disappearing");
 Check(intro.PresentationRoot.IsChildOf(station.transform),"Modal belongs to portal system");
 foreach(var text in intro.PresentationRoot.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"Modal TMP fits "+text.text);}
 Capture("09-prologue-main-visible.png",false,true);
 var skip=intro.PresentationRoot.GetComponentsInChildren<BankInteractable>().First(t=>t.Action==BankAction.SkipIntro);view.LookAt(skip.GetComponent<Collider>().bounds.center);deadline=Time.realtimeSinceStartup+3;
 while(intro.IsPlaying&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(!intro.IsPlaying,"Real gaze activates skip without chasing the control");yield return null;yield return null;
 Check(Mathf.Approximately(main.GetComponent<CanvasGroup>().alpha,1),"Main portal brightness restored");Check(count==1,"No second mode activation during prologue");
 var seat=Swift.VR.AnalystSeat.Current;Check(seat!=null&&seat.Request(true),"Existing seated transition accepted");deadline=Time.realtimeSinceStartup+4;while(seat.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;Check(seat.IsSeated,"Actual seated mode entered");
 view.rotation=Quaternion.identity;int completed=0;intro.Play(()=>completed++);int shot=-1;deadline=Time.realtimeSinceStartup+15;
 while(intro.IsPlaying&&Time.realtimeSinceStartup<deadline){
 if(shot!=intro.CurrentMoment&&intro.PresentationRoot.GetComponentInChildren<Canvas>()!=null){shot=intro.CurrentMoment;Canvas.ForceUpdateCanvases();foreach(var t in intro.PresentationRoot.GetComponentsInChildren<TMPro.TMP_Text>()){t.ForceMeshUpdate();Check(!t.isTextOverflowing,"Seated shot "+shot+" TMP fits");}if(shot==0){for(int i=0;i<20;i++)yield return null;Capture("10-prologue-seated.png",false,true);}}
 yield return null;
 }
 Check(shot==2&&!intro.IsPlaying&&completed==1,"All three prologue moments end automatically once while seated");
 yield return null;yield return null;Check(main.gameObject.activeInHierarchy&&Mathf.Approximately(main.GetComponent<CanvasGroup>().alpha,1),"Portal restored after automatic end");


 }
}}

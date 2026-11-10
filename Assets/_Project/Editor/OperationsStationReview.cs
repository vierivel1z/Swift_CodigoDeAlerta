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
[InitializeOnLoad] public static class OperationsStationReview {
 const string Dir="Artifacts/OperationsStationFinal", Pending="SWIFT_OPERATIONS_FINAL";
 static IEnumerator routine;static int frame=-1,errors;static string run;
 static OperationsStationReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
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
 static void Apply(){
 var backup=File.ReadAllText(Dir+"/backup-path.txt").Trim();var recovery="Assets/_Recovery/OperationsStation_Before_"+DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff")+".unity";
 Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),recovery,true),"Open Editor scene backed up with unsaved work");AssetDatabase.ImportAsset(recovery);File.Copy(recovery,backup+"/EditorBefore.unity");File.Copy(recovery+".meta",backup+"/EditorBefore.unity.meta");
 var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();Undo.RecordObject(ui,"Immersive bank monitor station");ui.ImmersiveStation=true;ui.StationFrame=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/OfficeVisual/PortalOperations/UI_ChamferFrame_256.png");ui.StationFill=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/OfficeVisual/PortalOperations/UI_White_4.png");
 Check(ui.StationFrame!=null&&ui.StationFill!=null,"Reuse existing small UI sprites");
 var glow=new GameObject("Main monitor - subtle desk spill");Undo.RegisterCreatedObjectUndo(glow,"Monitor light");glow.transform.SetParent(ui.PanelAnchor,false);glow.transform.localPosition=new Vector3(0,-.40f,-.24f);var light=glow.AddComponent<Light>();light.type=LightType.Point;light.range=1.3f;light.intensity=.30f;light.color=new Color(.42f,.76f,.86f);light.shadows=LightShadows.None;light.renderMode=LightRenderMode.ForceVertex;light.lightmapBakeType=LightmapBakeType.Realtime;
 EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Inspect();
 }
 static void FinalizeVisual(){Inspect();}
 static void Capture(string name){Inspect();}
 static void Inspect(){
 var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();var seat=UnityEngine.Object.FindFirstObjectByType<Swift.VR.AnalystSeat>();
 var all=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None);
 File.WriteAllLines(Dir+"/geometry.txt",all.Where(t=>t==ui.PanelAnchor||t==seat.SeatedEye||t==seat.StandingFloor||t==seat.Rig||t==seat.Eye||t.name.Contains("monitor")||t.name.Contains("Computer")||t.name.Contains("Seat")||t.name.Contains("Sit")||t.name.Contains("Stand")||t.name.Contains("Format reference")||t.name.Contains("Format ")).Select(t=>t.name+" parent="+t.parent?.name+" pos="+t.position+" rot="+t.eulerAngles+" scale="+t.lossyScale+" bounds="+(t.GetComponent<Renderer>()!=null?t.GetComponent<Renderer>().bounds.ToString():"none")+" collider="+(t.GetComponent<Collider>()!=null?t.GetComponent<Collider>().bounds.ToString():"none")));
 }
 const string Gallery="Docs/Previews/OperationsStationFinal/20261104";
 static VRInteractionManager gaze;static OperationsStation station;static UIManager ui;static Transform banking;
 static BankInteractable Control(BankAction action)=>banking.GetComponentsInChildren<BankInteractable>(true).First(t=>t.Action==action&&t.gameObject.activeInHierarchy);
 static bool Reaches(BankInteractable t){Physics.SyncTransforms();var delta=t.GetComponent<Collider>().bounds.center-gaze.View.transform.position;return Physics.Raycast(gaze.View.transform.position,delta.normalized,out var hit,delta.magnitude+.05f)&&hit.collider.GetComponentInParent<BankInteractable>()==t;}
 static IEnumerator Select(BankAction action){
 gaze.View.transform.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 var target=Control(action);Check(Reaches(target),"Real seated ray reaches "+action);bool fired=false;System.Action<BankInteractable> count=t=>{if(t==target)fired=true;};gaze.Activated+=count;
 try{gaze.View.transform.LookAt(target.GetComponent<Collider>().bounds.center);float deadline=Time.realtimeSinceStartup+5;while(!fired&&Time.realtimeSinceStartup<deadline)yield return null;Check(fired,"Real 1.5 s dwell activates "+action);yield return null;}finally{gaze.Activated-=count;}
 }
 static void TextFits(Transform parent){Canvas.ForceUpdateCanvases();foreach(var text in parent.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Check((!text.isTextOverflowing||text.overflowMode==TMPro.TextOverflowModes.Page),"TMP fits/pages "+text.transform.parent.name+"/"+text.name+" font="+text.fontSize);}}
 static void Render(string name,Transform look=null,bool actual=false){
 Directory.CreateDirectory(Gallery);var source=gaze!=null?gaze.View:UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>().View;var u=ui!=null?ui:UnityEngine.Object.FindFirstObjectByType<UIManager>();var go=new GameObject("Temporary station review camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(source);camera.enabled=false;camera.tag="Untagged";camera.aspect=16f/9;camera.transform.SetPositionAndRotation(source.transform.position,source.transform.rotation);if(!actual)camera.transform.LookAt(look!=null?look.position:u.PanelAnchor.position);
 var rt=new RenderTexture(1600,900,24);var previous=RenderTexture.active;Texture2D image=null;
 try{Canvas.ForceUpdateCanvases();rt.Create();UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(Gallery+"/"+name,image.EncodeToPNG());File.AppendAllText(Gallery+"/receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" Unity="+Application.unityVersion+" eye="+camera.transform.position+" look="+camera.transform.forward+" sourceForward="+source.transform.forward+" Play="+Application.isPlaying+" drawCallsAggregate="+UnityEditor.UnityStats.drawCalls+" trianglesAggregate="+UnityEditor.UnityStats.triangles+" observer="+!actual+" mode="+(u.Manager.Session!=null?u.Manager.Session.Mode.ToString():"none")+" dwell="+(gaze!=null?gaze.DwellProgress:0)+" NOT_ANDROID\n");}finally{RenderTexture.active=previous;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(go);}
 }
 static IEnumerator LightReview(){
 yield return null;yield return null;yield return null;gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();station=ui.GetComponent<OperationsStation>();foreach(var adapter in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;gaze.SetEditorGazeSimulation(true);var seat=Swift.VR.AnalystSeat.Current;seat.Rig.position+=seat.StandingFloor.position+Vector3.up*1.6f-gaze.View.transform.position;gaze.View.transform.rotation=Quaternion.LookRotation(Vector3.up);
 var light=GameObject.Find("Main monitor - subtle desk spill").GetComponent<Light>();Check(light.type==LightType.Point&&light.shadows==LightShadows.None&&light.range<=1.3f,"One short-range monitor light without shadows");
 light.enabled=false;yield return null;yield return null;Render("14-monitor-spill-off.png");light.enabled=true;yield return null;yield return null;Render("15-monitor-spill-on.png");
 gaze.View.transform.rotation=Quaternion.identity;yield return null;yield return null;Check(!station.ContextVisible,"Actual exploration pose has no entry prompt");Render("16-standing-actual-exploration.png",null,true);gaze.View.transform.LookAt(station.EntryTarget.GetComponent<Collider>().bounds.center);yield return null;yield return null;Check(station.ContextVisible,"Actual monitor-facing pose reveals entry prompt");Render("17-monitor-focus-actual-gaze.png",null,true);gaze.View.transform.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 Check(seat.Request(true),"Seated layout probe uses existing transition");while(seat.IsTransitioning)yield return null;ui.Manager.StartScenario(Swift.Bank.ScenarioMode.Attack);banking=ui.PanelAnchor.GetComponentsInChildren<Canvas>(true).First(c=>c.name=="Expedientes - World Space").transform;var session=ui.Manager.Session;var docs=ui.GetComponent<BankDocumentUI>();
 ui.Manager.Notify(string.Join(" ",Enumerable.Repeat("Observacion extensa de prueba de interfaz; sin datos nuevos de banco.",100)));docs.ExpandObservations();yield return null;var text=banking.GetComponentsInChildren<TMPro.TMP_Text>().First(t=>t.name=="Detalle paginado");text.ForceMeshUpdate();Check(text.textInfo.pageCount>1,"Long observations use actual TMP pages without shrinking below readable size");var next=Select(BankAction.ObservationNext);while(next.MoveNext())yield return null;text=banking.GetComponentsInChildren<TMPro.TMP_Text>().First(t=>t.name=="Detalle paginado");Check(text.pageToDisplay==2,"SIGUIENTE advances the real observation page");var prev=Select(BankAction.ObservationPrevious);while(prev.MoveNext())yield return null;text=banking.GetComponentsInChildren<TMPro.TMP_Text>().First(t=>t.name=="Detalle paginado");Check(text.pageToDisplay==1,"ANTERIOR returns to first observation page");Check(ui.Manager.Session==session&&session.Transactions.Records.All(r=>r.Status==Swift.Bank.TransactionStatus.Pending),"UI pagination never changes bank records");
 }
 static IEnumerator StationPC(){
 yield return null;yield return null;yield return null;
 gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();station=ui.GetComponent<OperationsStation>();var seat=Swift.VR.AnalystSeat.Current;foreach(var adapter in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;
 seat.Rig.position+=seat.StandingFloor.position+Vector3.up*1.6f-gaze.View.transform.position;gaze.View.transform.LookAt(ui.PanelAnchor.position);gaze.SetEditorGazeSimulation(false);var previous=Mouse.current;var mouse=InputSystem.AddDevice<Mouse>("Station hold verification");int activations=0;System.Action<BankInteractable> count=t=>{if(t==station.EntryTarget)activations++;};gaze.Activated+=count;
 try{
 void Pointer(BankInteractable target,bool held){mouse.MakeCurrent();var point=gaze.View.WorldToScreenPoint(target.GetComponent<Collider>().bounds.center);InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(point.x,point.y),buttons=(ushort)(held?1:0)});InputSystem.Update();}
 float deadline=Time.realtimeSinceStartup+.3f;while(Time.realtimeSinceStartup<deadline){Pointer(station.EntryTarget,false);yield return null;}
 Check(gaze.FocusedTarget==station.EntryTarget&&station.EntryProgress.fillAmount==0,"PC hover owns zero hidden dwell");deadline=Time.realtimeSinceStartup+4;while(gaze.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline){Pointer(station.EntryTarget,true);yield return null;}
 Check(station.EntryProgress.fillAmount>.45f&&station.EntryProgress.fillAmount<.6f,"PC hold reaches real half charge");Pointer(station.EntryTarget,false);yield return null;yield return null;Check(gaze.DwellProgress==0&&station.EntryProgress.fillAmount==0&&activations==0,"PC release resets station charge without movement");
 deadline=Time.realtimeSinceStartup+5;while(activations==0&&Time.realtimeSinceStartup<deadline){Pointer(station.EntryTarget,true);yield return null;}while(seat.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;yield return null;
 File.AppendAllText(Dir+"/stationpc-state.txt","activations="+activations+" seated="+seat.IsSeated+" transition="+seat.IsTransitioning+" visible="+station.DetailedVisible+" failure="+seat.LastFailure+" currentMouse="+Mouse.current?.name+" dwell="+gaze.DwellProgress+"\n");Check(activations==1&&seat.IsSeated&&station.DetailedVisible,"PC fresh hold enters once");Pointer(station.ExitTarget,false);yield return null;yield return null;
 Check(gaze.FocusedTarget==station.ExitTarget&&gaze.DwellProgress==0,"PC exit hover starts empty");deadline=Time.realtimeSinceStartup+5;while(seat.IsSeated&&Time.realtimeSinceStartup<deadline){Pointer(station.ExitTarget,true);yield return null;}while(seat.IsTransitioning&&Time.realtimeSinceStartup<deadline)yield return null;yield return null;
 Check(!seat.IsSeated&&!station.DetailedVisible&&station.Desktop.gameObject.activeInHierarchy,"PC 1.5 s exit returns to active desktop");
 }finally{gaze.Activated-=count;if(mouse.added)InputSystem.RemoveDevice(mouse);if(previous!=null&&previous.added)previous.MakeCurrent();}
 }
 static IEnumerator Probe(){
 if(run=="light"){var lighting=LightReview();while(lighting.MoveNext())yield return null;yield break;}
 if(run=="stationpc"){var pc=StationPC();while(pc.MoveNext())yield return null;yield break;}
 if(run!="review"){
 PortalOperationsReview.SuppressCaptures=true;IEnumerator original;
 if(run=="gaze")original=(IEnumerator)typeof(PortalOperationsReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,null);
 else if(run=="pc")original=(IEnumerator)typeof(PortalUniformFillReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,null);
 else original=(IEnumerator)typeof(UXGameplayReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,new object[]{run});
 try{while(original.MoveNext())yield return null;}finally{PortalOperationsReview.SuppressCaptures=false;if(run=="pc"){var field=typeof(PortalUniformFillReview).GetField("mouse",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static);var mouse=field.GetValue(null) as Mouse;if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);field.SetValue(null,null);var previous=typeof(PortalUniformFillReview).GetField("previousMouse",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).GetValue(null) as Mouse;if(previous!=null&&previous.added)previous.MakeCurrent();}}
 yield break;
 }
 yield return null;yield return null;yield return null;
 gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();station=ui.GetComponent<OperationsStation>();var seat=Swift.VR.AnalystSeat.Current;var game=ui.Manager;var docs=ui.GetComponent<BankDocumentUI>();
 Check(station!=null&&ui.ImmersiveStation,"Station enabled in saved bank scene");banking=ui.PanelAnchor.GetComponentsInChildren<Canvas>(true).First(c=>c.name=="Expedientes - World Space").transform;
 foreach(var input in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))input.enabled=false;gaze.SetEditorGazeSimulation(true);
 var view=gaze.View.transform;seat.Rig.position+=seat.StandingFloor.position+Vector3.up*1.6f-view.position;view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 Check(!seat.IsSeated&&!station.DetailedVisible&&!station.SecondaryVisible&&station.Desktop.gameObject.activeInHierarchy,"Standing: detailed screens hidden, physical monitors and active desktops retained");Check(!station.ContextVisible&&station.EntryProgress.fillAmount==0,"Entry prompt absent without focus");TextFits(station.Desktop);Render("01-standing-desktop.png");
 game.EnterScenario(Swift.Bank.ScenarioMode.Attack);Check(game.IntroPlaying,"Existing prologue still starts");game.SkipIntro();var session=game.Session;var origin=seat.Rig.position;int enters=0;gaze.Activated+=t=>{if(t==station.EntryTarget)enters++;};
 view.LookAt(station.EntryTarget.GetComponent<Collider>().bounds.center);float deadline=Time.realtimeSinceStartup+5;while(gaze.DwellProgress<.1f&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(gaze.FocusedTarget==station.EntryTarget&&station.ContextVisible,"Main monitor focus reveals contextual entry only");Render("02-monitor-focused.png");while(gaze.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(station.EntryProgress.fillAmount>.45f&&station.EntryProgress.fillAmount<.6f&&Mathf.Abs(station.EntryProgress.fillAmount-gaze.DwellProgress)<.001f,"Entry 0.75 s equals real dwell near 50 percent");Render("03-entry-half.png");
 view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;Check(station.EntryProgress.fillAmount==0&&!station.ContextVisible&&seat.Rig.position==origin&&enters==0,"Look-away cancels entry, hides prompt and never moves player");
 view.LookAt(station.EntryTarget.GetComponent<Collider>().bounds.center);var heading=view.rotation;deadline=Time.realtimeSinceStartup+5;while((!seat.IsSeated||seat.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(seat.IsSeated&&!seat.IsTransitioning&&enters==1,"One actual entry with existing fade and seated target");Check(Quaternion.Angle(heading,view.rotation)<.1f,"Seat preserves head orientation");Check(Vector3.Distance(view.position,seat.SeatedEye.position)<.02f,"Existing seated eye position retained");Check(station.DetailedVisible&&station.SecondaryVisible,"Both detailed monitor screens enabled while seated");
 Check(UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Where(t=>t.Action==BankAction.Teleport).All(t=>!t.GetComponent<Collider>().enabled),"Ground teleports blocked while seated");
 Render("04-seated-expediente.png");TextFits(banking);
 foreach(var pair in new[]{(BankAction.Registry,"05-registro.png"),(BankAction.Alerts,"06-alertas.png"),(BankAction.Procedure,"07-procedimiento.png"),(BankAction.ViewDossier,"04b-expediente.png"),(BankAction.MoreObservations,"08-observaciones.png")}){var step=Select(pair.Item1);while(step.MoveNext())yield return null;TextFits(banking);Render(pair.Item2);}
 Check(docs.CurrentPage==BankDocumentUI.Page.Observations,"VER M?S shows full values and authored observations");var back=Select(BankAction.BackToDocument);while(back.MoveNext())yield return null;Check(docs.CurrentPage==BankDocumentUI.Page.Dossier,"VOLVER restores the dossier");
 Render("11-secondary-monitor.png",UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>().transform);TextFits(UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>().transform);
 var entryLabels=banking.GetComponentsInChildren<TMPro.TMP_Text>().Where(t=>t.name.StartsWith("Valor ")).ToArray();foreach(var t in entryLabels){var value=t.text;t.text=new string('W',180);t.ForceMeshUpdate();Check(t.overflowMode==TMPro.TextOverflowModes.Ellipsis&&t.textBounds.size.x<=t.rectTransform.rect.width+2,"Long value bounded in "+t.name);t.text=value;t.ForceMeshUpdate();}
 foreach(var action in new[]{BankAction.Approve,BankAction.Return,BankAction.Hold}){var step=Select(action);while(step.MoveNext())yield return null;TextFits(banking);TextFits(UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>().transform);}
 Check(game.Session.Correct==3&&game.Session.Transactions.Records.Count(t=>t.Status!=Swift.Bank.TransactionStatus.Pending)==3,"Approve, Return and Hold keep authored banking results");float hold=Time.realtimeSinceStartup+.5f;while(Time.realtimeSinceStartup<hold)yield return null;Check(game.Session.Correct==3,"Holding after a decision never activates a second record");var id=game.Session.Transactions.Current.Data.Id;
 var close=Select(BankAction.End);while(close.MoveNext())yield return null;Check(!game.Session.Ended&&game.Session.Transactions.Current.Data.Id==id,"CERRAR TURNO preserves incomplete-turn rule and does not exit seat");
 var registry=Select(BankAction.Registry);while(registry.MoveNext())yield return null;Check(game.Message.Contains("cuenta no encontrada"),"Unknown account uses existing registry query");Render("13-registro-cuenta-no-encontrada.png");
 view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;view.LookAt(station.ExitTarget.GetComponent<Collider>().bounds.center);deadline=Time.realtimeSinceStartup+4;while(gaze.DwellProgress<.3f&&Time.realtimeSinceStartup<deadline)yield return null;Render("09-exit-focused.png");while((seat.IsSeated||seat.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;yield return null;
 Check(!seat.IsSeated&&!station.DetailedVisible&&!station.SecondaryVisible&&station.Desktop.gameObject.activeInHierarchy,"Exit restores standing active desktops");Check(game.Session==session&&game.Session.Transactions.Current.Data.Id==id,"Exit preserves session and current record");Check(UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsSortMode.None).Where(t=>t.Action==BankAction.Teleport).All(t=>t.GetComponent<Collider>().enabled),"Teleport colliders restored after leaving");Render("10-after-exit.png");
 view.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;view.LookAt(station.EntryTarget.GetComponent<Collider>().bounds.center);deadline=Time.realtimeSinceStartup+5;while((!seat.IsSeated||seat.IsTransitioning)&&Time.realtimeSinceStartup<deadline)yield return null;Check(seat.IsSeated&&game.Session==session&&game.Session.Transactions.Current.Data.Id==id&&docs.CurrentPage==BankDocumentUI.Page.Registry,"Re-entering preserves banking progress and selected page");
 foreach(var action in new[]{BankAction.Hold,BankAction.Hold,BankAction.Hold}){var step=Select(action);while(step.MoveNext())yield return null;TextFits(banking);TextFits(UnityEngine.Object.FindFirstObjectByType<FormatReferencePanel>().transform);}Check(game.Session.Ended&&game.Session.Correct==6,"All six original recommended decisions remain correct");
 game.EnterScenario(Swift.Bank.ScenarioMode.Defense);Check(game.Session.Mode==Swift.Bank.ScenarioMode.Defense&&!game.IntroPlaying,"Defense retains existing mode and prologue lifecycle");var defense=Select(BankAction.Defense);while(defense.MoveNext())yield return null;for(int i=0;i<6;i++){var step=Select(BankAction.Approve);while(step.MoveNext())yield return null;}Check(game.Session.Ended&&game.LastDefenseLoss==0,"Defense calculations unchanged");Render("12-results.png");TextFits(banking);
 }

}}


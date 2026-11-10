using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using Swift.Interaction;
using Swift.UI;
namespace Swift.EditorTools {
[InitializeOnLoad] public static class PortalOperationsReview {
 const string Dir="Artifacts/PortalOperations", Gallery="Docs/Previews/PortalOperationsReview";
 internal static bool SuppressCaptures;
 static IEnumerator routine; static string command; static int frame; static int errors;
 static PortalOperationsReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
 static T[] All<T>() where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
 static void Tick(){
 if(routine!=null){if(frame==Time.frameCount)return;frame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
 var q=Dir+"/command.txt";if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
 command=File.ReadAllText(q).Trim();File.Delete(q);try{
 if(command=="before"){Check(!SceneManager.GetActiveScene().isDirty,"No unsaved scene discarded");Capture("before.png");File.WriteAllLines(Dir+"/before-hierarchy.txt",All<Canvas>().SelectMany(c=>c.GetComponentsInChildren<Transform>(true)).Select(t=>t.name+" active="+t.gameObject.activeInHierarchy+" scale="+t.lossyScale));Finish(null);}
 else if(command=="build"){Build();Finish(null);}
 else if(command=="polish"){Polish();Finish(null);}
 else if(command=="probe"){SessionState.SetBool("PortalProbe",true);EditorApplication.EnterPlaymode();}
 }catch(Exception e){Finish(e);}}
 static void Changed(PlayModeStateChange s){if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("PortalProbe",false)){SessionState.EraseBool("PortalProbe");command="probe";frame=-1;errors=0;Application.logMessageReceived+=Log;routine=Probe();}}
 static void Log(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert){errors++;File.AppendAllText(Dir+"/console-errors.txt",message+"\n"+stack);}}
 static void Finish(Exception e){Application.logMessageReceived-=Log;routine=null;if(e==null&&errors>0)e=new Exception("Runtime errors="+errors);Directory.CreateDirectory(Dir);File.WriteAllText(Dir+"/"+command+"-result.txt",(e==null?"PASS ":"FAIL ")+command+" "+DateTime.UtcNow.ToString("O")+" "+e);if(command=="probe"&&EditorApplication.isPlaying)EditorApplication.ExitPlaymode();}
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);Directory.CreateDirectory(Dir);File.AppendAllText(Dir+"/checks.txt","PASS "+label+"\n");}
 static void Capture(string name){if(SuppressCaptures)return;
 Directory.CreateDirectory(Gallery);var source=All<Swift.Interaction.VRInteractionManager>().Single().View;
 var go=new GameObject("Temporary portal capture");var cam=go.AddComponent<Camera>();cam.CopyFrom(source);cam.enabled=false;cam.tag="Untagged";cam.fieldOfView=source.fieldOfView;cam.aspect=16f/9;
 var face=All<Transform>().First(t=>t.name=="A - Inclined mode console face");cam.transform.position=new Vector3(0,2.35f,-4.35f);cam.transform.LookAt(face.position);if(name.Contains("initial-player")){cam.transform.position=source.transform.position;cam.transform.LookAt(face.position);}
 var rt=new RenderTexture(1600,900,24);var previous=RenderTexture.active;Texture2D tex=null;
 try{rt.Create();Canvas.ForceUpdateCanvases();RenderPipeline.SubmitRenderRequest(cam,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();File.WriteAllBytes(Gallery+"/"+name,tex.EncodeToPNG());File.AppendAllText(Gallery+"/receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" Play="+Application.isPlaying+" FOV="+cam.fieldOfView+" eye="+cam.transform.position+"\n");}
 finally{RenderTexture.active=previous;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(go);}}
 static Color Amber=new Color(1,.65f,.23f),Cyan=new Color(.24f,.86f,.96f),Navy=new Color(.018f,.043f,.075f);
 static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 pos){var g=new GameObject(name,typeof(RectTransform));var r=g.GetComponent<RectTransform>();r.SetParent(parent,false);r.sizeDelta=size;r.anchoredPosition=pos;return r;}
 static Image Box(string name,Transform parent,Vector2 size,Vector2 pos,Color color){var r=Rect(name,parent,size,pos);var i=r.gameObject.AddComponent<Image>();i.color=color;i.raycastTarget=false;return i;}
 static TMP_Text Text(string name,Transform parent,string value,Vector2 size,Vector2 pos,float font,Color color){var r=Rect(name,parent,size,pos);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.text=value;t.fontSize=font;t.color=color;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
 static void Polish(){var station=All<ModeStationVisual>().Single();var root=station.transform;var header=root.Find("WorldSpaceCanvas/Header");header.GetComponent<TMP_Text>().fontSize=65;header.GetComponent<RectTransform>().sizeDelta=new Vector2(2300,130);header.GetComponent<RectTransform>().anchoredPosition=new Vector2(-100,495);
 var icon=Rect("Institutional bank",root.Find("WorldSpaceCanvas"),new Vector2(160,110),new Vector2(1230,495));Line(icon,new Vector2(-65,30),new Vector2(0,65),Cyan);Line(icon,new Vector2(0,65),new Vector2(65,30),Cyan);Box("Base",icon,new Vector2(140,8),new Vector2(0,-45),Cyan);for(int k=0;k<3;k++)Box("Column",icon,new Vector2(10,65),new Vector2(-45+k*45,-5),Cyan);
 PrefabUtility.SaveAsPrefabAsset(root.gameObject,"Assets/_Project/OfficeVisual/PortalOperations/PortalOperations.prefab");EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("after-normal-0.png");}
 static void Line(Transform parent,Vector2 a,Vector2 b,Color tint){var i=Box("Icon line",parent,new Vector2(Vector2.Distance(a,b),12),(a+b)*.5f,tint);i.rectTransform.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg);}
 static void Build(){
 Check(!SceneManager.GetActiveScene().isDirty,"Clean scene before reconstruction");
 var face=All<Transform>().Single(t=>t.name=="A - Inclined mode console face");var station=All<ModeStationVisual>().Single();
 Check(station.Attack!=null&&station.Defense!=null,"Original action identities retained");
 foreach(var c in face.GetComponentsInChildren<Canvas>(true))c.gameObject.SetActive(false);
 if(station.AttackLamp!=null)station.AttackLamp.gameObject.SetActive(false);if(station.DefenseLamp!=null)station.DefenseLamp.gameObject.SetActive(false);
 var previous=face.Find("PortalOperations");if(previous!=null){Check(station.transform!=previous,"Portal already finalized; preserve original actions");UnityEngine.Object.DestroyImmediate(previous.gameObject);}
 var root=new GameObject("PortalOperations");Undo.RegisterCreatedObjectUndo(root,"Rebuild portal presentation");root.transform.SetParent(face,false);
 var canvas=Rect("WorldSpaceCanvas",root.transform,new Vector2(2800,1180),Vector2.zero);canvas.localPosition=new Vector3(0,0,-.13f);canvas.localScale=Vector3.one*.001f;
 var cv=canvas.gameObject.AddComponent<Canvas>();cv.renderMode=RenderMode.WorldSpace;
 Box("Frame",canvas,new Vector2(2800,1180),Vector2.zero,Cyan*.6f);Box("Background",canvas,new Vector2(2784,1164),Vector2.zero,Navy);
 Text("Header",canvas,"<b>SWIFT</b> / PORTAL DE OPERACIONES",new Vector2(2480,130),new Vector2(0,495),70,Color.white);
 Box("Header separator",canvas,new Vector2(2620,3),new Vector2(0,410),Cyan*.55f);
 var tex=new Texture2D(4,4);tex.SetPixels(Enumerable.Repeat(Color.white,16).ToArray());tex.Apply();const string asset="Assets/_Project/OfficeVisual/PortalOperations";Directory.CreateDirectory(asset);File.WriteAllBytes(asset+"/UI_White_4.png",tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(asset+"/UI_White_4.png");var importer=(TextureImporter)AssetImporter.GetAtPath(asset+"/UI_White_4.png");importer.textureType=TextureImporterType.Sprite;importer.mipmapEnabled=false;importer.SaveAndReimport();var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(asset+"/UI_White_4.png");
 for(int n=0;n<2;n++){
 bool attack=n==0;var tint=attack?Amber:Cyan;var card=Rect(attack?"AttackCard":"DefenseCard",canvas,new Vector2(1280,920),new Vector2(attack?-680:680,-80));
 var border=Box("FocusBorder",card,new Vector2(1280,920),Vector2.zero,tint*.55f);Box("Background",card,new Vector2(1266,906),Vector2.zero,new Color(.025f,.065f,.105f));
 // Native UI icon with explicit bounds: no imported sprite or uncontrolled scaling.
 var icon=Rect("Icon",card,new Vector2(280,200),new Vector2(0,285));
 if(attack){Box("Pediment",icon,new Vector2(250,12),new Vector2(0,72),tint);Box("Base",icon,new Vector2(270,16),new Vector2(0,-70),tint);for(int i=0;i<4;i++)Box("Column",icon,new Vector2(22,120),new Vector2(-90+i*60,0),tint);Line(icon,new Vector2(-125,78),new Vector2(0,135),tint);Line(icon,new Vector2(0,135),new Vector2(125,78),tint);}
 else{var points=new[]{new Vector2(0,120),new Vector2(110,75),new Vector2(95,-45),new Vector2(0,-125),new Vector2(-95,-45),new Vector2(-110,75),new Vector2(0,120)};for(int k=1;k<points.Length;k++)Line(icon,points[k-1],points[k],tint);Box("Lock shackle",icon,new Vector2(74,72),new Vector2(0,15),tint);Box("Lock shackle opening",icon,new Vector2(48,52),new Vector2(0,10),Navy);Box("Lock",icon,new Vector2(110,72),new Vector2(0,-32),tint);Box("Keyhole",icon,new Vector2(12,27),new Vector2(0,-32),Navy);}
 Text("Title",card,attack?"ATAQUE":"DEFENSA",new Vector2(1160,120),new Vector2(0,120),100,tint);
 Text("Description",card,attack?"<b>Banco vulnerable</b>\nRevisa \u00f3rdenes con\ncontroles limitados.":"<b>Banco protegido</b>\nActiva controles y\ncompara resultados.",new Vector2(1150,225),new Vector2(0,-70),55,Color.white);
 Box("Gaze well",card,new Vector2(1160,205),new Vector2(0,-320),Navy);
 var status=Text("GazePrompt",card,"MIRA PARA SELECCIONAR",new Vector2(1130,75),new Vector2(0,-280),46,Color.white);
 Box("Progress track",card,new Vector2(1080,40),new Vector2(0,-365),new Color(.07f,.14f,.19f));var fill=Box("GazeProgress",card,new Vector2(1060,26),new Vector2(0,-365),tint);fill.sprite=sprite;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;fill.fillOrigin=0;fill.fillAmount=0;
 if(attack){station.AttackProgress=fill;station.AttackBorder=border;station.AttackStatus=status;}else{station.DefenseProgress=fill;station.DefenseBorder=border;station.DefenseStatus=status;}
 var action=attack?station.Attack:station.Defense;action.GetComponent<BoxCollider>().size=new Vector3(1.28f,.92f,.04f);action.transform.localPosition=new Vector3(attack?-.68f:.68f,-.08f,-.16f);
 }
 EditorUtility.SetDirty(station);EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
 var modular=root.AddComponent<ModeStationVisual>();modular.Attack=station.Attack;modular.Defense=station.Defense;modular.AttackStatus=station.AttackStatus;modular.DefenseStatus=station.DefenseStatus;modular.AttackProgress=station.AttackProgress;modular.DefenseProgress=station.DefenseProgress;modular.AttackBorder=station.AttackBorder;modular.DefenseBorder=station.DefenseBorder;
 station.Attack.transform.SetParent(root.transform,true);station.Defense.transform.SetParent(root.transform,true);
 var frameObject=face.Find("Integrated portal screen");if(frameObject!=null)frameObject.SetParent(root.transform,true);
 UnityEngine.Object.DestroyImmediate(station);station=modular;
 PrefabUtility.SaveAsPrefabAsset(root,asset+"/PortalOperations.prefab");EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("after-normal-0.png");
 Check(station.Attack.Manager!=null&&station.Defense.Manager!=null,"Scene manager references preserved");
 }
 static IEnumerator Probe(){
 yield return null;yield return null;
 var g=All<VRInteractionManager>().Single();var station=All<ModeStationVisual>().Single();var game=station.Attack.Manager;
 foreach(var adapter in All<EditorInputAdapter>())adapter.enabled=false;
 File.AppendAllText(Dir+"/checks.txt","RUN "+DateTime.UtcNow.ToString("O")+"\n");g.SetEditorGazeSimulation(true);Check(Mathf.Approximately(g.DwellSeconds,1.5f),"Production dwell 1.5 seconds");
 var eye=g.View.transform;int attackCount=0,defenseCount=0;
 g.Activated+=t=>{if(t==station.Attack)attackCount++;if(t==station.Defense)defenseCount++;};
 // Observe actual physics raycasts in Update and rendered Image.fillAmount after LateUpdate.
 eye.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 Check(station.AttackProgress.fillAmount==0&&station.DefenseProgress.fillAmount==0,"Both bars empty without focus");Capture("play-0.png");Capture("initial-player.png");
 eye.LookAt(station.Attack.GetComponent<Collider>().bounds.center);Physics.SyncTransforms();
 float deadline=Time.realtimeSinceStartup+8;
 while(g.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(g.FocusedTarget==station.Attack&&g.DwellProgress<.58f,"ATAQUE actual dwell approximately 0.75 seconds");
 Check(Mathf.Abs(station.AttackProgress.fillAmount-g.DwellProgress)<.001f&&station.DefenseProgress.fillAmount==0,"ATAQUE fill equals actual dwell; DEFENSA zero");
 File.AppendAllText(Dir+"/samples.txt","ATAQUE progress="+g.DwellProgress+" Image="+station.AttackProgress.fillAmount+" elapsed="+g.DwellProgress*g.DwellSeconds+"\n");Capture("attack-50.png");
 eye.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 Check(g.DwellProgress==0&&station.AttackProgress.fillAmount==0&&attackCount==0,"Looking away at half dwell resets without activation");
 eye.LookAt(station.Attack.GetComponent<Collider>().bounds.center);while(g.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline)yield return null;
 eye.LookAt(station.Defense.GetComponent<Collider>().bounds.center);yield return null;yield return null;
 Check(g.FocusedTarget==station.Defense&&g.DwellProgress<.2f&&station.AttackProgress.fillAmount==0,"Switch ATAQUE to DEFENSA never transfers progress");
 deadline=Time.realtimeSinceStartup+8;while(g.DwellProgress<.49f&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(Mathf.Abs(station.DefenseProgress.fillAmount-g.DwellProgress)<.001f&&station.AttackProgress.fillAmount==0&&g.DwellProgress<.58f,"DEFENSA real dwell approximately 50 percent");
 File.AppendAllText(Dir+"/samples.txt","DEFENSA progress="+g.DwellProgress+" Image="+station.DefenseProgress.fillAmount+" elapsed="+g.DwellProgress*g.DwellSeconds+"\n");Capture("defense-50.png");
 while(defenseCount==0&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(defenseCount==1&&g.SelectionLatched&&g.DwellProgress==1,"1.5 seconds selects DEFENSA once");
 Check(station.ConfirmationVisible&&!game.IntroPlaying&&station.DefenseProgress.fillAmount==1,"100 percent confirmation before prologue");Capture("defense-100-confirmation.png");
 while(!game.IntroPlaying&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(game.IntroPlaying,"Existing prologue starts after confirmation");game.SkipIntro();yield return null;
 Check(game.Session!=null&&game.Session.Mode==Swift.Bank.ScenarioMode.Defense,"Existing DEFENSA session starts");
 eye.rotation=Quaternion.LookRotation(Vector3.up);yield return null;yield return null;
 var teleports=All<BankInteractable>().Where(t=>t.Action==BankAction.Teleport&&t.Player!=null&&t.gameObject.activeInHierarchy).OrderBy(t=>Vector3.Distance(t.Destination,eye.position)).ToArray();
 Check(teleports.Length>=2,"Existing teleport routes available");var start=eye.position;teleports[teleports.Length-1].Activate();yield return null;teleports[0].Activate();yield return null;
 Check(Vector3.Distance(teleports[0].Player.position,teleports[0].Destination)<.01f,"Actual teleport away and back preserves destination");
 // Restore the same portal inspection point for the next real gaze test.
 var delta=start-eye.position;teleports[0].Player.position+=delta;yield return null;
 Check(station.AttackProgress.fillAmount==0&&station.DefenseProgress.fillAmount==0,"Return to portal resets both bars");Capture("portal-reset-0.png");
 eye.LookAt(station.Attack.GetComponent<Collider>().bounds.center);deadline=Time.realtimeSinceStartup+8;
 while(attackCount==0&&Time.realtimeSinceStartup<deadline)yield return null;
 Check(attackCount==1&&station.AttackProgress.fillAmount==1,"ATAQUE selects once at 100 percent");Capture("attack-100-confirmation.png");
 float hold=Time.time+2;while(Time.time<hold)yield return null;
 Check(attackCount==1&&defenseCount==1&&g.SelectionLatched,"Holding gaze after activation never activates twice");
 Check(game.Session.Mode==Swift.Bank.ScenarioMode.Attack,"Existing ATAQUE session starts without replaying prologue");
 foreach(var text in station.AttackProgress.transform.parent.parent.GetComponentsInChildren<TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"TMP fits "+text.name);}
 Check(g.Reticle!=null&&g.Reticle.gameObject.activeInHierarchy,"Existing reticle retained");
 g.SetEditorGazeSimulation(false);Check(g.FocusedTarget==null&&g.DwellProgress==0,"Changing desktop input mode clears old fixation");g.SetEditorGazeSimulation(true);yield return null;yield return null;
 Check(g.DwellProgress<.2f&&attackCount==1,"Desktop gaze restarts without inheriting PC hover time");
 var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/OfficeVisual/PortalOperations/PortalOperations.prefab");var saved=prefab.GetComponent<ModeStationVisual>();Check(saved.Attack!=null&&saved.Defense!=null&&saved.AttackProgress!=null&&saved.DefenseProgress!=null,"Reusable prefab contains both physical actions and bound bars");
 }
}}

using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Swift.Interaction;
using Swift.UI;
namespace Swift.EditorTools {
[InitializeOnLoad] public static class IntroCameraFollowReview {
 const string Dir="Artifacts/IntroCameraFollow", Pending="SWIFT_INTRO_CAMERA_FOLLOW";
 static IEnumerator routine;static int frame=-1,errors;static string run;
 static IntroCameraFollowReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
 static void Tick(){
 if(routine!=null){if(frame==Time.frameCount)return;frame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
 var q=Dir+"/command.txt";if(File.Exists(q)&&EditorApplication.isPlaying&&!EditorApplication.isCompiling){EditorApplication.ExitPlaymode();return;}if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
 run=File.ReadAllText(q).Trim();File.Delete(q);try{if(SceneManager.GetActiveScene().path!="Assets/_Project/Scenes/BancoCorresponsal.unity"){if(SceneManager.GetActiveScene().isDirty)throw new Exception("Unsaved scene preserved");EditorSceneManager.OpenScene("Assets/_Project/Scenes/BancoCorresponsal.unity");}
 SessionState.SetString(Pending,run);EditorApplication.EnterPlaymode();}catch(Exception e){Finish(e);}}
 static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode)return;run=SessionState.GetString(Pending,"");if(run=="")return;SessionState.EraseString(Pending);errors=0;frame=-1;Application.logMessageReceived+=Log;routine=Probe();}
 static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert){errors++;File.AppendAllText(Dir+"/console.txt",m+"\n"+s);}}
 static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;if(e==null&&errors>0)e=new Exception("Runtime errors="+errors);File.WriteAllText(Dir+"/"+run+"-result.txt",(e==null?"PASS ":"FAIL ")+run+" "+DateTime.UtcNow.ToString("O")+" "+e);if(EditorApplication.isPlaying)EditorApplication.ExitPlaymode();}
 static void Check(bool ok,string label){File.AppendAllText(Dir+"/"+run+"-checks.txt",(ok?"PASS ":"FAIL ")+label+"\n");if(!ok)throw new Exception(label);}
 static void Capture(string name){
 const string gallery="Docs/Previews/IntroCameraFollow";Directory.CreateDirectory(gallery);
 var source=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>().View;
 var go=new GameObject("Temporary intro capture");var camera=go.AddComponent<Camera>();camera.CopyFrom(source);camera.tag="Untagged";camera.enabled=false;camera.aspect=source.aspect;camera.transform.SetPositionAndRotation(source.transform.position,source.transform.rotation);
 int width=1600,height=Mathf.RoundToInt(width/source.aspect);var rt=new RenderTexture(width,height,24);var previous=RenderTexture.active;Texture2D image=null;
 try{Canvas.ForceUpdateCanvases();rt.Create();UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes(gallery+"/"+name,image.EncodeToPNG());File.AppendAllText(gallery+"/receipt.txt",DateTime.UtcNow.ToString("O")+" "+name+" Play=True eye="+camera.transform.position+" rotation="+camera.transform.eulerAngles+" FOV="+camera.fieldOfView+" aspect="+camera.aspect+"\n");}
 finally{RenderTexture.active=previous;if(image!=null)UnityEngine.Object.DestroyImmediate(image);rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(go);}}
 static void Fits(Swift.Game.IntroSequence intro,Camera view,string step){
 var canvas=intro.PresentationRoot.GetComponentInChildren<Canvas>();var corners=new Vector3[4];canvas.GetComponent<RectTransform>().GetWorldCorners(corners);
 foreach(var corner in corners){var p=view.WorldToViewportPoint(corner);Check(p.z>view.nearClipPlane&&p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1,step+" full panel inside actual camera frustum");}
 foreach(var text in canvas.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,step+" TMP fits "+text.name);}
 Check(intro.ViewingDistance<=1.101f&&intro.ViewingDistance>view.nearClipPlane,step+" close readable panel before near clip");
 }
 static IEnumerator Probe(){
 if(run!="review"){
 var original=(IEnumerator)typeof(UXGameplayReview).GetMethod("Probe",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,new object[]{run});while(original.MoveNext())yield return null;yield break;
 }
 yield return null;yield return null;
 var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();var game=station.Attack.Manager;var camera=gaze.View;
 foreach(var adapter in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;
 gaze.SetEditorGazeSimulation(true);var rig=camera.transform.parent;rig.position=new Vector3(0,0,-3.6f);camera.transform.LookAt(station.Attack.transform.parent.position);Physics.SyncTransforms();
 int skipCount=0;gaze.Activated+=t=>{if(t.Action==BankAction.SkipIntro)skipCount++;};
 station.Attack.Activate();float until=Time.realtimeSinceStartup+3;while(!game.IntroPlaying&&Time.realtimeSinceStartup<until)yield return null;
 Check(game.IntroPlaying&&game.Session==null,"Portal confirmation enters prologue before the bank session");
 var intro=game.GetComponent<Swift.Game.IntroSequence>();until=Time.realtimeSinceStartup+.3f;while(Time.realtimeSinceStartup<until)yield return null;
 Check(!station.AttackProgress.gameObject.activeInHierarchy,"Portal graphics hidden while prologue is visible");Fits(intro,camera,"At portal");Capture("01-portal-near-prologue.png");
 var eye=camera.transform.position;var originalRotation=camera.transform.rotation;
 camera.transform.rotation=Quaternion.AngleAxis(55,Vector3.up)*originalRotation;rig.position+=new Vector3(.3f,0,-.2f);until=Time.realtimeSinceStartup+.7f;while(Time.realtimeSinceStartup<until)yield return null;
 Check(Vector3.Distance(eye,camera.transform.position)>.25f,"Test translated the actual player camera");
 var toward=intro.PresentationRoot.position-camera.transform.position;Check(Vector3.Angle(camera.transform.forward,toward)<2,"Prologue follows camera turn and translation");Fits(intro,camera,"After turning");Capture("02-follow-turn-and-move.png");
 // Return very near the real console: proximity must not place the storyboard behind it.
 rig.position=new Vector3(0,0,-2.75f);camera.transform.LookAt(station.Attack.transform.parent.position);until=Time.realtimeSinceStartup+.7f;while(Time.realtimeSinceStartup<until)yield return null;
 Fits(intro,camera,"Near console");Check(intro.ViewingDistance<1,"Real console geometry brings panel forward");Capture("03-close-console-clearance.png");
 var skip=intro.PresentationRoot.GetComponentInChildren<BankInteractable>();Check(skip.Action==BankAction.SkipIntro,"Original skip bank action retained");
 var desired=(skip.GetComponent<Collider>().bounds.center-camera.transform.position).normalized;
 camera.transform.rotation=Quaternion.LookRotation(desired,camera.transform.up);Physics.SyncTransforms();yield return null;yield return null;
 Fits(intro,camera,"Looking at skip");Capture("04-gaze-skip-stays-reachable.png");
 until=Time.realtimeSinceStartup+3;while(game.IntroPlaying&&Time.realtimeSinceStartup<until)yield return null;
 Check(!game.IntroPlaying&&skipCount==1&&game.Session!=null&&game.Session.Mode==Swift.Bank.ScenarioMode.Attack,"Actual 1.5 s gaze reaches Skip without chasing the camera-follow panel");
 Check(intro.PresentationRoot==null||!intro.PresentationRoot.gameObject.activeInHierarchy,"Skip cleans up the floating panel");yield return null;
 Check(station.AttackProgress.gameObject.activeInHierarchy,"Portal graphics restored after prologue");
 }
}}

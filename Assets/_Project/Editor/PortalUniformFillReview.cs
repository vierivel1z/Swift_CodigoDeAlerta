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
[InitializeOnLoad] public static class PortalUniformFillReview {
 const string Dir="Artifacts/PortalUniformFill", Pending="SWIFT_UNIFORM_FILL";
 static IEnumerator routine;static int frame=-1,errors;static Mouse mouse,previousMouse;static string run;
 static PortalUniformFillReview(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=Changed;}
 static void Tick(){
 if(routine!=null){if(frame==Time.frameCount)return;frame=Time.frameCount;try{if(!routine.MoveNext())Finish(null);}catch(Exception e){Finish(e);}return;}
 var q=Dir+"/command.txt";if(File.Exists(q)&&EditorApplication.isPlaying&&!EditorApplication.isCompiling){EditorApplication.ExitPlaymode();return;}if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
 run=File.ReadAllText(q).Trim();File.Delete(q);try{if(SceneManager.GetActiveScene().isDirty)throw new Exception("Unsaved scene preserved");
 if(SceneManager.GetActiveScene().path!="Assets/_Project/Scenes/BancoCorresponsal.unity")EditorSceneManager.OpenScene("Assets/_Project/Scenes/BancoCorresponsal.unity");
 SessionState.SetString(Pending,run);EditorApplication.EnterPlaymode();}catch(Exception e){Finish(e);}}
 static void Changed(PlayModeStateChange state){if(state!=PlayModeStateChange.EnteredPlayMode)return;run=SessionState.GetString(Pending,"");if(run=="")return;SessionState.EraseString(Pending);errors=0;frame=-1;Application.logMessageReceived+=Log;routine=Probe();}
 static void Log(string m,string s,LogType t){if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert){errors++;File.AppendAllText(Dir+"/console.txt",m+"\n"+s);}}
 static void Finish(Exception e){routine=null;Application.logMessageReceived-=Log;if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);mouse=null;if(previousMouse!=null&&previousMouse.added)previousMouse.MakeCurrent();if(e==null&&errors>0)e=new Exception("Runtime errors="+errors);File.WriteAllText(Dir+"/"+run+"-result.txt",(e==null?"PASS ":"FAIL ")+run+" "+DateTime.UtcNow.ToString("O")+" "+e);if(EditorApplication.isPlaying)EditorApplication.ExitPlaymode();}
 static void Check(bool ok,string label){File.AppendAllText(Dir+"/"+run+"-checks.txt",(ok?"PASS ":"FAIL ")+label+"\n");if(!ok)throw new Exception(label);}
 static void Input(Camera view,BankInteractable target,bool held){mouse.MakeCurrent();var point=view.WorldToScreenPoint(target.GetComponent<Collider>().bounds.center);InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(point.x,point.y),buttons=(ushort)(held?1:0)});InputSystem.Update();}
 static void Capture(string name){var folder="UniformFix-20261103";Directory.CreateDirectory("Docs/Previews/PortalOperationsReview/"+folder);typeof(PortalOperationsReview).GetMethod("Capture",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static).Invoke(null,new object[]{folder+"/"+name});}
 static IEnumerator Probe(){
 if(run=="gaze"){
 PortalOperationsReview.SuppressCaptures=true;
 var original=(IEnumerator)typeof(PortalOperationsReview).GetMethod("Probe",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,null);
 try{while(original.MoveNext())yield return null;}finally{PortalOperationsReview.SuppressCaptures=false;}
 yield break;
 }
 yield return null;yield return null;
 var gaze=UnityEngine.Object.FindFirstObjectByType<VRInteractionManager>();var station=UnityEngine.Object.FindFirstObjectByType<ModeStationVisual>();var game=station.Attack.Manager;
 foreach(var adapter in UnityEngine.Object.FindObjectsByType<EditorInputAdapter>(FindObjectsSortMode.None))adapter.enabled=false;
 gaze.SetEditorGazeSimulation(false);previousMouse=Mouse.current;mouse=InputSystem.AddDevice<Mouse>("Portal hold regression mouse");mouse.MakeCurrent();
 var camera=gaze.View;camera.transform.LookAt(station.Attack.GetComponent<Collider>().bounds.center);Physics.SyncTransforms();int activations=0;gaze.Activated+=t=>{if(t==station.Attack||t==station.Defense)activations++;};
 float until=Time.time+.8f;while(Time.time<until){Input(camera,station.Attack,false);yield return null;}
 Check(gaze.FocusedTarget==station.Attack,"Actual PC physics ray focuses ATAQUE");
 Check(gaze.DwellProgress==0&&station.AttackProgress.fillAmount==0&&activations==0,"PC hover never accumulates invisible dwell");Capture("PC-0.png");
 float started=Time.time,previous=0;int samples=0;until=Time.time+5;
 while(gaze.DwellProgress<.49f&&Time.time<until){Input(camera,station.Attack,true);yield return null;float value=gaze.DwellProgress;
 Check(value+1e-5f>=previous,"Held progress is monotonic sample "+samples);
 Check(Mathf.Abs(station.AttackProgress.fillAmount-value)<.001f,"Rendered fill equals real dwell sample "+samples);
 Check(Mathf.Abs(value-previous-Time.deltaTime/1.5f)<.025f,"Linear fill increment sample "+samples);
 previous=value;samples++;}
 Check(activations==0&&gaze.DwellProgress>=.49f&&gaze.DwellProgress<.59f,"PC hold near 0.75 s is approximately 50 percent; no instant click activation");
 Capture("PC-50.png");File.AppendAllText(Dir+"/samples.txt","PC half: "+gaze.DwellProgress+" rendered="+station.AttackProgress.fillAmount+"\n");
 Input(camera,station.Attack,false);yield return null;Input(camera,station.Attack,false);yield return null;
 Check(gaze.DwellProgress==0&&station.AttackProgress.fillAmount==0&&activations==0,"Release at half resets both real and displayed progress");Capture("PC-release-0.png");
 until=Time.time+.5f;while(Time.time<until){Input(camera,station.Attack,false);yield return null;}
 Check(gaze.DwellProgress==0,"Released input never keeps charging");
 Input(camera,station.Attack,true);yield return null;Input(camera,station.Attack,true);yield return null;
 Check(gaze.DwellProgress<.2f&&activations==0,"Repress starts from zero, never near completion");
 until=Time.time+5;while(gaze.DwellProgress<.49f&&Time.time<until){Input(camera,station.Attack,true);yield return null;}
 Input(camera,station.Defense,true);yield return null;Input(camera,station.Defense,true);yield return null;
 Check(gaze.FocusedTarget==station.Defense&&gaze.DwellProgress<.2f&&station.AttackProgress.fillAmount==0,"PC card change restarts charge without transfer");
 previous=gaze.DwellProgress;until=Time.time+5;
 while(activations==0&&Time.time<until){Input(camera,station.Defense,true);yield return null;
 Check(gaze.DwellProgress+1e-5f>=previous&&Mathf.Abs(station.DefenseProgress.fillAmount-gaze.DwellProgress)<.001f,"PC progress stays visible and uniform until completion");previous=gaze.DwellProgress;}
 Check(activations==1&&gaze.DwellProgress==1&&station.ConfirmationVisible&&!game.IntroPlaying,"Only full 1.5 s hold confirms once before prologue");Capture("PC-100.png");
 until=Time.time+1;while(Time.time<until){Input(camera,station.Defense,true);yield return null;}
 Check(activations==1&&game.IntroPlaying,"Holding after completion never repeats selection");
 Check(gaze.DwellProgress==0&&station.DefenseProgress.fillAmount==0,"Prologue clears real charge as well as displayed fill");
 game.SkipIntro();Input(camera,station.Defense,false);yield return null;Input(camera,station.Defense,true);yield return null;
 Check(gaze.DwellProgress<.2f&&activations==1,"Repress after prologue never resumes an almost-complete charge");
 camera.transform.rotation=Quaternion.LookRotation(Vector3.up);InputSystem.QueueStateEvent(mouse,new MouseState{position=new Vector2(camera.pixelWidth*.5f,camera.pixelHeight*.5f)});InputSystem.Update();yield return null;yield return null;
 Check(gaze.FocusedTarget==null&&gaze.DwellProgress==0,"Looking away rearms without retained charge");
 }
}}

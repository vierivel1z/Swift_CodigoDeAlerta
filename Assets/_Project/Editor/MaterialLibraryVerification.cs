using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

namespace Swift.EditorTools {
    // Read-only verification of the committed catalog. Never saves or rebuilds a scene/material.
    [InitializeOnLoad] public static class MaterialLibraryVerification {
        const string Dir="Artifacts/MaterialLibraryVerify";
        const string ScenePath="Assets/_Project/MaterialLibrary/Showcase/MaterialLibrary_Studio.unity";
        const string Pending="SWIFT_MATERIAL_VERIFY_PLAY";
        static bool running;static int errors,warnings,frames;static double deadline;
        static string Session=>File.ReadAllText(Dir+"/session-path.txt").Trim();
        static string Gallery=>File.ReadAllText(Dir+"/gallery-path.txt").Trim();
        static MaterialLibraryVerification(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=StateChanged;}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
        static void Check(bool ok,string text){if(!ok)throw new Exception(text);File.AppendAllText(Session+"/checks.txt","PASS "+text+"\n");}
        static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors++;if(type==LogType.Warning)warnings++;File.AppendAllText(Session+"/test-console.txt",type+": "+message+"\n");}
        static void Tick(){
            if(running){if(EditorApplication.isPlaying&&EditorApplication.timeSinceStartup>deadline&&++frames>3){try{Validate();Capture("Play");Check(errors==0,"No new runtime errors");File.WriteAllText(Session+"/play-result.txt","PASS Play Mode "+DateTime.UtcNow.ToString("O")+" errors="+errors+" warnings="+warnings+" EDITOR ONLY; ANDROID NOT TESTED");}catch(Exception e){File.WriteAllText(Session+"/play-result.txt","FAIL "+e);}finally{running=false;Application.logMessageReceived-=Log;EditorApplication.ExitPlaymode();}}return;}
            var q=Dir+"/command.txt";if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning)return;
            try{File.ReadAllText(q);File.Delete(q);Check(SceneManager.sceneCount<=1,"No multiple user scenes replaced");Check(Enumerable.Range(0,SceneManager.sceneCount).All(i=>!SceneManager.GetSceneAt(i).isDirty),"No unsaved user changes discarded");
                errors=warnings=0;Application.logMessageReceived+=Log;EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);Validate();Capture("Editor");
                File.WriteAllText(Session+"/editor-result.txt","PASS reopen and render "+DateTime.UtcNow.ToString("O")+" Unity="+Application.unityVersion+" errors="+errors+" warnings="+warnings);
                Check(errors==0,"No errors while reopening/rendering catalog");SessionState.SetBool(Pending,true);Application.logMessageReceived-=Log;EditorApplication.EnterPlaymode();
            }catch(Exception e){Application.logMessageReceived-=Log;File.WriteAllText(Session+"/editor-result.txt","FAIL "+e);}
        }
        static void StateChanged(PlayModeStateChange state){
            if(state==PlayModeStateChange.ExitingEditMode&&SessionState.GetBool(Pending,false)){Application.logMessageReceived-=Log;Application.logMessageReceived+=Log;}
            if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Pending,false)){running=true;errors=warnings=frames=0;Application.logMessageReceived-=Log;Application.logMessageReceived+=Log;deadline=EditorApplication.timeSinceStartup+3;}
            if(state==PlayModeStateChange.EnteredEditMode&&SessionState.GetBool(Pending,false)){SessionState.EraseBool(Pending);Application.logMessageReceived-=Log;File.WriteAllText(Session+"/complete.txt","Verification exited Play Mode "+DateTime.UtcNow.ToString("O"));}
        }
        static void Validate(){
            Check(SceneManager.GetActiveScene().path==ScenePath,"Catalog scene active");
            typeof(MaterialLibraryReview).GetMethod("Validate",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
            var mats=All<MeshRenderer>().Where(r=>r.GetComponent<TextMeshPro>()==null).SelectMany(r=>r.sharedMaterials).Distinct().ToArray();Check(mats.Length==13,"12 reusable samples plus one neutral control");Check(mats.All(m=>m!=null&&m.shader!=null&&m.shader.isSupported),"All visible mesh shaders supported");
            Check(mats.All(m=>m.shader.name=="Universal Render Pipeline/Lit"),"All mesh materials URP Lit");
            Check(All<Transform>().Count(t=>t.name.StartsWith("Sample tile "))==12,"12 flat samples");
            foreach(var name in new[]{"Sphere ","Cube ","Cylinder "})Check(All<Transform>().Count(t=>t.name.StartsWith(name))==12,"12 samples of "+name);
            var probe=All<ReflectionProbe>().Single();Check(probe.mode==ReflectionProbeMode.Custom&&probe.customBakedTexture is Cubemap&&probe.customBakedTexture.width==128,"Persistent offline cubemap128 restored");
            Check(All<Light>().Length==2&&All<Light>().All(l=>l.color==Color.white),"Two neutral white studio lights");
            Check(RenderSettings.skybox==null&&!RenderSettings.fog,"No skybox color cast or fog");
            Check(All<MonoBehaviour>().All(x=>x!=null),"No missing MonoBehaviours");
            foreach(var text in All<TextMeshPro>()){text.ForceMeshUpdate();Check(!text.isTextOverflowing,"TMP fits "+text.name);}
            File.WriteAllText(Session+"/render-configuration.txt","Unity="+Application.unityVersion+"\nAPI="+SystemInfo.graphicsDeviceType+"\nBuildTarget="+EditorUserBuildSettings.activeBuildTarget+"\nPipeline="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline)+"\nEditor only; NOT Android performance test");
        }
        static void Capture(string phase){
            var cam=All<Camera>().Single();var p=cam.transform.position;var q=cam.transform.rotation;var size=cam.orthographicSize;var aspect=cam.aspect;var enabled=cam.enabled;var previous=RenderTexture.active;
            Directory.CreateDirectory(Gallery+"/"+phase);RenderTexture rt=null;Texture2D image=null;
            try{cam.enabled=false;cam.aspect=1600f/900;rt=new RenderTexture(1600,900,24);Check(rt.Create(),"RenderTexture allocated");
                for(int i=-1;i<6;i++){
                    var origin=i<0?new Vector3(0,2.8f,0):new Vector3((i%3-1)*4.7f,(i<3?3.6f:.45f)+.65f,0);cam.transform.position=origin+Vector3.back*(i<0?15:8);cam.transform.LookAt(origin);cam.orthographicSize=i<0?4.15f:1.45f;
                    var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};Check(RenderPipeline.SupportsRenderRequest(cam,request),"URP request supported");RenderPipeline.SubmitRenderRequest(cam,request);RenderTexture.active=rt;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();var name=(i+1).ToString("00")+".png";File.WriteAllBytes(Gallery+"/"+phase+"/"+name,image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);image=null;
                    File.AppendAllText(Gallery+"/receipt.txt",DateTime.UtcNow.ToString("O")+" "+phase+"/"+name+" origin="+origin+" orthographicSize="+cam.orthographicSize+" EditorPlay="+Application.isPlaying+" NOT_ANDROID\n");
                }
            }finally{cam.transform.SetPositionAndRotation(p,q);cam.orthographicSize=size;cam.aspect=aspect;cam.enabled=enabled;RenderTexture.active=previous;if(image!=null)UnityEngine.Object.DestroyImmediate(image);if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}}
        }
    }
}

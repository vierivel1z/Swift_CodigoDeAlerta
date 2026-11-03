using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

namespace Swift.EditorTools {
    // Explicit Editor-only commands. Never changes an existing scene or source material.
    [InitializeOnLoad] public static class MaterialLibraryReview {
        const string Dir="Artifacts/MaterialLibrary", Out="Assets/_Project/MaterialLibrary";
        const string ScenePath=Out+"/Showcase/MaterialLibrary_Studio.unity";
        const string Gallery="Docs/Previews/MaterialLibrary";
        const string Existing="Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/";
        static readonly string[] Families={"MADERA","PLASTICO","METAL PINTADO","TELA","PAPEL","ARQUITECTURA"};
        static readonly string[] Names={"Wood_DesaturatedOak","Wood_WalnutInset","Plastic_EquipmentMatte","Plastic_Keycaps","Metal_GraphitePaint","Metal_ArchiveEnamel","Fabric_AcousticSlate","Fabric_SeatCharcoal","Paper_Ivory","Paper_StackEdges","Architecture_Mineral","Architecture_CarpetSlate"};
        static readonly string[] Labels={"Roble desaturado","Nogal / inserto","Carcasa mate","Teclas","Grafito pintado","Esmalte de archivo","Mampara acustica","Asiento carbon","Papel marfil","Cantos de papel","Pared mineral","Moqueta pizarra"};
        static readonly string[] Uses={"Tableros y estanterias","Cajones e interiores","Monitor, PC e impresora","Teclados y controles fisicos","Marcos, soportes y patas","Archivadores y gabinetes","Mamparas y separadores","Sillas y sofa","Hojas y libretas","Pilas de documentos","Paredes y revestimientos","Suelo de puestos"};
        [Serializable] class Entry {public string name,path,status,family,application,shader,baseTexture; public float smoothness,metallic; public Color color;}
        [Serializable] class Library {public string unity,urp,scene;public List<Entry> materials=new List<Entry>();public string studioMaterial,studioStatus;}
        static MaterialLibraryReview(){EditorApplication.update+=Tick;}
        static void Check(bool condition,string message){if(!condition)throw new Exception(message);File.AppendAllText(Dir+"/checks.txt","PASS "+message+"\n");}
        static T[] All<T>()where T:Component=>SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<T>(true)).ToArray();
        static void Tick(){
            var q=Dir+"/command.txt";if(!File.Exists(q)||EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode||Lightmapping.isRunning)return;
            string cmd;try{cmd=File.ReadAllText(q).Trim();File.Delete(q);}catch(IOException){return;}
            try{if(cmd=="create")Create();else if(cmd=="refine-lighting"){Check(SceneManager.GetActiveScene().path==ScenePath,"Only studio lighting refined");var control=AssetDatabase.LoadAssetAtPath<Material>(Out+"/Materials/Studio_NeutralGrey_Matte.mat");control.SetFloat("_ReceiveShadows",0);control.EnableKeyword("_RECEIVE_SHADOWS_OFF");EditorUtility.SetDirty(control);foreach(var r in All<MeshRenderer>().Where(r=>r.name.StartsWith("Studio")||r.name=="Neutral background")){r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;}var fill=All<Light>().Single(l=>l.name=="Neutral fill");fill.transform.eulerAngles=new Vector3(15,50,0);fill.intensity=.45f;var probe=All<ReflectionProbe>().Single();probe.mode=ReflectionProbeMode.Baked;var hidden=All<Renderer>().Where(r=>r.name.StartsWith("Sample tile")||r.name.StartsWith("Sphere")||r.name.StartsWith("Cube")||r.name.StartsWith("Cylinder")||r is TextMeshPro).ToArray();try{foreach(var r in hidden)r.forceRenderingOff=true;Check(Lightmapping.BakeReflectionProbe(probe,Out+"/Showcase/Studio_NeutralReflection_128.exr"),"Neutral reflection rebaked without colored samples");}finally{foreach(var r in hidden)r.forceRenderingOff=false;}AssetDatabase.Refresh();BindReflection(probe);Validate();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Neutral studio lighting saved");AssetDatabase.SaveAssets();CaptureAll();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Catalog saved after final captures");}
                else if(cmd=="refine-labels"){Check(SceneManager.GetActiveScene().path==ScenePath,"Only isolated labels refined");foreach(var t in All<TextMeshPro>()){Undo.RecordObject(t,"Catalog label legibility");if(t.name.StartsWith("Family")){t.fontSize=3;t.rectTransform.sizeDelta=new Vector2(4.5f,.7f);}else{t.fontSize=2;t.rectTransform.sizeDelta=new Vector2(2.02f,.8f);t.transform.position+=Vector3.down*.1f;}}Validate();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Label layout saved");CaptureAll();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Only isolated catalog saved");}
                else if(cmd=="reopen"){Check(SceneManager.GetActiveScene().path==ScenePath&&!SceneManager.GetActiveScene().isDirty,"Clean catalog before reopening");EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);Validate();CaptureAll();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Reopened catalog saved");}
                else if(cmd=="capture"){Check(SceneManager.GetActiveScene().path==ScenePath,"Only catalog captured");CaptureAll();Validate();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Catalog saved after temporary captures");}else throw new Exception("Unknown material library command");
                File.WriteAllText(Dir+"/"+cmd+"-result.txt","PASS "+cmd+" "+DateTime.UtcNow.ToString("O")+" Unity="+Application.unityVersion+" EDITOR ONLY; ANDROID NOT TESTED");}
            catch(Exception e){File.WriteAllText(Dir+"/"+cmd+"-result.txt","FAIL "+cmd+" "+e);Debug.LogException(e);}
        }
        static string F(float v)=>v.ToString("R",CultureInfo.InvariantCulture);
        static string Signature(Material m){
            var b=new System.Text.StringBuilder(m.shader.name+"|queue="+m.renderQueue+"|keywords="+string.Join(",",m.shaderKeywords.OrderBy(x=>x)));
            for(int i=0;i<ShaderUtil.GetPropertyCount(m.shader);i++){var p=ShaderUtil.GetPropertyName(m.shader,i);b.Append('|').Append(p).Append('=');switch(ShaderUtil.GetPropertyType(m.shader,i)){
                case ShaderUtil.ShaderPropertyType.Color:var c=m.GetColor(p);b.Append(F(c.r)+","+F(c.g)+","+F(c.b)+","+F(c.a));break;
                case ShaderUtil.ShaderPropertyType.Vector:var v=m.GetVector(p);b.Append(F(v.x)+","+F(v.y)+","+F(v.z)+","+F(v.w));break;
                case ShaderUtil.ShaderPropertyType.TexEnv:var t=m.GetTexture(p);b.Append(AssetDatabase.GetAssetPath(t)).Append(m.GetTextureScale(p).ToString("F6")).Append(m.GetTextureOffset(p).ToString("F6"));break;
                default:b.Append(F(m.GetFloat(p)));break;
            }}return b.ToString();
        }
        static void Inventory(){
            var rows=new List<string>{"path\tname\tshader\tbaseColor\tsmoothness\tmetallic\tbaseTexture"};var materials=new List<Material>();
            foreach(var guid in AssetDatabase.FindAssets("t:Material",new[]{"Assets/_Project/OfficeVisual","Assets/Models"}))materials.AddRange(AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)).OfType<Material>());
            materials=materials.Distinct().ToList();foreach(var m in materials)rows.Add(AssetDatabase.GetAssetPath(m)+"\t"+m.name+"\t"+m.shader.name+"\t"+(m.HasProperty("_BaseColor")?m.GetColor("_BaseColor").ToString():"n/a")+"\t"+(m.HasProperty("_Smoothness")?F(m.GetFloat("_Smoothness")):"n/a")+"\t"+(m.HasProperty("_Metallic")?F(m.GetFloat("_Metallic")):"n/a")+"\t"+(m.HasProperty("_BaseMap")?AssetDatabase.GetAssetPath(m.GetTexture("_BaseMap")):"n/a"));
            File.WriteAllLines(Dir+"/existing-materials.tsv",rows);
            File.WriteAllLines(Dir+"/equivalent-material-groups.txt",materials.Where(m=>m.shader.name=="Universal Render Pipeline/Lit").GroupBy(Signature).Where(g=>g.Count()>1).Select(g=>string.Join("\n",g.Select(m=>AssetDatabase.GetAssetPath(m)+" :: "+m.name))+"\n"));
            File.WriteAllLines(Dir+"/model-material-bindings.tsv",AssetDatabase.FindAssets("t:Model",new[]{"Assets/Models"}).Select(AssetDatabase.GUIDToAssetPath).SelectMany(p=>{var model=AssetDatabase.LoadAssetAtPath<GameObject>(p);return model==null?new string[0]:model.GetComponentsInChildren<Renderer>(true).Select(r=>p+"\t"+r.name+"\t"+string.Join(",",r.sharedMaterials.Select(m=>m?.name)));}));
        }
        static GameObject Shape(string name,PrimitiveType type,Vector3 p,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.position=p;g.transform.localScale=scale;g.GetComponent<MeshRenderer>().sharedMaterial=m;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
        static TextMeshPro Text(string name,string value,Vector3 p,Vector2 size,float font,Color color){var go=new GameObject(name);go.transform.position=p;var t=go.AddComponent<TextMeshPro>();t.font=TMP_Settings.defaultFontAsset;t.text=value;t.fontSize=font;t.color=color;t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=size;t.textWrappingMode=TextWrappingModes.NoWrap;t.enableAutoSizing=false;return t;}
        static void Create(){
            Check(SceneManager.sceneCount==1&&!SceneManager.GetActiveScene().isDirty,"Current scene clean; no user edits discarded");Check(!File.Exists(ScenePath),"No existing catalog replaced");
            Inventory();Directory.CreateDirectory(Out+"/Showcase");Directory.CreateDirectory(Out+"/Materials");AssetDatabase.Refresh();
            var neutral=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Studio_NeutralGrey_Matte"};neutral.SetColor("_BaseColor",new Color(.46f,.46f,.46f,1));neutral.SetFloat("_Smoothness",.08f);neutral.SetFloat("_Metallic",0);neutral.SetFloat("_ReceiveShadows",0);neutral.EnableKeyword("_RECEIVE_SHADOWS_OFF");
            var match=AssetDatabase.FindAssets("t:Material",new[]{"Assets/_Project/OfficeVisual"}).Select(g=>AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g))).FirstOrDefault(m=>m!=null&&m.shader==neutral.shader&&Signature(m)==Signature(neutral));
            string studioStatus;if(match!=null){UnityEngine.Object.DestroyImmediate(neutral);neutral=match;studioStatus="REUSED";}else{AssetDatabase.CreateAsset(neutral,Out+"/Materials/Studio_NeutralGrey_Matte.mat");studioStatus="NEW - neutral calibration only";}
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            RenderSettings.skybox=null;RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.48f,.48f,.48f);RenderSettings.ambientIntensity=1;RenderSettings.fog=false;RenderSettings.reflectionIntensity=.7f;
            Shape("Neutral background",PrimitiveType.Cube,new Vector3(0,3,1.6f),new Vector3(30,16,.12f),neutral);
            Shape("Studio floor",PrimitiveType.Cube,new Vector3(0,-1,0),new Vector3(32,.1f,20),neutral);
            Shape("Studio ceiling reflection card",PrimitiveType.Cube,new Vector3(0,10,0),new Vector3(32,.1f,20),neutral);
            LightRig("Neutral key",new Vector3(40,-30,0),1.4f,true);LightRig("Neutral fill",new Vector3(15,50,0),.45f,false);foreach(var r in All<MeshRenderer>()){r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;}
            var probe=new GameObject("Neutral studio reflection 128").AddComponent<ReflectionProbe>();probe.transform.position=new Vector3(0,3,-1);probe.mode=ReflectionProbeMode.Baked;probe.resolution=128;probe.size=new Vector3(30,18,18);probe.clearFlags=ReflectionProbeClearFlags.SolidColor;probe.backgroundColor=new Color(.46f,.46f,.46f);probe.intensity=.7f;
            Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath),"Isolated neutral studio created with new scene GUID");
            Check(Lightmapping.BakeReflectionProbe(probe,Out+"/Showcase/Studio_NeutralReflection_128.exr"),"Neutral reflection baked before color samples");AssetDatabase.Refresh();BindReflection(probe);
            var library=new Library{unity=Application.unityVersion,urp="17.3.0",scene=ScenePath,studioMaterial=AssetDatabase.GetAssetPath(neutral),studioStatus=studioStatus};
            for(int family=0;family<6;family++){
                var origin=new Vector3((family%3-1)*4.7f,family<3?3.6f:.45f,0);
                Text("Family "+family,Families[family],origin+new Vector3(0,1.55f,-.35f),new Vector2(4.5f,.7f),3f,new Color(.95f,.95f,.95f));
                for(int variant=0;variant<2;variant++){
                    int id=family*2+variant;var m=AssetDatabase.LoadAssetAtPath<Material>(Existing+Names[id]+".mat");Check(m!=null,"Reuse canonical "+Names[id]);var p=origin+Vector3.right*(variant==0?-1.08f:1.08f);
                    Shape("Sample tile "+id,PrimitiveType.Cube,p+new Vector3(0,.65f,.40f),new Vector3(1.95f,1.05f,.065f),m);
                    Shape("Shelf "+id,PrimitiveType.Cube,p+new Vector3(0,.05f,-.15f),new Vector3(1.95f,.08f,.95f),neutral);
                    Shape("Sphere "+id,PrimitiveType.Sphere,p+new Vector3(-.61f,.44f,-.25f),Vector3.one*.65f,m);
                    var cube=Shape("Cube "+id,PrimitiveType.Cube,p+new Vector3(.02f,.43f,-.25f),new Vector3(.48f,.64f,.48f),m);cube.transform.rotation=Quaternion.Euler(0,22,0);
                    Shape("Cylinder "+id,PrimitiveType.Cylinder,p+new Vector3(.64f,.43f,-.25f),new Vector3(.40f,.32f,.40f),m);
                    Text("Label "+id,(id+1).ToString("00")+"  "+Labels[id]+"\nS "+m.GetFloat("_Smoothness").ToString("0.00",CultureInfo.InvariantCulture)+"   M "+m.GetFloat("_Metallic").ToString("0.00",CultureInfo.InvariantCulture),p+new Vector3(0,-.40f,-.68f),new Vector2(2.02f,.8f),2f,new Color(.95f,.95f,.95f));
                    library.materials.Add(new Entry{name=Names[id],path=AssetDatabase.GetAssetPath(m),status="REUSED - GraphicsPolishV2",family=Families[family],application=Uses[id],shader=m.shader.name,smoothness=m.GetFloat("_Smoothness"),metallic=m.GetFloat("_Metallic"),color=m.GetColor("_BaseColor"),baseTexture=AssetDatabase.GetAssetPath(m.GetTexture("_BaseMap"))});
                }
            }
            var camera=new GameObject("Catalog camera - NOT XR").AddComponent<Camera>();camera.transform.position=new Vector3(0,2.8f,-15);camera.transform.LookAt(new Vector3(0,2.8f,0));camera.orthographic=true;camera.orthographicSize=4.15f;camera.nearClipPlane=.05f;camera.farClipPlane=60;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.30f,.30f,.30f);camera.GetUniversalAdditionalCameraData().renderPostProcessing=false;
            File.WriteAllText(Out+"/MaterialLibrary_Index.json",JsonUtility.ToJson(library,true));AssetDatabase.Refresh();AssetDatabase.SaveAssets();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Catalog saved with references to original material GUIDs");Validate();CaptureAll();Check(EditorSceneManager.SaveScene(SceneManager.GetActiveScene()),"Only catalog saved after capture");
        }
        static void BindReflection(ReflectionProbe probe){var cube=AssetDatabase.LoadAssetAtPath<Cubemap>(Out+"/Showcase/Studio_NeutralReflection_128.exr");Check(cube!=null,"Neutral cubemap imported as Cube");probe.mode=ReflectionProbeMode.Custom;probe.customBakedTexture=cube;EditorUtility.SetDirty(probe);}
        static void LightRig(string name,Vector3 euler,float intensity,bool shadows){var l=new GameObject(name).AddComponent<Light>();l.type=LightType.Directional;l.color=Color.white;l.intensity=intensity;l.transform.eulerAngles=euler;l.shadows=shadows?LightShadows.Soft:LightShadows.None;}
        static void Validate(){
            foreach(var name in Names){var m=AssetDatabase.LoadAssetAtPath<Material>(Existing+name+".mat");Check(m.shader.name=="Universal Render Pipeline/Lit"&&m.shader.isSupported,"Supported URP Lit "+name);Check(m.GetFloat("_Surface")==0,"Opaque "+name);Check(m.GetFloat("_Smoothness")<=.4f,"Controlled smoothness "+name);var t=m.GetTexture("_BaseMap");Check(t==null||(t.width<=256&&t.height<=256),"Small texture or no texture "+name);}
            Check(All<ReflectionProbe>().Single().customBakedTexture!=null,"Persistent offline reflection bound to scene");Check(All<Collider>().Length==0,"No colliders or gameplay in catalog");Check(All<Camera>().Length==1,"One non-XR catalog camera");
            foreach(var t in All<TextMeshPro>()){t.ForceMeshUpdate();Check(!t.isTextOverflowing,"Label fits "+t.name);}
        }
        static void CaptureAll(){Capture("00-catalogo.png",new Vector3(0,2.8f,-15),new Vector3(0,2.8f,0),4.15f);for(int family=0;family<6;family++){var p=new Vector3((family%3-1)*4.7f,(family<3?3.6f:.45f)+.65f,0);Capture((family+1).ToString("00")+"-"+Families[family].ToLowerInvariant().Replace(' ','-')+".png",p+Vector3.back*8,p,1.45f);}}
        static void Capture(string file,Vector3 eye,Vector3 target,float size){
            Directory.CreateDirectory(Gallery);var g=new GameObject("Temporary material capture"){hideFlags=HideFlags.HideAndDontSave};var previous=RenderTexture.active;RenderTexture rt=null;Texture2D tex=null;
            try{var c=g.AddComponent<Camera>();c.enabled=false;c.transform.position=eye;c.transform.LookAt(target);c.orthographic=true;c.orthographicSize=size;c.nearClipPlane=.05f;c.farClipPlane=60;c.aspect=1600f/900;c.stereoTargetEye=StereoTargetEyeMask.None;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(.30f,.30f,.30f);c.GetUniversalAdditionalCameraData().renderPostProcessing=false;rt=new RenderTexture(1600,900,24);Check(rt.Create(),"Capture allocated");var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};Check(RenderPipeline.SupportsRenderRequest(c,request),"URP capture supported");RenderPipeline.SubmitRenderRequest(c,request);RenderTexture.active=rt;tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();File.WriteAllBytes(Gallery+"/"+file,tex.EncodeToPNG());File.AppendAllText(Gallery+"/capture-receipt.txt",DateTime.UtcNow.ToString("O")+" "+file+" eye="+eye+" target="+target+" ortho="+size+" Unity="+Application.unityVersion+" RP="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline)+" EditorOnly NOT_ANDROID\n");}
            finally{RenderTexture.active=previous;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}UnityEngine.Object.DestroyImmediate(g);}
        }
    }
}

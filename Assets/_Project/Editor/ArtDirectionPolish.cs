using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Swift.Interaction;

namespace Swift.EditorTools
{
    // Explicit Editor commands only. No scene or asset is changed on import.
    public static class ArtDirectionPolish
    {
        const string Main = "Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Variant = "Assets/_Project/Scenes/Review/BancoCorresponsal_ArtDirection_20261110.unity";
        const string Materials = "Assets/_Project/OfficeVisual/GraphicsPolishV2/Materials/";
        static string Root => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        static string Gallery => Path.Combine(Root, "Docs/Previews/ArtDirectionPolish");
        static string Artifacts => Path.Combine(Root, "Artifacts/ArtDirectionPolish");

        sealed class View
        {
            public readonly string File;
            public readonly Vector3 Eye, Look;
            public readonly float Ortho;
            public View(string file, Vector3 eye, Vector3 look, float ortho = 0) { File = file; Eye = eye; Look = look; Ortho = ortho; }
        }

        static readonly View[] Views = {
            new View("01-entrada.png",new Vector3(0,1.6f,-6.6f),new Vector3(0,1.45f,3.1f)),
            new View("02-general.png",new Vector3(8.8f,2.2f,-6.9f),new Vector3(0,1.3f,3.6f)),
            new View("03-cubiculos.png",new Vector3(-3.6f,1.65f,-3.1f),new Vector3(-6.6f,1.05f,0)),
            new View("04-puesto-pie.png",new Vector3(-1.8f,1.8f,-1.1f),new Vector3(.25f,1.2f,1.7f)),
            new View("05-puesto-sentado.png",new Vector3(0,1.25f,.05f),new Vector3(0,1.4f,1.8f)),
            new View("06-documentacion.png",new Vector3(-1.5f,1.65f,1),new Vector3(-4.2f,1.2f,4.3f)),
            new View("07-supervision.png",new Vector3(2.7f,1.6f,4.3f),new Vector3(7.4f,1.4f,8.2f)),
            new View("08-nocturna.png",new Vector3(0,1.6f,8.3f),new Vector3(7.7f,2,-.3f)),
            new View("09-planta.png",new Vector3(0,21,1),new Vector3(0,0,1),11)
        };

        public static void Baseline()
        {
            var scene = EditorSceneManager.OpenScene(Main, OpenSceneMode.Single);
            Require(!scene.isDirty && !EditorApplication.isPlayingOrWillChangePlaymode, "Clean saved scene required");
            Directory.CreateDirectory(Artifacts);
            File.WriteAllText(Path.Combine(Artifacts,"main-before.sha256"), Hash(Path.Combine(Root,Main)));
            Inventory("Before");
            Capture("Before");
            Debug.Log("SWIFT_ART_DIRECTION_BASELINE_PASS " + Gallery);
        }

        public static void MaterialVariant()
        {
            Require(!AssetDatabase.LoadAssetAtPath<SceneAsset>(Variant), "Never overwrite an existing review scene");
            string original = Hash(Path.Combine(Root,Main));
            Require(AssetDatabase.CopyAsset(Main,Variant), "Copy through AssetDatabase must succeed");
            var scene = EditorSceneManager.OpenScene(Variant, OpenSceneMode.Single);
            var changes = ApplyMaterialFamilies();
            Require(changes > 0, "At least one material should be reassigned");
            Require(EditorSceneManager.SaveScene(scene), "Save isolated variant");
            Require(Hash(Path.Combine(Root,Main)) == original, "Main scene remains byte-identical");
            Directory.CreateDirectory(Artifacts);
            File.WriteAllText(Path.Combine(Artifacts,"material-changes.txt"),changes.ToString());
            Inventory("AfterMaterials");
            Capture("AfterMaterials");
            Debug.Log("SWIFT_ART_DIRECTION_MATERIAL_PASS reassigned=" + changes);
        }

        public static void ArchitectureVariant()
        {
            var scene=EditorSceneManager.OpenScene(Variant,OpenSceneMode.Single);
            Require(!scene.isDirty,"Saved variant required");
            string original=Hash(Path.Combine(Root,Main));
            int pieces=ApplyArchitecture();
            Require(EditorSceneManager.SaveScene(scene),"Save architecture variant");
            Require(Hash(Path.Combine(Root,Main))==original,"Main scene remains unchanged");
            File.WriteAllText(Path.Combine(Artifacts,"architecture-pieces.txt"),pieces.ToString());
            Inventory("AfterArchitecture");Capture("AfterArchitecture");
            Debug.Log("SWIFT_ART_DIRECTION_ARCHITECTURE_PASS pieces="+pieces);
        }

        public static void LightingVariant()
        {
            var scene=EditorSceneManager.OpenScene(Variant,OpenSceneMode.Single);
            Require(!scene.isDirty,"Saved architecture variant required");
            string original=Hash(Path.Combine(Root,Main));
            int adjusted=ApplyLightingHierarchy();
            Require(EditorSceneManager.SaveScene(scene),"Save light settings before bake");
            Require(Lightmapping.Bake(),"Bake variant lighting");
            Require(EditorSceneManager.SaveScene(scene),"Save baked variant");
            Require(LightmapSettings.lightmaps.Length>0,"Variant has lightmaps");
            Require(Hash(Path.Combine(Root,Main))==original,"Main scene remains unchanged after bake");
            File.WriteAllText(Path.Combine(Artifacts,"lighting-adjustments.txt"),adjusted.ToString());
            Inventory("AfterLighting");Capture("AfterLighting");
            Debug.Log("SWIFT_ART_DIRECTION_LIGHTING_PASS adjusted="+adjusted);
        }

        public static void ApplyMain()
        {
            Require(!EditorApplication.isPlayingOrWillChangePlaymode,"Apply only outside Play Mode");
            Require(SceneManager.sceneCount==1&&!SceneManager.GetActiveScene().isDirty,"Do not discard an unsaved scene");
            Require(Hash(Path.Combine(Root,Main))==File.ReadAllText(Path.Combine(Artifacts,"main-before.sha256")),"Main scene changed since the baseline");
            Require(Directory.GetDirectories(Artifacts,"Backup-*").Any(d=>File.Exists(Path.Combine(d,"BancoCorresponsal.unity"))&&Hash(Path.Combine(d,"BancoCorresponsal.unity"))==Hash(Path.Combine(Root,Main))),"Recoverable byte-identical main backup required");
            Require(Hash(Path.Combine(Root,Variant))==File.ReadAllLines(Path.Combine(Artifacts,"AfterLighting-inventory.txt")).First(x=>x.StartsWith("SceneSHA256=")).Substring(12),"Reviewed variant changed");
            foreach(var mode in new[]{"flow","office","smoke","clipboard","phone"})
                Require(File.ReadAllText(Path.Combine(Artifacts,"variant-"+mode+"-result.txt")).StartsWith("PASS "+mode+" "),"Variant Play test required: "+mode);
            var scene=EditorSceneManager.OpenScene(Main,OpenSceneMode.Single);
            int materials=ApplyMaterialFamilies(),pieces=ApplyArchitecture(),lights=ApplyLightingHierarchy();
            Require(materials>0&&pieces==5&&lights>=12,"Reviewed visual recipe applied");
            Require(EditorSceneManager.SaveScene(scene),"Save integrated scene before bake");
            Require(Lightmapping.Bake(),"Bake integrated lighting");
            Require(EditorSceneManager.SaveScene(scene),"Save integrated baked scene");
            Require(LightmapSettings.lightmaps.Length>0,"Integrated scene has baked lighting");
            Inventory("FinalMain");Capture("FinalMain");
            File.WriteAllText(Path.Combine(Artifacts,"main-after.sha256"),Hash(Path.Combine(Root,Main)));
            Debug.Log("SWIFT_ART_DIRECTION_MAIN_PASS materials="+materials+" pieces="+pieces+" lights="+lights);
        }

        static int ApplyArchitecture()
        {
            Require(GameObject.Find("Art direction - institutional details")==null,"Architecture stage runs once");
            var root=new GameObject("Art direction - institutional details");
            Undo.RegisterCreatedObjectUndo(root,"Reused institutional architecture");
            var grid=GameObject.Find("Modular ceiling grid")?.GetComponent<Renderer>();
            Require(grid!=null,"Existing modular grid present");Undo.RecordObject(grid,"Replace coarse grid");grid.enabled=false;
            var oldCity=GameObject.Find("Distant city office windows")?.GetComponent<Renderer>();
            Require(oldCity!=null,"Existing city windows present");Undo.RecordObject(oldCity,"Replace repeated city dots");oldCity.enabled=false;
            var specs=new[]{
                new[]{"V2_modular_ceiling_joints","Architecture_CeilingJoint","Ceiling joints"},
                new[]{"V2_window_sill_reveals","Metal_AluminiumTrim","Window sill reveals"},
                new[]{"V2_cubicle_plinth_channels","Metal_GraphitePaint","Cubicle plinths"},
                new[]{"V2_column_base_and_capital_collars","Metal_AluminiumTrim","Column collars"},
                new[]{"V2_sparse_city_lights","Exterior_WarmWindows","Sparse city lights"}
            };
            foreach(var spec in specs){
                var mesh=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/_Project/OfficeVisual/GraphicsPolishV2/Geometry/"+spec[0]+".asset");
                var material=AssetDatabase.LoadAssetAtPath<Material>(Materials+spec[1]+".mat");
                Require(mesh!=null&&material!=null,"Reusable V2 mesh and material: "+spec[0]);
                var go=new GameObject(spec[2],typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root.transform,false);
                go.GetComponent<MeshFilter>().sharedMesh=mesh;
                var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;
                renderer.shadowCastingMode=spec[0]=="V2_sparse_city_lights"?ShadowCastingMode.Off:ShadowCastingMode.On;
                GameObjectUtility.SetStaticEditorFlags(go,StaticEditorFlags.BatchingStatic |
                    (spec[0]=="V2_sparse_city_lights"?0:StaticEditorFlags.ContributeGI));
            }
            var column=AssetDatabase.LoadAssetAtPath<Material>(Materials+"Architecture_ColumnMineral.mat");
            foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.name=="Perimeter column")){
                Undo.RecordObject(r,"Quiet mineral columns");r.sharedMaterial=column;
            }
            return specs.Length;
        }

        static int ApplyLightingHierarchy()
        {
            int count=0;
            foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None)){
                if(!light.isActiveAndEnabled||light.lightmapBakeType!=LightmapBakeType.Baked)continue;
                Undo.RecordObject(light,"Restrained nocturnal office hierarchy");
                if(light.name=="Baked analyst task light"){
                    light.color=new Color(1f,.88f,.70f);light.intensity=3.5f;
                }else if(light.name.Contains("warm work light")||light.name=="Supervision work light"){
                    light.color=new Color(.91f,.95f,1f);light.intensity=light.name=="Supervision work light"?2.6f:2.2f;
                }else if(light.name=="Baked inspection soft fill"){
                    light.color=new Color(.98f,.94f,.88f);light.intensity=15f;
                }else if(light.name.Contains("ceiling")||light.name.Contains("Rear baked")){
                    light.color=new Color(.93f,.965f,1f);
                }else continue;
                count++;
            }
            Require(count>=12,"Expected baked zone lights found");
            Require(UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None)
                .Count(l=>l.isActiveAndEnabled&&l.lightmapBakeType!=LightmapBakeType.Baked)==2,"No extra realtime lights");
            return count;
        }

        static int ApplyMaterialFamilies()
        {
            Material Mat(string name) {
                var m = AssetDatabase.LoadAssetAtPath<Material>(Materials + name + ".mat");
                Require(m != null && m.shader != null && m.shader.isSupported, "Missing URP material " + name);
                return m;
            }
            var wood=Mat("Wood_DesaturatedOak"); var inset=Mat("Wood_WalnutInset");
            var metal=Mat("Metal_GraphitePaint"); var archive=Mat("Metal_ArchiveEnamel");
            var trim=Mat("Metal_AluminiumTrim"); var plastic=Mat("Plastic_EquipmentMatte");
            var keys=Mat("Plastic_Keycaps"); var fabric=Mat("Fabric_AcousticSlate");
            var seat=Mat("Fabric_SeatCharcoal"); var paper=Mat("Paper_Ivory");
            var edge=Mat("Paper_StackEdges"); var walls=Mat("Architecture_Mineral");
            var ceiling=Mat("Architecture_CeilingIvory"); var carpet=Mat("Architecture_CarpetSlate");
            var aisle=Mat("Architecture_CarpetAisle");
            var map=new Dictionary<string,Material> {
                {"Oak_Satin",wood},{"Oak_DrawerInset",inset},{"Oak_Worksurface",wood},
                {"Steel_Powdercoat",metal},{"Painted_Metal",metal},{"Window_Frame",metal},
                {"Acoustic_Woven",fabric},{"Seat_Upholstery",seat},
                {"Equipment_Matte",plastic},{"Equipment_Polymer",plastic},{"Paper_Warm",paper},
                {"Wall_Mineral",walls},{"Ceiling_Acoustic",ceiling},
                {"Carpet_BlueGrey",carpet},{"Aisle_Slate",aisle}
            };
            int changed=0;
            foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None)) {
                if(!r.gameObject.activeInHierarchy || !r.enabled) continue;
                string path=PathOf(r.transform);
                var current=r.sharedMaterials;
                var next=current.Select((m,i)=> {
                    if(m==null) return m;
                    if(path.Contains("Interactive Archive") || path.Contains("Supervisor files")) return i==0?archive:trim;
                    if(path.Contains("Ordered papers") || path.Contains("Fictional transfer documents")) return i==0?paper:edge;
                    if(path.Contains("Keyboard") && !path.Contains("Keyboard1")) return i==0?plastic:keys;
                    if(path.Contains("Mouse") && !path.Contains("Comp_Mouse")) return plastic;
                    if(r.name=="Ceiling cross beam" || r.name=="Supervision canopy") return metal;
                    return map.TryGetValue(m.name,out var replacement)?replacement:m;
                }).ToArray();
                if(current.SequenceEqual(next)) continue;
                Undo.RecordObject(r,"Institutional material family");
                r.sharedMaterials=next;
                if(PrefabUtility.IsPartOfPrefabInstance(r)) PrefabUtility.RecordPrefabInstancePropertyModifications(r);
                changed++;
            }
            return changed;
        }

        static void Inventory(string phase)
        {
            Directory.CreateDirectory(Artifacts);
            var renderers=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(r=>r.enabled && r.gameObject.activeInHierarchy).ToArray();
            var lights=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(l=>l.isActiveAndEnabled && l.gameObject.activeInHierarchy).ToArray();
            var rows=new List<string>{"Unity="+Application.unityVersion,"Scene="+SceneManager.GetActiveScene().path,
                "SceneSHA256="+Hash(Path.Combine(Root,SceneManager.GetActiveScene().path)),
                "MeshRenderers="+renderers.Length,"MeshMaterials="+renderers.SelectMany(r=>r.sharedMaterials).Distinct().Count(),
                "Triangles="+renderers.Sum(r=>{var f=r.GetComponent<MeshFilter>();return f!=null&&f.sharedMesh!=null?f.sharedMesh.triangles.Length/3:0;}),
                "BakedLights="+lights.Count(l=>l.lightmapBakeType==LightmapBakeType.Baked),
                "RealtimeLights="+lights.Count(l=>l.lightmapBakeType!=LightmapBakeType.Baked),
                "Lightmaps="+LightmapSettings.lightmaps.Length,"ReflectionProbes="+UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).Length,
                "--- LIGHTS ---"};
            rows.AddRange(lights.OrderBy(l=>l.name).Select(l=>l.name+" "+l.lightmapBakeType+" intensity="+l.intensity+" range="+l.range+" color="+l.color+" pos="+l.transform.position));
            rows.Add("--- FOCAL OBJECTS ---");
            rows.AddRange(renderers.Where(r=> {
                var n=r.name.ToLowerInvariant();return n.Contains("desk")||n.Contains("monitor")||n.Contains("screen")||n.Contains("ceiling")||n.Contains("cubicle")||n.Contains("chair")||n.Contains("lamp")||n.Contains("printer")||n.Contains("archive")||n.Contains("clipboard")||n.Contains("floor")||n.Contains("wall");
            }).OrderBy(r=>PathOf(r.transform)).Select(r=>PathOf(r.transform)+" bounds="+r.bounds+" mats="+string.Join(",",r.sharedMaterials.Select(m=>m==null?"NULL":m.name))));
            rows.Add("--- INTERACTIONS ---");
            rows.AddRange(UnityEngine.Object.FindObjectsByType<BankInteractable>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .OrderBy(x=>PathOf(x.transform)).Select(x=>PathOf(x.transform)+" action="+x.Action+" pos="+x.transform.position+" collider="+(x.GetComponent<Collider>()!=null?x.GetComponent<Collider>().bounds.ToString():"none")));
            File.WriteAllLines(Path.Combine(Artifacts,phase+"-inventory.txt"),rows);
        }

        static void Capture(string phase)
        {
            var scene=SceneManager.GetActiveScene();
            Require(!scene.isDirty && SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null, "Saved scene and GPU required for real captures");
            var dir=Path.Combine(Gallery,phase);Directory.CreateDirectory(dir);
            var receipt=new List<string>{"# "+phase,"", "Unity="+Application.unityVersion,
                "Scene="+scene.path,"SceneSHA256="+Hash(Path.Combine(Root,scene.path)),
                "Pipeline="+AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),
                "GPU="+SystemInfo.graphicsDeviceName,
                "Real URP Editor render. Monoscopic; Android/Cardboard not tested. Top view hides ceiling temporarily.",""};
            foreach(var view in Views){
                var file=Path.Combine(dir,view.File);Render(view,file);
                receipt.Add("- ["+view.File+"]("+view.File+") eye="+view.Eye+" look="+view.Look+" ortho="+view.Ortho+" sha256="+Hash(file));
            }
            File.WriteAllLines(Path.Combine(dir,"README.md"),receipt);
        }

        static void Render(View view,string file)
        {
            var ceilings=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(r=>r.name=="Ceiling" || r.name=="Modular ceiling grid" || r.name=="V2 modular ceiling joints").ToArray();
            var old=ceilings.Select(r=>r.forceRenderingOff).ToArray();
            var active=RenderTexture.active;
            var go=new GameObject("Art review camera - temporary"){hideFlags=HideFlags.HideAndDontSave};
            RenderTexture rt=null;Texture2D tex=null;
            try{
                var camera=go.AddComponent<Camera>();camera.enabled=false;
                camera.nearClipPlane=.05f;camera.farClipPlane=60;camera.clearFlags=CameraClearFlags.SolidColor;
                camera.backgroundColor=new Color(.08f,.1f,.12f);camera.aspect=1600f/900;camera.fieldOfView=60;
                camera.orthographic=view.Ortho>0;camera.orthographicSize=view.Ortho;
                camera.transform.position=view.Eye;var d=view.Look-view.Eye;
                camera.transform.rotation=Quaternion.LookRotation(d,Mathf.Abs(Vector3.Dot(d.normalized,Vector3.up))>.99f?Vector3.forward:Vector3.up);
                var urp=camera.GetUniversalAdditionalCameraData();urp.renderPostProcessing=false;urp.renderShadows=true;
                rt=new RenderTexture(1600,900,24,RenderTextureFormat.ARGB32);Require(rt.Create(),"Review render target created");
                if(view.Ortho>0)foreach(var c in ceilings)c.forceRenderingOff=true;
                var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};
                Require(RenderPipeline.SupportsRenderRequest(camera,request),"URP supports review render");
                RenderPipeline.SubmitRenderRequest(camera,request);RenderTexture.active=rt;
                tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();
                File.WriteAllBytes(file,tex.EncodeToPNG());
            }finally{
                for(int i=0;i<ceilings.Length;i++)if(ceilings[i]!=null)ceilings[i].forceRenderingOff=old[i];
                RenderTexture.active=active;if(tex!=null)UnityEngine.Object.DestroyImmediate(tex);
                if(rt!=null){rt.Release();UnityEngine.Object.DestroyImmediate(rt);}UnityEngine.Object.DestroyImmediate(go);
            }
        }

        static string PathOf(Transform t){var path=t.name;while(t.parent!=null){t=t.parent;path=t.name+"/"+path;}return path;}
        static string Hash(string path){using(var sha=SHA256.Create())using(var stream=File.OpenRead(path))return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");}
        static void Require(bool okay,string message){if(!okay)throw new InvalidOperationException(message);}
    }
}

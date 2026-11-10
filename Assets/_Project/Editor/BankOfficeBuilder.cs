using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using Swift.Game;
using Swift.Interaction;
using Swift.UI;

namespace Swift.EditorTools
{
    // Uses native FBX/OBJ importers; never changes pipeline, XR or source ZIPs.
    public static class BankOfficeBuilder
    {
        const string ScenePath = "Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string Output = "Assets/_Project/OfficeVisual";
        const string Kenney = "Assets/Models/KenneyFurniture/Models/FBX format/";
        static readonly Dictionary<string, Material> materials = new Dictionary<string, Material>();
        static GameManager manager;
        static Transform rig;
        static readonly List<string> audit = new List<string>();

        static Material Solid(string name, Color color)
        {
            string path = Output + "/Materials/" + name + ".mat";
            var value = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (value != null) return value;
            value = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
            value.SetColor("_BaseColor", color); value.SetFloat("_Smoothness", .1f);
            AssetDatabase.CreateAsset(value, path); return value;
        }
        static Material ConvertMaterial(Material original, string assetName, int slot)
        {
            var color = original != null && original.HasProperty("_Color") ? original.GetColor("_Color") : Color.gray;
            var texture = original != null && original.HasProperty("_MainTex") ? original.GetTexture("_MainTex") : null;
            if (texture != null && color.maxColorComponent < .01f) color = Color.white;
            string textureId = texture == null ? "none" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
            string key = ColorUtility.ToHtmlStringRGBA(color) + "_" + textureId;
            if (materials.TryGetValue(key, out var cached)) return cached;
            string path = Output + "/Materials/Imported_" + key + ".mat";
            var result = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (result == null) {
                result = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = assetName + "_" + slot };
                result.SetColor("_BaseColor", color); result.SetFloat("_Smoothness", .12f);
                if (texture != null) { result.SetTexture("_BaseMap", texture); result.SetTextureScale("_BaseMap", original.mainTextureScale); result.SetTextureOffset("_BaseMap", original.mainTextureOffset); }
                AssetDatabase.CreateAsset(result, path);
            }
            materials[key] = result; return result;
        }
        static Bounds BoundsOf(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length == 0) throw new Exception("No mesh renderers: " + obj.name);
            var bounds = renderers[0].bounds; foreach(var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            return bounds;
        }
        static GameObject Model(string name, string source, float height, float yaw = 0)
        {
            string path = Output + "/Prefabs/" + name + ".prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            var importer = AssetImporter.GetAtPath(source) as ModelImporter;
            if (importer == null) throw new Exception("Native model importer missing: " + source);
            importer.importAnimation = false; importer.isReadable = false; importer.addCollider = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.SaveAndReimport();
            var sourceAsset = AssetDatabase.LoadAssetAtPath<GameObject>(source);
            if (sourceAsset == null) throw new Exception("Model import failed: " + source);
            var wrapper = new GameObject(name);
            try {
                var child = (GameObject)PrefabUtility.InstantiatePrefab(sourceAsset);
                child.transform.SetParent(wrapper.transform, false); child.transform.localRotation = Quaternion.Euler(0,yaw,0);
                var originalBounds = BoundsOf(wrapper);
                if (originalBounds.size.y < .00001f) throw new Exception("Degenerate model: " + source);
                child.transform.localScale *= height / originalBounds.size.y;
                var bounds = BoundsOf(wrapper); child.transform.position -= new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
                foreach(var renderer in child.GetComponentsInChildren<MeshRenderer>()) {
                    renderer.sharedMaterials = renderer.sharedMaterials.Select((m,i)=>ConvertMaterial(m,name,i)).ToArray();
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                bounds = BoundsOf(wrapper);
                var collider = wrapper.AddComponent<BoxCollider>(); collider.center = bounds.center; collider.size = Vector3.Max(bounds.size,Vector3.one * .02f);
                int triangles = wrapper.GetComponentsInChildren<MeshFilter>().Where(x=>x.sharedMesh != null).Sum(x=>(int)x.sharedMesh.GetIndexCount(0)/3);
                audit.Add($"{name}: source={source}; size={bounds.size}; triangles(first submesh)={triangles}");
                return PrefabUtility.SaveAsPrefabAsset(wrapper,path);
            } finally { UnityEngine.Object.DestroyImmediate(wrapper); }
        }
        static GameObject Place(GameObject prefab, Transform parent, string name, Vector3 position, float yaw = 0)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name; instance.transform.SetParent(parent); instance.transform.position = position; instance.transform.rotation = Quaternion.Euler(0,yaw,0);
            return instance;
        }
        static GameObject Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = name; obj.transform.SetParent(parent);
            obj.transform.position = position; obj.transform.localScale = size; obj.GetComponent<Renderer>().sharedMaterial = material;
            obj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; return obj;
        }
        static void Action(GameObject obj, BankAction action)
        { var target = obj.AddComponent<BankInteractable>(); target.Manager = manager; target.Action = action; }
        static Transform Group(Transform parent, string name) { var group = new GameObject(name).transform; group.SetParent(parent); return group; }
        static void Sign(Transform parent, string name, string text, Vector3 position, Vector2 physicalSize, float yaw = 0, int fontSize = 36)
        {
            var canvasObj = new GameObject(name,typeof(RectTransform),typeof(Canvas)); canvasObj.transform.SetParent(parent);
            canvasObj.transform.position = position; canvasObj.transform.rotation = Quaternion.Euler(0,yaw,0); canvasObj.transform.localScale = Vector3.one * .002f;
            var rect = canvasObj.GetComponent<RectTransform>(); rect.sizeDelta = physicalSize / .002f;
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            var panel = new GameObject("Fondo", typeof(RectTransform),typeof(UnityEngine.UI.Image)); panel.transform.SetParent(rect,false);
            panel.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta; panel.GetComponent<UnityEngine.UI.Image>().color = new Color(.025f,.07f,.1f); panel.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
            var label = new GameObject("Texto",typeof(RectTransform),typeof(TextMeshProUGUI)); label.transform.SetParent(rect,false);
            label.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta - new Vector2(25,15);
            var tmp = label.GetComponent<TextMeshProUGUI>(); tmp.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            tmp.text = text; tmp.fontSize = fontSize; tmp.color = Color.white; tmp.alignment = TextAlignmentOptions.Center; tmp.raycastTarget = false;
        }
        static void Teleport(Transform parent, string name, Vector3 destination, Material color)
        {
            var pad = Box(parent,name,destination + Vector3.up * .025f,new Vector3(.7f,.05f,.7f),color);
            var target = pad.AddComponent<BankInteractable>(); target.Action = BankAction.Teleport; target.Player = rig; target.Destination = destination;
        }
        [MenuItem("SWIFT/Construir oficina con modelos importados")]
        public static void Build()
        {
            materials.Clear(); audit.Clear();
            foreach(string folder in new[]{Output,Output+"/Materials",Output+"/Prefabs","Assets/_Project/Scenes/Backups"}) Directory.CreateDirectory(folder);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            if (!File.Exists(ScenePath)) throw new Exception("Base BancoCorresponsal scene missing; original prototype is required.");
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            if (scene.GetRootGameObjects().Any(x=>x.name == "BankOfficeVisualV1")) { Debug.Log("SWIFT: oficina ya generada; se conservan tus ediciones."); return; }
            string backup = AssetDatabase.GenerateUniqueAssetPath("Assets/_Project/Scenes/Backups/BancoCorresponsal_BeforeOffice_20261028.unity");
            if (!AssetDatabase.CopyAsset(ScenePath,backup)) throw new Exception("Scene backup failed");
            manager = scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<GameManager>()).Single();
            var system = manager.gameObject; system.name = "00_Systems";
            var input = system.GetComponentInChildren<VRInteractionManager>(); rig = input.View.transform.parent;
            var legacy = Group(system.transform,"Legacy_Prototype_Preserved");
            foreach(var child in system.transform.Cast<Transform>().ToArray()) {
                if(child == legacy || child == rig || child == input.Reticle) continue;
                child.SetParent(legacy,true);
            }
            legacy.gameObject.SetActive(false);
            rig.position = new Vector3(0,0,-3.3f); input.View.transform.localRotation = Quaternion.identity;
            input.Reticle.localScale = Vector3.one * .015f; input.Reticle.position = input.View.transform.position + input.View.transform.forward * 2;
            var root = new GameObject("BankOfficeVisualV1").transform;
            var shell = Group(root,"01_Architecture"); var entry = Group(root,"02_Zone_A_Entry");
            var deskZone = Group(root,"03_Zone_B_Inspection"); var evidence = Group(root,"04_Zone_C_Evidence");
            var security = Group(root,"05_Zone_D_Security"); var decor = Group(root,"06_Secondary_Furniture");
            var blue = Solid("Bank_Navy",new Color(.055f,.12f,.18f)); var wall = Solid("Wall_WarmWhite",new Color(.7f,.72f,.67f));
            var floor = Solid("Floor_Slate",new Color(.17f,.23f,.27f)); var teal = Solid("Interactive_Teal",new Color(.04f,.46f,.4f));
            var brass = Solid("Accent_Brass",new Color(.66f,.48f,.24f));
            Box(shell,"Floor",new Vector3(0,-.1f,0),new Vector3(10,.2f,10),floor);
            Box(shell,"Ceiling",new Vector3(0,3.4f,0),new Vector3(10,.16f,10),wall);
            Box(shell,"NorthWall",new Vector3(0,1.65f,5),new Vector3(10,3.3f,.2f),wall);
            Box(shell,"WestWall",new Vector3(-5,1.65f,0),new Vector3(.2f,3.3f,10),wall);
            Box(shell,"EastWall",new Vector3(5,1.65f,0),new Vector3(.2f,3.3f,10),wall);
            Box(shell,"EntranceWallLeft",new Vector3(-3.1f,1.65f,-5),new Vector3(3.8f,3.3f,.2f),wall);
            Box(shell,"EntranceWallRight",new Vector3(3.1f,1.65f,-5),new Vector3(3.8f,3.3f,.2f),wall);
            Box(shell,"EntranceLintel",new Vector3(0,2.95f,-5),new Vector3(2.4f,.7f,.2f),wall);
            Box(shell,"NorthBlueBand",new Vector3(0,.45f,4.87f),new Vector3(9.8f,.9f,.04f),blue);
            foreach(int side in new[]{-1,1}) Box(shell,"SideBlueBand",new Vector3(side*4.87f,.45f,0),new Vector3(.04f,.9f,9.8f),blue);
            Box(shell,"Entrance infill left",new Vector3(-.91f,1.3f,-5),new Vector3(.58f,2.6f,.2f),wall);
            Box(shell,"Entrance infill right",new Vector3(.91f,1.3f,-5),new Vector3(.58f,2.6f,.2f),wall);
            var desk = Model("Quaternius_Desk","Assets/Models/Desk_Quaternius/Desk.fbx",.8f);
            var computer = Model("Google_Computer","Assets/Models/Computer_PolyGoogle/Computer.obj",.78f,180);
            var printer = Model("Bruno_Printer","Assets/Models/Printer_BrunoOliveira/model.obj",1.05f,180);
            var papers = Model("Jarlan_Papers","Assets/Models/Paper_JarlanPerez/model.obj",.07f);
            var stapler = Model("Google_Stapler","Assets/Models/Stapler_PolyGoogle/Stapler.obj",.09f);
            var chair = Model("Kenney_OfficeChair",Kenney+"chairDesk.fbx",1.08f);
            var cabinet = Model("Kenney_Archive",Kenney+"bookcaseClosedDoors.fbx",1.7f);
            var books = Model("Kenney_Books",Kenney+"books.fbx",.24f);
            var doorway = Model("Kenney_Doorway",Kenney+"doorwayFront.fbx",2.6f);
            var keyboard = Model("Kenney_Keyboard",Kenney+"computerKeyboard.fbx",.055f);
            var mouse = Model("Kenney_Mouse",Kenney+"computerMouse.fbx",.045f);
            var plant = Model("Kenney_Plant",Kenney+"pottedPlant.fbx",1.1f);
            var sofa = Model("Kenney_Sofa",Kenney+"loungeSofa.fbx",.85f);
            var sideTable = Model("Kenney_SideTable",Kenney+"sideTable.fbx",.68f);
            var trash = Model("Kenney_Trash",Kenney+"trashcan.fbx",.5f);
            Place(doorway,entry,"Entrance door frame",new Vector3(0,0,-4.87f));
            Box(entry,"Entrance door leaf provisional",new Vector3(0,1.2f,-4.96f),new Vector3(1.1f,2.4f,.08f),blue);
            var mainDesk = Place(desk,deskZone,"Main analyst desk",new Vector3(0,0,1.6f)); mainDesk.transform.localScale = new Vector3(1.65f,1,2.2f);
            var terminal = Place(computer,deskZone,"Interactive Computer",new Vector3(0,.82f,1.6f)); Action(terminal,BankAction.Inspect);
            Place(keyboard,deskZone,"Spare keyboard",new Vector3(-.98f,.82f,1.75f),90); Place(mouse,deskZone,"Mouse",new Vector3(.65f,.82f,1.05f));
            Place(chair,deskZone,"Analyst chair",new Vector3(0,0,2.5f),180);
            Place(stapler,deskZone,"Stapler",new Vector3(.87f,.82f,1.5f));
            var paper = Place(papers,deskZone,"Fictional transfer documents",new Vector3(-.65f,.82f,1.02f)); Action(paper,BankAction.Evidence);
            var copier = Place(printer,evidence,"Interactive Printer",new Vector3(-2.8f,0,1.2f)); Action(copier,BankAction.Evidence);
            for(int i=0;i<2;i++) {var archive = Place(cabinet,evidence,"Interactive Archive "+(i+1),new Vector3(-4.2f,0,2.7f+i*1.1f),90); Action(archive,BankAction.Registry);}
            var referenceDesk = Place(sideTable,evidence,"Evidence table",new Vector3(-2.8f,0,3.15f));
            Place(papers,evidence,"Fictional printed evidence",new Vector3(-2.8f,.7f,3.15f));
            var alerts = Box(security,"Interactive Alert Panel",new Vector3(3.0f,1.55f,4.82f),new Vector3(1.4f,.9f,.08f),blue); Action(alerts,BankAction.Alerts);
            Sign(security,"Alert instructions","D - ALERTAS\nCoincidencia no confirma fraude\nConsulta anomalias del expediente",new Vector3(3.0f,1.55f,4.76f),new Vector2(1.35f,.83f),0,28);
            var controls = Box(security,"Interactive Defense Panel",new Vector3(4.35f,1.55f,4.82f),new Vector3(1.1f,.9f,.08f),teal); Action(controls,BankAction.Defense);
            Sign(security,"Defense instructions","SEGURIDAD\nActivar verificacion reforzada\nDisponible en modo DEFENSA",new Vector3(4.35f,1.55f,4.76f),new Vector2(1.05f,.83f),0,28);
            var end = Box(security,"End shift",new Vector3(2.4f,1,1.4f),new Vector3(.55f,.35f,.3f),brass); Action(end,BankAction.End);
            Sign(security,"End label","CERRAR TURNO",new Vector3(2.4f,1.27f,1.19f),new Vector2(.9f,.2f),0,30);
            foreach(int sign in new[]{-1,1}) {
                var portal = Box(entry,sign<0 ? "Portal ATAQUE" : "Portal DEFENSA",new Vector3(sign*1.35f,1.3f,-2.1f),new Vector3(.95f,.6f,.12f),sign<0 ? blue : teal);
                Box(entry,portal.name+" stand",new Vector3(sign*1.35f,.5f,-2.06f),new Vector3(.12f,1,.12f),blue);
                Box(entry,portal.name+" base",new Vector3(sign*1.35f,.025f,-2.06f),new Vector3(.65f,.05f,.4f),blue);
                Action(portal,sign<0 ? BankAction.Attack : BankAction.DefenseMode);
                Sign(entry,portal.name+" label",sign<0 ? "ATAQUE\nEntrenamiento ficticio" : "DEFENSA\nControles adicionales",portal.transform.position+new Vector3(0,0,-.075f),new Vector2(.9f,.5f),0,28);
            }
            Sign(entry,"Welcome","SWIFT: CODIGO DE ALERTA\nBanco Corresponsal Internacional - 2016\nElige ATAQUE o DEFENSA. Mira las baldosas para trasladarte.\nPC: clic / boton derecho + mouse / WASD",new Vector3(-4.3f,1.7f,-2.3f),new Vector2(2,.8f),-90,36);
            Sign(evidence,"Evidence zone label","C - EVIDENCIAS\nImpresora y registros ficticios",new Vector3(-3.2f,2.08f,4.82f),new Vector2(2.2f,.4f),0,40);
            Sign(deskZone,"Bank title","BANCO CORRESPONSAL INTERNACIONAL\nOperaciones - Simulacion educativa",new Vector3(0,2.85f,4.82f),new Vector2(4,.5f),0,72);
            Place(plant,decor,"Plant west",new Vector3(-4,0,-3.8f)); Place(plant,decor,"Plant east",new Vector3(4,0,4));
            Place(sofa,decor,"Visitor seating",new Vector3(3.7f,0,-3.5f),-90);
            Place(sideTable,decor,"Visitor table",new Vector3(2.9f,0,-3.5f));
            Place(books,decor,"Reference books",new Vector3(2.9f,.7f,-3.5f));
            Place(trash,decor,"Waste basket",new Vector3(-1.8f,0,2.8f));
            // A static wall clock and plain documents are provisional, unbranded geometry.
            var clock = GameObject.CreatePrimitive(PrimitiveType.Cylinder); clock.name = "Wall clock provisional"; clock.transform.SetParent(decor);
            clock.transform.position = new Vector3(-3.75f,2.65f,4.82f); clock.transform.rotation = Quaternion.Euler(90,0,0); clock.transform.localScale = new Vector3(.5f,.025f,.5f); clock.GetComponent<Renderer>().sharedMaterial = Solid("Clock_Ivory",Color.white);
            Box(decor,"Clock hour hand",new Vector3(-3.8f,2.69f,4.78f),new Vector3(.12f,.025f,.015f),blue);
            Box(decor,"Clock minute hand",new Vector3(-3.75f,2.56f,4.78f),new Vector3(.02f,.18f,.015f),blue);
            Teleport(entry,"Teleport Entry",new Vector3(0,0,-3.3f),teal);
            Teleport(deskZone,"Teleport Inspection",new Vector3(0,0,-.65f),teal);
            Teleport(evidence,"Teleport Evidence",new Vector3(-2.8f,0,-.1f),teal);
            Teleport(security,"Teleport Security",new Vector3(3.3f,0,2.0f),teal);
            var lightObj = new GameObject("Key light",typeof(Light)); lightObj.transform.SetParent(shell); lightObj.transform.rotation = Quaternion.Euler(50,-25,0);
            var light = lightObj.GetComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.15f; light.shadows = LightShadows.None;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = new Color(.55f,.58f,.61f);
            var ui = system.GetComponent<UIManager>(); var mount = Group(terminal.transform,"Expanded document screen mount"); mount.position = new Vector3(0,1.68f,1.12f);
            ui.PanelAnchor = mount; ui.CompactMonitor = true; ui.BuildInterface();
            Sign(decor,"Model credits","Modelos: Kenney / Quaternius (CC0)\nComputer + Stapler: Poly by Google (CC BY 3.0)\nPrinter: Bruno Oliveira / Papers: Jarlan Perez (CC BY 3.0)\nEscala, orientacion y materiales adaptados a URP. Ver AssetCredits.",new Vector3(-3.4f,1.7f,-4.8f),new Vector2(2.5f,.8f),180,25);
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene,ScenePath);
            Directory.CreateDirectory("Artifacts/Office20261028"); File.WriteAllLines("Artifacts/Office20261028/model-import-audit.txt",audit);
            Validate(); Debug.Log("SWIFT_OFFICE_BUILD_PASS");
        }
        public static void FinalizeScene()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            var input=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<VRInteractionManager>()).Single();
            input.Reticle.localScale=Vector3.one*.015f;
            input.Reticle.position=input.View.transform.position+input.View.transform.forward*2;
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,ScenePath);
            Validate();
        }
        static void UpdateSign(string name,Vector3 position,Vector2 size,float yaw,int fontSize)
        {
            var obj=GameObject.Find(name); if(obj==null) throw new Exception("Sign missing: "+name);
            obj.transform.position=position;obj.transform.rotation=Quaternion.Euler(0,yaw,0);
            var rect=obj.GetComponent<RectTransform>();rect.sizeDelta=size/.002f;
            obj.transform.Find("Fondo").GetComponent<RectTransform>().sizeDelta=rect.sizeDelta;
            var text=obj.GetComponentInChildren<TextMeshProUGUI>();text.rectTransform.sizeDelta=rect.sizeDelta-new Vector2(25,15);text.fontSize=fontSize;
        }
        public static void RefineLayout()
        {
            var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            if(GameObject.Find("Entrance infill left")==null){
                var shell=GameObject.Find("01_Architecture").transform;var wall=Solid("Wall_WarmWhite",new Color(.7f,.72f,.67f));
                Box(shell,"Entrance infill left",new Vector3(-.91f,1.3f,-5),new Vector3(.58f,2.6f,.2f),wall);
                Box(shell,"Entrance infill right",new Vector3(.91f,1.3f,-5),new Vector3(.58f,2.6f,.2f),wall);
            }
            var desk=GameObject.Find("Main analyst desk");desk.transform.position=new Vector3(0,0,1.6f);desk.transform.localScale=new Vector3(1.65f,1,2.2f);
            var keyboard=GameObject.Find("Keyboard") ?? GameObject.Find("Spare keyboard");keyboard.name="Spare keyboard";keyboard.transform.SetPositionAndRotation(new Vector3(-.98f,.82f,1.75f),Quaternion.Euler(0,90,0));
            var alerts=GameObject.Find("Interactive Alert Panel");alerts.transform.position=new Vector3(3,1.55f,4.82f);alerts.transform.localScale=new Vector3(1.4f,.9f,.08f);
            var controls=GameObject.Find("Interactive Defense Panel");controls.transform.position=new Vector3(4.35f,1.55f,4.82f);controls.transform.localScale=new Vector3(1.1f,.9f,.08f);
            UpdateSign("Alert instructions",new Vector3(3,1.55f,4.76f),new Vector2(1.35f,.83f),0,28);
            UpdateSign("Defense instructions",new Vector3(4.35f,1.55f,4.76f),new Vector2(1.05f,.83f),0,28);
            UpdateSign("Welcome",new Vector3(-4.3f,1.7f,-2.3f),new Vector2(2,.8f),-90,36);
            UpdateSign("Evidence zone label",new Vector3(-3.2f,2.08f,4.82f),new Vector2(2.2f,.4f),0,40);
            UpdateSign("Bank title",new Vector3(0,2.85f,4.82f),new Vector2(4,.5f),0,72);
            var pad=GameObject.Find("Teleport Security").GetComponent<BankInteractable>();pad.Destination=new Vector3(3.3f,0,2);pad.transform.position=pad.Destination+Vector3.up*.025f;
            foreach(var portal in new[]{GameObject.Find("Portal ATAQUE"),GameObject.Find("Portal DEFENSA")}){
                if(GameObject.Find(portal.name+" stand")!=null)continue;
                var color=portal.GetComponent<Renderer>().sharedMaterial;
                Box(portal.transform.parent,portal.name+" stand",new Vector3(portal.transform.position.x,.5f,-2.06f),new Vector3(.12f,1,.12f),color);
                Box(portal.transform.parent,portal.name+" base",new Vector3(portal.transform.position.x,.025f,-2.06f),new Vector3(.65f,.05f,.4f),color);
            }
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,ScenePath);FinalizeScene();
        }
        public static void StartPlayValidation()
        {
            EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            RefineLayout();
            new GameObject("Office validation probe").AddComponent<Swift.Testing.BankOfficeProbe>();
            EditorApplication.EnterPlaymode();
        }
        public static void Validate()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects(); var active = roots.SelectMany(x=>x.GetComponentsInChildren<Transform>()).ToArray();
            var renderers = roots.SelectMany(x=>x.GetComponentsInChildren<MeshRenderer>()).ToArray();
            if (active.Any(x=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject)>0)) throw new Exception("Missing script reference");
            if(renderers.Any(x=>x.sharedMaterials.Any(m=>m==null || !m.shader.name.StartsWith("Universal Render Pipeline/")))) throw new Exception("Missing or non-URP mesh material");
            var actions = roots.SelectMany(x=>x.GetComponentsInChildren<BankInteractable>()).ToArray();
            if(actions.Any(x=>x.GetComponent<Collider>()==null)) throw new Exception("Interaction collider missing");
            foreach(var required in new[]{BankAction.Inspect,BankAction.Evidence,BankAction.Registry,BankAction.Alerts,BankAction.Defense}) if(!actions.Any(x=>x.Action==required)) throw new Exception("Missing banking action "+required);
            BankNavigationValidation.Check(actions);
            long triangles = roots.SelectMany(x=>x.GetComponentsInChildren<MeshFilter>()).Where(x=>x.sharedMesh!=null).Sum(x=>Enumerable.Range(0,x.sharedMesh.subMeshCount).Sum(i=>(long)x.sharedMesh.GetIndexCount(i)/3));
            Directory.CreateDirectory("Artifacts/Office20261028");
            File.WriteAllText("Artifacts/Office20261028/scene-validation.txt",$"PASS: scene reopened; {renderers.Length} active mesh renderers; {actions.Length} collider-backed actions; {triangles} active mesh triangles; five required roles; four original destinations preserved; {actions.Count(x=>x.Action==BankAction.Teleport)} total teleports. XR hardware not tested.");
            Debug.Log("SWIFT_OFFICE_VALIDATION_PASS");
        }
    }
}





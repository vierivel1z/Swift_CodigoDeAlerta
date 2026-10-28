using UnityEngine;
using Swift.Game;
using Swift.Interaction;
using Swift.UI;
using Swift.VR;
namespace Swift.Testing
{
    public static class OfficeFactory
    {
        public static GameObject Build(ScenarioData scenario)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.4f,.45f,.5f);
            var root = new GameObject("Banco Corresponsal - Prototipo");
            var manager = root.AddComponent<GameManager>(); manager.Scenario = scenario;
            Material MakeMaterial(string name, Color color) { var m = new Material(Shader.Find("Universal Render Pipeline/Lit")); m.name = name; m.color = color; return m; }
            var room = MakeMaterial("Pared - gris azul", new Color(.15f,.2f,.25f));
            var furniture = MakeMaterial("Muebles - madera", new Color(.32f,.2f,.12f));
            var accent = MakeMaterial("Interactivo - verde", new Color(.05f,.45f,.38f));
            GameObject Box(string name, Vector3 p, Vector3 size, Material material) {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = name; obj.transform.SetParent(root.transform);
                obj.transform.position = p; obj.transform.localScale = size; obj.GetComponent<Renderer>().sharedMaterial = material; return obj;
            }
            Box("Suelo", new Vector3(0,-.1f,0), new Vector3(10,.2f,10), room);
            Box("Techo", new Vector3(0,4.2f,0), new Vector3(10,.2f,10), room);
            Box("Pared norte", new Vector3(0,2,5), new Vector3(10,4,.2f), room);
            Box("Pared sur", new Vector3(0,2,-5), new Vector3(10,4,.2f), room);
            Box("Pared oeste", new Vector3(-5,2,0), new Vector3(.2f,4,10), room);
            Box("Pared este", new Vector3(5,2,0), new Vector3(.2f,4,10), room);
            Box("Escritorio", new Vector3(0,.8f,2.25f), new Vector3(3,.15f,1.2f), furniture);
            for(int i=-1;i<=1;i+=2) Box("Pata escritorio", new Vector3(i,.4f,2.25f), new Vector3(.15f,.8f,.8f), furniture);
            Box("Silla asiento", new Vector3(0,.45f,3.35f), new Vector3(.65f,.15f,.65f), furniture);
            Box("Silla respaldo", new Vector3(0,.9f,3.65f), new Vector3(.65f,.9f,.15f), furniture);
            var rig = new GameObject("Player Rig"); rig.transform.SetParent(root.transform); rig.transform.position = new Vector3(0,0,-1.5f);
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)); cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(rig.transform); cameraObject.transform.localPosition = new Vector3(0,1.6f,0);
            var camera = cameraObject.GetComponent<Camera>(); camera.nearClipPlane = .05f; camera.farClipPlane = 40;
            var input = cameraObject.AddComponent<EditorInputAdapter>(); input.Rig = rig.transform;
            cameraObject.AddComponent<HeadPoseAdapter>(); cameraObject.AddComponent<CardboardLifecycle>();
            var interactor = cameraObject.AddComponent<VRInteractionManager>(); interactor.View = camera;
            var reticle = new GameObject("Reticula", typeof(MeshFilter), typeof(MeshRenderer));
            // A sphere mesh without a collider cannot obstruct interaction rays.
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reticle.GetComponent<MeshFilter>().sharedMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
            if (Application.isPlaying) { sphere.SetActive(false); Object.Destroy(sphere); } else Object.DestroyImmediate(sphere);
            reticle.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { color = Color.cyan };
            reticle.transform.SetParent(root.transform); interactor.Reticle = reticle.transform;
            void Interactive(string name, Vector3 position, Vector3 size, BankAction action) {
                var obj = Box(name, position, size, accent); var target = obj.AddComponent<BankInteractable>(); target.Manager = manager; target.Action = action;
                // TextMesh is built in; document UI uses TMP separately.
                var label = new GameObject(name + " etiqueta", typeof(TextMesh)); label.transform.SetParent(root.transform);
                label.transform.position = position + new Vector3(0, size.y/2 + .18f, -.1f);
                var text = label.GetComponent<TextMesh>(); text.text = name; text.fontSize = 36; text.characterSize = .045f; text.anchor = TextAnchor.MiddleCenter; text.color = Color.white;
            }
            Interactive("Monitor", new Vector3(0,1.2f,2.4f), new Vector3(1,.6f,.12f), BankAction.Inspect);
            Interactive("Archivador", new Vector3(-3,.7f,1.5f), new Vector3(.8f,1.4f,.7f), BankAction.Registry);
            Interactive("Alertas", new Vector3(3,1.4f,2), new Vector3(1,.7f,.15f), BankAction.Alerts);
            Interactive("Registrar evidencia", new Vector3(-1.9f,1.1f,1.3f), new Vector3(.55f,.35f,.4f), BankAction.Evidence);
            Interactive("Activar defensa", new Vector3(2,1.1f,1.3f), new Vector3(.65f,.4f,.4f), BankAction.Defense);
            Interactive("Finalizar turno", new Vector3(3.5f,1,-.5f), new Vector3(.7f,.5f,.4f), BankAction.End);
            Interactive("Portal ATAQUE", new Vector3(-2,1.6f,0), new Vector3(.9f,1.2f,.2f), BankAction.Attack);
            Interactive("Portal DEFENSA", new Vector3(2,1.6f,0), new Vector3(.9f,1.2f,.2f), BankAction.DefenseMode);
            for(int i=0;i<2;i++) {
                var obj = Box("Teletransporte " + (i+1), new Vector3(i == 0 ? -2 : 2,.04f,-2), new Vector3(.8f,.08f,.8f), accent);
                var target = obj.AddComponent<BankInteractable>(); target.Action = BankAction.Teleport; target.Player = rig.transform; target.Destination = new Vector3(i == 0 ? -2 : 2,0,-2);
            }
            var lightObject = new GameObject("Luz principal", typeof(Light)); lightObject.transform.SetParent(root.transform);
            var light = lightObject.GetComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.2f; light.shadows = LightShadows.None;
            lightObject.transform.rotation = Quaternion.Euler(45,-30,0);
            var ui = root.AddComponent<UIManager>(); ui.Manager = manager;
            var audio = root.AddComponent<Swift.Audio.FeedbackAudio>(); audio.Manager = manager;
            return root;
        }
    }
}


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Swift.EditorTools
{
    // Editor-only. No persistent cameras, scene edits, network access or runtime dependency.
    public sealed class SceneReviewCapture : EditorWindow
    {
        const string ScenePath = "Assets/_Project/Scenes/BancoCorresponsal.unity";
        const int Width = 1600, Height = 1000;

        [Serializable]
        public sealed class View
        {
            public string file;
            public Vector3 position, target;
            public float fieldOfView = 60, orthographicSize;
            public bool hideCeiling;
        }

        [Serializable]
        sealed class Receipt
        {
            public string utc, scene, sceneSha256, dependencyHash, unity, pipeline, pipelinePackage, quality, graphicsDevice;
            public string note = "Real Editor render of saved scene; not a device/VR performance test. Ceiling hidden temporarily only in top view.";
            public int width = Width, height = Height;
            public View[] views;
            public string[] pngSha256;
        }

        [SerializeField] View[] views;
        Vector2 scroll;

        static View[] Defaults() => new[]
        {
            new View { file = "01-oficina.png", position = new Vector3(0, 2.65f, -4.5f), target = new Vector3(0, 1.1f, 1), fieldOfView = 85 },
            new View { file = "02-escritorio.png", position = new Vector3(1.9f, 2.2f, -.9f), target = new Vector3(0, 1.1f, 1.6f) },
            new View { file = "03-computadora-expedientes.png", position = new Vector3(-.15f, 1.85f, -.3f), target = new Vector3(-.15f, 1.4f, 1.4f), fieldOfView = 65 },
            new View { file = "04-distribucion-superior.png", position = new Vector3(0, 12, 0), target = Vector3.zero, orthographicSize = 5.7f, hideCeiling = true }
        };

        [MenuItem("SWIFT/Revision visual/Camaras y capturas PNG")]
        public static void Open() => GetWindow<SceneReviewCapture>("SWIFT Capturas");

        void OnEnable() { if (views == null || views.Length != 4) views = Defaults(); }

        void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.HelpBox("Abre y guarda BancoCorresponsal. Captura la escena real en modo Edicion. Ajusta encuadres tras la primera revision; no inicia Play ni regenera la oficina.", MessageType.Info);
            foreach (var view in views)
            {
                EditorGUILayout.LabelField(view.file, EditorStyles.boldLabel);
                view.position = EditorGUILayout.Vector3Field("Posicion", view.position);
                view.target = EditorGUILayout.Vector3Field("Mirar hacia", view.target);
                if (view.orthographicSize > 0)
                    view.orthographicSize = EditorGUILayout.Slider("Tamano ortografico", view.orthographicSize, 1, 20);
                else view.fieldOfView = EditorGUILayout.Slider("Campo visual", view.fieldOfView, 20, 100);
                if (GUILayout.Button("Usar encuadre de Scene View") && SceneView.lastActiveSceneView != null)
                {
                    var camera = SceneView.lastActiveSceneView.camera;
                    view.position = camera.transform.position;
                    view.target = camera.transform.position + camera.transform.forward * 5;
                    view.fieldOfView = camera.fieldOfView;
                    view.orthographicSize = camera.orthographic ? camera.orthographicSize : 0;
                }
            }
            if (GUILayout.Button("Restaurar cuatro encuadres iniciales")) views = Defaults();
            if (GUILayout.Button("Generar cuatro PNG y reporte"))
            {
                try { Debug.Log("SWIFT_REVIEW_CAPTURE_PASS " + Capture(views)); }
                catch (Exception error) { Debug.LogException(error); }
            }
            EditorGUILayout.EndScrollView();
        }

        // For -executeMethod with a licensed Editor and a graphics device (never -nographics).
        public static void CaptureBatch()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || AnyDirtyScene())
                throw new InvalidOperationException("Stop Play and save scenes before batch capture.");
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
                Debug.Log("SWIFT_REVIEW_CAPTURE_PASS " + Capture(Defaults()));
            }
            finally { EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }

        static bool AnyDirtyScene() => Enumerable.Range(0, SceneManager.sceneCount)
            .Any(i => SceneManager.GetSceneAt(i).isDirty);

        static string Hash(string path)
        {
            using (var hash = SHA256.Create())
            using (var stream = File.OpenRead(path))
                return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        static string Capture(View[] requestedViews)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
                throw new InvalidOperationException("Wait for import/compilation and stop Play.");
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                throw new InvalidOperationException("A working graphics device is required; remove -nographics.");
            if (SceneManager.sceneCount != 1 || SceneManager.GetActiveScene().path != ScenePath || AnyDirtyScene())
                throw new InvalidOperationException("Open only BancoCorresponsal and save it before capture.");
            if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset))
                throw new InvalidOperationException("URP must already be active. Pipeline will not be changed.");

            var scene = SceneManager.GetActiveScene();
            var root = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var output = Path.Combine(root, "Docs", "Previews", DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6));
            Directory.CreateDirectory(output);
            var receipt = new Receipt
            {
                utc = DateTime.UtcNow.ToString("O"), scene = ScenePath,
                sceneSha256 = Hash(Path.Combine(root, ScenePath)),
                dependencyHash = AssetDatabase.GetAssetDependencyHash(ScenePath).ToString(),
                unity = Application.unityVersion,
                quality = QualitySettings.names[QualitySettings.GetQualityLevel()],
                graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                pipeline = AssetDatabase.GetAssetPath(GraphicsSettings.currentRenderPipeline),
                pipelinePackage = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(UniversalRenderPipeline).Assembly)?.version,
                views = requestedViews, pngSha256 = new string[requestedViews.Length]
            };
            Canvas.ForceUpdateCanvases();
            try
            {
                for (int i = 0; i < requestedViews.Length; i++)
                {
                    var path = Path.Combine(output, requestedViews[i].file);
                    Render(scene, requestedViews[i], path);
                    receipt.pngSha256[i] = Hash(path);
                }
                File.WriteAllText(Path.Combine(output, "capture.json"), JsonUtility.ToJson(receipt, true));
                var markdown = "# Revision de BancoCorresponsal\n\nUTC: " + receipt.utc + "\n\nUnity: " + receipt.unity + " / URP " + receipt.pipelinePackage + "\n\nEscena SHA-256: `" + receipt.sceneSha256 + "`\n\nCapturas reales del Editor en modo Edicion. No certifican VR, rendimiento ni textos creados solo durante Play. La vista superior oculta temporalmente el techo.\n\n";
                foreach (var view in requestedViews) markdown += "## " + view.file + "\n\n![" + view.file + "](" + view.file + ")\n\n";
                File.WriteAllText(Path.Combine(output, "README.md"), markdown);
                var latest = Path.Combine(root, "Docs", "Previews", "LATEST.md");
                var pendingLatest = latest + "." + Guid.NewGuid().ToString("N") + ".tmp";
                try
                {
                    File.WriteAllText(pendingLatest, "# Ultima captura completa\n\n[Ver las cuatro perspectivas](" + Path.GetFileName(output) + "/README.md)\n\nGenerada: " + receipt.utc + "\n");
                    if (File.Exists(latest)) File.Replace(pendingLatest, latest, null);
                    else File.Move(pendingLatest, latest);
                }
                finally { if (File.Exists(pendingLatest)) File.Delete(pendingLatest); }
                return output;
            }
            catch
            {
                // Keep earlier complete reviews and LATEST intact; do not publish a partial run.
                if (Directory.Exists(output)) Directory.Delete(output, true);
                throw;
            }
        }

        static void Render(Scene scene, View view, string path)
        {
            var ceilings = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Renderer>(true))
                .Where(r => r.gameObject.name == "Ceiling").ToArray();
            if (view.hideCeiling && ceilings.Length == 0)
                throw new InvalidOperationException("Top view needs a renderer named Ceiling; inspect the scene before adapting the tool.");
            var previousVisibility = ceilings.Select(r => r.forceRenderingOff).ToArray();
            var previousTarget = RenderTexture.active;
            var cameraObject = new GameObject("SWIFT_ReviewCamera_Temporary") { hideFlags = HideFlags.HideAndDontSave };
            RenderTexture target = null;
            Texture2D image = null;
            try
            {
                SceneManager.MoveGameObjectToScene(cameraObject, scene);
                var camera = cameraObject.AddComponent<Camera>();
                camera.enabled = false;
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                camera.cameraType = CameraType.Game;
                camera.nearClipPlane = .05f; camera.farClipPlane = 50;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(.08f, .1f, .12f);
                camera.aspect = (float)Width / Height;
                camera.fieldOfView = view.fieldOfView;
                camera.orthographic = view.orthographicSize > 0;
                camera.orthographicSize = view.orthographicSize;
                camera.transform.position = view.position;
                var direction = view.target - view.position;
                if (direction.sqrMagnitude < .0001f) throw new InvalidOperationException("Camera position and target must differ.");
                camera.transform.rotation = Quaternion.LookRotation(direction, Mathf.Abs(Vector3.Dot(direction.normalized, Vector3.up)) > .99f ? Vector3.forward : Vector3.up);
                var data = camera.GetUniversalAdditionalCameraData();
                data.renderPostProcessing = false;
                data.renderShadows = true;
                data.antialiasing = AntialiasingMode.None;
                target = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 1 };
                if (!target.Create()) throw new InvalidOperationException("Could not allocate capture RenderTexture.");
                if (view.hideCeiling) foreach (var ceiling in ceilings) ceiling.forceRenderingOff = true;
                var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
                if (!RenderPipeline.SupportsRenderRequest(camera, request))
                    throw new InvalidOperationException("Active renderer does not support URP SingleCameraRequest.");
                RenderPipeline.SubmitRenderRequest(camera, request);
                RenderTexture.active = target;
                image = new Texture2D(Width, Height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                for (int i = 0; i < ceilings.Length; i++) if (ceilings[i] != null) ceilings[i].forceRenderingOff = previousVisibility[i];
                RenderTexture.active = previousTarget;
                if (image != null) DestroyImmediate(image);
                if (target != null) { target.Release(); DestroyImmediate(target); }
                DestroyImmediate(cameraObject);
            }
        }
    }
}

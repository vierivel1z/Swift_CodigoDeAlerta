using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Swift.Game;
using Swift.Testing;
namespace Swift.EditorTools
{
    public static class PrototypeBuilder
    {
        public const string ScenePath = "Assets/_Project/Scenes/BancoCorresponsal.unity";
        [MenuItem("SWIFT/Crear escena BancoCorresponsal")]
        public static void CreateScene()
        {
            if (File.Exists(ScenePath)) { Debug.Log("SWIFT: escena existente conservada: " + ScenePath); return; }
            var previous = SceneManagerSetup();
            if (!Application.isBatchMode && string.IsNullOrEmpty(previous.path)) throw new InvalidOperationException("Guarda la escena abierta antes de generar otra; se conserva tu trabajo.");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, Application.isBatchMode ? NewSceneMode.Single : NewSceneMode.Additive);
            try {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
                var dataPath = "Assets/_Project/Resources/TrainingScenario.asset";
                var data = AssetDatabase.LoadAssetAtPath<ScenarioData>(dataPath);
                if (data == null) { data = ScriptableObject.CreateInstance<ScenarioData>(); AssetDatabase.CreateAsset(data, dataPath); }
                // Build all static geometry and references through Unity APIs.
                var root = OfficeFactory.Build(data);
                // Persist procedurally created materials as project assets.
                foreach(var material in root.GetComponentsInChildren<Renderer>().SelectMany(x => x.sharedMaterials).Where(x => x != null).Distinct()) {
                    if (AssetDatabase.Contains(material)) continue;
                    AssetDatabase.CreateAsset(material, AssetDatabase.GenerateUniqueAssetPath("Assets/_Project/Materials/" + material.name.Replace('/','_') + ".mat"));
                }
                EditorSceneManager.SaveScene(scene, ScenePath);
                const string vrPath = "Assets/_Project/Scenes/CardboardSmokeTest.unity";
                if (!File.Exists(vrPath)) EditorSceneManager.SaveScene(scene, vrPath, true);
                if (!EditorBuildSettings.scenes.Any(x => x.path == ScenePath)) EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }.Concat(EditorBuildSettings.scenes).ToArray();
                AssetDatabase.SaveAssets(); Debug.Log("SWIFT scene generated: " + ScenePath);
            } finally { if (UnityEngine.SceneManagement.SceneManager.sceneCount > 1) EditorSceneManager.CloseScene(scene, true); if (previous.IsValid()) UnityEngine.SceneManagement.SceneManager.SetActiveScene(previous); }
        }
        static UnityEngine.SceneManagement.Scene SceneManagerSetup() => UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        [MenuItem("SWIFT/Importar recursos esenciales TMP")]
        public static void ImportTmpResources()
        {
            if (File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset")) return;
            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMPro.TMP_Text).Assembly);
            AssetDatabase.ImportPackage(Path.Combine(package.resolvedPath, "Package Resources/TMP Essential Resources.unitypackage"), false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("SWIFT_TMP_ESSENTIALS_IMPORT_REQUESTED");
        }
        public static void ImportTmpBatch()
        {
            if (File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset")) { EditorApplication.Exit(0); return; }
            AssetDatabase.importPackageCompleted += packageName => {
                Debug.Log("SWIFT_TMP_ESSENTIALS_IMPORTED: " + packageName);
                EditorApplication.delayCall += () => EditorApplication.Exit(File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset") ? 0 : 1);
            };
            AssetDatabase.importPackageFailed += (packageName, error) => { Debug.LogError(error); EditorApplication.Exit(1); };
            ImportTmpResources();
        }
        public static void BuildAndroid()
        {
            CardboardSetup.Configure(); CreateScene();
            Directory.CreateDirectory("Artifacts");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { ScenePath }, locationPathName = "Artifacts/SWIFT-prototype.apk",
                target = BuildTarget.Android, options = BuildOptions.Development
            });
            File.WriteAllText("Artifacts/android-build.txt", report.summary.result + " | errors=" + report.summary.totalErrors + " | size=" + report.summary.totalSize);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new Exception("Android build failed; inspect log.");
        }
        public static void StartPlaySmoke()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            new GameObject("SWIFT smoke probe").AddComponent<PrototypeSmokeProbe>();
            EditorApplication.EnterPlaymode();
        }
        public static void ValidateBatch()
        {
            CardboardSetup.Configure();
            CreateScene();
            if (!File.Exists(ScenePath)) throw new Exception("Scene generation failed");
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var roots = scene.GetRootGameObjects();
            if (roots.SelectMany(x=>x.GetComponentsInChildren<GameManager>()).Count() != 1 || roots.SelectMany(x=>x.GetComponentsInChildren<Swift.Interaction.BankInteractable>()).Count() < 10 || roots.SelectMany(x=>x.GetComponentsInChildren<Camera>()).Count() != 1) throw new Exception("Missing prototype references");
            // Keep the single validation scene open until batch exits.
            Directory.CreateDirectory("Artifacts"); File.WriteAllText("Artifacts/unity-validation.txt", "PASS: scripts compiled; scene generated and reopened; GameManager, camera and interactions present. Play Mode and Android pending.");
            Debug.Log("SWIFT_BATCH_VALIDATION_PASS");
        }
    }
}








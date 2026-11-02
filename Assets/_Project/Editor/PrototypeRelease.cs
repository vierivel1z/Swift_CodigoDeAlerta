using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build.Profile;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Swift.EditorTools
{
    // Does not regenerate scenes or change Player, URP, XR or package settings.
    public static class PrototypeRelease
    {
        public const string BankScene = "Assets/_Project/Scenes/BancoCorresponsal.unity";
        static string OutputDirectory
        {
            get
            {
                var path = Environment.GetEnvironmentVariable("SWIFT_SESSION_DIR");
                if (string.IsNullOrWhiteSpace(path))
                    throw new InvalidOperationException("Set SWIFT_SESSION_DIR to a new release evidence directory.");
                Directory.CreateDirectory(path);
                return path;
            }
        }

        [MenuItem("SWIFT/Prototipo funcional/Preparar escenas del juego")]
        public static void PrepareScenes()
        {
            if (!File.Exists(BankScene)) throw new FileNotFoundException(BankScene);
            // Keep every existing entry, including HelloCardboard, as an available test scene.
            var remaining = EditorBuildSettings.scenes.Where(x => x.path != BankScene)
                .Select(x => new EditorBuildSettingsScene(x.path, false));
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(BankScene, true) }
                .Concat(remaining).ToArray();
            var profile = BuildProfile.GetActiveBuildProfile();
            if (profile != null && profile.overrideGlobalScenes)
            {
                profile.scenes = EditorBuildSettings.scenes;
                EditorUtility.SetDirty(profile);
                AssetDatabase.SaveAssetIfDirty(profile);
            }
            AssetDatabase.SaveAssets();
            File.WriteAllLines(Path.Combine(OutputDirectory, "build-scenes.txt"),
                EditorBuildSettings.scenes.Select(x => (x.enabled ? "ENABLED " : "TEST/DISABLED ") + x.path));
            Debug.Log("SWIFT_RELEASE_SCENES_PASS: BancoCorresponsal is the only enabled game scene; test scenes retained.");
        }

        public static void ValidateFlow()
        {
            PrepareScenes();
            GameplayFlowValidation.Run();
        }

        public static void ValidateOffice()
        {
            // The older StartPlayValidation calls RefineLayout. Test the saved scene as-is instead.
            EditorSceneManager.OpenScene(BankScene, OpenSceneMode.Single);
            new GameObject("Office validation probe").AddComponent<Swift.Testing.BankOfficeProbe>();
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("SWIFT/Prototipo funcional/Compilar APK actual sin reconfigurar XR")]
        public static void BuildAndroid()
        {
            string output = OutputDirectory;
            string apk = Path.Combine(output, "SWIFT-BancoCorresponsal-current.apk");
            if (File.Exists(apk)) throw new IOException("Refusing to overwrite existing APK: " + apk);
            var enabled = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
            if (enabled.Length != 1 || enabled[0] != BankScene)
                throw new InvalidOperationException("Prepare the game scene list before building.");
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                throw new InvalidOperationException("Launch Unity with -buildTarget Android; settings are preserved.");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = enabled, locationPathName = apk, target = BuildTarget.Android,
                options = BuildOptions.Development | BuildOptions.DetailedBuildReport
            });
            File.WriteAllLines(Path.Combine(output, "android-build.txt"), new[] {
                "Result=" + report.summary.result,
                "Errors=" + report.summary.totalErrors,
                "Warnings=" + report.summary.totalWarnings,
                "Bytes=" + report.summary.totalSize,
                "Duration=" + report.summary.totalTime,
                "Unity=" + Application.unityVersion,
                "Output=" + apk,
                "InitialScene=" + enabled[0],
                "Scenes=" + string.Join(";", enabled),
                "DeviceTest=NOT_RUN"
            });
            File.WriteAllLines(Path.Combine(output, "build-packed-assets.txt"), report.packedAssets
                .SelectMany(x => x.contents).Select(x => x.sourceAssetPath).Distinct().OrderBy(x => x));
            if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0)
                throw new Exception("Android build failed. See android-build.txt and Unity log.");
            Debug.Log("SWIFT_RELEASE_APK_PASS: " + apk);
        }
    }
}

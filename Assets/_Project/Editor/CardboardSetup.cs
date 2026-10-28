using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
namespace Swift.EditorTools
{
    public static class CardboardSetup
    {
        [MenuItem("SWIFT/Configurar Android Cardboard")]
        public static void Configure()
        {
            const string folder = "Assets/_Project/XR";
            Directory.CreateDirectory(folder); AssetDatabase.Refresh();
            if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget settings) || settings == null) {
                settings = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(settings, AssetDatabase.GenerateUniqueAssetPath(folder + "/XRGeneralSettings.asset"));
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, settings, true);
            }
            if (!settings.HasSettingsForBuildTarget(BuildTargetGroup.Android)) settings.CreateDefaultSettingsForBuildTarget(BuildTargetGroup.Android);
            if (!settings.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android)) settings.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
            var general = settings.SettingsForBuildTarget(BuildTargetGroup.Android);
            general.InitManagerOnStart = true;
            if (!XRPackageMetadataStore.AssignLoader(general.Manager, "Google.XR.Cardboard.XRLoader", BuildTargetGroup.Android)) throw new Exception("Cardboard loader assignment failed");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
            PlayerSettings.Android.optimizedFramePacing = false;
            PlayerSettings.Android.forceInternetPermission = true;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            // Preserve an explicitly chosen application ID.
            var identifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
            if (identifier.Contains("template") || identifier == "com.DefaultCompany.SWIFT_CodigoDeAlerta") PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.atodaolla.swiftcodigoalerta");
            EditorUtility.SetDirty(settings); EditorUtility.SetDirty(general); EditorUtility.SetDirty(general.Manager);
            AssetDatabase.SaveAssets(); Debug.Log("SWIFT_CARDDBOARD_CONFIGURED: Android loader, API26/35, ARM64 IL2CPP, GLES3, Activity.");
        }
    }
}

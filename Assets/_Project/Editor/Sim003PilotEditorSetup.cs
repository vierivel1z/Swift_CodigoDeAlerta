using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Swift.UI;
using Swift.DeskTools;

namespace Swift.EditorTools
{
    public static class Sim003PilotEditorSetup
    {
        public const string ReviewScene="Assets/_Project/Scenes/Review/BancoCorresponsal_SIM003Pilot_20261111.unity";
        public const string ReviewProfile="Assets/_Project/DeskTools/DeskTools_SIM003Pilot.asset";
        const string MainScene="Assets/_Project/Scenes/BancoCorresponsal.unity";
        const string MainProfile="Assets/_Project/DeskTools/DeskTools_Trial.asset";

        public static void Prepare()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||SceneManager.sceneCount!=1||SceneManager.GetActiveScene().isDirty||SceneManager.GetActiveScene().path!=MainScene)
                throw new InvalidOperationException("Open the clean main scene before preparing the isolated pilot.");
            if(File.Exists(ReviewScene)||File.Exists(ReviewProfile))
                throw new InvalidOperationException("Pilot review assets already exist; refusing to overwrite them.");
            Directory.CreateDirectory("Artifacts/SIM003Pilot");
            File.WriteAllText("Artifacts/SIM003Pilot/main-scene-sha256.txt",Hash(MainScene));
            if(!AssetDatabase.CopyAsset(MainProfile,ReviewProfile))throw new InvalidOperationException("Could not duplicate the tools profile.");
            if(!AssetDatabase.CopyAsset(MainScene,ReviewScene))throw new InvalidOperationException("Could not duplicate the main scene.");
            var profile=AssetDatabase.LoadAssetAtPath<DeskToolsProfile>(ReviewProfile);
            if(profile==null)throw new InvalidOperationException("Duplicated profile did not import.");
            profile.Sim003PilotEnabled=true;
            profile.Sim003PrinterEnabled=true;
            profile.Sim003PrinterFailsInAttack=true;
            profile.PolishedClipboardVisual=true;
            EditorUtility.SetDirty(profile);AssetDatabase.SaveAssets();
            var scene=EditorSceneManager.OpenScene(ReviewScene,OpenSceneMode.Single);
            var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();
            if(ui==null)throw new InvalidOperationException("Review scene has no UIManager.");
            ui.DeskTools=profile;EditorUtility.SetDirty(ui);EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            if(Hash(MainScene)!=File.ReadAllText("Artifacts/SIM003Pilot/main-scene-sha256.txt"))
                throw new InvalidOperationException("Main scene changed during review setup.");
            File.WriteAllText("Artifacts/SIM003Pilot/review-scene-sha256.txt",Hash(ReviewScene));
        }
        static string Hash(string path)
        {
            using(var sha=System.Security.Cryptography.SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-","");
        }
    }
}

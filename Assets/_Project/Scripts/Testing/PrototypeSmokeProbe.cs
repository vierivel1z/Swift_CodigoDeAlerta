using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using Swift.Bank;
using Swift.Game;
namespace Swift.Testing
{
    public sealed class PrototypeSmokeProbe : MonoBehaviour
    {
        int errors;
        void OnEnable() { Application.logMessageReceived += Log; }
        void OnDisable() { Application.logMessageReceived -= Log; }
        void Log(string message, string stack, LogType type) { if(type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors++; }
        IEnumerator Start()
        {
            yield return null; yield return null;
            string result;
            try {
                var notices = Resources.Load<TextAsset>("ThirdPartyNotices");
                if (notices == null || !notices.text.Contains("SIL OPEN FONT LICENSE")) throw new Exception("Bundled license notices missing");
                var game = FindFirstObjectByType<GameManager>();
                if (game == null) throw new Exception("GameManager missing");
                var texts = FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsSortMode.None);
                if (texts.Length < 6 || texts.Any(x => x.font == null || x.font.characterTable.Count < 50)) throw new Exception("World Space TMP UI missing font or buttons");
                if (texts.Count(x=>x.text == "APROBAR" || x.text == "RETENER" || x.text == "DEVOLVER") != 3) throw new Exception("Decision button labels missing");
                game.StartScenario(ScenarioMode.Attack);
                game.RegisterEvidence(); game.InspectRegistry(); game.InspectAlerts();
                for(int i=0;i<6;i++) game.Decide(Decision.Approve);
                if (!game.Session.Ended || game.Session.SimulatedLoss != 965000) throw new Exception("Attack result mismatch");
                game.StartScenario(ScenarioMode.Defense); game.ApplyDefense();
                for(int i=0;i<6;i++) game.Decide(Decision.Approve);
                if (!game.Session.Ended || game.Session.SimulatedLoss != 0) throw new Exception("Defense result mismatch");
                var teleports = FindObjectsByType<Swift.Interaction.BankInteractable>(FindObjectsSortMode.None).Where(x=>x.Action==Swift.Interaction.BankAction.Teleport).ToArray();
                if (teleports.Length < 2) throw new Exception("Expected at least two teleport points");
                foreach(var target in teleports) { target.Activate(); if(Vector3.Distance(target.Player.position,target.Destination)>.01f) throw new Exception("Teleport failed"); }
                // Restore a view of the desk before capture.
                teleports[0].Player.position = new Vector3(0,0,-1.5f);
                if(errors != 0) throw new Exception("Runtime logged errors: " + errors);
                result = "PASS: Play Mode, TMP panel/fonts, six decisions per mode, evidence, registry, alerts, defense override, results and available teleports.";
            } catch(Exception ex) { result = "FAIL: " + ex; }
            if(result.StartsWith("PASS")) {
                var notices = Resources.Load<TextAsset>("ThirdPartyNotices");
                if (notices == null || !notices.text.Contains("SIL OPEN FONT LICENSE")) throw new Exception("Bundled license notices missing");
                var game = FindFirstObjectByType<GameManager>();
                var input = FindFirstObjectByType<Swift.Interaction.VRInteractionManager>();
                input.SimulateGazeInEditor = true; input.DwellSeconds = .2f;
                input.View.transform.localRotation = Quaternion.identity;
                var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
                target.transform.position = input.View.transform.position + Vector3.forward;
                target.transform.localScale = Vector3.one * .15f;
                var action = target.AddComponent<Swift.Interaction.BankInteractable>(); action.Manager = game; action.Action = Swift.Interaction.BankAction.Registry;
                int activations = 0; Action count = () => activations++; game.Changed += count;
                yield return new WaitForSecondsRealtime(.45f);
                if (activations != 1) result = "FAIL: gaze did not activate once: " + activations;
                yield return new WaitForSecondsRealtime(.35f);
                if (activations != 1) result = "FAIL: gaze retriggered without leaving target";
                input.View.transform.localRotation = Quaternion.Euler(0,70,0);
                yield return new WaitForSecondsRealtime(.1f);
                input.View.transform.localRotation = Quaternion.identity;
                yield return new WaitForSecondsRealtime(.35f);
                if (activations != 2) result = "FAIL: gaze did not reset after leaving target: " + activations;
                game.Changed -= count; Destroy(target); input.SimulateGazeInEditor = false;
                if (result.StartsWith("PASS")) result += " Gaze dwell/latch/reset also passed.";
                game.Notify("Compara los turnos ficticios. El resumen historico permanece fijo.");
            }
            yield return null; yield return null;
            if (errors != 0) result = "FAIL: runtime errors logged: " + errors;
            Directory.CreateDirectory("Artifacts");
#if UNITY_EDITOR
            UnityEditor.ShaderUtil.allowAsyncCompilation = false;
#endif
            if(SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null) {
                var camera = Camera.main;
                var previous = camera.targetTexture;
                var active = RenderTexture.active;
                var image = new RenderTexture(1280,960,24);
                var texture = new Texture2D(1280,960,TextureFormat.RGB24,false);
                try { camera.targetTexture = image; camera.Render(); RenderTexture.active = image; texture.ReadPixels(new Rect(0,0,1280,960),0,0); texture.Apply(); File.WriteAllBytes("Artifacts/prototype-preview.png",texture.EncodeToPNG()); }
                finally { camera.targetTexture = previous; RenderTexture.active = active; Destroy(texture); image.Release(); Destroy(image); }
            }
            File.WriteAllText("Artifacts/playmode-smoke.txt", result);
            Debug.Log(result);
            yield return null;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(result.StartsWith("PASS") ? 0 : 1);
#endif
        }
    }
}






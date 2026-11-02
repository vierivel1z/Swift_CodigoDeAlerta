using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using TMPro;
using Swift.Game;
using Swift.Bank;
using Swift.Interaction;
namespace Swift.Testing
{
    public sealed class BankOfficeProbe : MonoBehaviour
    {
        public bool ExitEditorOnComplete = true;
        int errors;
        void OnEnable() { Application.logMessageReceived += OnLog; }
        void OnDisable() { Application.logMessageReceived -= OnLog; }
        void OnLog(string text,string stack,LogType type) { if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert) errors++; }
        void Capture(string name, Vector3 position, Vector3 look)
        {
#if UNITY_EDITOR
            UnityEditor.ShaderUtil.allowAsyncCompilation = false;
#endif
            var camera = Camera.main; var previousPosition=camera.transform.position; var previousRotation=camera.transform.rotation;
            camera.transform.position=position; camera.transform.LookAt(look);
            var previousTarget=camera.targetTexture; var previousActive=RenderTexture.active;
            var target=new RenderTexture(1440,1000,24); var image=new Texture2D(1440,1000,TextureFormat.RGB24,false);
            try {camera.targetTexture=target; camera.Render(); RenderTexture.active=target; image.ReadPixels(new Rect(0,0,1440,1000),0,0); image.Apply(); File.WriteAllBytes("Artifacts/Office20261028/"+name+".png",image.EncodeToPNG());}
            finally{camera.targetTexture=previousTarget;RenderTexture.active=previousActive;camera.transform.SetPositionAndRotation(previousPosition,previousRotation);target.Release();Destroy(target);Destroy(image);}
        }
        IEnumerator Start()
        {
            Directory.CreateDirectory("Artifacts/Office20261028");
            yield return null; yield return null;
            // Let font geometry and render resources settle before camera captures.
            yield return new WaitForSecondsRealtime(1);
            string result;
            try{
                var game=FindFirstObjectByType<GameManager>();
                var targets=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None);
                var ui=FindFirstObjectByType<Swift.UI.UIManager>();
                if(game==null || ui==null || ui.PanelAnchor==null) throw new Exception("Bank systems/monitor anchor missing");
                if(FindObjectsByType<Camera>(FindObjectsSortMode.None).Length!=1) throw new Exception("Expected one active player camera");
                if(ui.PanelAnchor.GetComponentsInChildren<Canvas>().Length!=1) throw new Exception("Expected one serialized banking canvas");
                if(FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None).Any(t=>t.font==null)) throw new Exception("Missing TMP font");
                targets.First(x=>x.Action==BankAction.Attack).Activate();
                if (!game.IntroPlaying) throw new Exception("Intro did not start from portal");
                game.SkipIntro();
                if (game.IntroPlaying || game.Session == null) throw new Exception("Intro skip did not start selected mode");
                // Decision buttons are inactive in the menu and become active after the portal.
                targets=FindObjectsByType<BankInteractable>(FindObjectsSortMode.None);
                targets.First(x=>x.Action==BankAction.Inspect).Activate();
                targets.First(x=>x.name=="Interactive Printer").Activate();
                if(game.Session.Evidence.Entries.Count!=1) throw new Exception("Printer/evidence action failed");
                targets.First(x=>x.Action==BankAction.Registry).Activate();
                targets.First(x=>x.Action==BankAction.Alerts).Activate();
                Physics.SyncTransforms();
                var eyePoints = targets.Where(x=>x.Action==BankAction.Teleport)
                    .Select(x=>x.Destination + Vector3.up * 1.6f).Append(Camera.main.transform.position).ToArray();
                var rayEvidence = new System.Collections.Generic.List<string>();
                var output = Environment.GetEnvironmentVariable("SWIFT_SESSION_DIR") ?? "Artifacts/Office20261028";
                foreach (var required in new[] { BankAction.Inspect, BankAction.Registry, BankAction.Evidence,
                    BankAction.Alerts, BankAction.Defense, BankAction.End, BankAction.Approve, BankAction.Hold, BankAction.Return })
                {
                    bool reachable = false;
                    foreach (var target in targets.Where(x=>x.Action==required))
                    foreach (var collider in target.GetComponentsInChildren<Collider>())
                    foreach (var origin in eyePoints)
                    {
                        var delta = collider.bounds.center - origin;
                        bool hitSomething = Physics.Raycast(origin, delta.normalized, out var hit,
                            delta.magnitude + .05f, ~0, QueryTriggerInteraction.Ignore);
                        rayEvidence.Add(required + " target=" + target.name + " center=" + collider.bounds.center
                            + " origin=" + origin + " hit=" + (hitSomething ? hit.collider.name + " distance=" + hit.distance : "none"));
                        if (delta.magnitude <= 12f && hitSomething && hit.collider.GetComponentInParent<BankInteractable>() == target)
                        { reachable = true; rayEvidence.Add(required + " reachable from " + origin + " via " + target.name); }
                    }
                    File.WriteAllLines(Path.Combine(output,"interaction-raycasts.txt"),rayEvidence);
                    if (!reachable) throw new Exception("No unobstructed gaze ray from any teleport point for " + required);
                }
                File.WriteAllLines(Path.Combine(output,"interaction-raycasts.txt"),rayEvidence);
                game.Decide(Decision.Approve);
                if(game.Session.Transactions.Records[0].Status!=TransactionStatus.Approved) throw new Exception("Decision not recorded");
                targets.First(x=>x.Action==BankAction.DefenseMode).Activate();
                targets.First(x=>x.Action==BankAction.Defense).Activate();
                if(!game.Session.Defense.Active) throw new Exception("Security panel failed");
                foreach(var pad in targets.Where(x=>x.Action==BankAction.Teleport)){pad.Activate();if(Vector3.Distance(pad.Player.position,pad.Destination)>.01f)throw new Exception("Teleport failed");}
                var rig=Camera.main.transform.parent;rig.position=new Vector3(0,0,-.65f);Camera.main.transform.localRotation=Quaternion.identity;
                game.Notify("Puesto de inspeccion. Todos los datos son ficticios.");
                result="PASS: Play Mode; one camera and serialized monitor canvas; computer, printer/evidence, archive, alerts, defense; nine interaction roles reachable by physics raycasts from available eye points; transaction decision and four teleports. No Android hardware test.";
            }catch(Exception ex){result="FAIL: "+ex;}
            yield return null; yield return null;
            try{
                Capture("office-overview",new Vector3(-.7f,2.65f,-4.2f),new Vector3(0,1.2f,1.7f));
                Capture("office-inspection",new Vector3(0,1.65f,-.65f),new Vector3(0,1.55f,1.5f));
                Capture("office-evidence",new Vector3(-2.7f,1.6f,-.6f),new Vector3(-3.2f,1,2.3f));
                Capture("office-entry",new Vector3(0,1.8f,-.5f),new Vector3(0,1.4f,-4.8f));
                Capture("office-security",new Vector3(3.3f,1.6f,2f),new Vector3(3.6f,1.55f,4.8f));
            }catch(Exception ex){result="FAIL capture: "+ex;}
            if(errors>0) result="FAIL: logged runtime errors="+errors+"; "+result;
            File.WriteAllText("Artifacts/Office20261028/playmode-office.txt",result); Debug.Log(result);
#if UNITY_EDITOR
            if(ExitEditorOnComplete) UnityEditor.EditorApplication.Exit(result.StartsWith("PASS")?0:1);
#endif
        }
    }
}

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Swift.Game;
using Swift.Bank;
using Swift.Interaction;
namespace Swift.Testing
{
    public sealed class GameplayFlowProbe : MonoBehaviour
    {
        public bool ExitEditorOnComplete = true;
        IEnumerator Start()
        {
            yield return null; yield return null;
            var game = FindFirstObjectByType<GameManager>();
            string error = null;
            try {
                game.EnterScenario(ScenarioMode.Attack);
                if (!game.IntroPlaying || game.Session != null) throw new Exception("Portal must defer turn until intro finishes");
                game.SkipIntro();
                if (game.IntroPlaying || game.Session?.Mode != ScenarioMode.Attack) throw new Exception("Skip must enter selected mode");
                for (int i=0;i<6;i++) game.Decide(Decision.Approve);
                if (!game.Session.Ended || game.LastAttackLoss != 965000) throw new Exception("Attack completion mismatch");
                var ui = FindFirstObjectByType<Swift.UI.UIManager>();
                var buttons = ui.PanelAnchor.GetComponentsInChildren<BankInteractable>(true).Where(x=>x.Action==BankAction.Approve || x.Action==BankAction.Hold || x.Action==BankAction.Return).ToArray();
                if (buttons.Length!=3 || buttons.Any(x=>x.gameObject.activeSelf)) throw new Exception("Results must hide all decision buttons");
                game.EnterScenario(ScenarioMode.Defense);
                if(game.IntroPlaying || buttons.Any(x=>!x.gameObject.activeSelf)) throw new Exception("Second portal must restore decisions without repeating intro");
                game.ApplyDefense();
                for(int i=0;i<6;i++) game.Decide(Decision.Approve);
                if (!game.Session.Ended || game.LastDefenseLoss != 0 || game.LastAttackLoss != 965000) throw new Exception("Comparison mismatch");
                // Replay the full production timing, rather than an accelerated stand-in.
                var intro = game.GetComponent<IntroSequence>();
                intro.Play(()=>game.StartScenario(ScenarioMode.Attack));
            } catch(Exception ex) { error = ex.ToString(); }
            yield return new WaitForSecondsRealtime(13f);
            if(error==null && (game.IntroPlaying || game.Session.Mode!=ScenarioMode.Attack || game.Session.Ended)) error="Timed intro failed to finish into fresh selected turn";
            if (error == null)
            {
                try
                {
                    var ui = FindFirstObjectByType<Swift.UI.UIManager>();
                    var buttons = ui.PanelAnchor.GetComponentsInChildren<BankInteractable>(true);
                    foreach (var decision in new[] { BankAction.Approve, BankAction.Return, BankAction.Hold, BankAction.Hold, BankAction.Hold, BankAction.Hold })
                        buttons.Single(x => x.Action == decision).Activate();
                    if (!game.Session.Ended || game.Session.Correct != 6 || game.Session.SimulatedLoss != 0)
                        throw new Exception("Approve/Return/Hold collider actions did not resolve all six records correctly");
                }
                catch (Exception ex) { error = ex.ToString(); }
            }
            var result = error == null ? "PASS: intro skip, full 12-second timed completion, both modes, comparison, button lifecycle and Approve/Return/Hold collider actions (6/6 correct). No hardware/visual certification." : "FAIL: " + error;
            System.IO.Directory.CreateDirectory("Artifacts/GameplayFlow");
            System.IO.File.WriteAllText("Artifacts/GameplayFlow/result.txt",result); Debug.Log(result);
#if UNITY_EDITOR
            if(ExitEditorOnComplete) UnityEditor.EditorApplication.Exit(error==null?0:1);
#endif
        }
    }
}

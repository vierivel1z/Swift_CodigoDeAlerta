using UnityEngine;
using Swift.Game;
using Swift.Bank;
namespace Swift.Interaction
{
    public enum BankAction { Inspect, Registry, Alerts, Evidence, Defense, End, Attack, DefenseMode, Approve, Hold, Return, Teleport }
    public sealed class BankInteractable : MonoBehaviour
    {
        public GameManager Manager;
        public BankAction Action;
        public Transform Player;
        public Vector3 Destination;
        public void Activate()
        {
            if (Action == BankAction.Teleport) { if (Player != null) Player.position = Destination; return; }
            if (Manager == null) return;
            switch (Action) {
                case BankAction.Attack: Manager.StartScenario(ScenarioMode.Attack); break;
                case BankAction.DefenseMode: Manager.StartScenario(ScenarioMode.Defense); break;
                case BankAction.Approve: Manager.Decide(Decision.Approve); break;
                case BankAction.Hold: Manager.Decide(Decision.Hold); break;
                case BankAction.Return: Manager.Decide(Decision.Return); break;
                case BankAction.Registry: Manager.InspectRegistry(); break;
                case BankAction.Alerts: Manager.InspectAlerts(); break;
                case BankAction.Evidence: Manager.RegisterEvidence(); break;
                case BankAction.Defense: Manager.ApplyDefense(); break;
                case BankAction.End: Manager.EndScenario(); break;
                default: Manager.Notify("Consulta el expediente central y compara el registro y las alertas."); break;
            }
        }
    }
}

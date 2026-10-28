using UnityEngine;
namespace Swift.Testing
{
    public sealed class OfficeBootstrap : MonoBehaviour
    {
        public Swift.Game.ScenarioData Scenario;
        void Awake() { if (FindFirstObjectByType<Swift.Game.GameManager>() == null) OfficeFactory.Build(Scenario); }
    }
}

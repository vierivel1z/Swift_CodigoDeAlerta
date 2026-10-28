using UnityEngine;
using Swift.Bank;
namespace Swift.Game
{
    [CreateAssetMenu(menuName = "SWIFT/Scenario Data")]
    public sealed class ScenarioData : ScriptableObject
    {
        public TransactionData[] Transactions = TrainingData.Create();
        [TextArea(4, 10)] public string HistoricalContext = TrainingData.HistoricalSummary;
    }
}

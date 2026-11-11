using System;
using System.Linq;
using Swift.Bank;
static class Program
{
    static int checks;
    static void Check(bool value, string name) { if (!value) throw new Exception(name); checks++; Console.WriteLine("PASS " + name); }
    static void Main()
    {
        var data = TrainingData.Create();
        Check(data.Length == 6 && data.Select(x=>x.Id).Distinct().Count()==6, "six unique fictional records");
        Check(TransactionManager.Recommended(data[0]) == Decision.Approve, "legitimate approves");
        Check(TransactionManager.Recommended(data[1]) == Decision.Return, "missing intermediary returns");
        Check(data.Skip(2).All(x=>TransactionManager.Recommended(x)==Decision.Hold), "risk requires review");
        Check(!data[5].SimulatedFraud && data[5].Anomalies == Anomaly.AlertMatch, "alert match can be legitimate");
        var attack = new GameSession(ScenarioMode.Attack, data);
        Check(!attack.EndScenario(), "cannot finish incomplete turn");
        Check(!attack.ApplyDefense(), "attack cannot enable defense");
        Check(attack.RegisterEvidence("observacion") && !attack.RegisterEvidence("observacion"), "evidence deduplicates");
        Check(!attack.RegisterEvidence(" "), "blank evidence rejected");
        foreach(var row in data) Check(attack.Decide(Decision.Approve), "attack decision " + row.Id);
        Check(attack.SimulatedLoss == 965000, "simulated loss tracked separately from historical totals");
        Check(attack.EndScenario() && !attack.Decide(Decision.Return) && !attack.EndScenario(), "finished turn immutable");
        var defense = new GameSession(ScenarioMode.Defense, data);
        Check(defense.ApplyDefense() && !defense.ApplyDefense(), "defense enables once");
        foreach(var row in data) defense.Decide(Decision.Approve);
        Check(defense.SimulatedLoss == 0, "controls prevent simulated fraudulent approvals");
        Check(defense.Transactions.Records[0].Status == TransactionStatus.Approved, "controls preserve legitimate approval");
        Check(defense.Transactions.Records[1].Status == TransactionStatus.Returned, "controls distinguish format");
        Check(defense.Transactions.Records[5].Status == TransactionStatus.Held, "alert held without confirming fraud");
        Check(defense.EndScenario(), "defense can finish");
        // Demonstration: identical analyst choices isolate the reinforced review's effect.
        var demoChoices = new[] { Decision.Approve, Decision.Return, Decision.Approve, Decision.Hold, Decision.Hold, Decision.Hold };
        var demoAttack = new GameSession(ScenarioMode.Attack, TrainingData.Create());
        var demoDefense = new GameSession(ScenarioMode.Defense, TrainingData.Create());
        Check(demoDefense.ApplyDefense(), "demo defense control activated explicitly");
        foreach (var choice in demoChoices) {
            Check(demoAttack.Decide(choice) && demoDefense.Decide(choice), "identical demo decision accepted");
        }
        Check(demoAttack.Correct == 5 && demoDefense.Correct == 5, "control does not improve analyst score");
        Check(demoAttack.SimulatedLoss == 20000 && demoDefense.SimulatedLoss == 0, "same demo choices isolate simulated loss difference");
        Check(demoAttack.Transactions.Records[2].Status == TransactionStatus.Approved &&
              demoDefense.Transactions.Records[2].Status == TransactionStatus.Held &&
              demoDefense.Transactions.Records[2].DefenseOverride, "SIM-003 original approval differs from effective defended state");
        var correct = new GameSession(ScenarioMode.Defense, TrainingData.Create());
        foreach(var row in data) correct.Decide(TransactionManager.Recommended(row));
        Check(correct.Correct == 6 && correct.SimulatedLoss == 0, "recommended decisions score correctly");
        Check(data.All(x=>!string.IsNullOrEmpty(x.Account)), "source data unmodified by sessions");
        var invalid = new GameSession(ScenarioMode.Attack, data);
        Check(!invalid.Decide((Decision)999) && invalid.Transactions.Current.Data.Id == data[0].Id, "invalid decision rejected without advancing");
        Check(TrainingData.HistoricalSummary.Contains("951") && TrainingData.HistoricalSummary.Contains("81"), "historical summary remains fixed");
        Console.WriteLine($"TOTAL {checks} checks passed");
    }
}

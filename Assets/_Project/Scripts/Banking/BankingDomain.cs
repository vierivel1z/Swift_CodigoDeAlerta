using System;
using System.Collections.Generic;
using System.Linq;

namespace Swift.Bank
{
    public enum ScenarioMode { Attack, Defense }
    public enum TransactionStatus { Pending, Approved, Held, Returned }
    public enum Decision { Approve, Hold, Return }
    [Flags] public enum Anomaly { None = 0, MissingIntermediary = 1, NameMismatch = 2, UnknownAccount = 4, UnusualAmount = 8, AlertMatch = 16 }
    [Serializable]
    public sealed class TransactionData
    {
        public string Id, OriginBank, Beneficiary, Account, IntermediaryBank, Date, RegisteredName;
        public long Amount;
        public Anomaly Anomalies;
        // Author-only label for outcome metrics; never shown as automatic proof to the player.
        public bool SimulatedFraud;
        public TransactionData(string id, string beneficiary, long amount, Anomaly anomalies = Anomaly.None, bool simulatedFraud = false)
        {
            Id = id; Beneficiary = beneficiary; Amount = amount; Anomalies = anomalies; SimulatedFraud = simulatedFraud;
            OriginBank = "BANCO ANDINO (ficticio)"; Account = "FIC-" + id; Date = "05/02/2016 (simulada)";
            IntermediaryBank = (anomalies & Anomaly.MissingIntermediary) != 0 ? "" : "Banco Pacifico Insular (ficticio)";
            RegisteredName = (anomalies & Anomaly.NameMismatch) != 0 ? "Fundacion Horizonte de Papel" : beneficiary;
        }
    }
    public sealed class TransactionRecord
    {
        public TransactionData Data { get; }
        public TransactionStatus Status { get; internal set; }
        public bool DefenseOverride { get; internal set; }
        public bool? CorrectDecision { get; internal set; }
        public TransactionRecord(TransactionData data) { Data = data; }
    }
    public sealed class TransactionManager
    {
        readonly List<TransactionRecord> records;
        public IReadOnlyList<TransactionRecord> Records => records.AsReadOnly();
        public TransactionRecord Current => records.FirstOrDefault(x => x.Status == TransactionStatus.Pending);
        public bool Complete => Current == null;
        public TransactionManager(IEnumerable<TransactionData> data)
        {
            var source = data.ToArray();
            if (source.Length == 0 || source.Any(x => x == null || string.IsNullOrWhiteSpace(x.Id)) || source.Select(x => x.Id).Distinct().Count() != source.Length)
                throw new ArgumentException("Expected nonempty transactions with unique IDs.");
            records = source.Select(x => new TransactionRecord(x)).ToList();
        }
        public static Decision Recommended(TransactionData data)
        {
            if ((data.Anomalies & Anomaly.MissingIntermediary) != 0) return Decision.Return;
            return data.Anomalies == Anomaly.None ? Decision.Approve : Decision.Hold;
        }
        public bool Decide(Decision decision, bool defense)
        {
            var current = Current;
            if (current == null || !Enum.IsDefined(typeof(Decision), decision)) return false;
            current.CorrectDecision = decision == Recommended(current.Data);
            var effective = decision;
            // Reinforced review acts on observable risk, not on the hidden fraud label.
            if (defense && decision == Decision.Approve && current.Data.Anomalies != Anomaly.None)
            { effective = Recommended(current.Data); current.DefenseOverride = true; }
            current.Status = effective == Decision.Approve ? TransactionStatus.Approved : effective == Decision.Hold ? TransactionStatus.Held : TransactionStatus.Returned;
            return true;
        }
    }
    public sealed class EvidenceManager
    {
        readonly HashSet<string> entries = new HashSet<string>();
        public IReadOnlyCollection<string> Entries => entries.ToArray();
        public bool RegisterEvidence(string transactionId, string observation)
        {
            if (string.IsNullOrWhiteSpace(transactionId) || string.IsNullOrWhiteSpace(observation)) return false;
            return entries.Add(transactionId.Trim() + ": " + observation.Trim());
        }
    }
    public sealed class DefenseManager
    {
        public bool Active { get; private set; }
        public bool ApplyDefense(ScenarioMode mode)
        { if (mode != ScenarioMode.Defense || Active) return false; Active = true; return true; }
    }
    public sealed class GameSession
    {
        public ScenarioMode Mode { get; }
        public TransactionManager Transactions { get; }
        public EvidenceManager Evidence { get; } = new EvidenceManager();
        public DefenseManager Defense { get; } = new DefenseManager();
        public bool Ended { get; private set; }
        public int Correct => Transactions.Records.Count(x => x.CorrectDecision == true);
        public long SimulatedLoss => Transactions.Records.Where(x => x.Data.SimulatedFraud && x.Status == TransactionStatus.Approved).Sum(x => x.Data.Amount);
        public GameSession(ScenarioMode mode, IEnumerable<TransactionData> data) { Mode = mode; Transactions = new TransactionManager(data); }
        public bool Decide(Decision decision) => !Ended && Transactions.Decide(decision, Defense.Active);
        public bool RegisterEvidence(string observation) => !Ended && Transactions.Current != null && Evidence.RegisterEvidence(Transactions.Current.Data.Id, observation);
        public bool ApplyDefense() => !Ended && Defense.ApplyDefense(Mode);
        public bool EndScenario() { if (Ended || !Transactions.Complete) return false; Ended = true; return true; }
    }
    public static class TrainingData
    {
        public static TransactionData[] Create() => new[] {
            new TransactionData("SIM-001", "Taller Nube Azul", 1200),
            new TransactionData("SIM-002", "Cooperativa Loma Clara", 8200, Anomaly.MissingIntermediary),
            new TransactionData("SIM-003", "Fundacion Horizonte de Papell", 20000, Anomaly.NameMismatch, true),
            new TransactionData("SIM-004", "Servicios Isla Inventada", 45000, Anomaly.UnknownAccount, true),
            new TransactionData("SIM-005", "Comercio Brisa Ficticia", 900000, Anomaly.UnusualAmount, true),
            new TransactionData("SIM-006", "Asociacion Faro de Tinta", 7000, Anomaly.AlertMatch)
        };
        public const string HistoricalSummary = "CONTEXTO 2016 (resumen del material docente)\n36 ordenes / US$951 millones; 30 (~US$850 millones) bloqueadas; 4 (US$81 millones) pagadas a Manila; 1 (US$20 millones) detenida/devuelta en Sri Lanka; 1 no reenviada.\nLas seis ordenes jugables son inventadas y no reproducen esas transferencias.";
        public static string Alerts(TransactionData data)
        {
            var lines = new List<string>();
            if ((data.Anomalies & Anomaly.MissingIntermediary) != 0) lines.Add("Falta banco intermediario: devolver por formato.");
            if ((data.Anomalies & Anomaly.NameMismatch) != 0) lines.Add("Nombre difiere del registro: retener y verificar.");
            if ((data.Anomalies & Anomaly.UnknownAccount) != 0) lines.Add("Cuenta no consta en el registro ficticio: retener.");
            if ((data.Anomalies & Anomaly.UnusualAmount) != 0) lines.Add("Monto fuera del perfil simulado: revisar.");
            if ((data.Anomalies & Anomaly.AlertMatch) != 0) lines.Add("Coincidencia parcial en alertas: no confirma fraude; revisar.");
            return lines.Count == 0 ? "Sin discrepancias en las comprobaciones simuladas." : string.Join("\n", lines);
        }
    }
}

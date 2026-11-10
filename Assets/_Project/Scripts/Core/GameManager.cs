using System;
using UnityEngine;
using Swift.Bank;
namespace Swift.Game
{
    public sealed class GameManager : MonoBehaviour
    {
        public ScenarioData Scenario;
        public GameSession Session { get; private set; }
        public string Message { get; private set; } = "Elige un modo en el portal para comenzar.";
        public long? LastAttackLoss { get; private set; }
        public long? LastDefenseLoss { get; private set; }
        public event Action Changed;
        // Observation only; emitted after the unchanged domain accepts the decision.
        public event Action<GameSession, TransactionRecord, Decision> DecisionCommitted;
        public bool IntroPlaying => GetComponent<IntroSequence>()?.IsPlaying == true;
        public void EnterScenario(ScenarioMode mode)
        {
            if (IntroPlaying) return;
            var intro = GetComponent<IntroSequence>() ?? gameObject.AddComponent<IntroSequence>();
            if (intro.HasPlayed) StartScenario(mode);
            else intro.Play(() => StartScenario(mode));
        }
        public void SkipIntro() { GetComponent<IntroSequence>()?.Skip(); }
        public void StartScenario(ScenarioMode mode)
        {
            Session = new GameSession(mode, Scenario != null ? Scenario.Transactions : TrainingData.Create());
            Notify(mode == ScenarioMode.Attack ? "Entrenamiento ataque: las decisiones son ficticias; historia fija al final." : "Defensa simulada: activa controles en el panel de seguridad.");
        }
        public void Decide(Decision decision)
        {
            var committingSession = Session;
            var committingRecord = Session?.Transactions.Current;
            if (IntroPlaying || Session == null || !Session.Decide(decision)) { Notify("Inicia un turno o consulta resultados."); return; }
            DecisionCommitted?.Invoke(committingSession, committingRecord, decision);
            var records = Session.Transactions.Records;
            var last = System.Linq.Enumerable.Last(records, x => x.Status != TransactionStatus.Pending);
            Notify(last.DefenseOverride ? "Control reforzado: operacion enviada a revision/formato." : last.CorrectDecision == true ? "Decision coherente con la evidencia disponible." : "Revisa los criterios: formato se devuelve; riesgo se retiene.");
            if (Session.Transactions.Complete) EndScenario();
        }
        public void RegisterEvidence() { RegisterEvidence(Session?.Transactions.Current == null ? "" : TrainingData.Alerts(Session.Transactions.Current.Data)); }
        public void RegisterEvidence(string observation)
        { Notify(Session != null && Session.RegisterEvidence(observation) ? "Evidencia registrada." : "Sin expediente activo o evidencia ya registrada."); }
        public void ApplyDefense()
        { Notify(Session != null && Session.ApplyDefense() ? "Controles activos: formato, registro y revision reforzada." : "Disponible una vez por turno del modo defensa."); }
        public void EndScenario()
        {
            if (Session == null || !Session.EndScenario()) { Notify(Session?.Ended == true ? "Turno ya cerrado." : "Resuelve todos los expedientes antes de finalizar."); return; }
            if (Session.Mode == ScenarioMode.Attack) LastAttackLoss = Session.SimulatedLoss; else LastDefenseLoss = Session.SimulatedLoss;
            Notify("Turno finalizado. Revisa el resumen y compara ambos modos.");
        }
        public void InspectRegistry()
        {
            var data = Session?.Transactions.Current?.Data;
            Notify(data == null ? "No hay expediente activo." : (data.Anomalies & Anomaly.UnknownAccount) != 0 ? "REGISTRO FICTICIO: cuenta no encontrada; requiere verificacion." : "REGISTRO FICTICIO: " + data.Account + " / Titular: " + data.RegisteredName);
        }
        public void InspectAlerts() { var data = Session?.Transactions.Current?.Data; Notify(data == null ? "No hay expediente activo." : TrainingData.Alerts(data)); }
        public void Notify(string value) { Message = value; Changed?.Invoke(); }
    }
}

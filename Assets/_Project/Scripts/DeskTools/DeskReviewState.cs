using System;
using System.Collections.Generic;
using System.Linq;
using Swift.Bank;

namespace Swift.DeskTools
{
    public enum VerificationOutcome { FieldStatement, Inconclusive, NoAnswer }
    public enum VerificationState { Ready, Requested, Ringing, Connected, Reviewing, Recorded, Cancelled }
    [Serializable]
    public sealed class VerificationScript
    {
        public string CallId = "SIM003-REGISTRY-PROTOTYPE";
        public string TransactionId = "SIM-003";
        public string Speaker = "Mesa de registros / conversación ficticia";
        public bool PrototypeContent = true;
        public VerificationOutcome Outcome;
        public string[] Lines = {
            "Solicito contrastar el nombre asociado a la cuenta del expediente.",
            "El nombre que figura en nuestro registro es: {RegisteredName}.",
            "Esta consulta no confirma la autorización de la transferencia."
        };
        public string Evidence = "[PRUEBA DE GUION] Consulta telefónica ficticia {Id}; fuente: Mesa de registros; nombre comunicado: {RegisteredName}; la autorización no fue confirmada.";
        public string Validate(IEnumerable<TransactionData> source)
        {
            if (string.IsNullOrWhiteSpace(CallId) || string.IsNullOrWhiteSpace(Speaker)) return "Falta identificador o fuente.";
            if (source == null || !source.Any(x => x != null && x.Id == TransactionId)) return "Expediente inexistente.";
            if (Lines == null || Lines.Length == 0 || Lines.Any(string.IsNullOrWhiteSpace) || string.IsNullOrWhiteSpace(Evidence)) return "Diálogo incompleto.";
            if (!Enum.IsDefined(typeof(VerificationOutcome), Outcome)) return "Resultado no reconocido.";
            foreach (var value in Lines.Concat(new[] { Evidence, Speaker })) {
                var safe = value.Replace("{RegisteredName}", "").Replace("{Account}", "").Replace("{Id}", "");
                if (safe.Contains("{") || safe.Contains("}")) return "Token no permitido.";
                foreach (var word in new[] { "APROBAR", "RETENER", "DEVOLVER", "SIMULATEDFRAUD", "RECOMMENDED" })
                    if (safe.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) return "La consulta revela una decisión o un dato interno.";
            }
            return null;
        }
        public static string Render(string value, TransactionData data) => (value ?? "").Replace("{Id}", data.Id ?? "")
            .Replace("{Account}", data.Account ?? "No informado").Replace("{RegisteredName}", data.RegisteredName ?? "No informado");
    }
    public sealed class VerificationConversation
    {
        public GameSession Session { get; }
        public TransactionRecord Record { get; }
        public VerificationScript Script { get; }
        public VerificationState State { get; private set; } = VerificationState.Ready;
        public VerificationConversation(GameSession session, VerificationScript script) {
            Session = session; Record = session?.Transactions.Current; Script = script;
        }
        public bool ContextMatches(GameSession live) => ReferenceEquals(live, Session) && live != null && !live.Ended &&
            live.Mode == ScenarioMode.Defense && ReferenceEquals(live.Transactions.Current, Record) && Record != null &&
            Record.Status == TransactionStatus.Pending && Record.Data.Id == Script?.TransactionId;
        public bool Request(GameSession live) { if (!ContextMatches(live) || State != VerificationState.Ready) return false; State=VerificationState.Requested; return true; }
        public bool Ring(GameSession live) { if (!ContextMatches(live) || State!=VerificationState.Requested) return false; State=VerificationState.Ringing; return true; }
        public bool Answer(GameSession live) {
            if (!ContextMatches(live) || State!=VerificationState.Ringing) return false;
            State=Script.Outcome==VerificationOutcome.NoAnswer?VerificationState.Reviewing:VerificationState.Connected; return true;
        }
        public bool FinishReading(GameSession live) {
            if (!ContextMatches(live) || State!=VerificationState.Connected) return false; State=VerificationState.Reviewing; return true;
        }
        public bool CanRegister(GameSession live) => ContextMatches(live) && State==VerificationState.Reviewing;
        public bool MarkRecorded(GameSession live) { if (!CanRegister(live)) return false; State=VerificationState.Recorded; return true; }
        public void Cancel() { if (State!=VerificationState.Recorded) State=VerificationState.Cancelled; }
        public string EvidenceText => Record==null?"":Script.Outcome==VerificationOutcome.NoAnswer?
            (Script.PrototypeContent?"[PRUEBA DE GUION] ":"")+"Consulta telefónica ficticia "+Record.Data.Id+"; fuente: "+Script.Speaker+"; no se obtuvo respuesta. No confirma autorización.":
            Script.Outcome==VerificationOutcome.Inconclusive?
            (Script.PrototypeContent?"[PRUEBA DE GUION] ":"")+"Consulta telefónica ficticia "+Record.Data.Id+"; fuente: "+Script.Speaker+"; no pudo confirmar la información.":
            VerificationScript.Render(Script.Evidence,Record.Data);
    }
    public sealed class ReviewNotes
    {
        public readonly bool[] ManualChecks = new bool[3];
        public readonly HashSet<string> Sources = new HashSet<string>();
        public string PhoneEvidence;
        public Decision? ChosenDecision;
        public TransactionStatus? EffectiveStatus;
        public bool DefenseIntervened;
    }
    // Presentation journal only. No changes to the banking session or score.
    public sealed class DeskSessionJournal
    {
        public GameSession Session { get; private set; }
        readonly Dictionary<string,ReviewNotes> entries = new Dictionary<string,ReviewNotes>();
        public void Bind(GameSession live) { if (ReferenceEquals(Session,live)) return; Session=live; entries.Clear(); }
        public ReviewNotes For(string id) {
            if (string.IsNullOrEmpty(id)) return null;
            if (!entries.TryGetValue(id,out var notes)) entries.Add(id,notes=new ReviewNotes());
            return notes;
        }
        public bool Toggle(GameSession live,string id,int group) {
            if (!ReferenceEquals(live,Session) || live?.Transactions.Current?.Data.Id!=id || group<0 || group>=3) return false;
            var notes=For(id); notes.ManualChecks[group]=!notes.ManualChecks[group]; return true;
        }
        public void Consult(string id,string source) { if (Session!=null&&!string.IsNullOrEmpty(id)) For(id).Sources.Add(source); }
        public bool RecordPhone(VerificationConversation call,GameSession live) {
            if (!ReferenceEquals(Session,live) || !call.ContextMatches(live) || call.State!=VerificationState.Recorded) return false;
            var notes=For(call.Record.Data.Id); if (notes.PhoneEvidence!=null) return false;
            notes.PhoneEvidence=call.EvidenceText; notes.Sources.Add("Teléfono"); return true;
        }
        public void DecisionCommitted(GameSession live,TransactionRecord row,Decision choice) {
            if (!ReferenceEquals(Session,live) || row==null || row.Status==TransactionStatus.Pending) return;
            var notes=For(row.Data.Id); notes.ChosenDecision=choice; notes.EffectiveStatus=row.Status; notes.DefenseIntervened=row.DefenseOverride;
        }
    }
}

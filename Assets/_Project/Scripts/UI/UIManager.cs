using UnityEngine;
using TMPro;
using Swift.Bank;
using Swift.Game;
using Swift.Interaction;
namespace Swift.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        public GameManager Manager;
        public TMP_FontAsset Font;
        public Transform PanelAnchor;
        public bool CompactMonitor;
        [SerializeField] TextMeshProUGUI dossier, feedback, summary;
        void Start()
        {
            BuildInterface();
            if (dossier == null || Manager == null) return;
            Manager.Changed += Refresh; Refresh();
        }
        public void BuildInterface()
        {
            if (dossier != null) return;
            if (Font == null) Font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (Font == null) { Debug.LogError("SWIFT: importa TMP Essential Resources antes de jugar."); enabled = false; return; }
            var canvasObject = new GameObject("Expedientes - World Space", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(PanelAnchor != null ? PanelAnchor : transform, false);
            if (PanelAnchor == null) canvasObject.transform.position = new Vector3(0, 2.1f, 1.9f);
            canvasObject.transform.localScale = Vector3.one * (CompactMonitor ? 0.0015f : 0.0027f);
            var rect = canvasObject.GetComponent<RectTransform>(); rect.sizeDelta = new Vector2(1100, CompactMonitor ? 800 : 1030);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            var backdrop = new GameObject("Fondo de lectura", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            backdrop.transform.SetParent(rect, false);
            backdrop.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;
            backdrop.GetComponent<UnityEngine.UI.Image>().color = new Color(.015f,.03f,.055f,1f);
            backdrop.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
            dossier = Text(rect, "Expediente", new Vector2(0, CompactMonitor ? 195 : 275), new Vector2(1080, CompactMonitor ? 390 : 450), CompactMonitor ? 28 : 30);
            feedback = Text(rect, "Feedback", new Vector2(0, -110), new Vector2(1080, CompactMonitor ? 120 : 140), CompactMonitor ? 26 : 28);
            summary = Text(rect, "Resumen", new Vector2(0, CompactMonitor ? -290 : -360), new Vector2(1080, CompactMonitor ? 220 : 350), CompactMonitor ? 16 : 24);
            Button(rect, "APROBAR", -350, CompactMonitor ? -15 : 20, BankAction.Approve);
            Button(rect, "RETENER", 0, CompactMonitor ? -15 : 20, BankAction.Hold);
            Button(rect, "DEVOLVER", 350, CompactMonitor ? -15 : 20, BankAction.Return);
            if (Manager != null) Refresh();
        }
        public TextMeshProUGUI Text(Transform parent, string name, Vector2 position, Vector2 size, float fontSize)
        {
            var obj = new GameObject(name, typeof(RectTransform)); obj.transform.SetParent(parent, false);
            var rect = obj.GetComponent<RectTransform>(); rect.anchoredPosition = position; rect.sizeDelta = size;
            var text = obj.AddComponent<TextMeshProUGUI>(); text.font = Font; text.fontSize = fontSize;
            text.text = name;
            text.color = Color.white; text.alignment = TextAlignmentOptions.TopLeft; text.raycastTarget = false;
            return text;
        }
        void Button(Transform parent, string label, float x, float y, BankAction action)
        {
            var text = Text(parent, label, new Vector2(x, y), new Vector2(310, 65), 32);
            text.alignment = TextAlignmentOptions.Center; text.color = new Color(0.4f, 1, 0.8f);
            var collider = text.gameObject.AddComponent<BoxCollider>(); collider.size = new Vector3(310, 65, 12);
            var target = text.gameObject.AddComponent<BankInteractable>(); target.Manager = Manager; target.Action = action;
        }
        void Refresh()
        {
            var session = Manager.Session;
            var current = session?.Transactions.Current;
            dossier.text = current == null ? "SWIFT: CODIGO DE ALERTA\nBanco Corresponsal Internacional\nUsa el portal ATAQUE o DEFENSA.\nPC: clic / boton derecho para mirar / WASD.\nVR: mira un objeto durante 1.5 segundos.\nTodos los expedientes y cuentas son ficticios." :
                $"{current.Data.Id} | MODO {(session.Mode == ScenarioMode.Attack ? "ATAQUE" : "DEFENSA")} | {StatusText(current.Status)}\nOrigen: {current.Data.OriginBank}\nBeneficiario: {current.Data.Beneficiary}\nCuenta: {current.Data.Account} | USD {current.Data.Amount:N0}\nIntermediario: {(string.IsNullOrEmpty(current.Data.IntermediaryBank) ? "AUSENTE" : current.Data.IntermediaryBank)}\nFecha: {current.Data.Date}\n{TrainingData.Alerts(current.Data)}";
            feedback.text = Manager.Message;
            if (session == null) summary.text = "Consulta monitor, archivador, alertas, evidencias y seguridad.\nCoincidencia sospechosa no equivale a fraude confirmado.";
            else if (!session.Ended) summary.text = $"Resueltos: {System.Linq.Enumerable.Count(session.Transactions.Records, x => x.Status != TransactionStatus.Pending)}/{session.Transactions.Records.Count} | Evidencias: {session.Evidence.Entries.Count}\nControles: {(session.Defense.Active ? "ACTIVOS" : "sin activar")}\nAprobar: sin anomalias. Devolver: formato. Retener: requiere revision.";
            else {
                dossier.text = "RESULTADOS DEL TURNO FICTICIO\n" + string.Join("\n", System.Linq.Enumerable.Select(session.Transactions.Records, x => $"{x.Data.Id}: {StatusText(x.Status)}{(x.DefenseOverride ? " (control reforzado)" : "")}"));
                summary.text = $"Decisiones coherentes: {session.Correct}/{session.Transactions.Records.Count} | Perdida ficticia: USD {session.SimulatedLoss:N0}\n" + (Manager.Scenario != null ? Manager.Scenario.HistoricalContext : TrainingData.HistoricalSummary) + "\nComparativa ficticia USD: ataque " + (Manager.LastAttackLoss?.ToString("N0") ?? "pendiente") + " / defensa " + (Manager.LastDefenseLoss?.ToString("N0") ?? "pendiente") + "\nDefensa = contrafactual; no cambia los hechos historicos.\nEquipo 05 - A Toda Olla. Primitivas/audio originales. Codigo asistido por IA (Codex). Cardboard: Apache-2.0. Fuente Liberation Sans: OFL-1.1.";
            }
        }
        static string StatusText(TransactionStatus status) => status == TransactionStatus.Approved ? "Aprobada" : status == TransactionStatus.Held ? "Retenida" : status == TransactionStatus.Returned ? "Devuelta" : "Pendiente";
        void OnDestroy() { if (Manager != null) Manager.Changed -= Refresh; }
    }
}









using UnityEngine;
using System.Linq;
using TMPro;
using Swift.Game;
using Swift.Bank;
namespace Swift.UI
{
    // Read-only projection of existing training data. Never calls Recommended, Decide, or hidden fraud labels.
    public sealed class FormatReferencePanel : MonoBehaviour
    {
        public GameManager Manager;
        public TMP_Text Observed, ExistingAlert;
        [TextArea] public string RuleText="REFERENCIA DE LA SIMULACION\nRevisa emisor, beneficiario, cuenta, monto e intermediario.\nContrasta REGISTRO y ALERTAS antes de decidir.";
        public TMP_Text Rules;
        public bool CompactSupport;
        public TMP_Text SupportRecord;
        void OnEnable(){if(Manager!=null)Manager.Changed+=Refresh;Refresh();}
        void OnDisable(){if(Manager!=null)Manager.Changed-=Refresh;}
        void Start(){Refresh();}
        public void Refresh()
        {
            if(Rules!=null)Rules.text=RuleText;
            var data=Manager?.Session?.Transactions.Current?.Data;
            if(CompactSupport){
                if(SupportRecord!=null)SupportRecord.text=data==null?"SIN EXPEDIENTE ACTIVO":"EXPEDIENTE / "+data.Id;
                if(Observed!=null)Observed.text=data==null?"Sin expediente activo":Value(data.Account)+"\n"+Value(data.Beneficiary);
                if(ExistingAlert!=null){var lines=data==null?new[]{"Consulta las pesta\u00f1as para contrastar informaci\u00f3n."}:ObservedAlerts(data).Split('\n');ExistingAlert.text=lines.Length<=2?string.Join("\n",lines):lines[0]+"\n+"+(lines.Length-1)+" observaciones. Ver ALERTAS.";}
                return;
            }

            if(Observed!=null)Observed.text=data==null?"SIN EXPEDIENTE ACTIVO\nSelecciona un modo en la consola de entrada.":
                "INFORMACION OBSERVADA / "+data.Id+"\nEmisor: "+Value(data.OriginBank)+"\nBeneficiario: "+Value(data.Beneficiary)+"\nCuenta: "+Value(data.Account)+"\nIntermediario: "+Value(data.IntermediaryBank);
            if(ExistingAlert!=null)ExistingAlert.text=data==null?"REVISION MANUAL\nLas coincidencias requieren contexto; no confirman fraude.":
                "ALERTA EXISTENTE\n"+ObservedAlerts(data)+"\nLa decision corresponde al analista.";
        }
        // Existing authored observations, with decision instructions removed only from presentation.
        public static string ObservedAlerts(TransactionData data)=>ObservationText(TrainingData.Alerts(data));
        public static string ObservationText(string value)=>(value??"")
            .Replace(": devolver por formato.",": campo no informado.")
            .Replace(": retener y verificar.",": discrepancia disponible.")
            .Replace(": retener.",".");
        static string Value(string s)=>string.IsNullOrEmpty(s)?"No informado":s;
        public void OpenReference(){FindFirstObjectByType<UIManager>()?.ShowDocument(BankDocumentUI.Page.Procedure);}
    }
}

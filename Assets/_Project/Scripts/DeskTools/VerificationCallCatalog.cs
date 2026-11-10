using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Swift.Bank;
namespace Swift.DeskTools
{
    [CreateAssetMenu(menuName="SWIFT/Verification call catalog")]
    public sealed class VerificationCallCatalog : ScriptableObject
    {
        [TextArea] public string ContentStatus="PROTOTIPO: guion propuesto por Astra; pendiente de aprobación editorial.";
        public VerificationScript[] Calls = { new VerificationScript() };
        public string Validate(IEnumerable<TransactionData> data) {
            if (Calls==null || Calls.Length==0) return "Catálogo vacío.";
            if (Calls.Any(x=>x==null)) return "Definición nula.";
            if (Calls.Select(x=>x.CallId).Distinct().Count()!=Calls.Length || Calls.Select(x=>x.TransactionId).Distinct().Count()!=Calls.Length) return "Llamadas duplicadas.";
            foreach (var call in Calls) { var error=call.Validate(data); if(error!=null)return error; }
            return null;
        }
        public VerificationScript For(string id) => Calls?.FirstOrDefault(x=>x!=null&&x.TransactionId==id);
    }
}

using UnityEngine;
namespace Swift.DeskTools
{
    public enum ReferencePresentation { LegacySecondary, ClipboardTrial, ClipboardPrimary }
    [CreateAssetMenu(menuName="SWIFT/Desk tools profile")]
    public sealed class DeskToolsProfile : ScriptableObject
    {
        public bool ClipboardEnabled,PhoneEnabled,SeatPolishEnabled,PrinterFeedbackEnabled,DocumentInspectionEnabled;
        public bool AllowPrototypeCalls;
        public bool PhysicalClipboardEnabled;
        public bool Sim003PilotEnabled;
        public bool Sim003PrinterEnabled;
        public bool Sim003PrinterFailsInAttack;
        public bool PolishedClipboardVisual;
        public ReferencePresentation Reference = ReferencePresentation.ClipboardTrial;
        public VerificationCallCatalog Calls;
        public GameObject PhonePrefab;
        public GameObject InspectionPrefab;
    }
}

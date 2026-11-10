using UnityEngine;
namespace Swift.DeskTools
{
    public enum DeskCommand { Clipboard, Previous, Next, Mark, Close, Phone, RequestCall, AnswerCall, EndCall, RegisterCall, Document, Debrief }
    public sealed class DeskActionTarget : MonoBehaviour
    {
        public DeskCommand Command;
        [System.NonSerialized] public DeskInteractionCoordinator Owner;
        public void Activate() { if (Owner!=null) Owner.Handle(Command); }
    }
}

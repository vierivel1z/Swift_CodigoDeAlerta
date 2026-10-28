using UnityEngine;
using UnityEngine.InputSystem;
namespace Swift.Interaction
{
    public sealed class VRInteractionManager : MonoBehaviour
    {
        public Camera View;
        public bool SimulateGazeInEditor;
        public float DwellSeconds = 1.5f;
        public float Reach = 12f;
        public Transform Reticle;
        BankInteractable focused;
        float elapsed;
        bool activated;
        void Update()
        {
            if (View == null) return;
            bool pc = (Application.isEditor || !Application.isMobilePlatform) && !SimulateGazeInEditor;
            Ray ray = pc && Mouse.current != null ? View.ScreenPointToRay(Mouse.current.position.ReadValue()) : new Ray(View.transform.position, View.transform.forward);
            BankInteractable next = null;
            float distance = 3f;
            if (Physics.Raycast(ray, out var hit, Reach, ~0, QueryTriggerInteraction.Ignore)) { next = hit.collider.GetComponentInParent<BankInteractable>(); distance = hit.distance; }
            if (next != focused) { focused = next; elapsed = 0; activated = false; }
            if (focused != null) {
                elapsed += Time.deltaTime;
                bool clicked = pc && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
                if (clicked || (!pc && !activated && elapsed >= Mathf.Max(0.2f, DwellSeconds))) {
                    // Latch before callback: callbacks can teleport or change the UI.
                    activated = true; focused.Activate();
                }
            }
            if (Reticle != null) {
                Reticle.position = ray.GetPoint(Mathf.Max(0.1f, distance - 0.025f));
                float size = Mathf.Max(0.008f, distance * 0.006f) * (focused != null ? 1.4f : 1);
                Reticle.localScale = Vector3.one * size;
            }
        }
        void OnDisable() { focused = null; elapsed = 0; activated = false; }
    }
}



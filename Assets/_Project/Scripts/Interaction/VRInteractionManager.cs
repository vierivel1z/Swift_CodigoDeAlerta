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
        public BankInteractable FocusedTarget => focused;
        public bool IsGazeInput => !((Application.isEditor || !Application.isMobilePlatform) && !SimulateGazeInEditor);
        public float DwellProgress => focused == null ? 0 : Mathf.Clamp01(elapsed / Mathf.Max(.2f,DwellSeconds));
        public bool SelectionLatched => activated;
        // Portal mouse holds and Cardboard gaze share this manager's single dwell clock.
        public bool UsesDwellSelection => IsGazeInput || RequiresHeldDwell(focused);
        static bool IsModeCard(BankInteractable target) => target != null &&
            (target.Action == BankAction.Attack || target.Action == BankAction.DefenseMode);
        static bool RequiresHeldDwell(BankInteractable target) => IsModeCard(target) || target!=null &&
            (target.Action==BankAction.Sit || target.Action==BankAction.Stand || target.GetComponent<Swift.UI.DocumentFocus>()!=null);
        public float LastActivationTime { get; private set; } = float.NegativeInfinity;
        public event System.Action<BankInteractable> Activated;
        // Explicit desktop input-mode changes start a fresh fixation; Cardboard is untouched.
        public void SetEditorGazeSimulation(bool value)
        {
            if (!Application.isEditor && Application.isMobilePlatform) return;
            if (SimulateGazeInEditor == value) return;
            SimulateGazeInEditor = value;
            ResetFixation();
        }
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
                bool blocked = !focused.CanActivate || (focused.Manager != null && focused.Manager.IntroPlaying && focused.Action != BankAction.SkipIntro);
                bool held = pc && Mouse.current != null && Mouse.current.leftButton.isPressed;
                bool dwell = !pc || RequiresHeldDwell(focused);
                if (blocked) {
                    // Never hide a running timer behind the prologue. Keep the completed
                    // fixation latched until look-away, but clear its charge.
                    elapsed = 0;
                } else if (!activated || (pc && !RequiresHeldDwell(focused))) {
                    // A hover/cancelled mouse press owns no dwell time. Gaze needs no press.
                    elapsed = dwell && (!pc || held)
                        ? Mathf.Min(elapsed + Time.deltaTime, Mathf.Max(.2f, DwellSeconds)) : 0;
                    bool clicked = pc && !dwell && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
                    if (clicked || (dwell && elapsed >= Mathf.Max(.2f, DwellSeconds))) {
                        activated = true;
                        LastActivationTime = Time.unscaledTime;
                        var target = focused;
                        Activated?.Invoke(target);
                        target.Activate();
                    }
                }
            }
            if (Reticle != null) {
                Reticle.position = ray.GetPoint(Mathf.Max(0.1f, distance - 0.025f));
                float size = Mathf.Max(0.008f, distance * 0.006f) * (focused != null ? 1.4f : 1);
                Reticle.localScale = Vector3.one * size;
            }
        }
        void ResetFixation() { focused = null; elapsed = 0; activated = false; }
        void OnDisable() { ResetFixation(); }
        void OnApplicationFocus(bool hasFocus) { if (!hasFocus) ResetFixation(); }
        void OnApplicationPause(bool paused) { if (paused) ResetFixation(); }
    }
}



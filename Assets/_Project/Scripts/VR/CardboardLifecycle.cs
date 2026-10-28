using UnityEngine;
namespace Swift.VR
{
    public sealed class CardboardLifecycle : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        void Start() { Screen.sleepTimeout = SleepTimeout.NeverSleep; Application.targetFrameRate = 60; if (!Google.XR.Cardboard.Api.HasDeviceParams()) Google.XR.Cardboard.Api.ScanDeviceParams(); }
        void Update() {
            if (Google.XR.Cardboard.Api.IsGearButtonPressed) Google.XR.Cardboard.Api.ScanDeviceParams();
            if (Google.XR.Cardboard.Api.IsCloseButtonPressed) Application.Quit();
            if (Google.XR.Cardboard.Api.IsTriggerHeldPressed) Google.XR.Cardboard.Api.Recenter();
            if (Google.XR.Cardboard.Api.HasNewDeviceParams()) Google.XR.Cardboard.Api.ReloadDeviceParams();
            Google.XR.Cardboard.Api.UpdateScreenParams();
        }
#endif
    }
}


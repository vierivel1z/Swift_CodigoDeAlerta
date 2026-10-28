using UnityEngine;
using UnityEngine.XR;
namespace Swift.VR
{
    // The official Cardboard sample binds centerEye pose; device pose is a fallback.
    public sealed class HeadPoseAdapter : MonoBehaviour
    {
        void OnEnable() { Application.onBeforeRender += UpdatePose; }
        void OnDisable() { Application.onBeforeRender -= UpdatePose; }
        void Update() { UpdatePose(); }
        void UpdatePose()
        {
            if (Application.isEditor || !Application.isMobilePlatform) return;
            var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (device.TryGetFeatureValue(CommonUsages.centerEyePosition, out Vector3 position) || device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                transform.localPosition = new Vector3(0, 1.6f, 0) + position;
            if (device.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion rotation) || device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                transform.localRotation = rotation;
        }
    }
}

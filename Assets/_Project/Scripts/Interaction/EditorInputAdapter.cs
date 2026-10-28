using UnityEngine;
using UnityEngine.InputSystem;
namespace Swift.Interaction
{
    public sealed class EditorInputAdapter : MonoBehaviour
    {
        public Transform Rig;
        float yaw, pitch;
        void Update()
        {
            if (!Application.isEditor && Application.isMobilePlatform) return;
            var mouse = Mouse.current;
            var keys = Keyboard.current;
            if (mouse != null && mouse.rightButton.isPressed) {
                var delta = mouse.delta.ReadValue(); yaw += delta.x * 0.12f; pitch = Mathf.Clamp(pitch - delta.y * 0.12f, -65, 65);
                transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
            }
            if (Rig != null && keys != null) {
                Vector3 direction = new Vector3((keys.dKey.isPressed ? 1 : 0) - (keys.aKey.isPressed ? 1 : 0), 0, (keys.wKey.isPressed ? 1 : 0) - (keys.sKey.isPressed ? 1 : 0));
                direction = Quaternion.Euler(0, yaw, 0) * Vector3.ClampMagnitude(direction, 1);
                var position = Rig.position + direction * (2 * Time.deltaTime);
                position.x = Mathf.Clamp(position.x, -4, 4); position.z = Mathf.Clamp(position.z, -3.5f, 3.5f); Rig.position = position;
            }
        }
    }
}

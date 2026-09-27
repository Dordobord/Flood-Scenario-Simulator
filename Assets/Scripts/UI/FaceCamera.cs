using UnityEngine;

namespace StormWaits
{
    public class FaceCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            
            Vector3 awayFromCamera = transform.position - mainCamera.transform.position;
            if (awayFromCamera.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(awayFromCamera);
        }
    }
}

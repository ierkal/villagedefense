using UnityEngine;

namespace _Scripts.Utility
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null) return;

            Vector3 camForward = _mainCamera.transform.forward;
            transform.forward = camForward;
        }
    }
}
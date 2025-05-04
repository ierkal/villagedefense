using System.Collections;
using _Scripts.Input;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using UnityEngine;
using Unity.Cinemachine;

namespace _Scripts.Cameras
{
    [LogTag("CameraOrbitController")]
    public class CameraOrbitController : MonoBehaviour, IGameService
    {
        [SerializeField] private Transform focusTarget;
        [SerializeField] private float orbitSpeed = 200f;
        [SerializeField] private float panSpeed = 0.5f;
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private float maxZoom = 5f;
        [SerializeField] private float startZoom = 2.5f;
        private float _zoomScale => orbital.RadialAxis.Value;

        private CinemachineOrbitalFollow orbital;
        private PlayerInputHandler inputHandler;
        private InputActions.GameplayActions input;

        private Vector2 previousMousePos;
        private Vector3 focusPoint;

        private Vector3 _minPanBounds;
        private Vector3 _maxPanBounds;
        private bool _panBoundsSet = false;


        private void Start()
        {
            inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
            input = inputHandler.InputActions.Gameplay;

            orbital = GetComponent<CinemachineOrbitalFollow>();
            focusPoint = focusTarget.position;

            // Set starting zoom
            orbital.RadialAxis.Value = Mathf.Clamp(startZoom, minZoom, maxZoom);
        }

        private void Update()
        {
            Vector2 mousePos = input.MousePosition.ReadValue<Vector2>();
            Vector2 delta = mousePos - previousMousePos;
            previousMousePos = mousePos;

            HandleOrbit(delta);
            HandlePan(delta);
        }

        private void HandleOrbit(Vector2 delta)
        {
            if (input.RightClick.IsPressed() && !input.LeftClick.IsPressed() && !input.MiddleClick.IsPressed())
            {
                orbital.HorizontalAxis.Value += delta.x * orbitSpeed * Time.fixedDeltaTime * (_zoomScale * 0.5f);
            }
        }

        private void HandlePan(Vector2 delta)
        {
            if ((input.LeftClick.IsPressed() || input.MiddleClick.IsPressed()) && !input.RightClick.IsPressed())
            {
                Vector3 right = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
                Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;

                Vector3 move = -(right * delta.x + forward * delta.y) * panSpeed * Time.fixedDeltaTime *
                               (_zoomScale * 0.5f);
                focusPoint += move;
                focusTarget.position = focusPoint;

                focusPoint += move;

                // Clamp to island bounds if set
                if (_panBoundsSet)
                {
                    focusPoint.x = Mathf.Clamp(focusPoint.x, _minPanBounds.x, _maxPanBounds.x);
                    focusPoint.z = Mathf.Clamp(focusPoint.z, _minPanBounds.z, _maxPanBounds.z);
                }

                focusTarget.position = focusPoint;
            }
        }

        public void SetPanBounds(Vector3 min, Vector3 max)
        {
            _minPanBounds = min;
            _maxPanBounds = max;
            _panBoundsSet = true;
        }

        public void ResetCameraToCenter(Vector3 centerPosition)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothReset(centerPosition));
        }

        private IEnumerator SmoothReset(Vector3 center)
        {
            Vector3 start = focusPoint;
            float t = 0f;
            float duration = 0.3f;

            while (t < duration)
            {
                t += Time.deltaTime;
                focusPoint = Vector3.Lerp(start, center, Mathf.SmoothStep(0, 1, t / duration));
                focusTarget.position = focusPoint;
                yield return null;
            }

            focusPoint = center;
            focusTarget.position = center;
        }

        public void UpdateMaxZoomByTileCount(int tileCount)
        {
            maxZoom = Mathf.Clamp(2f + tileCount * 0.1f, minZoom + 1f, 10f);
            orbital.RadialAxis.Value = Mathf.Clamp(orbital.RadialAxis.Value, minZoom, maxZoom);
        }
    }
}
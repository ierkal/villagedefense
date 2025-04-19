using _Scripts.Input;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Cameras
{
    [LogTag("CameraOrbitController")]
    public class CameraOrbitController : MonoBehaviour, IGameService
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _rotationSpeed = 50f;
        [SerializeField] private float _distance = 10f;
        [SerializeField] private Vector3 _offset = new(0, 5, -10);

        private PlayerInputHandler _inputHandler;
        private bool _isDragging = false;
        private float _inputDirection = 0f;

        private void Start()
        {
            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();

            _inputHandler.InputActions.Gameplay.RotateLeft.performed += _ => _inputDirection = -1f;
            _inputHandler.InputActions.Gameplay.RotateRight.performed += _ => _inputDirection = 1f;
            _inputHandler.InputActions.Gameplay.RotateLeft.canceled += _ => _inputDirection = 0f;
            _inputHandler.InputActions.Gameplay.RotateRight.canceled += _ => _inputDirection = 0f;

            _inputHandler.InputActions.Gameplay.MouseHold.performed += _ => _isDragging = true;
            _inputHandler.InputActions.Gameplay.MouseHold.canceled += _ => _isDragging = false;
        }

        private void Update()
        {
            float rotationAmount = 0f;

            if (_inputDirection != 0f)
                rotationAmount = _inputDirection * _rotationSpeed * Time.deltaTime;

            if (_isDragging)
                rotationAmount = Mouse.current.delta.ReadValue().x * Time.deltaTime * _rotationSpeed;

            if (Mathf.Abs(rotationAmount) > 0.01f)
                RotateAroundTarget(rotationAmount);

            // Always look at the island
            transform.LookAt(_target);
        }

        private void RotateAroundTarget(float angle)
        {
            transform.RotateAround(_target.position, Vector3.up, angle);
        }
    }
}

using _Scripts.Main.Services;
using _Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;


namespace _Scripts.Input
{
    public class PlayerInputHandler : MonoBehaviour, IGameService
    {
        private InputActions _inputActions;

        public InputActions InputActions => _inputActions;

        public System.Action OnPause;
        public System.Action OnCancel;
        public System.Action<Vector2> OnClick;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            _inputActions.Gameplay.Pause.performed += HandlePause;
            _inputActions.Gameplay.Cancel.performed += HandleCancel;
            _inputActions.Gameplay.Click.performed += HandleClick;
        }

        private void OnDisable()
        {
            _inputActions.Gameplay.Pause.performed -= HandlePause;
            _inputActions.Gameplay.Cancel.performed -= HandleCancel;
            _inputActions.Gameplay.Click.performed -= HandleClick;

            _inputActions.Disable();
        }

        private void HandlePause(InputAction.CallbackContext ctx)
        {
            OnPause?.Invoke();
        }

        private void HandleCancel(InputAction.CallbackContext ctx)
        {
            OnCancel?.Invoke();
        }

        private void HandleClick(InputAction.CallbackContext ctx)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            ServiceLocator.Instance.Get<ClickRouter>()?.RouteClick(mousePosition);
        }

    }
}
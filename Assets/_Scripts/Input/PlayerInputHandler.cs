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
        public System.Action OnRightClick;
        public System.Action<Vector2> OnClick;
        public System.Action OnResetCamera;

        private void Awake()
        {
            _inputActions = new InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            // Only bind common actions once — Click + RightClick
            // We'll control map switching through InputContextManager
            /*
            _inputActions.Gameplay.Click.performed += _ => HandleClick();
            */
            _inputActions.Gameplay.ResetCamera.performed += _ => OnResetCamera?.Invoke();

            
            _inputActions.Building.Click.performed += _ => HandleClick();
            _inputActions.UnitCommand.Click.performed += _ => HandleClick();

            _inputActions.Building.RightClick.performed += _ => HandleRightClick();
            _inputActions.UnitCommand.RightClick.performed += _ => HandleRightClick();

            /*
            _inputActions.Gameplay.RightClick.performed += _ => HandleRightClick();
            */

            _inputActions.Gameplay.Pause.performed += HandlePause;
        }

        private void OnDisable()
        {
            /*
            _inputActions.Gameplay.Click.performed -= _ => HandleClick();
            */
            _inputActions.Gameplay.ResetCamera.performed -= _ => OnResetCamera?.Invoke();

            
            _inputActions.Building.Click.performed -= _ => HandleClick();
            _inputActions.UnitCommand.Click.performed -= _ => HandleClick();

            /*
            _inputActions.Gameplay.RightClick.performed -= _ => HandleRightClick();
            */
            _inputActions.Building.RightClick.performed -= _ => HandleRightClick();
            _inputActions.UnitCommand.RightClick.performed -= _ => HandleRightClick();

            _inputActions.Gameplay.Pause.performed -= HandlePause;

            _inputActions.Disable();
        }

        private void HandlePause(InputAction.CallbackContext ctx)
        {
            OnPause?.Invoke();
        }

        private void HandleClick()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"[PlayerInputHandler] Click at {mousePosition}");
            OnClick?.Invoke(mousePosition);
        }

        private void HandleRightClick()
        {
            Debug.Log("[PlayerInputHandler] Right Click");
            OnRightClick?.Invoke();
        }


        /// <summary>
        /// Ensures only one input map is active to prevent multiple click logs.
        /// </summary>
        private bool IsOnlyOneMapEnabled(out string activeMap)
        {
            activeMap = null;

            bool gameplay = _inputActions.Gameplay.enabled;
            bool building = _inputActions.Building.enabled;
            bool unit = _inputActions.UnitCommand.enabled;

            int count = 0;
            if (gameplay) { activeMap = "Gameplay"; count++; }
            if (building) { activeMap = "Building"; count++; }
            if (unit) { activeMap = "UnitCommand"; count++; }

            return count == 1;
        }
    }
}

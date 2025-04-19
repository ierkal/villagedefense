using _Scripts.Event;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Input
{
    [LogTag("InputContextManager")]
    public class InputContextManager : MonoBehaviour, IGameService
    {
        private PlayerInputHandler _inputHandler;

        private void OnEnable()
        {
            EventBroker.Instance.AddEventListener<ServicesInitializedEvent>(OnServicesInitialized);
        }

        private void OnDisable()
        {
            EventBroker.Instance.RemoveEventListener<ServicesInitializedEvent>(OnServicesInitialized);
        }

        private void OnServicesInitialized(ServicesInitializedEvent e)
        {
            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
        }

        public void EnableGameplayControls()
        {
            _inputHandler.InputActions.UI.Disable();
            _inputHandler.InputActions.Gameplay.Enable();
            Log.Info(this, "Gameplay controls enabled", "green");
        }

        public void EnableUIControls()
        {
            _inputHandler.InputActions.Gameplay.Disable();
            _inputHandler.InputActions.UI.Enable();
            Log.Info(this, "UI controls enabled", "magenta");
        }

        public void DisableAllControls()
        {
            _inputHandler.InputActions.Gameplay.Disable();
            _inputHandler.InputActions.UI.Disable();
            Log.Info(this, "All controls disabled", "yellow");
        }
    }
}
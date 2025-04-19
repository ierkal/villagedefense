using System;
using _Scripts.Event;
using _Scripts.Input;
using _Scripts.Main.Services;
using _Scripts.StateMachine.Interface;
using UnityEngine;

namespace _Scripts.Main
{
    public class UIManager : MonoBehaviour, IGameService
    {
        [SerializeField] private Canvas _pauseMenuCanvas;
        private PlayerInputHandler _inputHandler;

        private GameStateMachine _uiStateMachine;

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


            _inputHandler.OnPause += HandlePauseInput;
            _uiStateMachine = new GameStateMachine();
        }


        public void Initialize()
        {
            _uiStateMachine = new GameStateMachine();
            _inputHandler.OnPause += HandlePauseInput;
        }

        private void HandlePauseInput()
        {
            if (_uiStateMachine.CurrentState is PauseMenuState)
            {
                Resume();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        private void OpenPauseMenu()
        {
            _uiStateMachine.PushState(new PauseMenuState(_pauseMenuCanvas));
        }

        public void Resume()
        {
            if (_uiStateMachine.CurrentState is PauseMenuState)
            {
                _uiStateMachine.PopState();
            }
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
                _inputHandler.OnPause -= HandlePauseInput;
        }


        public void TogglePauseMenu()
        {
            if (_uiStateMachine.CurrentState is PauseMenuState)
            {
                _uiStateMachine.PopState(); // Close pause
            }
            else
            {
                var pauseState = new PauseMenuState(_pauseMenuCanvas);
                _uiStateMachine.PushState(pauseState);
            }
        }


        public bool IsOverlayOpen => _uiStateMachine?.CurrentState != null;
    }
}
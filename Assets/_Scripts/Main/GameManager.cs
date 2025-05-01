using _Scripts.AI;
using _Scripts.Event;
using _Scripts.Input;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.StateMachine.Interface;
using _Scripts.StateMachine.States;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Main
{
    [LogTag("GameManager")]
    public class GameManager : MonoBehaviour, IGameService
    {
        private GameStateMachine _gameplayStateMachine;

        public IGameState CurrentGameplayState => _gameplayStateMachine.CurrentState;

        private void Awake()
        {
            _gameplayStateMachine = new GameStateMachine();
            Log.Info(this, "Initialized GameStateMachine", "orange");
            Log.Info(this, "Game Initialized", "green");
        }

        private void Start()
        {
            _gameplayStateMachine.SetState(new LoadingState());
        }

        private void Update()
        {
            _gameplayStateMachine.Tick();
        }

        // Public control methods
        public void SwitchToIsland()
        {
            Log.Info(this, "Switching to IslandState", "yellow");
            _gameplayStateMachine.SetState(new IslandState());
        }

        public void SwitchToBuild()
        {
            var buildManager = ServiceLocator.Instance.Get<BuildManager>();
            Log.Info(this, "Switching to BuildingState", "cyan");
            _gameplayStateMachine.SetState(new BuildingState(buildManager));
        }

        public void SwitchToCombat()
        {
            var combatManager = ServiceLocator.Instance.Get<CombatManager>();
            Log.Info(this, "Switching to CombatState", "red");
            _gameplayStateMachine.SetState(new CombatState(combatManager));
        }

        public void SwitchToGameOver()
        {
            Log.Warning(this, "Switching to GameOverState", "magenta");
            _gameplayStateMachine.SetState(new GameOverState());
        }
        public void SwitchToUnitCommand(Vector2 clickPos)
        {
            var unitCommandHandler = ServiceLocator.Instance.Get<UnitCommandHandler>();

            Log.Warning(this,"Switching to UnitCommandState", "cyan");

            unitCommandHandler.EnterUnitCommandMode();
            unitCommandHandler.HandleUnitClick(clickPos); // 🔥 Forward the click
        }



    }
}
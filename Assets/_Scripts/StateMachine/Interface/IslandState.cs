using _Scripts.AI.Player;
using _Scripts.Input;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using _Scripts.Waves;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("IslandState")]
    public class IslandState : BaseGameState
    {
        private TileInfoClickHandler _tileClickHandler;
        private bool _spawnedStartingUnit = false;
        private bool _waitingForServices = true;

        public override void Enter()
        {
            base.Enter();

            Debug.Log("[IslandState] Enter called.");

            ServiceLocator.Instance.Get<InputContextManager>()?.EnableUnitControls();

            _tileClickHandler = new TileInfoClickHandler();
            ServiceLocator.Instance.Get<PlayerInputHandler>().OnClick += _tileClickHandler.HandleClick;

            _waitingForServices = true;
        }


        public override void Exit()
        {
            var input = ServiceLocator.Instance.Get<PlayerInputHandler>();
            input.OnClick -= _tileClickHandler.HandleClick;

            base.Exit();
        }


        public override void Tick()
        {
        }
    }
}
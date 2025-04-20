using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("IslandState")]
    
    public class IslandState : BaseGameState
    {    
        private TileInfoClickHandler _tileClickHandler;

        public override void Enter()
        {
            base.Enter();
            _tileClickHandler ??= new TileInfoClickHandler();
            ServiceLocator.Instance.Get<ClickRouter>()?.SetClickHandler(_tileClickHandler);
            // Could add island-specific logic here
        }

        public override void Exit()
        {
            ServiceLocator.Instance.Get<ClickRouter>()?.ClearClickHandler();

            base.Exit();
            
        }

        public override void Tick() { }
    }
}

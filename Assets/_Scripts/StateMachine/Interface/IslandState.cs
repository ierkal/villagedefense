using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("IslandState")]
    public class IslandState : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            // Could add island-specific logic here
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Tick() { }
    }
}

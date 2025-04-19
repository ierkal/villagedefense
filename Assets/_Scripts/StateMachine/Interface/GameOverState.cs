using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("GameOverState")]
    public class GameOverState : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            // Show Game Over UI, stop game time, etc.
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Tick() { }
    }

}

using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.StateMachine.Interface
{
    [LogTag("MainMenuState")]
    public class MainMenuState : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            // Show main menu UI
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Tick() { }
    }
}

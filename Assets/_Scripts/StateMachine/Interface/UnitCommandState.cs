using _Scripts.Input;
using _Scripts.Main.Services;

namespace _Scripts.StateMachine.Interface
{
    public class UnitCommandState : BaseGameState
    {

        public override void Enter()
        {
            base.Enter();

            var inputContext = ServiceLocator.Instance.Get<InputContextManager>();
            inputContext.EnableUnitControls();

        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Tick()
        {
            // Optional: could add hover highlights, cancel logic, etc.
        }
    }
}
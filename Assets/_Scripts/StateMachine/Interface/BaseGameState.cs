using _Scripts.Utility;

namespace _Scripts.StateMachine.Interface
{
    public class BaseGameState : IGameState
    {
        protected GameStateMachine StateMachine { get; private set; }

        public void SetStateMachine(GameStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            Log.Info(this, $"Entered {GetType().Name}", "cyan");
        }

        public virtual void Tick() { }

        public virtual void Exit()
        {
            Log.Info(this, $"Exited {GetType().Name}", "cyan");
        }
    }
}
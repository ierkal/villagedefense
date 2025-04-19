namespace _Scripts.AI.Core
{
    public class UnitStateMachine
    {
        public IUnitState CurrentState { get; private set; }

        public void SetState(IUnitState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }

        public void Tick()
        {
            CurrentState?.Tick();
        }
    }
}
namespace _Scripts.StateMachine.Interface
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Tick(); // Optional: called every frame
    }
}

namespace _Scripts.AI
{
    public interface IUnitState
    {
        void Enter();
        void Exit();
        void Tick();
    }
}
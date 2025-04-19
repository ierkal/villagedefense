using _Scripts.AI.Core;

namespace _Scripts.AI.Player.FSM
{
    public class PlayerIdleState : IUnitState
    {
        private readonly PlayerTroopAI _troop;

        public PlayerIdleState(PlayerTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _troop.SetMovementSpeed(0f);
        }

        public void Exit() { }

        public void Tick()
        {
            // Wait for player command
        }
    }
}
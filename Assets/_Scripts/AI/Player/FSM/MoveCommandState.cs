using _Scripts.AI.Core;

namespace _Scripts.AI.Player.FSM
{
    public class MoveCommandState : IUnitState
    {
        private readonly PlayerTroopAI _troop;

        public MoveCommandState(PlayerTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _troop.SetMovementSpeed(1f); // or current velocity magnitude
        }

        public void Exit()
        {
            _troop.SetMovementSpeed(0f);
        }

        public void Tick()
        {
            _troop.MoveToTarget();

            if (_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new DefendTileState(_troop));
            }

            if (!_troop.HasTarget())
            {
                _troop.StateMachine.SetState(new PlayerIdleState(_troop));
            }
        }
    }
}
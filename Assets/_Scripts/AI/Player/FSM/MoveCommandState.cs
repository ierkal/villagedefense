using _Scripts.AI.Core;

namespace _Scripts.AI.Player.FSM
{
    public class MoveCommandState : IUnitState
    {
        private readonly PlayerTroopAI _troop;
        private readonly MovementOrder _movementOrder;

        public MoveCommandState(PlayerTroopAI troop)
        {
            _troop = troop;
            _movementOrder = troop.GetComponent<MovementOrder>();
        }

        public void Enter()
        {
            _troop.SetMovementSpeedSmooth(1f);
        }

        public void Exit()
        {
            _troop.SetMovementSpeedSmooth(0f);
        }

        public void Tick()
        {
            _troop.SetMovementSpeed(_movementOrder.IsMoving ? 1f : 0f);

            if (!_movementOrder.IsMoving)
            {
                _troop.StateMachine.SetState(new PlayerIdleState(_troop));
                return;
            }

            if (_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new DefendTileState(_troop));
                return;
            }

            if (!_troop.HasTarget())
            {
                _troop.StateMachine.SetState(new PlayerIdleState(_troop));
                return;
            }
        }

    }
}
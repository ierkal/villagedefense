using _Scripts.AI.Core;
using UnityEngine;

namespace _Scripts.AI.Player.FSM
{
    public class DefendTileState : IUnitState
    {
        private readonly PlayerTroopAI _troop;
        private float _attackCooldown = 1.2f;
        private float _lastAttackTime;

        public DefendTileState(PlayerTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _lastAttackTime = Time.time;
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            if (_troop.TargetIsDead())
            {
                _troop.StateMachine.SetState(new PlayerIdleState(_troop));
                return;
            }

            if (!_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new MoveCommandState(_troop));
                return;
            }

            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                /*_troop.PerformAttack();
                _troop.SetAttacking();*/
                _lastAttackTime = Time.time;
            }
        }
    }
}
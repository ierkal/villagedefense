using _Scripts.AI.Core;
using UnityEngine;

namespace _Scripts.AI.Enemies.FSM
{
    public class EnemyAttackState : IUnitState
    {
        private readonly EnemyTroopAI _troop;
        private float _attackCooldown = 1.5f;
        private float _lastAttackTime;

        public EnemyAttackState(EnemyTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _lastAttackTime = Time.time;
            _troop.SetAttacking();
        }

        public void Exit()
        {
            _troop.SetAttacking();
        }

        public void Tick()
        {
            if (_troop.TargetIsDead())
            {
                _troop.StateMachine.SetState(new InvadeTileState(_troop));
                return;
            }

            if (!_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new InvadeTileState(_troop));
                return;
            }

            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                _troop.PerformAttack();
                _lastAttackTime = Time.time;
                _troop.SetAttacking();
            }
        }
    }
}
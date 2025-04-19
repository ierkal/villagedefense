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
            _troop.SetAttacking(true);
            _lastAttackTime = Time.time;
        }

        public void Exit()
        {
            _troop.SetAttacking(false);
        }

        public void Tick()
        {
            if (_troop.TargetIsDead())
            {
                _troop.StateMachine.SetState(new InvadeTileState(_troop)); // Return to roam
                return;
            }

            if (!_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new InvadeTileState(_troop));
                return;
            }

            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                _troop.PerformAttack(); // Implement this on your AI
                _lastAttackTime = Time.time;
                _troop.SetAttacking(true);
            }
        }
    }
}
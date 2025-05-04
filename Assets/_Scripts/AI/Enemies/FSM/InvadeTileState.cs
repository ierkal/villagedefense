using _Scripts.AI.Core;
using UnityEngine;

namespace _Scripts.AI.Enemies.FSM
{
    public class InvadeTileState : IUnitState
    {
        private readonly EnemyTroopAI _troop;
        private bool _isCheering = false;
        private float _cheerStartTime;

        private bool _movementStarted = false;

        public InvadeTileState(EnemyTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _isCheering = false;
            _movementStarted = false;
            _troop.SetMovementSpeed(1f);

            if (_troop.TargetTileCenter != null && !_troop.HasTarget())
            {
                _troop.StartPathTo(_troop.TargetTileCenter); // Uses MovementOrder
                _troop.OnPathComplete += HandlePathComplete;
            }
        }

        public void Exit()
        {
            _troop.SetMovementSpeed(0f);
            _troop.OnPathComplete -= HandlePathComplete;
        }

        public void Tick()
        {
            if (_troop.HasTarget())
            {
                _troop.MoveToTarget();

                if (_troop.IsInAttackRange())
                {
                    _troop.StateMachine.SetState(new EnemyAttackState(_troop));
                }

                return;
            }

            if (_isCheering)
            {
                if (Time.time >= _cheerStartTime + _troop.CheerDuration)
                {
                    // Cheering done, stay idle or go back to roaming
                }
            }
        }

        private void HandlePathComplete()
        {
            if (_isCheering) return;

            _troop.PlayCheer();
            _cheerStartTime = Time.time;
            _isCheering = true;
            _troop.SetMovementSpeed(0f);
        }
    }
}
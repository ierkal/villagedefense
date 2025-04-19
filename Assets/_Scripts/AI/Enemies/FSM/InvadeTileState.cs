using _Scripts.AI.Core;
using UnityEngine;

namespace _Scripts.AI.Enemies.FSM
{
    public class InvadeTileState : IUnitState
    {
        private readonly EnemyTroopAI _troop;

        public InvadeTileState(EnemyTroopAI troop)
        {
            _troop = troop;
        }

        public void Enter()
        {
            _troop.SetMovementSpeed(1f);
        }

        public void Exit() { }

        public void Tick()
        {
            if (_troop.Target == null)
            {
                _troop.SetMovementSpeed(0f);
                return;
            }

            _troop.MoveToTarget();

            if (_troop.IsInAttackRange())
            {
                _troop.StateMachine.SetState(new EnemyAttackState(_troop));
            }
        }
    }
}
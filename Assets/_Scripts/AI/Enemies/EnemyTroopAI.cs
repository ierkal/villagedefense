using _Scripts.AI.Enemies.FSM;
using _Scripts.AI.Core;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Enemies
{
    [LogTag("EnemyTroopAI")]
    [RequireComponent(typeof(TroopAnimationController))]
    [RequireComponent(typeof(TroopStateMachine))]
    public class EnemyTroopAI : BaseTroopAI
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRange = 1.5f;

        public bool IsInitialized { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            StateMachine.SetState(new InvadeTileState(this));
            // Wait for target before launching FSM
        }

        /// <summary>
        /// Called by the ship or spawner when troop is placed.
        /// </summary>
        public void Initialize(Transform target)
        {
            Target = target;
            IsInitialized = true;

            Log.Info(this, "Troop initialized with target.");
        }

        public override bool HasTarget()
        {
            return Target != null;
        }

        public override bool TargetIsDead()
        {
            // Placeholder: maybe expand with health system later
            return Target == null;
        }

        public override bool IsInAttackRange()
        {
            if (Target == null) return false;
            return Vector3.Distance(transform.position, Target.position) <= _attackRange;
        }

        public override void MoveToTarget()
        {
            if (Target == null) return;

            Vector3 moveDir = (Target.position - transform.position).normalized;
            transform.position += moveDir * _moveSpeed * Time.deltaTime;
        }

        public override void PerformAttack()
        {
            Log.Info(this, "Enemy performed attack.");
            // Placeholder for future damage logic / events
        }
    }
}
using UnityEngine;

namespace _Scripts.AI.Core
{
    [RequireComponent(typeof(TroopAnimationController))]
    public abstract class BaseTroopAI : MonoBehaviour
    {
        public UnitStateMachine StateMachine { get; protected set; }
        protected TroopAnimationController _anim;

        [Header("Target Info")]
        public Transform Target;

        protected virtual void Awake()
        {
            StateMachine = new UnitStateMachine();
            _anim = GetComponent<TroopAnimationController>();
        }

        protected virtual void Update()
        {
            StateMachine.Tick();
        }

        // --- Animation Hooks ---

        /// <summary>Set movement speed for run/idle blend.</summary>
        public void SetMovementSpeed(float speed)
        {
            _anim.SetMovementSpeed(speed);
        }

        /// <summary>Start or stop attack loop.</summary>
        public void SetAttacking(bool isAttacking)
        {
            _anim.PlayAttack(isAttacking);
        }

        /// <summary>Play death animation.</summary>
        public void PlayDeath()
        {
            _anim.Die();
        }

        // --- Abstract logic to be defined by derived AI ---
        public abstract bool HasTarget();
        public abstract bool TargetIsDead();
        public abstract bool IsInAttackRange();
        public abstract void MoveToTarget();
        public abstract void PerformAttack();
    }
}
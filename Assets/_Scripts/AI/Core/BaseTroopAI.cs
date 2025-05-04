using _Scripts.Island;
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
        public HexTile CurrentTile { get; set; }

        protected virtual void Awake()
        {
            StateMachine = new UnitStateMachine();
            _anim = GetComponent<TroopAnimationController>();
        }

        protected virtual void Update()
        {
            StateMachine.Tick();
        }

        #region Animation Hooks

        /// <summary>
        /// Immediately sets movement speed for walk/run blend tree.
        /// Use this if precision is more important than smoothness.
        /// </summary>
        public void SetMovementSpeed(float speed)
        {
            _anim.SetMovementSpeed(speed);
        }

        /// <summary>
        /// Smoothly sets movement speed using animator damping for better transitions.
        /// </summary>
        public void SetMovementSpeedSmooth(float speed, float damping = 0.2f)
        {
            _anim.SetMovementSpeed(speed, damping);
        }

        /// <summary>
        /// Triggers attack animation. One-shot trigger, does not loop.
        /// </summary>
        public void SetAttacking()
        {
            _anim.PlayAttack();
        }

        /// <summary>
        /// Triggers death animation and sets IsDead flag.
        /// </summary>
        public void PlayDeath()
        {
            _anim.Die();
        }

        #endregion

        #region Abstract Combat Logic

        public abstract bool HasTarget();
        public abstract bool TargetIsDead();
        public abstract bool IsInAttackRange();
        public abstract void MoveToTarget();
        public abstract void PerformAttack();

        #endregion
    }
}
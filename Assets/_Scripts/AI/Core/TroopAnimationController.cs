using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;
using System.Collections.Generic;

namespace _Scripts.AI.Core
{
    [LogTag("TroopAnimationController")]
    public class TroopAnimationController : MonoBehaviour
    {
        [Header("Animator Setup")]
        [Tooltip("If empty, will auto-search all child animators at Start.")]
        [SerializeField] private List<Animator> _animators = new();

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackTriggerHash = Animator.StringToHash("AttackTrigger");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private static readonly int DieTriggerHash = Animator.StringToHash("DieTrigger");
        private static readonly int CheerTriggerHash = Animator.StringToHash("CheerTrigger");

        private void Start()
        {
            if (_animators.Count == 0)
            {
                _animators.AddRange(GetComponentsInChildren<Animator>());
                Log.Info(this, $"Found {_animators.Count} animators under troop.");
            }

            if (_animators.Count == 0)
                Log.Warning(this, "No animators found in troop visuals!");
        }

        #region Movement

        /// <summary>
        /// Instantly sets movement speed parameter.
        /// Use for immediate transitions.
        /// </summary>
        public void SetMovementSpeed(float speed)
        {
            foreach (var anim in _animators)
                anim?.SetFloat(SpeedHash, speed);
        }

        /// <summary>
        /// Smoothly sets movement speed with damping.
        /// Helps prevent abrupt transitions.
        /// </summary>
        public void SetMovementSpeed(float speed, float damping)
        {
            foreach (var anim in _animators)
                anim?.SetFloat(SpeedHash, speed, damping, Time.deltaTime);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Triggers one-shot attack animation.
        /// </summary>
        public void PlayAttack()
        {
            Log.Info(this, "Attack triggered.");
            foreach (var anim in _animators)
                anim?.SetTrigger(AttackTriggerHash);
        }

        /// <summary>
        /// Sets death parameters: boolean + trigger.
        /// </summary>
        public void Die()
        {
            foreach (var anim in _animators)
            {
                anim?.SetBool(IsDeadHash, true);
                anim?.SetTrigger(DieTriggerHash);
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Resets animation triggers (optional utility).
        /// </summary>
        public void ResetAllTriggers()
        {
            foreach (var anim in _animators)
            {
                anim?.ResetTrigger(AttackTriggerHash);
                anim?.ResetTrigger(DieTriggerHash);
            }
        }

        #endregion
        
        /// <summary>
        /// Triggers cheering animation (e.g., when tile is captured).
        /// </summary>
        public void PlayCheer()
        {
            foreach (var anim in _animators)
                anim?.SetTrigger(CheerTriggerHash);
        }
    }
}

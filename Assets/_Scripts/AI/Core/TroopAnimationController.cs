using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Core
{
    [LogTag("TroopAnimationController")]
    public class TroopAnimationController : MonoBehaviour
    {
        [Header("Animator Setup")]
        [SerializeField] private Animator _animator;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private static readonly int DieTriggerHash = Animator.StringToHash("DieTrigger");

        private void Awake()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_animator == null)
                Log.Error(this, "Animator component not assigned or found!");
        }

        public void SetMovementSpeed(float speed)
        {
            _animator?.SetFloat(SpeedHash, speed);
        }

        public void PlayAttack(bool value)
        {
            _animator?.SetBool(IsAttackingHash, value);
        }

        public void Die()
        {
            _animator?.SetBool(IsDeadHash, true);
            _animator?.SetTrigger(DieTriggerHash);
        }
    }
}
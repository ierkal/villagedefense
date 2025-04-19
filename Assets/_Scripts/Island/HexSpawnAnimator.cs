using System.Collections;
using UnityEngine;
using _Scripts.OdinAttributes;

namespace _Scripts.Island
{
    [LogTag("HexSpawnAnimator")]
    public class HexSpawnAnimator : MonoBehaviour
    {
        [Header("Animation Settings")] [SerializeField]
        private float _hexScaleDuration = 0.4f;

        [SerializeField] private float _settleDuration = 0.2f;
        [SerializeField] private float _childScaleDuration = 0.25f;
        [SerializeField] private float _childStartDelay = 0.1f;
        [SerializeField] private float _overshootScale = 1.15f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Effects")] [SerializeField] private GameObject _spawnParticlePrefab;

        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
            transform.localScale = Vector3.zero;

        }

        private void Start()
        {
            if (_spawnParticlePrefab != null)
                Instantiate(_spawnParticlePrefab, transform.position, Quaternion.identity, transform);

            StartCoroutine(AnimateHexGrow());
        }

        private IEnumerator AnimateHexGrow()
        {
            // Grow to overshoot scale
            Vector3 overshoot = _originalScale * _overshootScale;

            float t = 0f;
            while (t < _hexScaleDuration)
            {
                t += Time.deltaTime;
                float normalized = t / _hexScaleDuration;
                float scaled = _scaleCurve.Evaluate(normalized);
                transform.localScale = Vector3.LerpUnclamped(Vector3.zero, overshoot, scaled);
                yield return null;
            }

            // Settle to original scale
            t = 0f;
            while (t < _settleDuration)
            {
                t += Time.deltaTime;
                float normalized = t / _settleDuration;
                float scaled = _scaleCurve.Evaluate(normalized);
                transform.localScale = Vector3.LerpUnclamped(overshoot, _originalScale, scaled);
                yield return null;
            }

            transform.localScale = _originalScale;

            // Animate children
            /*foreach (Transform child in transform)
            {
                StartCoroutine(ScaleChild(child));
                yield return new WaitForSeconds(_childStartDelay);
            }*/
        }

        private IEnumerator ScaleChild(Transform child)
        {
            Vector3 original = child.localScale;
            Vector3 overshoot = original * _overshootScale;
            child.localScale = Vector3.zero;

            float t = 0f;
            while (t < _hexScaleDuration)
            {
                t += Time.deltaTime;
                float normalized = t / _hexScaleDuration;
                float scaled = _scaleCurve.Evaluate(normalized);
                child.localScale = Vector3.LerpUnclamped(Vector3.zero, overshoot, scaled);
                yield return null;
            }

            t = 0f;
            while (t < _settleDuration)
            {
                t += Time.deltaTime;
                float normalized = t / _settleDuration;
                float scaled = _scaleCurve.Evaluate(normalized);
                child.localScale = Vector3.LerpUnclamped(overshoot, original, scaled);
                yield return null;
            }

            child.localScale = original;
        }
    }
}
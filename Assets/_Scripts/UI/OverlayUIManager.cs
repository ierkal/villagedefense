using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using _Scripts.Main.Services;

namespace _Scripts.UI
{
    public class OverlayUIManager : MonoBehaviour, IGameService
    {
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private float _fadeDuration = 0.5f;

        private void Awake()
        {
            if (_loadingCanvasGroup != null)
            {
                _loadingCanvasGroup.alpha = 0f;
                _loadingCanvasGroup.blocksRaycasts = false;
            }
        }

        public void PlayLoadingAnimation(System.Action onFadeComplete)
        {
            StartCoroutine(FadeInThen(outCallback: onFadeComplete));
        }

        public void HideLoadingAnimation()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeInThen(System.Action outCallback)
        {
            _loadingCanvasGroup.blocksRaycasts = true;

            yield return FadeCanvas(0f, 1f);
            outCallback?.Invoke();
        }

        private IEnumerator FadeOut()
        {
            yield return FadeCanvas(1f, 0f);
            _loadingCanvasGroup.blocksRaycasts = false;
        }

        private IEnumerator FadeCanvas(float from, float to)
        {
            float t = 0f;
            while (t < _fadeDuration)
            {
                t += Time.deltaTime;
                _loadingCanvasGroup.alpha = Mathf.Lerp(from, to, t / _fadeDuration);
                yield return null;
            }
            _loadingCanvasGroup.alpha = to;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Event;
using _Scripts.Main;
using _Scripts.Main.Services;
using _Scripts.ScriptableObjects.EnvironmentSO;
using _Scripts.ScriptableObjects.SceneSO;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.SceneManagement
{
    public class SceneLoader : MonoSingleton<SceneLoader>, IGameService
    {
        [Header("Scene References")]
        [SerializeField] private SceneData _mainMenuScene;
        [SerializeField] private SceneData _gameScene;
        [SerializeField] private SceneData _overlayScene;

        private bool _overlayLoaded = false;
        private bool _masterSceneMarked = false;
        private string _masterSceneName => gameObject.scene.name; // ← current scene
        private OverlayUIManager _overlayUI;
        private void OnEnable()
        {
            EventBroker.Instance.AddEventListener<ServicesInitializedEvent>(OnServicesInitialized);
        }
        private void OnDisable()
        {
            EventBroker.Instance.RemoveEventListener<ServicesInitializedEvent>(OnServicesInitialized);
        }
        private void OnServicesInitialized(ServicesInitializedEvent e)
        {
            LoadScene(_gameScene);
        }
        private void Start()
        {
            _overlayUI = FindObjectOfType<OverlayUIManager>();

        }
        public void LoadScene(SceneData sceneData)
        {
            if (_overlayUI != null)
            {
                _overlayUI.PlayLoadingAnimation(() =>
                {
                    StartCoroutine(LoadSceneRoutine(sceneData));
                });
            }
            else
            {
                StartCoroutine(LoadSceneRoutine(sceneData));
            }
        }
        private IEnumerator LoadSceneRoutine(SceneData sceneData)
        {
            // 1. Load overlay scene once
            if (!_overlayLoaded)
            {
                yield return SceneManager.LoadSceneAsync(_overlayScene.SceneName, LoadSceneMode.Additive);
                _overlayLoaded = true;
            }

            // 2. Apply environment settings
            if (sceneData.EnvironmentProfile != null)
            {
                ApplyEnvironmentProfile(sceneData.EnvironmentProfile);
            }

            // 3. Refetch overlay UI
            _overlayUI = FindObjectOfType<OverlayUIManager>();

            // 4. Start loading target scene
            bool loadStarted = false;
            _overlayUI?.PlayLoadingAnimation(() =>
            {
                StartCoroutine(LoadTargetSceneRoutine(sceneData));
                loadStarted = true;
            });

            yield return new WaitUntil(() => loadStarted);
        }

        private IEnumerator LoadTargetSceneRoutine(SceneData sceneData)
        {
            var keepScenes = new HashSet<string>
            {
                _overlayScene.SceneName,
                gameObject.scene.name 
            };

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!keepScenes.Contains(scene.name))
                {
                    yield return SceneManager.UnloadSceneAsync(scene);
                }
            }
            yield return SceneManager.LoadSceneAsync(sceneData.SceneName, ConvertLoadMode(sceneData.LoadMode));

            yield return null;
            _overlayUI?.HideLoadingAnimation();
        }
        private LoadSceneMode ConvertLoadMode(SceneLoadMode mode)
        {
            return mode == SceneLoadMode.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }
        private void ApplyEnvironmentProfile(EnvironmentProfile profile)
        {
            RenderSettings.fog = profile.enableFog;
            RenderSettings.fogMode = profile.fogMode;
            RenderSettings.fogColor = profile.fogColor;
            RenderSettings.fogDensity = profile.fogDensity;
        }

    }
}
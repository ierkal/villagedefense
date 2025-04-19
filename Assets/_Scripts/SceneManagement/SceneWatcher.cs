using _Scripts.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.SceneManagement
{
    public class SceneWatcher : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            new SceneLoadedEvent(scene).Raise();
        }
    }
}
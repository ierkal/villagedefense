using System.Linq;
using _Scripts.Event;
using UnityEngine;

namespace _Scripts.Main.Services
{
    public class DynamicServiceRegister : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBroker.Instance.AddEventListener<SceneLoadedEvent>(OnSceneLoaded);
        }

        private void OnDisable()
        {
            EventBroker.Instance.RemoveEventListener<SceneLoadedEvent>(OnSceneLoaded);
        }

        private void OnSceneLoaded(SceneLoadedEvent e)
        {
            var services = GameObject
                .FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IGameService>();

            foreach (var service in services)
            {
                var type = service.GetType();
                if (!ServiceLocator.Instance.Has(type))
                {
                    ServiceLocator.Instance.Register(type, service);
                }
               
            }
        }
    }
}
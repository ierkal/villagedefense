using System.Collections;
using System.Linq;
using _Scripts.Event;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Main.Services
{
    [LogTag("GameInstaller")]
    public class GameInstaller : MonoBehaviour
    {
        private void Awake()
        {
            RegisterAllGameServices();
            StartCoroutine(RaiseInitializedEventNextFrame());
        }

        private void RegisterAllGameServices()
        {
            var services = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IGameService>();

            foreach (var service in services)
            {
                RegisterByAllInterfaces(service);
            }

            Log.Info(this, "All Registered.", "red");
        }

        private void RegisterByAllInterfaces(IGameService service)
        {
            var allInterfaces = service.GetType()
                .GetInterfaces()
                .Where(i => typeof(IGameService).IsAssignableFrom(i));

            foreach (var type in allInterfaces)
            {
                ServiceLocator.Instance.Register(type, service);
                Log.Info(this, $"Registered as interface: {service.GetType().Name} -> {type.Name}", "green");
            }

            ServiceLocator.Instance.Register(service.GetType(), service);
        }

        private IEnumerator RaiseInitializedEventNextFrame()
        {
            yield return null;
            new ServicesInitializedEvent().Raise();
        }
    }
}
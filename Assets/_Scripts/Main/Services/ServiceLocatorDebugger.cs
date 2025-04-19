using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
namespace _Scripts.Main.Services
{
    [ExecuteAlways]
    public class ServiceLocatorDebugger : MonoBehaviour
    {
        [Title("Registered Services (Runtime Only)")]
        [ShowInInspector, ReadOnly, HideIf("@!UnityEngine.Application.isPlaying")]
        [ListDrawerSettings(Expanded = true, ShowPaging = false)]
        private Dictionary<string, string> _registeredServices => GetServiceDictionary();

        private Dictionary<string, string> GetServiceDictionary()
        {
            if (!Application.isPlaying || ServiceLocator.Instance == null)
                return new Dictionary<string, string> { { "⛔", "Game not running or ServiceLocator missing." } };

            var field = typeof(ServiceLocator).GetField("_services", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null) return new Dictionary<string, string> { { "❌", "Could not reflect _services dictionary." } };

            var services = field.GetValue(ServiceLocator.Instance) as IDictionary<Type, IGameService>;
            if (services == null) return new Dictionary<string, string> { { "❌", "No services found." } };

            return services.ToDictionary(
                s => s.Key.Name,
                s => s.Value?.GetType().Name ?? "null"
            );
        }
    }
}
#endif
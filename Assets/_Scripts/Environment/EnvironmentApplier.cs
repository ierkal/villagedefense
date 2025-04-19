using _Scripts.ScriptableObjects.EnvironmentSO;
using UnityEngine;

namespace _Scripts.Environment
{
    public class EnvironmentApplier : MonoBehaviour
    {
        [SerializeField] private EnvironmentProfile _profile;

        private void OnEnable()
        {
            if (_profile == null)
            {
                Debug.LogWarning("[EnvironmentApplier] No EnvironmentProfile assigned.");
                return;
            }

            ApplyProfile(_profile);
        }

        private void ApplyProfile(EnvironmentProfile profile)
        {
            RenderSettings.fog = profile.enableFog;
            RenderSettings.fogMode = profile.fogMode;
            RenderSettings.fogColor = profile.fogColor;
            RenderSettings.fogDensity = profile.fogDensity;
        }
    }
}
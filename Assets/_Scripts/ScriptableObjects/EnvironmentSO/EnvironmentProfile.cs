using UnityEngine;

namespace _Scripts.ScriptableObjects.EnvironmentSO
{
    [CreateAssetMenu(fileName = "EnvironmentProfile", menuName = "Environment/Environment Profile")]
    public class EnvironmentProfile : ScriptableObject
    {
        public bool enableFog = true;
        public FogMode fogMode = FogMode.ExponentialSquared;
        public Color fogColor = new Color(0.7f, 0.8f, 0.9f, 1f);
        [Range(0f, 1f)]
        public float fogDensity = 0.015f;
    }
}
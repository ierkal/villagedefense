using System.Collections.Generic;
using _Scripts.AI.Enemies;
using _Scripts.Event;
using _Scripts.Main.Services;
using _Scripts.Waves;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.DebugTools
{
    public class WaveDebugTools : MonoBehaviour
    {
        [SerializeField] private WaveManager _waveManager;

        [Title("Wave Debugging")]
        [Button("Start Wave")]
        private void StartWave()
        {
            if (_waveManager != null)
            {
                _waveManager.StartNextWave();
            }
            else
            {
                Debug.LogWarning("[WaveDebugTools] WaveManager is not assigned!");
            }
        }

        [Button("Kill One Enemy")]
        private void KillOneEnemy()
        {
            var activeEnemiesField = typeof(WaveManager).GetField("_activeEnemies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (activeEnemiesField != null)
            {
                var activeEnemies = activeEnemiesField.GetValue(_waveManager) as Dictionary<string, GameObject>;
                if (activeEnemies != null && activeEnemies.Count > 0)
                {
                    var toKillKey = new List<string>(activeEnemies.Keys)[0];
                    GameObject toKill = activeEnemies[toKillKey];
                    toKill.GetComponent<Enemy>()?.Kill();
                    Debug.Log("[WaveDebugTools] Enemy killed manually.");
                }
                else
                {
                    Debug.Log("[WaveDebugTools] No enemies to kill.");
                }
            }
            else
            {
                Debug.LogError("[WaveDebugTools] Failed to reflect _activeEnemies field.");
            }
        }
    }
}
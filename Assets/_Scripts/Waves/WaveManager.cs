using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Scripts.AI.Enemies;
using _Scripts.AI.Spawning;
using _Scripts.Event;
using _Scripts.Main;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using _Scripts.Island;

namespace _Scripts.Waves
{
    [LogTag("WaveManager")]
    public class WaveManager : MonoBehaviour, IGameService
    {
        [Header("Wave Settings")]
        [SerializeField] private float _spawnInterval = 1f;
        [SerializeField] private float _spawnOffsetDistance = 2f;
        [SerializeField] private float _waveCountdown = 3f;
        [SerializeField] private GameObject _startWaveButton;

        private HexagonManager _hexManager;
        private GameManager _gameManager;
        private EnemySpawner _enemySpawner;

        private int _currentWave = 0;
        private bool _isWaveActive = false;
        private readonly Dictionary<string, GameObject> _activeEnemies = new();

        public int CurrentWave => _currentWave;

        private void Start()
        {
            _hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            _gameManager = ServiceLocator.Instance.Get<GameManager>();
            _enemySpawner = ServiceLocator.Instance.Get<EnemySpawner>();

            EventBroker.Instance.AddEventListener<EnemyDiedEvent>(OnEnemyDied);

            if (_startWaveButton != null)
                _startWaveButton.SetActive(true); // Show on start (or you can set false initially)
        }

        private void OnDestroy()
        {
            EventBroker.Instance.RemoveEventListener<EnemyDiedEvent>(OnEnemyDied);
        }

        public void StartNextWave()
        {
            if (_isWaveActive)
            {
                Log.Warning(this, "Wave already active.");
                return;
            }

            _startWaveButton?.SetActive(false);

            _currentWave++;
            Log.Info(this, $"Starting wave {_currentWave}", "red");

            _isWaveActive = true;
            new WaveStartedEvent(_currentWave).Raise();
            _gameManager.SwitchToCombat();

            StartCoroutine(SpawnWaveRoutine());
        }

        private IEnumerator SpawnWaveRoutine()
        {
            yield return new WaitForSeconds(_waveCountdown);

            _activeEnemies.Clear();

            int enemyCount = GetEnemyCountForWave(_currentWave);
            List<Vector3> edgeSpawns = _hexManager.GetEdgeSpawnPositions(_spawnOffsetDistance);

            if (edgeSpawns.Count == 0)
            {
                Log.Warning(this, "No valid edge spawn positions.");
                yield break;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = edgeSpawns[i % edgeSpawns.Count];
                HexTile closestTile = _hexManager.GetClosestHexTile(spawnPos);

                GameObject ship = _enemySpawner.SpawnShipAt(spawnPos, closestTile);
                Log.Info(this, $"Spawned ship toward tile {closestTile.GridPosition}", "cyan");

                yield return new WaitForSeconds(_spawnInterval);
            }

            Log.Info(this, $"All enemies spawned for wave {_currentWave}.", "yellow");
        }

        private int GetEnemyCountForWave(int wave)
        {
            int baseEnemies = 2;
            float growthRate = 0.5f;
            return baseEnemies + Mathf.FloorToInt(wave * growthRate);
        }

        private void OnEnemyDied(EnemyDiedEvent e)
        {
            var id = e.Enemy.GetComponent<Enemy>()?.EnemyId;
            if (id == null) return;

            if (_activeEnemies.ContainsKey(id))
            {
                _activeEnemies.Remove(id);
                Log.Info(this, $"Enemy died. Remaining: {_activeEnemies.Count}", "yellow");

                if (_activeEnemies.Count == 0 && _isWaveActive)
                {
                    EndWave();
                }
            }
        }

        public void RegisterEnemy(GameObject enemyGO)
        {
            var enemy = enemyGO.GetComponent<Enemy>();
            if (enemy == null) return;

            _activeEnemies[enemy.EnemyId] = enemyGO;
            Log.Info(this, $"Enemy registered: {enemy.EnemyId}", "gray");
        }

        private void EndWave()
        {
            _isWaveActive = false;
            Log.Info(this, $"Wave {_currentWave} completed. All enemies defeated.", "green");
            new WaveEndedEvent(_currentWave).Raise();
            _gameManager.SwitchToIsland();

            _startWaveButton?.SetActive(true);
        }
    }
}

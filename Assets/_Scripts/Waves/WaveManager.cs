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

            _currentWave++;
            Log.Info(this, $"Starting wave {_currentWave}", "red");

            _isWaveActive = true;
            new WaveStartedEvent(_currentWave).Raise();
            _gameManager.SwitchToCombat();

            StartCoroutine(SpawnWaveRoutine());
        }

        private IEnumerator SpawnWaveRoutine()
        {
            _activeEnemies.Clear();

            int enemyCount = 1; // simple scaling

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnOneShipWithTroops();
                yield return new WaitForSeconds(_spawnInterval);
            }

            Log.Info(this, $"All enemy ships spawned for wave {_currentWave}.", "yellow");
        }

        private void SpawnOneShipWithTroops()
        {
            List<Vector3> edgeSpawns = _hexManager.GetEdgeSpawnPositions(_spawnOffsetDistance);

            if (edgeSpawns.Count == 0)
            {
                Log.Warning(this, "No valid edge spawn positions.");
                return;
            }

            Vector3 spawnPos = edgeSpawns[Random.Range(0, edgeSpawns.Count)];
            HexTile closestTile = _hexManager.GetClosestHexTile(spawnPos);

            // 💥 Spawn ship via EnemySpawner
            GameObject ship = _enemySpawner.SpawnShipAt(spawnPos, closestTile);

            Log.Info(this, $"Spawned ship toward tile {closestTile.GridPosition}", "cyan");
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
        }
    }
}

using System.Collections.Generic;
using _Scripts.AI.Enemies;
using UnityEngine;
using _Scripts.Main.Services;
using _Scripts.Island;
using _Scripts.Utility;
using _Scripts.OdinAttributes;

namespace _Scripts.AI.Spawning
{
    [LogTag("EnemySpawner")]
    public class EnemySpawner : MonoBehaviour, IGameService
    {
        [SerializeField] private List<GameObject> _shipPrefabs;
        [SerializeField] private GameObject _spawnMarkerPrefab;
        [SerializeField] private float _spawnOffsetDistance = 2f;
        [SerializeField] private Transform _spawnParent;

        private HexagonManager _hexManager;

        private void Start()
        {
            _hexManager = ServiceLocator.Instance.Get<HexagonManager>();
        }

        public GameObject SpawnShipAt(Vector3 spawnPosition, HexTile targetTile)
        {
            if (_shipPrefabs.Count == 0 || _hexManager == null)
            {
                Log.Error(this, "Missing ship prefabs or HexagonManager.");
                return null;
            }

            GameObject prefab = _shipPrefabs[Random.Range(0, _shipPrefabs.Count)];

            Vector3 correctedSpawnPos = new Vector3(spawnPosition.x, 0.62f, spawnPosition.z);
            GameObject shipGO = Instantiate(prefab, correctedSpawnPos, Quaternion.identity, _spawnParent);

            EnemyShip ship = shipGO.GetComponent<EnemyShip>();
            if (ship != null && targetTile != null)
            {
                ship.SetTargetTile(targetTile);
            }
            else
            {
                Log.Warning(this, "Spawned ship missing EnemyShip script or targetTile is null.");
            }

            if (_spawnMarkerPrefab != null)
            {
                GameObject marker = Instantiate(_spawnMarkerPrefab, spawnPosition, Quaternion.identity);
                Destroy(marker, 2f);
            }

            return shipGO;
        }

        // Optional helper for spawning at random edge (not used in WaveManager anymore)
        public GameObject SpawnRandomShip()
        {
            if (_shipPrefabs.Count == 0 || _hexManager == null)
            {
                Log.Error(this, "Missing ship prefabs or HexagonManager.");
                return null;
            }

            List<Vector3> edgeSpots = _hexManager.GetEdgeSpawnPositions(_spawnOffsetDistance);
            if (edgeSpots.Count == 0)
            {
                Log.Warning(this, "No valid edge spawn positions.");
                return null;
            }

            Vector3 spawnPos = edgeSpots[Random.Range(0, edgeSpots.Count)];
            HexTile targetTile = _hexManager.GetClosestHexTile(spawnPos);

            return SpawnShipAt(spawnPos, targetTile);
        }
    }
}

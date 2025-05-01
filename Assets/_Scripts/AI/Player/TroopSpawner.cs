using UnityEngine;
using _Scripts.Main.Services;
using _Scripts.Utility;
using _Scripts.OdinAttributes;
using _Scripts.Island;
using _Scripts.AI.Player;

namespace _Scripts.AI.Player
{
    [LogTag("TroopSpawner")]
    public class TroopSpawner : MonoBehaviour, IGameService
    {
        [Header("Troop Spawning Settings")]
        [SerializeField] private GameObject _troopPrefab; // Prefab with PlayerTroopAI + TroopVisualPack
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private float _spawnOffsetY = 0.1f;

        public void SpawnStartingUnit(Vector3 spawnPosition)
        {
            if (_troopPrefab == null)
            {
                Log.Error(this, "Troop prefab not assigned!");
                return;
            }

            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            var closestTile = hexManager.GetClosestHexTile(spawnPosition);

            if (closestTile == null)
            {
                Log.Warning(this, "No valid tile found near spawn position!");
                return;
            }

            Vector3 adjustedPosition = closestTile.transform.position + Vector3.up * _spawnOffsetY;

            GameObject unitGO = Instantiate(_troopPrefab, adjustedPosition, Quaternion.identity, _spawnParent);

            if (unitGO.TryGetComponent<PlayerTroopAI>(out var troopAI))
            {
                troopAI.CurrentTile = closestTile;
                Log.Info(this, $"Spawned troop at tile {closestTile.GridPosition}.");
            }
            else
            {
                Log.Warning(this, "Spawned object is missing PlayerTroopAI component!");
            }
        }
    }
}
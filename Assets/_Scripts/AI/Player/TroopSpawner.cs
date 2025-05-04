using UnityEngine;
using _Scripts.Main.Services;
using _Scripts.Utility;
using _Scripts.OdinAttributes;
using _Scripts.Island;
using _Scripts.AI.Player;
using _Scripts.AI.Visuals;
using _Scripts.Data;

namespace _Scripts.AI.Player
{
    [LogTag("TroopSpawner")]
    public class TroopSpawner : MonoBehaviour, IGameService
    {
        [Header("Troop Spawning Settings")]
        [SerializeField] private TroopPackData _troopPack;
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private float _spawnOffsetY = 0.1f;

        private bool _hasSpawnedStartingUnit = false;

        private void Start()
        {
            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            hexManager.OnIslandReady += TrySpawnStartingUnit;
        }

        private void OnDestroy()
        {
            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            if (hexManager != null)
                hexManager.OnIslandReady -= TrySpawnStartingUnit;
        }

        public void TrySpawnStartingUnit()
        {
            if (_hasSpawnedStartingUnit || _troopPack == null || _troopPack.LogicPrefab == null)
            {
                Log.Warning(this, "Troop spawn skipped: already spawned or pack missing.");
                return;
            }

            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            var spawnTile = hexManager.GetClosestHexTile(Vector3.zero);

            if (spawnTile == null)
            {
                Log.Warning(this, "No valid tile found near spawn position!");
                return;
            }

            SpawnUnit(spawnTile);
            _hasSpawnedStartingUnit = true;
        }

        public void SpawnUnit(HexTile targetTile)
        {
            if (_troopPack == null || _troopPack.LogicPrefab == null || targetTile == null)
            {
                Log.Error(this, "Cannot spawn unit. Missing pack, logic prefab, or tile.");
                return;
            }

            Vector3 adjustedPosition = targetTile.transform.position + Vector3.up * _spawnOffsetY;
            GameObject logicGO = Instantiate(_troopPack.LogicPrefab, adjustedPosition, Quaternion.identity, _spawnParent);

            if (logicGO.TryGetComponent<PlayerTroopAI>(out var troopAI))
            {
                troopAI.CurrentTile = targetTile;

                var visualPack = logicGO.GetComponentInChildren<TroopVisualPack>(true);
                if (visualPack != null)
                    visualPack.SetPackData(_troopPack);

                Log.Info(this, $"Spawned troop at tile {targetTile.GridPosition}.");
            }

            else
            {
                Log.Warning(this, "Spawned unit missing PlayerTroopAI component.");
            }
        }
    }
}

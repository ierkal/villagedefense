using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Scripts.Event;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using _Scripts.Waves;

namespace _Scripts.AI.Enemies
{
    [LogTag("EnemyShip")]
    public class EnemyShip : MonoBehaviour
    {
        [Header("VFX")]
        [SerializeField] private GameObject _arrivalVFX;
        [SerializeField] private Vector3 _vfxOffset = Vector3.up;
        [SerializeField] private Transform _vfxParent;

        
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _arrivalOffset = 1.2f;
        [SerializeField] private float _arrivalThreshold = 0.1f;

        [Header("Troops")]
        [SerializeField] private TroopPackData _troopPack;

        private Transform _targetPositionMarker;
        private bool _isMoving = false;

        public HexTile TargetTile { get; private set; }

        private void Start()
        {
            if (TargetTile == null)
            {
                TryMoveToNearestTile();
            }
        }

        public void SetTargetTile(HexTile targetTile)
        {
            TargetTile = targetTile;

            Vector3 tilePos = TargetTile.transform.position;
            Vector3 directionFromTile = (transform.position - tilePos).normalized;
            Vector3 stopPoint = tilePos + directionFromTile * _arrivalOffset;


            _targetPositionMarker = new GameObject("ShipTargetMarker").transform;
            _targetPositionMarker.position = stopPoint;

            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving || _targetPositionMarker == null) return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPositionMarker.position,
                _moveSpeed * Time.deltaTime
            );

            // Smooth rotation to face where it's going
            Vector3 direction = (_targetPositionMarker.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            float distance = Vector3.Distance(transform.position, _targetPositionMarker.position);
            if (distance < _arrivalThreshold)
            {
                _isMoving = false;
                OnShipArrived();
            }
        }



        private void OnShipArrived()
        {
            Log.Info(this, $"Ship arrived at {TargetTile?.GridPosition}");

            Destroy(_targetPositionMarker?.gameObject);

            // 🔥 Raise event
            new ShipArrivedEvent(this).Raise();

            // ✨ Spawn VFX
            if (_arrivalVFX != null)
            {
                Vector3 spawnPos = transform.position + _vfxOffset;
                GameObject vfx = Instantiate(_arrivalVFX, spawnPos, Quaternion.identity, _vfxParent ?? transform);
                Destroy(vfx, 3f); // cleanup
            }

            // 🚢 Deploy troops
            if (_troopPack == null || _troopPack.TroopPrefabs == null || _troopPack.TroopPrefabs.Length == 0)
            {
                Log.Warning(this, "No valid TroopPackData assigned to ship.");
                return;
            }

            StartCoroutine(DisembarkTroops());
        }


        private IEnumerator DisembarkTroops()
        {
            Vector3 center = TargetTile.transform.position;
            Quaternion facing = Quaternion.LookRotation(TargetTile.transform.position - transform.position);

            float spacing = 0.8f;

            Vector3[] offsets = new Vector3[]
            {
                new Vector3(-spacing, 0f, -spacing),
                new Vector3(spacing, 0f, -spacing),
                new Vector3(0f, 0f, 0f)
            };

            for (int i = 0; i < _troopPack.TroopCount && i < offsets.Length; i++)
            {
                if (_troopPack.TroopPrefabs.Length == 0) yield break;

                GameObject prefab = _troopPack.TroopPrefabs[Random.Range(0, _troopPack.TroopPrefabs.Length)];
                Vector3 spawnPos = center + facing * offsets[i];

                GameObject troop = Instantiate(prefab, spawnPos, Quaternion.identity);

                new TroopDisembarkedEvent(troop, TargetTile).Raise();

                yield return new WaitForSeconds(_troopPack.DisembarkDelay);
            }

            new TroopPackDeployedEvent(this).Raise();

            Log.Info(this, $"Disembarked {_troopPack.TroopCount} troops.");
        }



        private void TryMoveToNearestTile()
        {
            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            if (hexManager == null)
            {
                Log.Error(this, "HexagonManager not found.");
                return;
            }

            TargetTile = hexManager.GetClosestHexTile(transform.position);
            if (TargetTile == null)
            {
                Log.Warning(this, "No nearby hex tile found.");
                return;
            }

            Vector3 tileCenter = TargetTile.transform.position;
            Vector3 shipPos = transform.position;

            // ✅ Direction from ship TO tile center
            Vector3 directionToTile = (tileCenter - shipPos).normalized;

            // ✅ Stop short of tile using offset
            Vector3 stopPoint = tileCenter - directionToTile * _arrivalOffset;

            if (_targetPositionMarker == null)
                _targetPositionMarker = new GameObject("ShipTargetMarker").transform;

            _targetPositionMarker.position = stopPoint;
            _isMoving = true;
        }



    }
}

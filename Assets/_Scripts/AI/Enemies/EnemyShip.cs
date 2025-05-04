using System.Collections;
using System.Collections.Generic;
using _Scripts.Data;
using UnityEngine;
using _Scripts.Event;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using _Scripts.Waves;
using _Scripts.AI.Enemies;
using _Scripts.AI.Visuals;

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
        [SerializeField] private Transform _disembarkParent;

        private Transform _targetPositionMarker;
        private bool _isMoving = false;

        public HexTile TargetTile { get; private set; }

        private void Start()
        {
            if (TargetTile == null)
                TryMoveToNearestTile();
        }

        public void SetTargetTile(HexTile targetTile)
        {
            TargetTile = targetTile;
            Vector3 tilePos = TargetTile.transform.position;
            Vector3 dir = (transform.position - tilePos).normalized;
            Vector3 stopPoint = tilePos + dir * _arrivalOffset;

            _targetPositionMarker = new GameObject("ShipTargetMarker").transform;
            _targetPositionMarker.position = stopPoint;

            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving || _targetPositionMarker == null) return;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_targetPositionMarker.position.x,transform.position.y,_targetPositionMarker.position.z), _moveSpeed * Time.deltaTime);

            // Smooth face
            Vector3 direction = (_targetPositionMarker.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;
                if (flatDirection != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

            }

            Vector2 shipXZ = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetXZ = new Vector2(_targetPositionMarker.position.x, _targetPositionMarker.position.z);
            float distance = Vector2.Distance(shipXZ, targetXZ);
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
            new ShipArrivedEvent(this).Raise();

            if (_arrivalVFX != null)
            {
                var vfx = Instantiate(_arrivalVFX, transform.position + _vfxOffset, Quaternion.identity, _vfxParent ?? transform);
                Destroy(vfx, 3f);
            }

            if (_troopPack == null || _troopPack.LogicPrefab == null)
            {
                Log.Warning(this, "Missing TroopPackData or LogicPrefab.");
                return;
            }

            StartCoroutine(DisembarkTroops());
        }

        private IEnumerator DisembarkTroops()
        {
            if (_troopPack.LogicPrefab == null)
            {
                Log.Warning(this, "TroopPack has no logic prefab assigned!");
                yield break;
            }

            Vector3 center = TargetTile.transform.position;
            Quaternion facing = Quaternion.LookRotation(center - transform.position);

            GameObject logicGO = Instantiate(_troopPack.LogicPrefab, center, facing, _disembarkParent);

            if (logicGO.TryGetComponent<EnemyTroopAI>(out var enemyAI))
            {
                enemyAI.Initialize(TargetTile.transform);
            }

            var visualPack = logicGO.GetComponentInChildren<TroopVisualPack>(true);
            if (visualPack != null)
            {
                visualPack.SetPackData(_troopPack); // This spawns 3 visuals
            }

            new TroopDisembarkedEvent(logicGO, TargetTile).Raise();

            yield return new WaitForSeconds(_troopPack.DisembarkDelay);

            new TroopPackDeployedEvent(this).Raise();
            Log.Info(this, $"Disembarked 1 logic unit with {_troopPack.TroopCount} visuals.");
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
            Vector3 direction = (tileCenter - transform.position).normalized;
            Vector3 stopPoint = tileCenter - direction * _arrivalOffset;

            if (_targetPositionMarker == null)
                _targetPositionMarker = new GameObject("ShipTargetMarker").transform;

            _targetPositionMarker.position = stopPoint;
            _isMoving = true;
        }
    }
}

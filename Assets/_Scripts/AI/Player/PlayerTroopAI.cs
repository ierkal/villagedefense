using System.Collections;
using System.Collections.Generic;
using _Scripts.AI.Player.FSM;
using _Scripts.AI.Core;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Player
{
    [LogTag("PlayerTroopAI")]
    [RequireComponent(typeof(TroopAnimationController))]
    [RequireComponent(typeof(TroopStateMachine))]
    [RequireComponent(typeof(MovementOrder))]
    public class PlayerTroopAI : BaseTroopAI
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private GameObject _selectionVisualPrefab;
        private GameObject _selectionVisualInstance;
        public HexTile CurrentTile { get; set; }
        public bool IsSelected { get; private set; }

        private MovementOrder _movementOrder;
        private float _lastMoveIssuedTime;


        protected override void Awake()
        {
            base.Awake();
            StateMachine.SetState(new PlayerIdleState(this));

            _movementOrder = GetComponent<MovementOrder>();
            _movementOrder.OnMovementComplete += RefreshCurrentTile;
        }

        private void OnDestroy()
        {
            if (_movementOrder != null)
                _movementOrder.OnMovementComplete -= RefreshCurrentTile;
        }

        private IEnumerator FinishMovementAfterArrival()
        {
            float arrivalThreshold = _movementOrder.ArrivalThreshold;
            Vector3 targetPos = CurrentTile.transform.position;

            while (Vector3.Distance(transform.position, targetPos) > arrivalThreshold)
                yield return null;

            yield return new WaitForSeconds(0.05f);
            SetMovementSpeed(0f);
        }

        private void RefreshCurrentTile()
        {
            CurrentTile = ServiceLocator.Instance.Get<HexagonManager>().GetClosestHexTile(transform.position);
            Log.Info(this, $"Troop CurrentTile refreshed to {CurrentTile.GridPosition}");
            StartCoroutine(FinishMovementAfterArrival());
        }

        public void SetMoveTarget(Transform newTarget)
        {
            Target = newTarget;
            StateMachine.SetState(new MoveCommandState(this));
        }

        public void MoveToTile(HexTile targetTile)
        {
            if (targetTile == null)
            {
                Log.Warning(this, "MoveToTile was given null target!");
                return;
            }

            if (Time.time - _lastMoveIssuedTime < 0.1f)
                return;

            _lastMoveIssuedTime = Time.time;

            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            CurrentTile = hexManager.GetClosestHexTile(transform.position);

            if (CurrentTile == null)
            {
                Log.Warning(this, "CurrentTile is null! Cannot pathfind.");
                return;
            }


            // Cancel current movement and restart
            _movementOrder.StopMovement();

            List<HexTile> path = Pathfinder.FindPath(CurrentTile, targetTile);

            if (path.Count == 0)
            {
                float distToCenter = Vector3.Distance(transform.position, targetTile.transform.position);
                if (distToCenter > _movementOrder.ArrivalThreshold)
                {
                    Log.Info(this, "Re-aligning to tile center.");
                    _movementOrder.StartMovement(new List<HexTile> { targetTile });
                    return;
                }

                Log.Warning(this, "No valid path to target tile!");
                return;
            }

            _movementOrder.StartMovement(path);
            Target = targetTile.transform;
            StateMachine.SetState(new MoveCommandState(this));
            Log.Info(this, $"Started moving to {targetTile.GridPosition} with {path.Count} steps.");
        }

        public void SnapToTile()
        {
            if (CurrentTile == null)
            {
                Log.Warning(this, "SnapToTile called but CurrentTile is null!");
                return;
            }

            transform.position = CurrentTile.transform.position;
            Log.Info(this, $"Snapped to tile {CurrentTile.GridPosition}");
        }
     

        public void EnterDefendMode(Transform threat)
        {
            Target = threat;
            StateMachine.SetState(new DefendTileState(this));
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;

            if (selected)
            {
                if (_selectionVisualInstance == null && _selectionVisualPrefab != null)
                {
                    _selectionVisualInstance = Instantiate(_selectionVisualPrefab, transform);
                    _selectionVisualInstance.transform.localPosition = new Vector3(0,2.5f,0.45f); // Adjust Y if needed
                }
                else if (_selectionVisualInstance != null)
                {
                    _selectionVisualInstance.SetActive(true);
                }
            }
            else
            {
                if (_selectionVisualInstance != null)
                {
                    _selectionVisualInstance.SetActive(false);
                }
            }
        }

        public override bool HasTarget() => Target != null;
        public override bool TargetIsDead() => Target == null;

        public override bool IsInAttackRange() =>
            Target != null && Vector3.Distance(transform.position, Target.position) <= _attackRange;

        public override void MoveToTarget() { }
        public override void PerformAttack() { }
    }
}

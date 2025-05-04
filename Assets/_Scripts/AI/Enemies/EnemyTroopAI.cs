using System.Collections.Generic;
using _Scripts.AI.Enemies.FSM;
using _Scripts.AI.Core;
using _Scripts.AI.Visuals;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Enemies
{
    [LogTag("EnemyTroopAI")]
    [RequireComponent(typeof(TroopAnimationController))]
    [RequireComponent(typeof(TroopStateMachine))]
    public class EnemyTroopAI : BaseTroopAI
    {
        private TroopAnimationController _animationController;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRange = 1.5f;

        [SerializeField] private float _cheerDuration = 2f;
        public float CheerDuration => _cheerDuration;

        public bool IsInitialized { get; private set; }
        [SerializeField] private Transform _targetTileCenter;
        public Transform TargetTileCenter => _targetTileCenter;
        [SerializeField] private MovementOrder _movement;
        private TroopVisualPack _troopPack;
        public System.Action OnPathComplete;

        public TroopAnimationController AnimationController => _animationController;

        protected override void Awake()
        {
            base.Awake();
            _animationController = GetComponent<TroopAnimationController>();
            _troopPack = GetComponentInChildren<TroopVisualPack>();
            StateMachine.SetState(new InvadeTileState(this));
            // Wait for target before launching FSM
        }

        /// <summary>
        /// Called by the ship or spawner when troop is placed.
        /// </summary>
        public void Initialize(Transform tileCenter)
        {
            _targetTileCenter = tileCenter;
            Target = null; // Make sure they’re not attacking a tile!

            _movement.SetSpeed(_troopPack.PackData.MoveSpeed);

            IsInitialized = true;

            Log.Info(this, "Troop initialized with tile destination.");
        }


        public override bool HasTarget()
        {
            return Target != null;
        }

        public override bool TargetIsDead()
        {
            // Placeholder: maybe expand with health system later
            return Target == null;
        }

        public override bool IsInAttackRange()
        {
            if (Target == null) return false;
            return Vector3.Distance(transform.position, Target.position) <= _attackRange;
        }

        public override void MoveToTarget()
        {
            if (Target == null || _movement == null) return;

            HexTile start = CurrentTile; // Add a ref to current tile
            HexTile target = Target.GetComponent<HexTile>(); // or find nearby

            if (start != null && target != null)
            {
                List<HexTile> path = Pathfinder.FindPath(start, target);
                _movement.StartMovement(path);
            }
        }


        public override void PerformAttack()
        {
            Log.Info(this, "Enemy performed attack.");
            // Placeholder for future damage logic / events
        }

        public void PlayCheer()
        {
            AnimationController.PlayCheer();
            
        }
        public void StartPathTo(Transform target)
        {
            if (_movement == null || CurrentTile == null) return;

            if (ServiceLocator.Instance.Get<HexagonManager>().TryGetTileAtWorldPos(target.position, out var goal))
            {
                var path = Pathfinder.FindPath(CurrentTile, goal);
                _movement.StartMovement(path);
                _movement.OnMovementComplete += HandlePathComplete;
            }
        }

        private void HandlePathComplete()
        {
            _movement.OnMovementComplete -= HandlePathComplete;
            OnPathComplete?.Invoke();
        }
        
    }
}
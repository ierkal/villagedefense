using System.Collections.Generic;
using _Scripts.AI.Player.FSM;
using _Scripts.AI.Core;
using _Scripts.AI.Pathfinding;
using _Scripts.Island;
using _Scripts.OdinAttributes;
using UnityEngine;

namespace _Scripts.AI.Player
{
    [LogTag("PlayerTroopAI")]
    [RequireComponent(typeof(TroopAnimationController))]
    [RequireComponent(typeof(TroopStateMachine))]
    public class PlayerTroopAI : BaseTroopAI
    {
        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackRange = 1.5f;

        public HexTile CurrentTile { get; set; }
        public bool IsSelected { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            StateMachine.SetState(new PlayerIdleState(this));
        }

        public void SetMoveTarget(Transform newTarget)
        {
            Target = newTarget;
            StateMachine.SetState(new MoveCommandState(this));
        }

        public void MoveToTile(HexTile tile, Dictionary<Vector2Int, GameObject> allTiles)
        {
            if (tile == null || CurrentTile == null) return;

            var path = Pathfinder.FindPath(CurrentTile, tile, allTiles);
            if (path.Count > 0)
            {
                Target = tile.transform;
                SetMoveTarget(tile.transform); // Triggers MoveCommandState
                CurrentTile = tile;
            }
        }

        public void EnterDefendMode(Transform threat)
        {
            Target = threat;
            StateMachine.SetState(new DefendTileState(this));
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
        }

        public override bool HasTarget() => Target != null;
        public override bool TargetIsDead() => Target == null;
        public override bool IsInAttackRange() =>
            Target != null && Vector3.Distance(transform.position, Target.position) <= _attackRange;

        public override void MoveToTarget()
        {
            if (Target == null) return;

            Vector3 dir = (Target.position - transform.position).normalized;
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }

        public override void PerformAttack()
        {
            // Later: damage logic
        }
    }
}

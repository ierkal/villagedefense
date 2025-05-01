using System;
using System.Collections.Generic;
using _Scripts.AI.Player;
using _Scripts.Island;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI.Core
{
    [LogTag("MovementOrder")]
    public class MovementOrder : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _arrivalThreshold = 0.1f;

        private List<HexTile> _path;
        private int _currentIndex;
        private bool _isMoving = false;
        public float ArrivalThreshold => _arrivalThreshold;

        public bool IsMoving => _isMoving;

        public Action OnMovementComplete; // 🔥 NEW

        public void StartMovement(List<HexTile> path)
        {
            if (path == null || path.Count == 0)
            {
                Log.Warning(this, "Tried to start movement with an empty path!");
                return;
            }

            _path = path;
            _currentIndex = 0;
            _isMoving = true;

            Log.Info(this, $"Movement started with {_path.Count} tiles.");
        }

        private void Update()
        {
            if (!_isMoving || _path == null || _currentIndex >= _path.Count)
                return;

            MoveTowardsCurrentTarget();
        }

        private void MoveTowardsCurrentTarget()
        {
            HexTile targetTile = _path[_currentIndex];
            Vector3 targetPos = targetTile.transform.position;

            // --- Movement ---
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);

            // --- Rotation ---
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction.sqrMagnitude > 0.001f) // safe to prevent errors if almost at target
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            // --- Arrival Check ---
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= _arrivalThreshold)
            {
                // ✅ First: if reached tile, update troop's CurrentTile immediately
                if (TryGetComponent<PlayerTroopAI>(out var troop))
                {
                    troop.CurrentTile = targetTile;
                }

                // ✅ Second: move to next tile
                _currentIndex++;

                // ✅ Third: check if movement is complete
                if (_currentIndex >= _path.Count)
                {
                    _isMoving = false;

                    // ✅ Now safe to snap + invoke
                    if (TryGetComponent<PlayerTroopAI>(out var t))
                    {
                        t.SnapToTile(); // Center perfectly
                    }

                    Log.Info(this, "Movement complete.");
                    OnMovementComplete?.Invoke(); // ✅ Safe now
                }
            }



        }


        public void StopMovement()
        {
            _isMoving = false;
            Log.Warning(this, "Movement forcibly stopped.");
        }
    }
}

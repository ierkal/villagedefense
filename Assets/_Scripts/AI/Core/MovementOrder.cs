using System.Collections;
using System.Collections.Generic;
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

        public bool IsMoving => _isMoving;

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

            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance <= _arrivalThreshold)
            {
                _currentIndex++;

                if (_currentIndex >= _path.Count)
                {
                    _isMoving = false;
                    OnMovementComplete();
                }
            }
        }

        private void OnMovementComplete()
        {
            Log.Info(this, "Movement complete.");
            // 🔥 You could later raise an event or call back here.
        }

        public void StopMovement()
        {
            _isMoving = false;
            Log.Warning(this, "Movement forcibly stopped.");
        }
    }
}

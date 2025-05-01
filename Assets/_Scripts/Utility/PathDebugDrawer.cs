using System.Collections.Generic;
using _Scripts.Island;
using UnityEngine;

namespace _Scripts.Utility
{
    public class PathDebugDrawer : MonoBehaviour
    {
        private List<Vector3> _pathPoints = new();

        public void DrawPath(List<HexTile> path)
        {
            _pathPoints.Clear(); // Always clear previous
            foreach (var tile in path)
            {
                _pathPoints.Add(tile.transform.position + Vector3.up * 0.5f); // Offset for visibility
            }
        }

        public void ClearPath()
        {
            _pathPoints.Clear();
        }

        private void OnDrawGizmos()
        {
            if (_pathPoints == null || _pathPoints.Count < 2)
                return;

            Gizmos.color = Color.yellow;

            for (int i = 0; i < _pathPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(_pathPoints[i], _pathPoints[i + 1]);
            }
        }
    }
}
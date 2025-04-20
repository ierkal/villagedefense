using System.Collections.Generic;
using UnityEngine;
using _Scripts.Island;

namespace _Scripts.AI.Pathfinding
{
    public static class Pathfinder
    {
        public static List<HexTile> FindPath(HexTile start, HexTile goal, Dictionary<Vector2Int, GameObject> allTiles)
        {
            if (start == null || goal == null || start == goal)
                return new List<HexTile>();

            Dictionary<HexTile, HexTile> cameFrom = new();
            Queue<HexTile> frontier = new();

            frontier.Enqueue(start);
            cameFrom[start] = null;

            while (frontier.Count > 0)
            {
                HexTile current = frontier.Dequeue();

                if (current == goal)
                    break;

                foreach (Vector2Int neighborCoord in HexagonUtility.GetNeighbors(current.GridPosition))
                {
                    if (!allTiles.TryGetValue(neighborCoord, out var neighborGO))
                        continue;

                    HexTile neighbor = neighborGO.GetComponent<HexTile>();
                    if (neighbor == null || !neighbor.IsWalkable || cameFrom.ContainsKey(neighbor))
                        continue;

                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

            // Reconstruct path
            List<HexTile> path = new();
            HexTile step = goal;

            while (step != null && cameFrom.ContainsKey(step))
            {
                path.Add(step);
                step = cameFrom[step];
            }

            path.Reverse();
            return path;
        }
    }
}
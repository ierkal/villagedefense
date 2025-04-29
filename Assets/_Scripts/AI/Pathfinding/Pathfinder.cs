using System.Collections.Generic;
using _Scripts.Island;
using _Scripts.Main.Services;
using UnityEngine;

namespace _Scripts.AI.Core
{
    public static class Pathfinder
    {
        public static List<HexTile> FindPath(HexTile startTile, HexTile targetTile)
        {
            Queue<HexTile> frontier = new();
            Dictionary<HexTile, HexTile> cameFrom = new();
            HashSet<HexTile> visited = new();

            frontier.Enqueue(startTile);
            visited.Add(startTile);

            while (frontier.Count > 0)
            {
                HexTile current = frontier.Dequeue();

                if (current == targetTile)
                    break;

                foreach (HexTile neighbor in GetWalkableNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        frontier.Enqueue(neighbor);
                        visited.Add(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            // Reconstruct path
            List<HexTile> path = new();
            HexTile step = targetTile;

            while (step != startTile)
            {
                if (!cameFrom.ContainsKey(step))
                {
                    Debug.LogWarning("Path not found!");
                    return new List<HexTile>();
                }

                path.Add(step);
                step = cameFrom[step];
            }

            path.Add(startTile);
            path.Reverse();
            return path;
        }

        private static List<HexTile> GetWalkableNeighbors(HexTile tile)
        {
            List<HexTile> neighbors = new();

            var hexManager = ServiceLocator.Instance.Get<HexagonManager>();
            if (hexManager == null) return neighbors;

            Vector2Int[] offsets = {
                new(1, 0), new(-1, 0),
                new(0, 1), new(0, -1),
                new(tile.GridPosition.x % 2 == 0 ? 1 : -1, tile.GridPosition.x % 2 == 0 ? -1 : 1),
                new(tile.GridPosition.x % 2 == 0 ? -1 : 1, tile.GridPosition.x % 2 == 0 ? -1 : 1)
            };

            foreach (var offset in offsets)
            {
                Vector2Int neighborPos = tile.GridPosition + offset;
                if (hexManager.PlacedTiles.TryGetValue(neighborPos, out GameObject neighborGO))
                {
                    HexTile neighborTile = neighborGO.GetComponent<HexTile>();
                    if (neighborTile != null && neighborTile.IsWalkable)
                        neighbors.Add(neighborTile);
                }
            }

            return neighbors;
        }
    }
}

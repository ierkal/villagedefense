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
            var openSet = new List<HexTile> { startTile };
            var cameFrom = new Dictionary<HexTile, HexTile>();
            var gScore = new Dictionary<HexTile, float>();
            var fScore = new Dictionary<HexTile, float>();

            gScore[startTile] = 0f;
            fScore[startTile] = Heuristic(startTile, targetTile);

            while (openSet.Count > 0)
            {
                HexTile current = GetTileWithLowestFScore(openSet, fScore);

                if (current == targetTile)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                foreach (HexTile neighbor in GetWalkableNeighbors(current))
                {
                    float tentativeG = gScore[current] + Distance(current, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, targetTile);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            Debug.LogWarning("A* Path not found!");
            return new List<HexTile>();
        }

        private static HexTile GetTileWithLowestFScore(List<HexTile> openSet, Dictionary<HexTile, float> fScore)
        {
            HexTile bestTile = null;
            float bestScore = float.MaxValue;

            foreach (var tile in openSet)
            {
                if (fScore.TryGetValue(tile, out float score) && score < bestScore)
                {
                    bestScore = score;
                    bestTile = tile;
                }
            }

            return bestTile;
        }

        private static List<HexTile> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
        {
            List<HexTile> path = new();

            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Reverse();
            return path;
        }

        private static float Heuristic(HexTile a, HexTile b)
        {
            return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        }

        private static float Distance(HexTile a, HexTile b)
        {
            return 1f; // All tiles are equal distance unless you add cost later
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

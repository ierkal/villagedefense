using System.Collections.Generic;
using System.Linq;
using _Scripts.Island;

namespace _Scripts.Utility
{
    public static class ReachabilityChecker
    {
        /// <summary>
        /// Returns a list of all walkable tiles reachable from a starting tile.
        /// </summary>
        public static List<HexTile> GetReachableTiles(HexTile startTile)
        {
            if (startTile == null || !startTile.IsWalkable)
                return new List<HexTile>();

            HashSet<HexTile> visited = new();
            Queue<HexTile> frontier = new();

            visited.Add(startTile);
            frontier.Enqueue(startTile);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                foreach (var neighbor in current.Neighbors)
                {
                    if (neighbor != null && neighbor.IsWalkable && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);
                    }
                }
            }

            return visited.ToList();
        }
    }
}
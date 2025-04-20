using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Island
{
    public static class HexagonUtility
    {
        public static List<Vector2Int> GetNeighbors(Vector2Int coord)
        {
            bool even = coord.x % 2 == 0;

            return even
                ? new List<Vector2Int>
                {
                    new(coord.x + 1, coord.y),
                    new(coord.x - 1, coord.y),
                    new(coord.x, coord.y + 1),
                    new(coord.x, coord.y - 1),
                    new(coord.x + 1, coord.y - 1),
                    new(coord.x - 1, coord.y - 1),
                }
                : new List<Vector2Int>
                {
                    new(coord.x + 1, coord.y),
                    new(coord.x - 1, coord.y),
                    new(coord.x, coord.y + 1),
                    new(coord.x, coord.y - 1),
                    new(coord.x + 1, coord.y + 1),
                    new(coord.x - 1, coord.y + 1),
                };
        }
    }
}
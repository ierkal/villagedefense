using UnityEngine;

namespace _Scripts.Island
{
    public class HexTile : MonoBehaviour
    {
        public Vector2Int GridPosition;
        public bool HasBuilding = false;
        public TileType TileType;

        public bool IsBuildable => !HasBuilding && (TileType == TileType.Grass || TileType == TileType.Forest);
        public bool IsWalkable => TileType == TileType.Grass || TileType == TileType.Forest;
    }

    public enum TileType
    {
        Grass,
        Forest,
        Mountain,
        Lake,
        // etc...
    }
}
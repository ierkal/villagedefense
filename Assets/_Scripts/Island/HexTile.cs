using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Island
{
    public class HexTile : MonoBehaviour
    {
        [Header("Tile Setup")]
        public Vector2Int GridPosition;
        public bool HasBuilding = false;
        public TileType TileType;

        [Header("Tile Properties")]
        public bool IsBuildable => !HasBuilding && (TileType == TileType.Grass || TileType == TileType.Forest);
        public bool IsWalkable => TileType == TileType.Grass || TileType == TileType.Forest;

        [Header("Neighbor Info")]
        public List<HexTile> Neighbors = new();
        
        [Header("Visual Feedback")]
        public MMF_Player MmfPlayer;
    }

    public enum TileType
    {
        Grass,
        Forest,
        Mountain,
        Lake,
        // You can expand later (e.g., Snow, Desert, etc.)
    }
}
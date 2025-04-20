using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Island
{
    public class TileInfoClickHandler : IClickHandler
    {
        public void HandleClick(Vector2 screenPos)
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out RaycastHit hit)) return;

            HexTile tile = hit.collider.GetComponentInParent<HexTile>();
            if (tile != null)
            {
                Debug.Log($"Clicked Tile: {tile.TileType} at {tile.GridPosition}");
                // TODO: Show tile popup UI
            }
        }
    }
}
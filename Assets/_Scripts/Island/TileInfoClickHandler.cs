using _Scripts.AI.Player;
using _Scripts.Main;
using _Scripts.Main.Services;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Island
{
    public class TileInfoClickHandler
    {
        public void HandleClick(Vector2 screenPos)
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out RaycastHit hit)) return;

            var unit = hit.collider.GetComponentInParent<PlayerTroopAI>();
            if (unit != null)
            {
                Debug.Log("[TileInfoClickHandler] Troop clicked, switching to UnitCommandState");

                ServiceLocator.Instance.Get<GameManager>().SwitchToUnitCommand(screenPos); // ✅ pass click forward
                return;
            }

            var tile = hit.collider.GetComponentInParent<HexTile>();
            if (tile != null)
            {
                Debug.Log($"[TileInfoClickHandler] Tile clicked: {tile.GridPosition}");
            }
        }


    }
}
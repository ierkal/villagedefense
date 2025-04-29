using System.Collections.Generic;
using _Scripts.AI.Player;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Input.ClickHandlers
{
    public class MoveCommandClickHandler : IClickHandler
    {
        private readonly List<PlayerTroopAI> _selectedTroops;

        public MoveCommandClickHandler(List<PlayerTroopAI> selectedTroops)
        {
            _selectedTroops = selectedTroops;
        }

        public void HandleClick(Vector2 screenPosition)
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            HexTile clickedTile = hit.collider.GetComponentInParent<HexTile>();
            if (clickedTile == null)
            {
                Log.Warning(this, "Clicked object is not a HexTile.");
                return;
            }

            // Assign move orders
            foreach (var troop in _selectedTroops)
            {
                if (troop == null) continue;

                troop.MoveToTile(clickedTile);
            }

            Log.Info(this, $"Issued move command to {_selectedTroops.Count} troops toward {clickedTile.GridPosition}", "green");
            
            // ⚡ After issuing move, return to normal selection click
            ServiceLocator.Instance.Get<ClickRouter>().SetClickHandler(new UnitSelectionClickHandler());
            ServiceLocator.Instance.Get<InputContextManager>().EnableGameplayControls(); // Switch back input if needed
        }
    }
}

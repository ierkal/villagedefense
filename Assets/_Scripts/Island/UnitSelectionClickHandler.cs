using _Scripts.AI.Player;
using _Scripts.Utility;
using UnityEngine;

public class UnitSelectionClickHandler : IClickHandler
{
    public void HandleClick(Vector2 screenPos)
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out RaycastHit hit)) return;

        PlayerTroopAI unit = hit.collider.GetComponentInParent<PlayerTroopAI>();
        if (unit != null)
        {
            unit.SetSelected(true);
            Debug.Log("Troop selected!");
            // TODO: Show troop UI / enter movement mode
        }
    }
}
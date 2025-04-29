using _Scripts.Island;
using _Scripts.Utility;
using UnityEngine;

public class BuildPlacementClickHandler : IClickHandler
{
    private readonly BuildManager _buildManager;

    public BuildPlacementClickHandler(BuildManager buildManager)
    {
        _buildManager = buildManager;
    }

    public void OnClick(Vector2 screenPosition)
    {
        _buildManager.HandleBuildClick(screenPosition);
    }

    public void HandleClick(Vector2 screenPosition)
    {
        throw new System.NotImplementedException();
    }
}
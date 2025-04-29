using System.Collections.Generic;
using _Scripts.Input;
using _Scripts.Main;
using _Scripts.Main.Services;
using _Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Island
{
    public class BuildManager : MonoBehaviour, IGameService
    {
        public HexagonManager HexManager;
        public GameObject FarmHexPrefab;

        public Material GreenHighlightMaterial;
        public Material RedHighlightMaterial;

        public Button BuildFarmButton;

        private bool _isPlacingFarm = false;
        private List<HexTile> _highlightedTiles = new();
        public Material DefaultMaterial;
        private BuildPlacementClickHandler _buildClickHandler;

    

        private void Start()
        {

            BuildFarmButton.onClick.AddListener(EnterBuildMode);
        }

        public void EnterBuildMode()
        {
            _isPlacingFarm = true;
            HighlightEligibleTiles();
    
            _buildClickHandler ??= new BuildPlacementClickHandler(this);
            ServiceLocator.Instance.Get<ClickRouter>()?.SetClickHandler(_buildClickHandler);

            ServiceLocator.Instance.Get<GameManager>().SwitchToBuild();
        }

        private void HighlightEligibleTiles()
        {
            _highlightedTiles.Clear(); // Always clear first!

            foreach (var kvp in HexManager.PlacedTiles)
            {
                GameObject hex = kvp.Value;
                if (hex == null) continue;

                HexTile tile = hex.GetComponent<HexTile>();
                Renderer rend = hex.GetComponentInChildren<Renderer>();
                if (tile == null || rend == null) continue;

                bool isEligible = !tile.HasBuilding;

                rend.material = isEligible ? GreenHighlightMaterial : RedHighlightMaterial;

                if (isEligible)
                {
                    _highlightedTiles.Add(tile);
                }
            }
        }

        private void ReplaceWithFarm(HexTile tile)
        {
            Vector2Int pos = tile.GridPosition;

            Destroy(HexManager.PlacedTiles[pos]);

            Vector3 worldPos = HexManager.OffsetToWorld(pos);
            GameObject newHex = Instantiate(FarmHexPrefab, worldPos, Quaternion.identity, HexManager.TileParent);
            HexTile newTile = newHex.GetComponent<HexTile>();
            newTile.GridPosition = pos;
            newTile.HasBuilding = true;

            HexManager.PlacedTiles[pos] = newHex;

            ExitBuildMode();
        }

        public void ExitBuildMode()
        {
            _isPlacingFarm = false;

            foreach (var kvp in HexManager.PlacedTiles)
            {
                Renderer rend = kvp.Value.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material = DefaultMaterial;
                }
            }

            _highlightedTiles.Clear();

            ServiceLocator.Instance.Get<GameManager>().SwitchToIsland();
            ServiceLocator.Instance.Get<ClickRouter>()?.ClearClickHandler();


        }
    
        public void HandleBuildClick(Vector2 screenPosition)
        {
            if (Camera.main == null) return;
        
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"[BuildManager] Hit: {hit.collider.gameObject.name}"); // 🔥
        
                HexTile clickedTile = hit.collider.GetComponentInParent<HexTile>();
                if (clickedTile != null && !clickedTile.HasBuilding && _highlightedTiles.Contains(clickedTile))
                {
                    ReplaceWithFarm(clickedTile);
                }
            }
            else
            {
                Debug.LogWarning("[BuildManager] No Raycast hit detected."); // 🔥
            }
        }



    }
}

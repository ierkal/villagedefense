using System.Collections.Generic;
using _Scripts.Input;
using _Scripts.Main;
using _Scripts.Main.Services;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Island
{
    public class BuildManager : MonoBehaviour, IGameService
    {
        private PlayerInputHandler _inputHandler;
    
        public HexagonManager HexManager;
        public GameObject FarmHexPrefab;

        public Material GreenHighlightMaterial;
        public Material RedHighlightMaterial;

        public Button BuildFarmButton;

        private bool _isPlacingFarm = false;
        private List<HexTile> _highlightedTiles = new();
        public Material DefaultMaterial;
    
    

        private void Start()
        {

            BuildFarmButton.onClick.AddListener(EnterBuildMode);
            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
        }

        public void EnterBuildMode()
        {
            _isPlacingFarm = true;
            HighlightEligibleTiles();

            _inputHandler.OnClick += HandleClick;
            _inputHandler.OnCancel += ExitBuildMode;
            ServiceLocator.Instance.Get<GameManager>().SwitchToBuild();

        }

        void HighlightEligibleTiles()
        {
            foreach (var kvp in HexManager.PlacedTiles)
            {
                GameObject hex = kvp.Value;
                HexTile tile = hex.GetComponent<HexTile>();
                Renderer rend = hex.GetComponentInChildren<Renderer>();

                if (tile.HasBuilding)
                {
                    rend.material = RedHighlightMaterial;
                }
                else
                {
                    rend.material = GreenHighlightMaterial;
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

            _inputHandler.OnClick -= HandleClick;
            _inputHandler.OnCancel -= ExitBuildMode;
            
            ServiceLocator.Instance.Get<GameManager>().SwitchToIsland();

        }
    
        private void HandleClick(Vector2 screenPosition)
        {
            if (Camera.main == null) return;
            
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            
            HexTile clickedTile = hit.collider.GetComponentInParent<HexTile>();
            if (clickedTile != null && !clickedTile.HasBuilding && _highlightedTiles.Contains(clickedTile))
            {
                ReplaceWithFarm(clickedTile);
            }
        }

    }
}

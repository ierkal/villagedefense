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
        private PlayerInputHandler _inputHandler;
    
        [Header("References")]
        public HexagonManager HexManager;
        public GameObject FarmHexPrefab;
        public Button BuildFarmButton;

        [Header("Materials")]
        public Material GreenHighlightMaterial;
        public Material RedHighlightMaterial;
        public Material DefaultMaterial;

        private bool _isPlacingFarm = false;
        private List<HexTile> _highlightedTiles = new();

        private void Start()
        {
            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();

            BuildFarmButton.onClick.AddListener(EnterBuildMode);
        }

        public void EnterBuildMode()
        {
            _isPlacingFarm = true;
            HighlightEligibleTiles();

            // 🔥 Subscribe to input
            _inputHandler.OnClick += HandleBuildClick;
            _inputHandler.OnRightClick += HandleCancelBuild;

            ServiceLocator.Instance.Get<InputContextManager>()?.EnableBuildControls();
            ServiceLocator.Instance.Get<GameManager>()?.SwitchToBuild();

            Debug.Log("[BuildManager] Entered Build Mode.");
        }

        public void ExitBuildMode()
        {
            _isPlacingFarm = false;

            // 🔥 Unsubscribe to input
            _inputHandler.OnClick -= HandleBuildClick;
            _inputHandler.OnRightClick -= HandleCancelBuild;

            RestoreTileMaterials();
            _highlightedTiles.Clear();

            ServiceLocator.Instance.Get<GameManager>()?.SwitchToIsland();

            Debug.Log("[BuildManager] Exited Build Mode.");
        }

        private void HighlightEligibleTiles()
        {
            foreach (var kvp in HexManager.PlacedTiles)
            {
                if (!kvp.Value) continue;

                var tile = kvp.Value.GetComponent<HexTile>();
                var rend = kvp.Value.GetComponentInChildren<Renderer>();

                if (tile == null || rend == null) continue;

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

        private void RestoreTileMaterials()
        {
            foreach (var kvp in HexManager.PlacedTiles)
            {
                if (!kvp.Value) continue;

                var rend = kvp.Value.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    rend.material = DefaultMaterial;
                }
            }
        }

        private void HandleBuildClick(Vector2 screenPosition)
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            HexTile clickedTile = hit.collider.GetComponentInParent<HexTile>();
            if (clickedTile != null && !clickedTile.HasBuilding && _highlightedTiles.Contains(clickedTile))
            {
                ReplaceWithFarm(clickedTile);
            }
            else
            {
                Debug.LogWarning("[BuildManager] Clicked an invalid or occupied tile.");
            }
        }

        private void HandleCancelBuild()
        {
            Debug.Log("[BuildManager] Build mode canceled via Right Click.");
            ExitBuildMode();
        }

        private void ReplaceWithFarm(HexTile tile)
        {
            if (tile == null) return;

            // 1. Mark the tile logically
            tile.HasBuilding = true;
            tile.TileType = TileType.Grass; // You can define a TileType.Farm if needed

            // 2. Destroy old visuals
            foreach (Transform child in tile.transform)
            {
                Destroy(child.gameObject);
            }

            // 3. Instantiate new visual prefab under tile
            GameObject newVisual = Instantiate(FarmHexPrefab, tile.transform);
            newVisual.transform.localPosition = Vector3.zero;
            newVisual.transform.localRotation = Quaternion.identity;
            newVisual.transform.localScale = Vector3.one;

            // 4. Play spawn animation if available
            var spawnAnimator = newVisual.GetComponent<HexSpawnAnimator>();
            if (spawnAnimator != null)
            {
                spawnAnimator.PlaySpawnAnimation();
            }

            Debug.Log($"[BuildManager] Replaced tile at {tile.GridPosition} with farm.");

            ExitBuildMode();
        }
    }
}

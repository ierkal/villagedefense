using System;
using UnityEngine;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.AI.Player;
using System.Collections.Generic;
using _Scripts.AI.Core;
using MoreMountains.Feedbacks;

namespace _Scripts.Utility
{
    public class TileHighlightVisualizer : MonoBehaviour, IGameService
    {
        [Header("Hover Marker")]
        [SerializeField] private GameObject _hoverMarkerPrefab;

        [SerializeField] private float _hoverYOffset = 1.2f; // ✅ Higher to appear above units/objects

        private GameObject _hoverMarkerInstance;
        private HexTile _lastHovered;

        private PlayerTroopAI _selectedUnit;
        private MMF_Player _mmfPlayer;

        private void Awake()
        {
            _mmfPlayer = GetComponentInChildren<MMF_Player>(true);

        }

        public void SetSelectedUnit(PlayerTroopAI unit)
        {
            _selectedUnit = unit;
        }

        public void UpdateHover(HexTile tile)
        {
            Debug.Log(tile.IsWalkable);
            Debug.Log(tile.TileType);
            if (tile == null || tile == _lastHovered)
                return;

            if (_selectedUnit == null || !_selectedUnit.IsSelected || !tile.IsWalkable)
            {
                Clear();
                return;
            }

            var currentTile = _selectedUnit.CurrentTile;
            if (currentTile == null)
            {
                Clear();
                return;
            }

            List<HexTile> path = Pathfinder.FindPath(currentTile, tile);
            if (path == null || path.Count == 0)
            {
                Clear();
                return;
            }

            _lastHovered = tile;
            Vector3 hoverPos = tile.transform.position + new Vector3(0f, _hoverYOffset, 0f);

            if (_hoverMarkerInstance == null)
            {
                _hoverMarkerInstance = Instantiate(_hoverMarkerPrefab, hoverPos, Quaternion.identity, transform);
                _mmfPlayer = _hoverMarkerInstance.GetComponentInChildren<MMF_Player>(true);
            }
            else
            {
                _hoverMarkerInstance.transform.position = hoverPos;
                _hoverMarkerInstance.SetActive(true);
            }

            if (_mmfPlayer != null)
            {
                _mmfPlayer.StopFeedbacks();  // Stop previous loop if any
                _mmfPlayer.PlayFeedbacks();  // Play again
            }
        }


        public void Clear()
        {
            if (_hoverMarkerInstance != null)
                _hoverMarkerInstance.SetActive(false);

            _lastHovered = null;
        }
    }
}

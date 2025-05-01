using System;
using _Scripts.AI.Player;
using _Scripts.Input;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.AI
{
    public class UnitCommandHandler : MonoBehaviour, IGameService
    {
        private PlayerInputHandler _inputHandler;
        private PlayerTroopAI _selectedTroop;
        private bool _isWaitingForMove = false;
        private TileHoverDetector _hoverDetector;

        private void Awake()
        {
            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
        }

        private void Start()
        {
            _hoverDetector = ServiceLocator.Instance.Get<TileHoverDetector>();
        }

        public void EnterUnitCommandMode()
        {
            Debug.Log("[UnitCommandHandler] Entered Unit Command Mode.");

            ServiceLocator.Instance.Get<InputContextManager>()?.EnableUnitControls();

            _inputHandler.OnClick -= HandleUnitClick;
            _inputHandler.OnClick += HandleUnitClick;

            _inputHandler.OnRightClick -= HandleCancelCommand;
            _inputHandler.OnRightClick += HandleCancelCommand;

            _selectedTroop = null;
            _isWaitingForMove = false;
 
            _hoverDetector.enabled = false; // initially off

        }

        public void ExitUnitCommandMode()
        {
            Debug.Log("[UnitCommandHandler] Exited Unit Command Mode.");

            _inputHandler.OnClick -= HandleUnitClick;
            _inputHandler.OnRightClick -= HandleCancelCommand;

            if (_selectedTroop != null)
                _selectedTroop.SetSelected(false);

            _selectedTroop = null;
            _isWaitingForMove = false;

            Time.timeScale = 1f;

            ServiceLocator.Instance.Get<TileHighlightVisualizer>()?.Clear();
            _hoverDetector.enabled = false;
        }

        public void HandleUnitClick(Vector2 screenPos)
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            if (!_isWaitingForMove)
            {
                PlayerTroopAI unit = hit.collider.GetComponentInParent<PlayerTroopAI>();
                if (unit != null)
                {
                    _selectedTroop = unit;
                    _selectedTroop.SetSelected(true);
                    ServiceLocator.Instance.Get<TileHighlightVisualizer>()?.SetSelectedUnit(_selectedTroop);

                    Time.timeScale = 0.1f;
                    _isWaitingForMove = true;

                    Debug.Log("[UnitCommandHandler] Unit selected. Waiting for move target...");

                    // ✅ Start hover visuals
                    ServiceLocator.Instance.Get<TileHighlightVisualizer>()?.Clear();
                    _hoverDetector.enabled = true;
                }
            }
            else
            {
                HexTile tile = hit.collider.GetComponentInParent<HexTile>();
                if (tile != null && tile.IsWalkable)
                {
                    _selectedTroop?.MoveToTile(tile);
                    tile.MmfPlayer?.PlayFeedbacks();

                    Debug.Log($"[UnitCommandHandler] Move command issued to tile {tile.GridPosition}");

                    ExitUnitCommandMode();
                }
                else
                {
                    Debug.LogWarning("[UnitCommandHandler] Invalid move target.");
                }
            }
        }

        private void HandleCancelCommand()
        {
            Debug.Log("[UnitCommandHandler] Right-click: Canceling move mode.");
            ExitUnitCommandMode();
        }
    }
}

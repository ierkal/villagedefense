using System.Collections.Generic;
using _Scripts.AI.Player;
using _Scripts.Input;
using _Scripts.Main.Services;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Main
{
    [LogTag("PlayerTroopSelector")]
    public class PlayerTroopSelector : MonoBehaviour, IGameService
    {
        [SerializeField] private LayerMask _unitLayer;
        [SerializeField] private LayerMask _tileLayer;
        [SerializeField] private float _hexSpacing = 2f;

        private Camera _mainCamera;
        private PlayerInputHandler _inputHandler;
        private InputContextManager _contextManager;

        private PlayerTroopAI _selectedUnit;
        private bool _awaitingMoveTarget = false;

        private void Start()
        {
            _mainCamera = Camera.main;

            _inputHandler = ServiceLocator.Instance.Get<PlayerInputHandler>();
            _contextManager = ServiceLocator.Instance.Get<InputContextManager>();

            _inputHandler.OnClick += HandleClick;
            _inputHandler.OnCancel += CancelSelection;
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
            {
                _inputHandler.OnClick -= HandleClick;
                _inputHandler.OnCancel -= CancelSelection;
            }
        }

        private void HandleClick(Vector2 screenPos)
        {
            if (_awaitingMoveTarget)
            {
                Ray ray = _mainCamera.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, _tileLayer))
                {
                    Vector3 center = hit.point;
                    IssueFormationMove(center);
                    CancelSelection();
                    return;
                }
            }

            // Selection Phase
            Ray unitRay = _mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(unitRay, out RaycastHit unitHit, 100f, _unitLayer))
            {
                var troop = unitHit.collider.GetComponent<PlayerTroopAI>();
                if (troop != null)
                {
                    _selectedUnit?.SetSelected(false);
                    _selectedUnit = troop;
                    _selectedUnit.SetSelected(true);
                    _awaitingMoveTarget = true;

                    _contextManager.EnableUIControls(); // switch to UI mode
                    Time.timeScale = 0.1f;

                    Log.Info(this, "Troop selected, waiting for destination tile.", "cyan");
                }
            }
        }

        private void CancelSelection()
        {
            Log.Info(this, "Selection cancelled.");
            _selectedUnit?.SetSelected(false);
            _selectedUnit = null;
            _awaitingMoveTarget = false;

            Time.timeScale = 1f;
            _contextManager.EnableGameplayControls();
        }

        private void IssueFormationMove(Vector3 center)
        {
            if (_selectedUnit == null) return;

            List<PlayerTroopAI> selectedUnits = new() { _selectedUnit };

            for (int i = 0; i < selectedUnits.Count; i++)
            {
                Vector3 offset = GetFormationOffset(i);
                Vector3 targetPos = center + offset;
                selectedUnits[i].SetMoveTarget(CreateDummyTarget(targetPos));
            }
        }

        private Vector3 GetFormationOffset(int index)
        {
            float angle = index * Mathf.PI * 2f / 6f;
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * _hexSpacing;
        }

        private Transform CreateDummyTarget(Vector3 pos)
        {
            GameObject dummy = new GameObject("MoveTarget");
            dummy.transform.position = pos;
            return dummy.transform;
        }
    }
}

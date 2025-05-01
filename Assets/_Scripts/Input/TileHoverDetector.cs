using UnityEngine;
using _Scripts.Island;
using _Scripts.Main.Services;
using _Scripts.Utility;

namespace _Scripts.Input
{
    public class TileHoverDetector : MonoBehaviour, IGameService
    {
        private Camera _cam;
        private TileHighlightVisualizer _visualizer;

        private void Start()
        {
            _cam = Camera.main;
            _visualizer = ServiceLocator.Instance.Get<TileHighlightVisualizer>();
        }

        private void Update()
        {
            if (_cam == null || _visualizer == null) return;

            if (Physics.Raycast(_cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hit))
            {
                var tile = hit.collider.GetComponentInParent<HexTile>();
                if (tile != null)
                    _visualizer.UpdateHover(tile);
            }
        }
    }
}
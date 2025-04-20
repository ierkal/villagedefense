using _Scripts.Main.Services;
using UnityEngine;

namespace _Scripts.Utility
{
    public class ClickRouter : MonoBehaviour, IGameService
    {
        private IClickHandler _currentHandler;

        public void SetClickHandler(IClickHandler handler) => _currentHandler = handler;
        public void ClearClickHandler() => _currentHandler = null;

        public void RouteClick(Vector2 screenPos) => _currentHandler?.HandleClick(screenPos);
    }
}
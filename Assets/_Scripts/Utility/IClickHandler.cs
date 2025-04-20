using UnityEngine;

namespace _Scripts.Utility
{
    public interface IClickHandler
    {
        void HandleClick(Vector2 screenPosition);
    }
}
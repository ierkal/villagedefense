using _Scripts.Event;
using UnityEngine;

namespace _Scripts.AI.Enemies
{
    public class Enemy : MonoBehaviour
    {
        public string EnemyId { get; private set; }

        private void Awake()
        {
            EnemyId = System.Guid.NewGuid().ToString();
        }

        public void Kill()
        {
            new EnemyDiedEvent(gameObject).Raise();
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

namespace _Scripts.AI.Enemies
{
    [CreateAssetMenu(menuName = "Enemy/Troop Pack")]
    public class TroopPackData : ScriptableObject
    {
        [Tooltip("Enemy prefabs that will be spawned from this troop pack.")]
        public GameObject[] TroopPrefabs;

        [Min(1), Tooltip("How many troops should disembark?")]
        public int TroopCount = 3;

        [Min(0.1f), Tooltip("Time delay between each disembark (seconds)")]
        public float DisembarkDelay = 0.5f;
    }
}
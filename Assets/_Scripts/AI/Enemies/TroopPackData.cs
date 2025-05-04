using UnityEngine;

namespace _Scripts.Data
{
    [CreateAssetMenu(menuName = "Troops/Troop Pack")]
    public class TroopPackData : ScriptableObject
    {
        [Header("Logic Prefab (PlayerTroopAI or EnemyTroopAI)")]
        public GameObject LogicPrefab;

        [Header("Visual Prefabs (for TroopVisualPack)")]
        public GameObject[] VisualPrefabs;

        [Min(1), Tooltip("How many troops in this pack?")]
        public int TroopCount = 3;

        [Min(0.1f), Tooltip("Time delay between disembarks (for enemies)")]
        public float DisembarkDelay = 0.5f;
        
        [Header("Movement Settings")]
        [Min(0.1f)]
        public float MoveSpeed = 2f;

    }
}
using UnityEngine;
using _Scripts.Data;

namespace _Scripts.AI.Visuals
{
    public class TroopVisualPack : MonoBehaviour
    {
        [Header("Troop Configuration")]
        public TroopPackData PackData;

        [Header("Formation Settings")]
        [SerializeField] private float _spacing = 0.4f;

        private void Awake()
        {
            if (PackData == null || PackData.VisualPrefabs == null || PackData.VisualPrefabs.Length == 0)
            {
                Debug.LogWarning("[TroopVisualPack] Missing or invalid TroopPackData.");
                return;
            }

            SpawnFormation();
        }

        private void SpawnFormation()
        {
            for (int i = 0; i < PackData.TroopCount; i++)
            {
                Vector3 offset = GetVFormationOffset(i);
                GameObject prefab = GetRandomVisualPrefab();

                if (prefab == null)
                {
                    Debug.LogWarning("[TroopVisualPack] Null prefab in pack.");
                    continue;
                }

                GameObject visual = Instantiate(prefab, transform);
                visual.transform.localPosition = offset;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = Vector3.one;
            }
        }

        private GameObject GetRandomVisualPrefab()
        {
            var prefabs = PackData.VisualPrefabs;
            return prefabs[Random.Range(0, prefabs.Length)];
        }

        private Vector3 GetVFormationOffset(int index)
        {
            return index switch
            {
                0 => new Vector3(0, 0, 0),
                1 => new Vector3(-_spacing, 0, _spacing),
                2 => new Vector3(_spacing, 0, _spacing),
                3 => new Vector3(-_spacing * 1.5f, 0, _spacing * 2f),
                4 => new Vector3(_spacing * 1.5f, 0, _spacing * 2f),
                _ => new Vector3(Random.Range(-_spacing, _spacing), 0, Random.Range(_spacing * 2f, _spacing * 3f))
            };
        }

        // Optional: allow runtime assignment if needed
        public void SetPackData(TroopPackData data)
        {
            PackData = data;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.AI.Visuals
{
    public class TroopVisualPack : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private List<GameObject> _visualPrefabs;
        [SerializeField] private int _unitCount = 3;
        [SerializeField] private float _spacing = 0.4f;

        private List<Animator> _spawnedAnimators = new();

        private void Awake()
        {
            SpawnFormation();
        }

        private void SpawnFormation()
        {
            for (int i = 0; i < _unitCount; i++)
            {
                Vector3 offset = GetVFormationOffset(i);
                GameObject visual = Instantiate(GetRandomPrefab(), transform);
                visual.transform.localPosition = offset;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = Vector3.one;

                if (visual.TryGetComponent<Animator>(out var anim))
                    _spawnedAnimators.Add(anim);
            }
        }

        private GameObject GetRandomPrefab()
        {
            if (_visualPrefabs == null || _visualPrefabs.Count == 0)
                return null;

            return _visualPrefabs[Random.Range(0, _visualPrefabs.Count)];
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
                _ => Vector3.zero,
            };
        }

        public void SetMovementSpeed(float speed)
        {
            foreach (var anim in _spawnedAnimators)
                anim.SetFloat("Speed", speed);
        }

        public void PlayAttack()
        {
            foreach (var anim in _spawnedAnimators)
                anim.SetTrigger("AttackTrigger");
        }

        public void Die()
        {
            foreach (var anim in _spawnedAnimators)
            {
                anim.SetBool("IsDead", true);
                anim.SetTrigger("DieTrigger");
            }
        }
    }
}

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.ScriptableObjects.SceneSO
{
    [CreateAssetMenu(fileName = "SceneList", menuName = "Game/Scene List")]
    public class SceneList : ScriptableObject
    {
        [SerializeField] private List<SceneData> _allScenes = new();
        public IReadOnlyList<SceneData> AllScenes => _allScenes;
        
        [ValueDropdown("@AllScenes")]
        [SerializeField] private SceneData _selectedScene;

    }
}
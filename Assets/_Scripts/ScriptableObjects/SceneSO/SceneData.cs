using _Scripts.ScriptableObjects.EnvironmentSO;
using UnityEngine;

namespace _Scripts.ScriptableObjects.SceneSO
{
    public enum SceneLoadMode
    {
        Single,
        Additive
    }

    [CreateAssetMenu(fileName = "NewSceneData", menuName = "Game/Scene Data")]
    public class SceneData : ScriptableObject
    {
        [Header("Scene Settings")]
        [SerializeField] private string _sceneName;
        [SerializeField] private SceneLoadMode _loadMode = SceneLoadMode.Additive;

        [Header("UI Metadata (Optional)")]
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [TextArea] [SerializeField] private string _description;

        [Header("Optional")]
        public EnvironmentProfile EnvironmentProfile;

        public string SceneName => _sceneName;
        public SceneLoadMode LoadMode => _loadMode;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public string Description => _description;
    }
}
#if UNITY_EDITOR
using System.Linq;
using _Scripts.ScriptableObjects.SceneSO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SceneListValidator : EditorWindow
    {
        private SceneList _sceneList;

        [MenuItem("Tools/Scene Validator")]
        public static void Open()
        {
            GetWindow<SceneListValidator>("Scene Validator");
        }

        private void OnGUI()
        {
            _sceneList = (SceneList)EditorGUILayout.ObjectField("Scene List", _sceneList, typeof(SceneList), false);

            if (_sceneList == null) return;

            var buildScenes = EditorBuildSettings.scenes.Select(s => s.path).ToList();

            foreach (var sceneData in _sceneList.AllScenes)
            {
                string path = AssetDatabase.GetAssetPath(sceneData);
                bool existsInBuild = buildScenes.Any(buildScenePath => buildScenePath.Contains(sceneData.SceneName));

                GUI.color = existsInBuild ? Color.green : Color.red;
                EditorGUILayout.LabelField(sceneData.SceneName, existsInBuild ? "✔ In Build Settings" : "✘ MISSING");
            }

            GUI.color = Color.white;
        }
    }
}
#endif
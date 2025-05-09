//  Distant Lands 2025
//  COZY: Stylized Weather 3
//  All code included in this file is protected under the Unity Asset Store Eula

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DistantLands.Cozy.EditorScripts
{
    public class CozySceneTools : MonoBehaviour
    {

        [MenuItem("Tools/Cozy: Stylized Weather 3/Create Volume", false, 102)]
        public static void CreateVolume()
        {


            Camera view = SceneView.lastActiveSceneView.camera;


            GameObject i = new GameObject();
            i.name = "Cozy Volume";
            i.AddComponent<BoxCollider>().isTrigger = true;
            i.AddComponent<CozyVolume>();
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create Cozy Volume");
            Selection.activeGameObject = i;


        }


        [MenuItem("Tools/Cozy: Stylized Weather 3/Create FX Block Zone", false, 102)]
        public static void CreateFXBlockZone()
        {


            Camera view = SceneView.lastActiveSceneView.camera;


            GameObject i = new GameObject();
            i.name = "Cozy FX Block Zone";
            i.AddComponent<BoxCollider>().isTrigger = true;
            i.tag = "FX Block Zone";
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create Cozy FX Block Zone");
            Selection.activeGameObject = i;


        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Create Fog Culling Zone", false, 102)]
        public static void CreateFogCullingZone()
        {
            Camera view = SceneView.lastActiveSceneView.camera;

            GameObject i = GameObject.CreatePrimitive(PrimitiveType.Cube);
            i.name = "Cozy Fog Cull Zone";
            i.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Fog Culling Zone");
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create Cozy Fog Culling Zone");
            Selection.activeGameObject = i;

        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Create Local Biome", false, 102)]
        public static void CreateLocalBiome()
        {


            Camera view = SceneView.lastActiveSceneView.camera;


            GameObject i = new GameObject();
            i.name = "Local COZY Biome";
            CozyBiome biome = i.AddComponent<CozyBiome>();
            i.transform.position = (view.transform.forward * 5) + view.transform.position;
            biome.mode = CozyBiome.BiomeMode.Local;
            i.AddComponent<BoxCollider>();

            Undo.RegisterCreatedObjectUndo(i, "Create Local COZY Biome");
            Selection.activeGameObject = i;


        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Create Global Biome", false, 102)]
        public static void CreateGlobalBiome()
        {

            Camera view = SceneView.lastActiveSceneView.camera;

            GameObject i = new GameObject();
            i.name = "Global COZY Biome";
            i.AddComponent<CozyBiome>();
            i.transform.position = (view.transform.forward * 5) + view.transform.position;

            Undo.RegisterCreatedObjectUndo(i, "Create Global COZY Biome");
            Selection.activeGameObject = i;


        }


        [MenuItem("Tools/Cozy: Stylized Weather 3/Toggle Tooltips", false, 300)]
        public static void ToggleTooltips()
        {

            EditorPrefs.SetBool("CZY_Tooltips", !EditorPrefs.GetBool("CZY_Tooltips"));

        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Documentation", false, 1000)]
        public static void OpenDocs()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/welcome/hello");
        }


        [MenuItem("Tools/Cozy: Stylized Weather 3/Discord", false, 1000)]
        public static void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/eZXxGCp9ww");
        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/FAQs", false, 1000)]
        public static void OpenFAQs()
        {
            Application.OpenURL("https://distant-lands.gitbook.io/cozy-stylized-weather-documentation/appendix/frequently-asked-questions-faqs");
        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Setup Scene (No Modules)", false, 1)]
        public static void SetupSceneNoModules()
        {

            if (CozyWeather.instance)
            {
                EditorUtility.DisplayDialog("Cozy:Weather", "You already have a Cozy:Weather system in your scene!", "Ok");
                return;
            }

            if (!Camera.main)
            {
                EditorUtility.DisplayDialog("Cozy:Weather", "You need a main camera in your scene to setup for Cozy:Weather!", "Ok");
                return;
            }


            foreach (Light i in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (i.type == LightType.Directional)
                    if (EditorUtility.DisplayDialog("You already have a directional light!", "Do you want to delete " + i.gameObject.name + "? Cozy:Weather will properly light your scene", "Delete", "Keep this light"))
                        DestroyImmediate(i.gameObject);
            }

            if (!Camera.main.GetComponent<FlareLayer>())
                Camera.main.gameObject.AddComponent<FlareLayer>();



#if UNITY_POST_PROCESSING_STACK_V2 && !(COZY_URP || COZY_HDRP)

            if (!FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>())
            {
                List<string> path = new List<string>();
                path.Add("Assets/Cozy Weather/Post FX/");


                GameObject i = new GameObject();

                i.name = "Post FX Volume";
                i.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile = GetAssets<UnityEngine.Rendering.PostProcessing.PostProcessProfile>(path.ToArray(), "Post FX")[0];
                i.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().isGlobal = true;
                i.layer = 1;

                if (!Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
                    Camera.main.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().volumeLayer = 2;
            }
#endif


            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

            GameObject weatherSphere = Instantiate(Resources.Load("Cozy Prefabs/Empty Weather Sphere Reference") as GameObject);

            weatherSphere.name = "Cozy Weather Sphere";


        }

        [MenuItem("Tools/Cozy: Stylized Weather 3/Setup Scene", false, 1)]
        public static void SetupScene()
        {

            if (FindObjectOfType<CozyWeather>())
            {
                EditorUtility.DisplayDialog("Cozy:Weather", "You already have a Cozy:Weather system in your scene!", "Ok");
                return;
            }

            if (!Camera.main)
            {
                EditorUtility.DisplayDialog("Cozy:Weather", "You need a main camera in your scene to setup for Cozy:Weather!", "Ok");
                return;
            }

            foreach (Light i in FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (i.type == LightType.Directional)
                    if (EditorUtility.DisplayDialog("You already have a directional light!", "Do you want to delete " + i.gameObject.name + "? Cozy:Weather will properly light your scene", "Delete", "Keep this light"))
                        DestroyImmediate(i.gameObject);
            }
            
            if (!Camera.main.GetComponent<FlareLayer>())
                Camera.main.gameObject.AddComponent<FlareLayer>();



            // #if UNITY_POST_PROCESSING_STACK_V2

            //             if (!FindObjectOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>())
            //             {
            //                 List<string> path = new List<string>();
            //                 path.Add("Assets/Cozy Weather/Post FX/");

            //                 GameObject i = new GameObject();

            //                 i.name = "Post FX Volume";
            //                 i.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile = GetAssets<UnityEngine.Rendering.PostProcessing.PostProcessProfile>(path.ToArray(), "Post FX")[0];
            //                 i.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().isGlobal = true;
            //                 i.layer = 1;

            //                 if (!Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>())
            //                     Camera.main.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>().volumeLayer = 2;
            //             }
            // #endif


            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;

            GameObject weatherSphere = Instantiate(Resources.Load("Cozy Prefabs/Cozy Weather Sphere") as GameObject);

            weatherSphere.name = "Cozy Weather Sphere";


        }

        public static List<T> GetAssets<T>(string[] _foldersToSearch, string _filter) where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(_filter, _foldersToSearch);
            List<T> a = new List<T>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }
            return a;
        }

    }
}
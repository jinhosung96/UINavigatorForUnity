using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoraeGames.Library.Other
{
    public class PrefabResourcePath : MonoBehaviour
    {
        [SerializeField, HideInInspector] public string resourcePath;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                string assetPath = AssetDatabase.GetAssetPath(gameObject);
                int resourcesIndex = assetPath.IndexOf("Resources/");
                if (resourcesIndex != -1)
                {
                    resourcePath = assetPath.Substring(resourcesIndex + "Resources/".Length);
                    resourcePath = Path.ChangeExtension(resourcePath, null);
                    Debug.Log($"Resource Path set to: {resourcePath}");
                }
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(PrefabResourcePath))]
    public class PrefabResourcePathEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            PrefabResourcePath prefabResourcePath = (PrefabResourcePath)target;
            string assetPath = AssetDatabase.GetAssetPath(prefabResourcePath.gameObject);

            int resourcesIndex = assetPath.IndexOf("Resources/");
            if (resourcesIndex != -1)
            {
                string relativePath = assetPath.Substring(resourcesIndex + "Resources/".Length);
                relativePath = Path.ChangeExtension(relativePath, null);
                prefabResourcePath.resourcePath = relativePath;
                EditorUtility.SetDirty(prefabResourcePath);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PrefabResourcePath prefabResourcePath = (PrefabResourcePath)target;

            if (!string.IsNullOrEmpty(prefabResourcePath.resourcePath))
            {
                EditorGUILayout.LabelField("Resources Path", prefabResourcePath.resourcePath);
            }
        }
    }

#endif
}
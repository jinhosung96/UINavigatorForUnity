#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MoraeGames.Library.Editor.ScriptTemplateModifier
{
    public sealed class ScriptTemplateProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string metaPath)
        {
            var suffixIndex = metaPath.LastIndexOf(".meta");
            if (suffixIndex < 0) return;

            var scriptPath = metaPath.Substring(0, suffixIndex);
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            var extname = Path.GetExtension(scriptPath);
            var fullPath = Path.Combine(Application.dataPath, scriptPath.Substring("Assets/".Length));
            var folderName = new DirectoryInfo(fullPath.Substring(0, scriptName.Length - 4)).Name;
            var namespacePath = scriptPath.Substring("Assets/".Length, scriptPath.Length - "Assets/".Length - scriptName.Length - 4)
                .Replace(".", "")
                .Replace("/", ".")
                .Replace("#", "")
                .Replace(@"[0-9]", "");

            if (extname != ".cs") return;

            string templatePath = default;

            if (scriptName.EndsWith("Sheet")) templatePath = AssetDatabase.GUIDToAssetPath("b856b5b54f16f2e43abd3421aa7a741f");
            else if (scriptName.EndsWith("Page")) templatePath = AssetDatabase.GUIDToAssetPath("ea76c5002baf9544587bbc93d5ddb9ce");
            else if (scriptName.EndsWith("Modal")) templatePath = AssetDatabase.GUIDToAssetPath("4f88545e18c7b3a45a6137fa869becbb");
            else if (scriptName.EndsWith("Context")) templatePath = AssetDatabase.GUIDToAssetPath("51527f30dfa70604889a867d1d1755c0");
            else if (scriptName.EndsWith("Model")) templatePath = AssetDatabase.GUIDToAssetPath("8c7716432fa40a74c83a4a9b5da0336c");
            else if (scriptName.EndsWith("View")) templatePath = AssetDatabase.GUIDToAssetPath("1311c83fd8deab645a38c59b3759cf71");
            else if (scriptName.EndsWith("Presenter")) templatePath = AssetDatabase.GUIDToAssetPath("b9b5b34cecdd1284f88a5ffbbd408f40");
            else if (scriptName.EndsWith("ScriptableObject") || scriptName.EndsWith("SO") || scriptName.EndsWith("Setting")) templatePath = AssetDatabase.GUIDToAssetPath("02e9f3b7e4db83249bb3b60ea3362e9f");

            if (templatePath == default) return;

            string template = File.ReadAllText(templatePath);

            template = template.Replace("#NAMESPACE#", namespacePath);
            template = template.Replace("#SCRIPTNAME#", scriptName);
            template = template.Replace("#FOLDERNAME#", folderName);
            template = template.Replace("#NOTRIM#", "");
            File.WriteAllText(fullPath, template);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/CreateMVP", false, 0)]
        static void CreateMVP()
        {
            string folderPath = GetSelectedPathOrFallback();
            string folderName = new DirectoryInfo(folderPath).Name;

            CreateScript(folderPath, folderName, "Context", File.ReadAllText(AssetDatabase.GUIDToAssetPath("e49e02b5517aba148b70f7d44d5406be")));
            CreateScript(folderPath, folderName, "Model", File.ReadAllText(AssetDatabase.GUIDToAssetPath("bf4ef348ff43b3f4b83b965604a2c81d")));
            CreateScript(folderPath, folderName, "View", File.ReadAllText(AssetDatabase.GUIDToAssetPath("c0f71e86efe9ac2458ecf50718284a9f")));
            CreateScript(folderPath, folderName, "Presenter", File.ReadAllText(AssetDatabase.GUIDToAssetPath("b3613ac5dd2f4aa4c8cf6907ab6ebb0e")));

            AssetDatabase.Refresh();
        }

        static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return path;
        }

        static void CreateScript(string folderPath, string folderName, string suffixName, string template)
        {
            string scriptName = $"{folderName}{suffixName}";
            string scriptPath = Path.Combine(folderPath, $"{scriptName}.cs");
            var namespacePath = folderPath.Substring("Assets/".Length)
                .Replace(".", "")
                .Replace("/", ".")
                .Replace("#", "")
                .Replace(@"[0-9]", "");
            
            if (!File.Exists(scriptPath))
            {
                template = template.Replace("#NAMESPACE#", namespacePath);
                template = template.Replace("#SCRIPTNAME#", scriptName);
                template = template.Replace("#FOLDERNAME#", folderName);
                template = template.Replace("#NOTRIM#", "");
                File.WriteAllText(scriptPath, template);
                
                Debug.Log($"{folderName + suffixName} script created at {scriptPath}");
            }
            else
            {
                Debug.LogWarning($"{folderName + suffixName} script already exists at {scriptPath}");
            }
        }
    }
}
#endif
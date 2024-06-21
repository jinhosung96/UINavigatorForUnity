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
            var className = Path.GetFileNameWithoutExtension(scriptPath);
            var extname = Path.GetExtension(scriptPath);
            var fullPath = Path.Combine(Application.dataPath, scriptPath.Substring("Assets/".Length));
            var namespacePath = scriptPath.Substring(0, scriptPath.Length - 3).Substring("Assets/".Length).Replace(".", "").Replace("/", ".").Replace("#", "");
            namespacePath = Regex.Replace(namespacePath, @"[0-9]", "");
            
            if (extname != ".cs") return;

            string templatePath = default;

            if (className.Contains("Context"))
            {
                if (className.Contains("Sheet")) templatePath = AssetDatabase.GUIDToAssetPath("b856b5b54f16f2e43abd3421aa7a741f");
                else if (className.Contains("Page")) templatePath = AssetDatabase.GUIDToAssetPath("ea76c5002baf9544587bbc93d5ddb9ce");
                else if(className.Contains("Modal")) templatePath = AssetDatabase.GUIDToAssetPath("4f88545e18c7b3a45a6137fa869becbb");
                else templatePath = AssetDatabase.GUIDToAssetPath("e49e02b5517aba148b70f7d44d5406be");
            }
            else if (className.Contains("Presenter")) templatePath = AssetDatabase.GUIDToAssetPath("b3613ac5dd2f4aa4c8cf6907ab6ebb0e");
            else if (className.Contains("ScriptableObject")) templatePath = AssetDatabase.GUIDToAssetPath("02e9f3b7e4db83249bb3b60ea3362e9f");
            
            if(templatePath == default) return;
            
            string content = File.ReadAllText(templatePath);

            content = content.Replace("#NAMESPACE#", namespacePath);
            content = content.Replace("#SCRIPTNAME#", className);
            content = content.Replace("#NOTRIM#", "");
            File.WriteAllText(fullPath, content);
            AssetDatabase.Refresh();
        }
    }
}
#endif
#if UNITY_EDITOR
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#if CSV_HELPER_SUPPORT
using CsvHelper;
using CsvHelper.Configuration;
#endif
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoraeGames.Library.Editor.ScriptTemplateModifier
{
    public sealed class ScriptTemplateProcessor : AssetModificationProcessor
    {
        #region Fields

        static bool isProcessing;

        #endregion

        #region Unity Events

        public static void OnWillCreateAsset(string metaPath)
        {
            if (isProcessing) return;

            var suffixIndex = metaPath.LastIndexOf(".meta");
            if (suffixIndex < 0) return;

            var scriptPath = metaPath.Substring(0, suffixIndex);
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            var extname = Path.GetExtension(scriptPath);
            var folderPath = Path.GetDirectoryName(scriptPath);

            if (extname != ".cs") return;
            
            if (scriptName.EndsWith("Sheet")) GenerateScript(folderPath, scriptName, "20352736d22781c438616f0745b9f902", true);
            else if (scriptName.EndsWith("Page")) GenerateScript(folderPath, scriptName, "ea76c5002baf9544587bbc93d5ddb9ce", true);
            else if (scriptName.EndsWith("Modal")) GenerateScript(folderPath, scriptName, "4f88545e18c7b3a45a6137fa869becbb", true);
            else if (scriptName.EndsWith("Context")) GenerateScript(folderPath, scriptName, "51527f30dfa70604889a867d1d1755c0", true);
            else if (scriptName.EndsWith("Model")) GenerateScript(folderPath, scriptName, "8c7716432fa40a74c83a4a9b5da0336c", true);
            else if (scriptName.EndsWith("View")) GenerateScript(folderPath, scriptName, "1311c83fd8deab645a38c59b3759cf71", true);
            else if (scriptName.EndsWith("Presenter")) GenerateScript(folderPath, scriptName, "b9b5b34cecdd1284f88a5ffbbd408f40", true);
            else if (scriptName.EndsWith("ScriptableObject") || scriptName.EndsWith("SO") || scriptName.EndsWith("Setting")) GenerateScript(folderPath, scriptName, "02e9f3b7e4db83249bb3b60ea3362e9f", true);
            
            AssetDatabase.Refresh();
        }

        #endregion

        #region Menu Items

        [MenuItem("Assets/Create/Generate MVP Class", false, 0)]
        static void GenerateMVP()
        {
            isProcessing = true;

            string folderPath = GetSelectedPathOrFallback();
            string folderName = new DirectoryInfo(folderPath).Name;

            GenerateScript(folderPath, $"{folderName}Context", "e49e02b5517aba148b70f7d44d5406be", false, ("#FOLDERNAME#", folderName));
            GenerateScript(folderPath, $"{folderName}Model", "bf4ef348ff43b3f4b83b965604a2c81d", false, ("#FOLDERNAME#", folderName));
            GenerateScript(folderPath, $"{folderName}View", "c0f71e86efe9ac2458ecf50718284a9f", false, ("#FOLDERNAME#", folderName));
            GenerateScript(folderPath, $"{folderName}Presenter", "b3613ac5dd2f4aa4c8cf6907ab6ebb0e", false, ("#FOLDERNAME#", folderName));

            AssetDatabase.Refresh();

            isProcessing = false;
        }

#if CSV_HELPER_SUPPORT
        [MenuItem("Assets/Create/Generate C# Class from CSV", true)]
        static bool ValidateGetHeaders()
        {
            var selected = Selection.activeObject;
            return selected != null && AssetDatabase.GetAssetPath(selected).EndsWith(".csv");
        }

        [MenuItem("Assets/Create/Generate C# Class from CSV", false, 0)]
        static void GenerateCSV()
        {
            isProcessing = true;

            var csvPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var scriptName = $"{Path.GetFileNameWithoutExtension(csvPath)}Row";
            var folderPath = Path.GetDirectoryName(csvPath);

            if (string.IsNullOrEmpty(csvPath))
            {
                Debug.LogError("Invalid CSV file path.");
                return;
            }

            string csvText = File.ReadAllText(csvPath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                ShouldSkipRecord = record => record.Row.Parser.Record![0].StartsWith("#") || string.IsNullOrWhiteSpace(record.Row.Parser.Record[0])
            };

            using var reader = new StringReader(csvText);
            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            GenerateScript(
                folderPath,
                scriptName,
                "20352736d22781c438616f0745b9f902",
                false,
                ("#ROWS#", string.Join("\n", headers.Select(header => $"        [field: SerializeField] public string {MakeValidPropertyName(header)} {{ get; set; }}"))),
                ("#MAPPING#", string.Join("\n", headers.Select(header => $"            Map(m => m.{MakeValidPropertyName(header)}).Name(\"{header}\");")))
            );

            AssetDatabase.Refresh();

            isProcessing = false;
        }
#endif

        #endregion

        #region Private Methods

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

        static void GenerateScript(string folderPath, string scriptName, string templateGuid, bool isReplace = false, params (string oldValue, string newValue)[] replacements)
        {
            string scriptPath = Path.Combine(folderPath, $"{scriptName}.cs");
            var namespacePath = GetNamespacePath(folderPath);
            string template = File.ReadAllText(AssetDatabase.GUIDToAssetPath(templateGuid));

            if (isReplace || !File.Exists(scriptPath))
            {
                template = template.Replace("#NAMESPACE#", namespacePath);
                template = template.Replace("#SCRIPTNAME#", scriptName);
                template = template.Replace("#NOTRIM#", "");
                foreach (var r in replacements) template = template.Replace(r.oldValue, r.newValue);
                File.WriteAllText(scriptPath, template);
            }
        }

        static string GetNamespacePath(string folderPath)
        {
            return folderPath.Substring("Assets/".Length)
                .Replace(".", "")
                .Replace("/", ".")
                .Replace("\\", ".")
                .Replace("#", "")
                .Replace(@"[0-9]", "");
        }

        static string MakeValidPropertyName(string header)
        {
            // Replace invalid characters with underscores
            string validName = Regex.Replace(header, @"[^a-zA-Z0-9]", "_");
    
            // Remove leading underscores if any
            validName = validName.Trim('_');

            // Convert to PascalCase
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            validName = textInfo.ToTitleCase(validName.ToLower()).Replace("_", string.Empty);
            
            // If the first character is a digit, prepend an underscore
            if (char.IsDigit(validName[0]))
            {
                validName = "_" + validName;
            }

            return validName;
        }

        #endregion
    }
}
#endif
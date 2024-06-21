#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Debug = MoraeGames.Library.Util.Debug.Debug;

namespace MoraeGames.Library.Editor.CopySelectFileGUID
{
    public class CopySelectFileGuid : EditorWindow
    {
        [MenuItem("Assets/GUID Copy")]
        private static void GetSelectedAssetGuid()
        {
            if (Selection.assetGUIDs.Length > 0)
            {
                string selectedGUID = Selection.assetGUIDs[0];
                Util.Debug.Debug.Log($"Selected asset GUID : {selectedGUID}");
                TextEditor textEditor = new UnityEngine.TextEditor();
                textEditor.text = selectedGUID;
                textEditor.SelectAll();
                textEditor.Copy();
            }
            else
            {
                Util.Debug.Debug.LogWarning("No asset selected");
            }
        }
    }
}
#endif
using UnityEditor;
using Debug = JHS.Library.Util.Debug.Debug;

namespace JHS.Library.Editor.RefreshOnPlay
{
    [InitializeOnLoad]
    public static class RefreshOnPlay
    {
        static RefreshOnPlay()
        {
            EditorApplication.playModeStateChanged += PlayRefresh;
        }

        static void PlayRefresh(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && !EditorPrefs.GetBool("kAutoRefresh"))
            {
                Debug.Log("Refresh on play..");
                AssetDatabase.Refresh();
            }
        }
    }
}
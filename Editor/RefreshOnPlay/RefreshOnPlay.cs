using MoraeGames.Library.Util.Debug;
using UnityEditor;
using Debug = MoraeGames.Library.Util.Debug.Debug;

namespace MoraeGames.Library.Editor.RefreshOnPlay
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
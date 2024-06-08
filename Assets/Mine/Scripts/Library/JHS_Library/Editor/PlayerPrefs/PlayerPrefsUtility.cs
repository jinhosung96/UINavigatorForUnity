using UnityEngine;
using UnityEditor;

public class PlayerPrefsUtility
{
    [MenuItem("PlayerPrefs/PlayerPrefs Delete All")]
    static void ClearAllSaveData()
    {
        if (EditorUtility.DisplayDialog("PlayerPrefs Delete All",
                "정말 삭제 하시겠습니까?", "네", "아니오"))
        {
            Debug.Log("삭제 완료");
            PlayerPrefs.DeleteAll();
        }
    }
}
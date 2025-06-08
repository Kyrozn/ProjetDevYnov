using UnityEditor;
using UnityEngine;

public class PlayerPrefsTools
{
    [MenuItem("Tools/Clear PlayerPrefs %#d")] // Ctrl+Shift+D (Windows) / Cmd+Shift+D (Mac)
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🧹 PlayerPrefs supprimés !");
    }
}

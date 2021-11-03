using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
[CustomEditor(typeof(AutoSaver))]
public class AutoSaver : EditorWindow
{
    public static float timeBetweenSaves = 300;
    public static bool console;
    private static double nextSave;
    private static int autoSaveLabel = 1;

    [MenuItem("Tools/AutoSaver")]
    static void Init()
    {
        AutoSaver window = (AutoSaver)GetWindowWithRect(
            typeof(AutoSaver),
            new Rect(0, 0, 160, 60));
        window.Show();
    }
    #region UI
    [System.Obsolete]
    void OnGUI()
    {
        EditorGUI.LabelField(new Rect(10, 10, 80, 30), "Save Each:");
        EditorGUI.LabelField(new Rect(80, 10, 80, 30), timeBetweenSaves + " secs");

        timeBetweenSaves = EditorGUILayout.FloatField("Time Between Saves", timeBetweenSaves);
        console = GUI.Toggle(new Rect(200, 20, 100, 30), console, "Console");
        double timeToSave = nextSave - EditorApplication.timeSinceStartup;

        EditorGUI.LabelField(new Rect(10, 30, 80, 20), "Next Save:");
        EditorGUI.LabelField(new Rect(80, 30, 80, 20), timeToSave.ToString("N1") + " secs");
        if (GUI.Button(new Rect(10, 50, 200, 25), "Save Changes"))
        {
            Save();
            if (console)
            {
                Debug.Log("Updated New SaveTime");
            }
        }
        this.Repaint();
        if (EditorApplication.timeSinceStartup > nextSave)
        {
            Save();
        }
    }
    #endregion
    #region autosave
    [System.Obsolete]
    static AutoSaver()
    {
        EditorApplication.playmodeStateChanged = () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Save();
            }
        };
    }
    [System.Obsolete]
    public static void Save()//saves
    {
        if (!EditorApplication.isPlaying)
        {
            autoSaveLabel = autoSaveLabel + 1;
            nextSave = EditorApplication.timeSinceStartup + timeBetweenSaves;
            _ = EditorApplication.SaveScene();
            AssetDatabase.SaveAssets();
            if (console)
            {
                Debug.Log("Auto-Saving Scene/Assets: " + EditorApplication.currentScene);
            }
        }
    }
    #endregion
}
using UnityEditor;

public class EditorWindows : Editor
{
    [MenuItem("Help The Giant/Control Panel", false, 0)]
    public static void OpenControlPanel()
    {
        Selection.activeObject = ControlPanel.Instance;
    }
}
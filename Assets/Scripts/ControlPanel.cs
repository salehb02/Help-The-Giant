using UnityEngine;

[CreateAssetMenu(menuName = "Current Project/Control Panel")]
public class ControlPanel : ScriptableObject
{
    public ItemClass[] items;

    [System.Serializable]
    public class ItemClass
    {
        public ItemType type;
        public Item positivePrefab;
        public Item negativePrefab;
    }

    #region Singleton
    private static ControlPanel instance;
    public static ControlPanel Instance
    {
        get => instance == null ? instance = Resources.Load("ControlPanel") as ControlPanel : instance;
    }
    #endregion
}
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

        [Header("Outline Settings")]
        [Space(2)]
        public Color positiveOutline = Color.green;
        public Color negativeOutline = Color.red;

        [Header("Particle Effect")]
        [Space(2)]
        public GameObject particleVFX;
    }

    #region Singleton
    private static ControlPanel instance;
    public static ControlPanel Instance
    {
        get => instance == null ? instance = Resources.Load("ControlPanel") as ControlPanel : instance;
    }
    #endregion
}
using UnityEngine;

[CreateAssetMenu(menuName = "Current Project/Control Panel")]
public class ControlPanel : ScriptableObject
{
    public ItemClass[] items;

    [Header("Particles")]
    public GameObject positiveItemParticle;
    public GameObject negativeItemParticle;
    public float itemParticleLerpSpeed = 10f;

    [Header("Colors")]
    public Color positiveItemColor = Color.green;
    public Color negativeItemColor = Color.red;

    [System.Serializable]
    public class ItemClass
    {
        public ItemType type;
        public Item prefab;

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
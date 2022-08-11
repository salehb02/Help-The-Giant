using UnityEngine;

[CreateAssetMenu(menuName = "Current Project/Control Panel")]
public class ControlPanel : ScriptableObject
{
    public ItemClass[] items;

    [Header("Particles")]
    public GameObject trailParticle;
    public float itemParticleLerpSpeed = 10f;

    [Header("Colors")]
    public Color positiveItemColor = Color.green;
    public Color negativeItemColor = Color.red;

    [System.Serializable]
    public class ItemClass
    {
        [Header("Main Settings")]
        public string title;
        public ItemType type;
        public Item prefab;
        public ItemStatue statue;
        public MathOperators amountConversion;
        public float multiplyAmount;
        public float changeAmount = 1;
        public float affectTime = 1f;

        [Header("Outline Settings")]
        [Space(2)]
        public bool changeOutlineColor;
        public Color positiveOutline = Color.green;

        [Header("Trail")]
        [Space(2)]
        public Color trailParticleColor = Color.green;

        [Header("Size Settings")]
        [Space(2)]
        public bool changeSize;
        public float changeSizeAmount;

        [Header("Shake Settings")]
        [Space(2)]
        public bool shakeOnAffection;
        public float shakeStrength = 3.5f;
        public int shakeVibrato = 15;

        [Header("Particle Effect")]
        [Space(2)]
        public GameObject headVFX;
        public GameObject rightHandVFX;
        public GameObject leftHandVFX;
        public GameObject bodyVFX;
        public bool destroyVFXOnEnd;
    }

    #region Singleton
    private static ControlPanel instance;
    public static ControlPanel Instance
    {
        get => instance == null ? instance = Resources.Load("ControlPanel") as ControlPanel : instance;
    }
    #endregion
}
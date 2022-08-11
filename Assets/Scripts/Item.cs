using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public TextMeshPro amountText;

    private Monster _alliedMonster;
    private bool _throwItem;
    private Vector3 _initPos;
    private float _progression;
    private SpriteRenderer _spriteRenderer;
    private GameObject _trailParticle;

    // Properties
    public float ChangeAmount { get; private set; }
    public float Multiplier { get; private set; }
    public MathOperators Operator { get; private set; }
    public ItemStatue Statue { get; private set; }
    public ControlPanel.ItemClass ItemClass { get; private set; }

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        foreach (var monster in FindObjectsOfType<Monster>())
            if (monster.allied)
                _alliedMonster = monster;

        _initPos = transform.position;
    }

    private void Update()
    {
        if (_throwItem)
        {
            _progression += Time.deltaTime * ControlPanel.Instance.itemParticleLerpSpeed / 10f;
            transform.position = Vector3.Lerp(_initPos, _alliedMonster.transform.position, _progression);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(GameManager.PlayerLayer))
        {
            if (_trailParticle)
            {
                var trailParticle = Instantiate(_trailParticle, transform.position, transform.rotation, transform).GetComponent<ParticleSystem>();
                var trailMain = trailParticle.main;
                trailMain.startColor = ItemClass.trailParticleColor;
            }

            _throwItem = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer(GameManager.MonsterLayer))
        {
            OnTrigger();
            Destroy(gameObject);
        }
    }

    public void OnTrigger()
    {
        _alliedMonster.PowerUpAffection(this);
    }

    public void SetupItem(ItemSpawnPoint itemSpawn, ControlPanel.ItemClass itemClass)
    {
        amountText.text = itemSpawn.GetOperatorText();
        ChangeAmount = itemClass.changeAmount;
        Multiplier = itemClass.multiplyAmount;
        Operator = itemClass.amountConversion;
        Statue = itemClass.statue;

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (Statue == ItemStatue.Negative)
        {
            _spriteRenderer.color = ControlPanel.Instance.negativeItemColor;
        }
        else
        {
            _spriteRenderer.color = ControlPanel.Instance.positiveItemColor;
        }

        _trailParticle = ControlPanel.Instance.trailParticle;
        _spriteRenderer.gameObject.SetActive(itemSpawn.amountTextVisibility == Visibility.Show ? true : false);

        ItemClass = itemClass;
    }
}
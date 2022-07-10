using System.Linq;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public ItemType itemType;
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
                Instantiate(_trailParticle, transform.position, transform.rotation, transform);
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
        switch (itemType)
        {
            case ItemType.Power:
                _alliedMonster.PowerAffection(this);
                break;
            case ItemType.AttackSpeed:
                _alliedMonster.AttackSpeedAffection(this);
                break;
            case ItemType.Health:
                _alliedMonster.ChangeHealth(this);
                break;
            case ItemType.Shield:
                _alliedMonster.ChangeShield(this);
                break;
            case ItemType.Poop:
                _alliedMonster.PoopAffection(this);
                break;
            default:
                break;
        }
    }

    public void SetupItem(ItemSpawnPoint itemSpawn, ControlPanel.ItemClass itemClass)
    {
        amountText.text = itemSpawn.GetOperatorText();
        ChangeAmount = itemSpawn.changeAmount;
        Multiplier = itemSpawn.multiplyAmount;
        Operator = itemSpawn.amountConversion;
        Statue = itemSpawn.statue;

        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (Statue == ItemStatue.Negative)
        {
            _spriteRenderer.color = ControlPanel.Instance.negativeItemColor;
            _trailParticle = ControlPanel.Instance.negativeItemParticle;
        }
        else
        {
            _spriteRenderer.color = ControlPanel.Instance.positiveItemColor;
            _trailParticle = ControlPanel.Instance.positiveItemParticle;
        }

        _spriteRenderer.gameObject.SetActive(itemSpawn.amountTextVisibility == Visibility.Show ? true : false);

        ItemClass = itemClass;
    }

    public GameObject GetVFX()
    {
        return ItemClass.particleVFX;
    }

    public Color GetOutlineColor()
    {
        if (Statue == ItemStatue.Positive)
            return ItemClass.positiveOutline;

        if (Statue == ItemStatue.Negative)
            return ItemClass.negativeOutline;

        return Color.white;
    }
}
using System.Linq;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public ItemType itemType;
    private float itemChangeAmount;
    [Space(2)]
    public TextMeshPro amountText;

    internal Monster alliedMonster;
    private bool throwItem = false;
    private Vector3 initPos;
    private float progression;
    private SpriteRenderer spriteRenderer;
    private GameObject trailParticle;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (var monster in FindObjectsOfType<Monster>())
            if (monster.allied)
                alliedMonster = monster;

        initPos = transform.position;
    }

    private void Update()
    {
        if (throwItem)
        {
            progression += Time.deltaTime * ControlPanel.Instance.itemParticleLerpSpeed / 10f;
            transform.position = Vector3.Lerp(initPos, alliedMonster.transform.position, progression);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(GameManager.PlayerLayer))
        {
            if (trailParticle)
            {
                Instantiate(trailParticle, transform.position, transform.rotation, transform);
            }

            throwItem = true;
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
                alliedMonster.ChangePower(itemChangeAmount);
                break;
            case ItemType.AttackSpeed:
                alliedMonster.ChangeAttackSpeed(itemChangeAmount, ControlPanel.Instance.items.SingleOrDefault(x => x.type == itemType));
                break;
            case ItemType.DNA:
                alliedMonster.ChangeDNA();
                break;
            case ItemType.Health:
                alliedMonster.ChangeHealth(itemChangeAmount, ControlPanel.Instance.items.SingleOrDefault(x => x.type == itemType));
                break;
            case ItemType.Shield:
                alliedMonster.ChangeShield(itemChangeAmount);
                break;
            default:
                break;
        }
    }

    public void SetupItem(string amountText, float addAmount, bool isNegative)
    {
        this.amountText.text = amountText;
        itemChangeAmount = addAmount;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (isNegative)
        {
            spriteRenderer.color = ControlPanel.Instance.negativeItemColor;
            trailParticle = ControlPanel.Instance.negativeItemParticle;
        }
        else
        {
            spriteRenderer.color = ControlPanel.Instance.positiveItemColor;
            trailParticle = ControlPanel.Instance.positiveItemParticle;
        }
    }
}
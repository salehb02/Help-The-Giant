using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject trailParticle;
    public float trailLerpSpeed = 5f;
    [Space(2)]
    public ItemType itemType;
    public float itemChangeAmount;

    internal Monster alliedMonster;
    private bool throwItem = false;
    private Vector3 initPos;
    private float progression;


    private void Start()
    {
        foreach(var monster in FindObjectsOfType<Monster>())
            if(monster.allied)
                alliedMonster = monster;

        initPos = transform.position;
    }

    private void Update()
    {
        if (throwItem)
        {
            progression += Time.deltaTime * trailLerpSpeed / 10f;
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
                alliedMonster.ChangeAttackSpeed(itemChangeAmount);
                break;
            case ItemType.DNA:
                alliedMonster.ChangeDNA();
                break;
            case ItemType.Health:
                alliedMonster.ChangeHealth(itemChangeAmount);
                break;
            case ItemType.Shield:
                alliedMonster.ChangeShield(itemChangeAmount);
                break;
            default:
                break;
        }
    }
}
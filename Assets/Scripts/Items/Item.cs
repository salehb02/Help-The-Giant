using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject trailParticle;
    public float trailLerpSpeed = 5f;

    internal AlliedMonster alliedMonster;
    private bool throwItem = false;
    private Vector3 initPos;
    private float progression;


    private void Start()
    {
        alliedMonster = FindObjectOfType<AlliedMonster>();
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

    public virtual void OnTrigger() { }
}
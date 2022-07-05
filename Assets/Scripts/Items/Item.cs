using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject trailParticle;
    public float trailLerpSpeed = 5f;

    internal AlliedMonster alliedMonster;
    private GameObject currentTrail;

    private void Start()
    {
        alliedMonster = FindObjectOfType<AlliedMonster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger();

        if (trailParticle)
        {
            currentTrail = Instantiate(trailParticle, transform.position, transform.rotation, null);
            var trail = currentTrail.AddComponent<ItemTrailParticle>();
            trail.SetSpeed(trailLerpSpeed);
            trail.SetTarget(alliedMonster.transform);
        }

        Destroy(gameObject);
    }

    public virtual void OnTrigger() { }
}
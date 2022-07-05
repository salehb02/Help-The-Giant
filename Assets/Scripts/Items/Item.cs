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

    private void Update()
    {
        if (currentTrail)
            currentTrail.transform.position = Vector3.Lerp(transform.position, alliedMonster.transform.position, Time.deltaTime * trailLerpSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger();

        if (trailParticle)
            currentTrail = Instantiate(trailParticle, transform.position, transform.rotation, null);

        gameObject.SetActive(false);
    }

    public virtual void OnTrigger() { }
}
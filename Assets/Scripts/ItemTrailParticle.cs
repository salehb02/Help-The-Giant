using UnityEngine;

public class ItemTrailParticle : MonoBehaviour
{
    private Transform target;
    private float speed;
    private Vector3 initPos;
    private float progression;

    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        if (!target)
            return;

        progression += Time.deltaTime * speed / 10f; 
        transform.position = Vector3.Lerp(initPos, target.position, progression);
    }

    public void SetSpeed(float speed) => this.speed = speed;

    public void SetTarget(Transform target) => this.target = target;
}
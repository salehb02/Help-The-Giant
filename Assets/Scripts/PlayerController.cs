using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalDragSpeed = 1f;
    public float maxHorizontalMovement = 1.2f;
    private float currentHorizontal;

    private Animator animator;
    private Vector3 previousPos;
    private float velocity;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        transform.transform.localPosition = new Vector3(Mathf.Lerp(transform.transform.localPosition.x, currentHorizontal, Time.deltaTime * 7f), transform.localPosition.y, transform.localPosition.z);

        velocity = ((transform.position - previousPos).magnitude) / Time.deltaTime;
        previousPos = transform.position;

        animator.SetFloat("Speed", velocity);
    }

    public void HorizontalMovement(float delta)
    {
        currentHorizontal += delta * horizontalDragSpeed;
        currentHorizontal = Mathf.Clamp(currentHorizontal, -maxHorizontalMovement, maxHorizontalMovement);
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalDragSpeed = 1f;
    public float maxHorizontalMovement = 1.2f;
    public LayerMask groundLayerMask;
    private float currentHorizontal;

    private Animator animator;
    private Vector3 previousPos;
    private float velocity;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Find ground position to stick to it
        var groundPos = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity, groundLayerMask))
            groundPos = hit.point;

        transform.localPosition = new Vector3(Mathf.Lerp(transform.transform.localPosition.x, currentHorizontal, Time.deltaTime * 7f), transform.localPosition.y, transform.localPosition.z);
        transform.position = new Vector3(transform.position.x, groundPos.y + 0.1f, transform.position.z);

        velocity = ((transform.position - previousPos).magnitude) / Time.deltaTime;
        previousPos = transform.position;

        animator.SetFloat("Speed", velocity);
    }

    public void HorizontalMovement(float delta)
    {
        if (gameManager.IsPaused)
            return;

        currentHorizontal += delta * horizontalDragSpeed;
        currentHorizontal = Mathf.Clamp(currentHorizontal, -maxHorizontalMovement, maxHorizontalMovement);
    }
}
using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    public GameObject startPosition;
    public GameObject endPosition;

    public float speed = 1f;

    private float currentProgression;
    private float distance;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        transform.position = startPosition.transform.position;
        distance = Vector3.Distance(startPosition.transform.position, endPosition.transform.position);
    }

    private void Update()
    {
        if (!gameManager.IsPaused)
            currentProgression += Time.deltaTime * speed;

        transform.position = Vector3.Lerp(startPosition.transform.position, endPosition.transform.position, currentProgression / distance);
        transform.LookAt(endPosition.transform);
    }
}
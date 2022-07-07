using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private bool sinusMovement;
    [SerializeField] private float sinusMovementSpeed;
    [SerializeField] private float sinusMovementMultiplier = 1f;

    private Vector3 _initPos;

    private void Start()
    {
        _initPos = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed);

        SinusMovement();
    }

    private void SinusMovement()
    {
        if (!sinusMovement)
            return;

        transform.localPosition = _initPos + (Vector3.up * Mathf.Sin(sinusMovementSpeed * Time.time) * sinusMovementMultiplier);
    }
}
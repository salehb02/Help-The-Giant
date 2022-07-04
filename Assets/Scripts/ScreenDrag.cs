using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenDrag : MonoBehaviour, IDragHandler
{
    private PlayerController controller;

    private void Update()
    {
        if (controller)
            return;

        controller = FindObjectOfType<PlayerController>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (controller == null)
            return;

        controller.HorizontalMovement(eventData.delta.x * Time.deltaTime);
    }
}
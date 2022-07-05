using System.Linq;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] ItemStatue statue;

    private void Start()
    {
        var currentItem = ControlPanel.Instance.items.SingleOrDefault(x => x.type == itemType);

        if (currentItem != null)
        {
            switch (statue)
            {
                case ItemStatue.Positive:
                    Instantiate(currentItem.positivePrefab, transform.position, transform.rotation, transform);
                    break;
                case ItemStatue.Negative:
                    Instantiate(currentItem.negativePrefab, transform.position, transform.rotation, transform);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
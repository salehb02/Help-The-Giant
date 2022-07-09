using System.Linq;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] ItemStatue statue;
    [SerializeField] ItemAmount amount;
    [SerializeField] float changeAmount = 1;

    private void Start()
    {
        InstantiateItem();
    }

    private void InstantiateItem()
    {
        var currentItem = ControlPanel.Instance.items.SingleOrDefault(x => x.type == itemType);

        if (currentItem != null)
        {
            var item = Instantiate(currentItem.prefab, transform.position, transform.rotation, transform);
            item.SetupItem(GetEnumText(amount), changeAmount, statue == ItemStatue.Negative ? true : false);
        }
    }

    private string GetEnumText(ItemAmount itemAmount) => itemAmount switch
    {
        ItemAmount.x1 => "X1",
        ItemAmount.x2 => "X2",
        ItemAmount.x3 => "X3",
        ItemAmount.Add1 => "+1",
        ItemAmount.Add2 => "+2",
        ItemAmount.Add3 => "+3",
        _ => throw new System.NotImplementedException(),
    };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
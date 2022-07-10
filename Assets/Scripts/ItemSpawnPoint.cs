using System.Linq;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] ItemStatue statue;
    [SerializeField] Visibility amountTextVisibility;
    [SerializeField] AmountConversion amountConversion;
    [SerializeField] float multiplyAmount;
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
            item.SetupItem(GetEnumText(amountConversion) + multiplyAmount, amountConversion, changeAmount, multiplyAmount, statue == ItemStatue.Negative ? true : false, amountTextVisibility == Visibility.Show ? true : false);
        }
    }

    private string GetEnumText(AmountConversion itemAmount) => itemAmount switch
    {
        AmountConversion.Add => "+",
        AmountConversion.Subtract => "-",
        AmountConversion.Multiply => "X",
        AmountConversion.Divide => "/",
        _ => throw new System.NotImplementedException(),
    };

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
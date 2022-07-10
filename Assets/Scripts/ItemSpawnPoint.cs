using System.Linq;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    public ItemType itemType;
    public ItemStatue statue;
    public Visibility amountTextVisibility;
    public MathOperators amountConversion;
    public float multiplyAmount;
    public float changeAmount = 1;

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
            item.SetupItem(this, currentItem);
        }
    }

    public string GetOperatorText()
    {
        string result = null;

        switch (amountConversion)
        {
            case MathOperators.Add:
                result += "+";
                break;
            case MathOperators.Subtract:
                result += "-";
                break;
            case MathOperators.Multiply:
                result += "x";
                break;
            case MathOperators.Divide:
                result += "/";
                break;
            default:
                break;
        }

        result += multiplyAmount;

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
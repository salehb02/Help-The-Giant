using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour,ISerializationCallbackReceiver
{
    public static List<string> Items;
    [ListToPopup(typeof(ItemSpawnPoint), "Items")]
    public string itemTitle;
    public Visibility amountTextVisibility;

    private ControlPanel.ItemClass item;

    private void Start()
    {
        InstantiateItem();
    }

    private void InstantiateItem()
    {
        item = ControlPanel.Instance.items.SingleOrDefault(x => x.title == itemTitle);

        if (item != null)
        {
            var instancedItem = Instantiate(item.prefab, transform.position, transform.rotation, transform);
            instancedItem.SetupItem(this, item);
        }
    }

    public string GetOperatorText()
    {
        string result = null;

        switch (item.amountConversion)
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

        result += item.multiplyAmount;

        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }

    private List<string> GetAllItemTitles()
    {
        var titles = new List<string>();

        foreach (var item in ControlPanel.Instance.items)
            titles.Add(item.title);

        return titles;
    }

    public void OnBeforeSerialize()
    {
        Items = GetAllItemTitles();
    }

    public void OnAfterDeserialize()
    {
    }
}
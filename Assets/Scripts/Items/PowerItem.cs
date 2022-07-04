using UnityEngine;

public class PowerItem : Item
{
    [Space(2)]
    [Header("Specific Settings")]
    public float powerAmount = 1;

    public override void OnTrigger()
    {
        base.OnTrigger();
        alliedMonster.ChangePower(powerAmount);
    }
}
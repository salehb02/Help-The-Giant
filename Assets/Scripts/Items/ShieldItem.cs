using UnityEngine;

public class ShieldItem : Item
{
    [Space(2)]
    [Header("Specific Settings")]
    [Range(-3, 3)] public float shieldAmount = 1;

    public override void OnTrigger()
    {
        base.OnTrigger();
        alliedMonster.ChangeShield(shieldAmount);
    }
}
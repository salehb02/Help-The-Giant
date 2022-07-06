using UnityEngine;

public class AttackSpeedItem : Item
{
    [Space(2)]
    [Header("Specific Settings")]
    [Range(-3, 3)] public float attackSpeedAmount = 1;

    public override void OnTrigger()
    {
        base.OnTrigger();
        alliedMonster.ChangeAttackSpeed(attackSpeedAmount);
    }
}
using UnityEngine;

public class HealthItem : Item
{
    [Space(2)]
    [Header("Specific Settings")]
    public float healthAmount = 1;

    public override void OnTrigger()
    {
        base.OnTrigger();
        alliedMonster.ChangeHealth(healthAmount);
    }
}
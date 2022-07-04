using UnityEngine;

public class DNAItem : Item
{
    [Space(2)]
    [Header("Specific Settings")]
    public float attackSpeedAmount = 1;

    public override void OnTrigger()
    {
        base.OnTrigger();
        alliedMonster.ChangeDNA();
    }
}
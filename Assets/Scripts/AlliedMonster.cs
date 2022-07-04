using UnityEngine;

public class AlliedMonster : MonoBehaviour
{
    public float Health { get; private set; }
    public float Power { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Shield { get; private set; }

    public void ChangeHealth(float amount)
    {
        Health += amount;
    }

    public void ChangePower(float amount)
    {
        Power += amount;
    }

    public void ChangeAttackSpeed(float amount)
    {
        AttackSpeed += amount;
    }

    public void ChangeShield(float amount)
    {
        Shield += amount;
        Shield = Mathf.Clamp(Shield, 1, 3);
    }

    public void ChangeDNA()
    {

    }
}
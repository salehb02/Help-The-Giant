using UnityEngine;

public class Monster : MonoBehaviour
{
    public float maxHealth = 100;
    public float power = 10f;
    public float attackSpeed = 0.5f;
    public float shield = 0f;

    public float Health { get; private set; }
    public float Power { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Shield { get; private set; }

    private bool dead;

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        InitHealth();
        InitPower();
        InitAttackSpeed();
        InitShield();
    }

    public void Damage(float amount)
    {
        Health -= amount;

        if (Health <= 0)
            Die();
    }

    public virtual void Die()
    {
        dead = true;
    }

    public bool IsDead() => dead;

    private void InitHealth()
    {
        Health = maxHealth;
    }

    private void InitPower()
    {
        Power = power;
    }

    private void InitAttackSpeed()
    {
        AttackSpeed = attackSpeed;
    }

    private void InitShield()
    {
        Shield = shield;
    }

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
using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 health = new Vector3(0, 100, 100);
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 power = new Vector3(1, 20, 10);
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 attackSpeed = new Vector3(0.5f, 3, 1);
    public float baseAttackDelay = 0.5f;
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 shield = new Vector3(0, 5, 0);

    public float Health { get; private set; }
    public float Power { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Shield { get; private set; }

    public event Action OnDamage;

    private bool dead;

    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";
    public const string ATTACK_SPEED = "AttackSpeed";

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

        StartCoroutine(CheckAttackCoroutine());
    }

    private IEnumerator CheckAttackCoroutine()
    {
        while(IsDead() == false)
        {
            Attack();

            yield return new WaitForSeconds(baseAttackDelay / AttackSpeed);
        }
    }

    public virtual void Attack()
    {

    }

    public void Damage(float amount)
    {
        Health -= amount;

        if (Health <= 0)
            Die();

        OnDamage?.Invoke();
    }

    public virtual void Die()
    {
        if (dead)
            return;

        dead = true;
    }

    public bool IsDead() => dead;

    private void InitHealth()
    {
        Health = health.z;
    }

    private void InitPower()
    {
        Power = power.z;
    }

    private void InitAttackSpeed()
    {
        AttackSpeed = attackSpeed.z;
    }

    private void InitShield()
    {
        Shield = shield.z;
    }

    public void ChangeHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, health.x, health.y);
    }

    public void ChangePower(float amount)
    {
        Power += amount;
        Power = Mathf.Clamp(Power, power.x, power.y);
    }

    public void ChangeAttackSpeed(float amount)
    {
        AttackSpeed += amount;
        AttackSpeed = Mathf.Clamp(AttackSpeed, attackSpeed.x, attackSpeed.y);
    }

    public void ChangeShield(float amount)
    {
        Shield += amount;
        Shield = Mathf.Clamp(Shield, shield.x, shield.y);
    }

    public void ChangeDNA()
    {

    }
}
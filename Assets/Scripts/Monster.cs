using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public bool allied;

    [VectorLabels("Min", "Max", "Default")]
    public Vector3 health = new Vector3(0, 100, 100);
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 power = new Vector3(1, 20, 10);
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 attackSpeed = new Vector3(0.5f, 3, 1);
    public float baseAttackDelay = 0.5f;
    [VectorLabels("Min", "Max", "Default")]
    public Vector3 shield = new Vector3(0, 5, 0);

    [Header("UI")]
    public Slider healthBar;

    public float Health { get; private set; }
    public float Power { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Shield { get; private set; }

    private bool dead;

    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";
    public const string ATTACK_SPEED = "AttackSpeed";

    private Animator animator;
    private Monster enemyMonster;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        animator = GetComponentInChildren<Animator>();

        foreach (var monster in FindObjectsOfType<Monster>())
            if (monster != this)
                enemyMonster = monster;

        InitHealth();
        InitPower();
        InitAttackSpeed();
        InitShield();

        StartCoroutine(CheckAttackCoroutine());
    }

    private IEnumerator CheckAttackCoroutine()
    {
        while (IsDead() == false)
        {
            yield return new WaitForSeconds(baseAttackDelay / AttackSpeed);
            Attack();
        }
    }

    public void Attack()
    {
        if (enemyMonster && enemyMonster.IsDead())
            return;

        animator?.SetFloat(ATTACK_SPEED, AttackSpeed);
        animator?.SetTrigger(ATTACK_TRIGGER);

        if (enemyMonster)
            enemyMonster.Damage(Power);
    }

    public void Damage(float amount)
    {
        Health -= amount;
        UpdateUI();

        if (Health <= 0)
            Die();
    }

    public void Die()
    {
        if (dead)
            return;

        dead = true;
        animator?.SetTrigger(DEAD_TRIGGER);
        healthBar.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        healthBar.value = Health;
    }

    public bool IsDead() => dead;

    private void InitHealth()
    {
        Health = health.z;

        healthBar.minValue = health.x;
        healthBar.maxValue = health.y;
        UpdateUI();
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
        UpdateUI();
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
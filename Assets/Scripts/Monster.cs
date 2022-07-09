using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EPOOutline;
using DG.Tweening;

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
    private Vector3 initScale;
    private Coroutine revertAttackSpeedCoroutine;

    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";
    public const string ATTACK_SPEED = "AttackSpeed";

    private Animator animator;
    private Monster enemyMonster;
    private Outlinable outline;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        animator = GetComponentInChildren<Animator>();
        outline = GetComponentInChildren<Outlinable>();

        initScale = animator.transform.localScale;

        foreach (var monster in FindObjectsOfType<Monster>())
            if (monster != this)
                enemyMonster = monster;

        InitHealth();
        InitPower();
        InitAttackSpeed();
        InitShield();

        StartCoroutine(CheckAttackCoroutine());

        if(allied)
        {
            outline.OutlineParameters.DOFade(0, 0);
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);
        }
    }

    private IEnumerator CheckAttackCoroutine()
    {
        // Warm up
        yield return new WaitForSeconds(0.5f);

        while (IsDead() == false)
        {
            Attack();

            yield return new WaitForSeconds(baseAttackDelay / AttackSpeed);
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
        Health -= amount / Mathf.Clamp(Shield, 1, Mathf.Infinity);
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

    public void ChangeHealth(float amount,AmountCoversion coversion,ControlPanel.ItemClass itemData)
    {
        Health = GetChangeAmount(Health, amount, coversion);
        Health = Mathf.Clamp(Health, health.x, health.y);
        UpdateUI();

        if (Health <= 0)
            Die();

        if (allied)
        {
            var time = 0.2f;
            var color = Color.white;

            if (amount > 0)
            {
                color = itemData.positiveOutline;
            }
            else
            {
                color = itemData.negativeOutline;
            }

            outline.OutlineParameters.Color = color;
            outline.OutlineParameters.DOFade(0, 0);
            outline.OutlineParameters.FillPass.SetColor("_PublicColor", color);
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);

            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0.1f, time).OnComplete(() =>
            {
                outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, time);
            });

            outline.OutlineParameters.DOFade(1, time).OnComplete(() =>
            {
                outline.OutlineParameters.DOFade(0, time);
            });

            if (itemData.particleVFX)
            {
                Instantiate(itemData.particleVFX, animator.transform.position, animator.transform.rotation * Quaternion.Euler(-90,0,0), animator.transform);
            }
        }
    }

    public void ChangePower(AmountCoversion amountCoversion,float amount)
    {
        Power = GetChangeAmount(Power, amount,amountCoversion);
        Power = Mathf.Clamp(Power, power.x, power.y);
    }

    public void ChangeAttackSpeed(float amount, ControlPanel.ItemClass itemData)
    {
        AttackSpeed = amount;
        AttackSpeed = Mathf.Clamp(AttackSpeed, attackSpeed.x, attackSpeed.y);

        if (revertAttackSpeedCoroutine != null)
        {
            StopCoroutine(revertAttackSpeedCoroutine);
            revertAttackSpeedCoroutine = null;
        }

        var time = 0.2f;

        if (amount > 1)
            animator.transform.DOScale(initScale + Vector3.one * 3f, time).OnComplete(() => revertAttackSpeedCoroutine = StartCoroutine(BackToNormalFromAttackSpeed()));
        else
            animator.transform.DOScale(initScale - Vector3.one * 3f, time).OnComplete(() => revertAttackSpeedCoroutine = StartCoroutine(BackToNormalFromAttackSpeed()));

        if (allied)
        {
            var color = Color.white;

            if (amount > 1)
            {
                color = itemData.positiveOutline;
            }
            else
            {
                color = itemData.negativeOutline;
            }

            outline.OutlineParameters.Color = color;
            outline.OutlineParameters.DOFade(0, 0);
            outline.OutlineParameters.FillPass.SetColor("_PublicColor", color);
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);

            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0.1f, time);

            outline.OutlineParameters.DOFade(1, time);

            if (itemData.particleVFX)
            {
                Instantiate(itemData.particleVFX, animator.transform.position, animator.transform.rotation * Quaternion.Euler(-90, 0, 0), animator.transform);
            }
        }
    }

    private IEnumerator BackToNormalFromAttackSpeed()
    {
        yield return new WaitForSeconds(1f);

        var time = 0.2f;
        animator.transform.DOScale(initScale, time);
        AttackSpeed = attackSpeed.z;
        outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, time);
        outline.OutlineParameters.DOFade(0, time);

        revertAttackSpeedCoroutine = null;
    }

    public void ChangeShield(float amount)
    {
        Shield += amount;
        Shield = Mathf.Clamp(Shield, shield.x, shield.y);
    }

    public void ChangeDNA()
    {

    }

    private float GetChangeAmount(float currentValue, float changeAmount, AmountCoversion coversion)
    {
        var newAmount = currentValue;

        switch (coversion)
        {
            case AmountCoversion.Add:
                newAmount += changeAmount;
                break;
            case AmountCoversion.Subtract:
                newAmount -= changeAmount;
                break;
            case AmountCoversion.Multiply:
                newAmount *= changeAmount;
                break;
            case AmountCoversion.Divide:
                newAmount /= changeAmount;
                break;
            default:
                break;
        }

        return newAmount;
    }
}
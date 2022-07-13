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

    [Header("Pivots")]
    public GameObject leftHandParticlePivot;
    public GameObject rightHandParticlePivot;
    public GameObject headParticlePivot;
    public GameObject bodyParticlePivot;

    public float Health { get; private set; }
    public float Power { get; private set; }
    public float AttackSpeed { get; private set; }
    public float Shield { get; private set; }
    public bool IsDead { get => dead; }
    public bool IsConfused { get; private set; }

    private bool dead;
    private float _currentAttackMode = 0;
    private Vector3 initScale;
    private Coroutine revertAttackSpeedCoroutine;
    private Coroutine revertPowerCoroutine;
    private Coroutine revertPoopCoroutine;

    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";
    public const string ATTACK_SPEED = "AttackSpeed";
    public const string ATTACK_MODE = "AttackMode";
    public const string CONFUSED_TRIGGER = "Confuse";

    private Animator animator;
    private Monster enemyMonster;
    private Outlinable outline;
    private GameManager gameManager;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        animator?.SetFloat(ATTACK_SPEED, AttackSpeed);
        animator?.SetFloat(ATTACK_MODE, _currentAttackMode);
    }

    public void Init()
    {
        animator = GetComponentInChildren<Animator>();
        outline = GetComponentInChildren<Outlinable>();
        gameManager = FindObjectOfType<GameManager>();

        initScale = animator.transform.localScale;

        foreach (var monster in FindObjectsOfType<Monster>())
            if (monster != this)
                enemyMonster = monster;

        InitHealth();
        InitPower();
        InitAttackSpeed();
        InitShield();

        StartCoroutine(CheckAttackCoroutine());

        if (allied)
        {
            outline.OutlineParameters.DOFade(0, 0);
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);
        }
    }

    private IEnumerator CheckAttackCoroutine()
    {
        // Warm up
        yield return new WaitForSeconds(allied ? 0.8f : 0.5f);

        while (IsDead == false)
        {
            if (!gameManager.IsPaused)
                Attack();

            yield return new WaitForSeconds(baseAttackDelay / AttackSpeed);
        }
    }

    public void Attack()
    {
        if (enemyMonster && enemyMonster.IsDead)
            return;

        if (IsConfused)
            return;

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
     
        if (allied)
            gameManager.LoseGame();
        else
            gameManager.WinGame();
    }

    private void UpdateUI()
    {
        healthBar.value = Health;
    }

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

    // OnHealthPickup
    public void ChangeHealth(Item item)
    {
        Health = GetChangeAmount(Health, item);
        Health = Mathf.Clamp(Health, health.x, health.y);
        UpdateUI();

        if (Health <= 0)
            Die();

        if (!allied)
            return;

        var time = 0.15f;
        var color = item.GetOutlineColor();

        outline.OutlineParameters.Color = color;
        outline.OutlineParameters.DOFade(0, 0);
        outline.OutlineParameters.FillPass.SetColor("_PublicColor", color);
        outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);

        outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0.25f, time).OnComplete(() =>
        {
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, time);
        });

        outline.OutlineParameters.DOFade(1, time).OnComplete(() =>
        {
            outline.OutlineParameters.DOFade(0, time);
        });

        animator.transform.DOShakeScale(time, 3.5f, 15);

        var vfx = item.GetVFX();

        if (vfx && bodyParticlePivot.transform.childCount < 3)
            Instantiate(vfx, bodyParticlePivot.transform.position, bodyParticlePivot.transform.rotation, bodyParticlePivot.transform);
    }

    // OnPowerPickup
    public void PowerAffection(Item item)
    {
        if (revertPowerCoroutine != null)
        {
            StopCoroutine(revertPowerCoroutine);
            revertPowerCoroutine = null;
        }
        else
        {
            Power = GetChangeAmount(Power, item);
            Power = Mathf.Clamp(Power, power.x, power.y);
            _currentAttackMode = 1;
        }

        var vfx = item.GetVFX();

        if (rightHandParticlePivot.transform.childCount == 0 && vfx)
            Instantiate(vfx, rightHandParticlePivot.transform.position, rightHandParticlePivot.transform.rotation, rightHandParticlePivot.transform);

        if (leftHandParticlePivot.transform.childCount == 0 && vfx)
            Instantiate(vfx, leftHandParticlePivot.transform.position, leftHandParticlePivot.transform.rotation, leftHandParticlePivot.transform);

        revertPowerCoroutine = StartCoroutine(RevertPowerCoroutine());
    }

    private IEnumerator RevertPowerCoroutine()
    {
        yield return new WaitForSeconds(1f);

        foreach (Transform obj in rightHandParticlePivot.transform)
            Destroy(obj.gameObject);

        foreach (Transform obj in leftHandParticlePivot.transform)
            Destroy(obj.gameObject);

        Power = power.z;
        _currentAttackMode = 0;
        revertPowerCoroutine = null;
    }

    // OnAttackSpeedPickup
    public void AttackSpeedAffection(Item item)
    {
        if (revertAttackSpeedCoroutine != null)
        {
            StopCoroutine(revertAttackSpeedCoroutine);
            revertAttackSpeedCoroutine = null;
        }
        else
        {
            AttackSpeed = GetChangeAmount(AttackSpeed, item);
            AttackSpeed = Mathf.Clamp(AttackSpeed, attackSpeed.x, attackSpeed.y);
        }

        var time = 0.15f;

        if (AttackSpeed >= 1)
            animator.transform.DOScale(initScale + Vector3.one * 3f, time).OnComplete(() => revertAttackSpeedCoroutine = StartCoroutine(RevertAttackSpeedCoroutine()));
        else
            animator.transform.DOScale(initScale - Vector3.one * 3f, time).OnComplete(() => revertAttackSpeedCoroutine = StartCoroutine(RevertAttackSpeedCoroutine()));

        if (!allied)
            return;

        var color = item.GetOutlineColor();

        outline.OutlineParameters.Color = color;
        outline.OutlineParameters.DOFade(0, 0);
        outline.OutlineParameters.FillPass.SetColor("_PublicColor", color);
        outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);

        outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0.25f, time).OnComplete(() =>
        {
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, time);
        });

        outline.OutlineParameters.DOFade(1, time).OnComplete(() =>
        {
            outline.OutlineParameters.DOFade(0, time);
        });

        var vfx = item.GetVFX();

        if (vfx && bodyParticlePivot.transform.childCount < 3)
            Instantiate(vfx, bodyParticlePivot.transform.position, bodyParticlePivot.transform.rotation, bodyParticlePivot.transform);
    }

    private IEnumerator RevertAttackSpeedCoroutine()
    {
        yield return new WaitForSeconds(1f);

        var time = 0.2f;
        animator.transform.DOScale(initScale, time);
        //outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, time);
        //outline.OutlineParameters.DOFade(0, time);
        AttackSpeed = attackSpeed.z;

        revertAttackSpeedCoroutine = null;
    }

    // OnPoopPickup
    public void PoopAffection(Item item)
    {
        if (revertPoopCoroutine != null)
        {
            StopCoroutine(revertPoopCoroutine);
            revertPoopCoroutine = null;
        }
        else
        {
            IsConfused = true;
            animator.SetTrigger(CONFUSED_TRIGGER);
        }

        if (headParticlePivot.transform.childCount == 0)
            Instantiate(item.GetVFX(), headParticlePivot.transform.position, headParticlePivot.transform.rotation, headParticlePivot.transform);

        revertPoopCoroutine = StartCoroutine(RevertPoopCoroutine());
    }

    private IEnumerator RevertPoopCoroutine()
    {
        yield return new WaitForSeconds(1f);

        IsConfused = false;

        foreach (Transform particle in headParticlePivot.transform)
            Destroy(particle.gameObject);

        revertPoopCoroutine = null;
    }

    // OnShieldPickup
    public void ChangeShield(Item item)
    {
        Shield += item.ChangeAmount;
        Shield = Mathf.Clamp(Shield, shield.x, shield.y);
    }

    private float GetChangeAmount(float currentValue, Item item)
    {
        var newAmount = currentValue;

        switch (item.Operator)
        {
            case MathOperators.Add:
                newAmount += item.Multiplier * item.ChangeAmount;
                break;
            case MathOperators.Subtract:
                newAmount -= item.Multiplier * item.ChangeAmount;
                break;
            case MathOperators.Multiply:
                newAmount *= item.Multiplier - item.ChangeAmount;
                break;
            case MathOperators.Divide:
                newAmount /= item.Multiplier + item.ChangeAmount;
                break;
            default:
                break;
        }

        return newAmount;
    }
}
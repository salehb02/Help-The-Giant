using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EPOOutline;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

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

    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";
    public const string ATTACK_SPEED = "AttackSpeed";
    public const string ATTACK_MODE = "AttackMode";
    public const string CONFUSED_TRIGGER = "Confuse";

    private Animator animator;
    private Monster enemyMonster;
    private Outlinable outline;
    private GameManager gameManager;

    private List<PowerUpCoroutine> coroutines = new List<PowerUpCoroutine>();

    [System.Serializable]
    public class PowerUpCoroutine
    {
        public ControlPanel.ItemClass itemClass;
        public Coroutine coroutine;

        public PowerUpCoroutine(ControlPanel.ItemClass itemClass, Coroutine coroutine)
        {
            this.itemClass = itemClass;
            this.coroutine = coroutine;
        }
    }

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

        InitMonster();

        StartCoroutine(CheckAttackCoroutine());

        if (allied)
        {
            outline.OutlineParameters.DOFade(0, 0);
            outline.OutlineParameters.FillPass.DOFade("_PublicColor", 0, 0);
        }
    }

    private void InitMonster()
    {
        Health = health.z;

        healthBar.minValue = health.x;
        healthBar.maxValue = health.y;
        UpdateUI(); Power = power.z;

        AttackSpeed = attackSpeed.z;
        Shield = shield.z;
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

    // OnPowerupPiuckup
    public void PowerUpAffection(Item item)
    {
        var similarCoroutine = coroutines.SingleOrDefault(x => x.itemClass.GetHashCode() == item.ItemClass.GetHashCode());

        if (similarCoroutine != null)
        {
            StopCoroutine(similarCoroutine.coroutine);
            coroutines.Remove(similarCoroutine);
        }

        switch (item.ItemClass.type)
        {
            case ItemType.Power:
                Power = GetChangeAmount(Power, item);
                Power = Mathf.Clamp(Power, power.x, power.y);
                _currentAttackMode = 1;
                break;
            case ItemType.AttackSpeed:
                AttackSpeed = GetChangeAmount(AttackSpeed, item);
                AttackSpeed = Mathf.Clamp(AttackSpeed, attackSpeed.x, attackSpeed.y);
                break;
            case ItemType.Health:
                Health = GetChangeAmount(Health, item);
                Health = Mathf.Clamp(Health, health.x, health.y);
                UpdateUI();

                if (Health <= 0)
                    Die();
                break;
            case ItemType.Shield:
                Shield += item.ChangeAmount;
                Shield = Mathf.Clamp(Shield, shield.x, shield.y);
                break;
            case ItemType.Confuser:
                IsConfused = true;
                animator.SetTrigger(CONFUSED_TRIGGER);
                break;
            default:
                break;
        }

        var time = 0.15f;

        if (item.ItemClass.changeSize)
            animator.transform.DOScale(initScale + Vector3.one * item.ItemClass.changeSizeAmount, time);

        if (item.ItemClass.shakeOnAffection)
            animator.transform.DOShakeScale(time, item.ItemClass.shakeStrength, item.ItemClass.shakeVibrato);

        if (!allied)
            return;

        if (item.ItemClass.changeOutlineColor)
        {
            var color = item.ItemClass.positiveOutline;

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
        }

        if (item.ItemClass.headVFX && headParticlePivot.transform.childCount == 0)
            Instantiate(item.ItemClass.headVFX, headParticlePivot.transform.position, headParticlePivot.transform.rotation, headParticlePivot.transform);

        if (item.ItemClass.rightHandVFX && rightHandParticlePivot.transform.childCount == 0)
            Instantiate(item.ItemClass.rightHandVFX, rightHandParticlePivot.transform.position, rightHandParticlePivot.transform.rotation, rightHandParticlePivot.transform);

        if (item.ItemClass.leftHandVFX && leftHandParticlePivot.transform.childCount == 0)
            Instantiate(item.ItemClass.leftHandVFX, leftHandParticlePivot.transform.position, leftHandParticlePivot.transform.rotation, leftHandParticlePivot.transform);

        if (item.ItemClass.bodyVFX && bodyParticlePivot.transform.childCount < 3)
            Instantiate(item.ItemClass.bodyVFX, bodyParticlePivot.transform.position, bodyParticlePivot.transform.rotation, bodyParticlePivot.transform);

        var startedCoroutine = StartCoroutine(RevertPowerUpAffection(item));
        coroutines.Add(new PowerUpCoroutine(item.ItemClass, startedCoroutine));
    }

    private IEnumerator RevertPowerUpAffection(Item item)
    {
        yield return new WaitForSeconds(item.ItemClass.affectTime);

        var time = 0.15f;

        switch (item.ItemClass.type)
        {
            case ItemType.Power:
                Power = power.z;
                _currentAttackMode = 0;
                break;
            case ItemType.AttackSpeed:
                AttackSpeed = attackSpeed.z;
                break;
            case ItemType.Confuser:
                IsConfused = false;
                break;
            default:
                break;
        }

        if (item.ItemClass.changeSize)
            animator.transform.DOScale(initScale, time);

        if (item.ItemClass.destroyVFXOnEnd)
        {
            foreach (Transform particle in headParticlePivot.transform)
                Destroy(particle.gameObject);

            foreach (Transform obj in rightHandParticlePivot.transform)
                Destroy(obj.gameObject);

            foreach (Transform obj in leftHandParticlePivot.transform)
                Destroy(obj.gameObject);

            foreach (Transform obj in bodyParticlePivot.transform)
                Destroy(obj.gameObject);
        }

        var toRemoveCoroutine = coroutines.SingleOrDefault(x => x.itemClass.GetHashCode() == item.ItemClass.GetHashCode());
        coroutines.Remove(toRemoveCoroutine);
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
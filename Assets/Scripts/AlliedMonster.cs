using UnityEngine;
using UnityEngine.UI;

public class AlliedMonster : Monster
{
    private Animator animator;
    private Monster enemyMonster;

    [Header("UI")]
    public Slider healthBar;

    public override void Init()
    {
        base.Init();

        enemyMonster = FindObjectOfType<EnemyMonster>();
        animator = GetComponentInChildren<Animator>();

        healthBar.minValue = health.x;
        healthBar.maxValue = health.y;
        healthBar.value = Health;

        OnDamage += OnDamageDone;
    }

    private void OnDisable()
    {
        OnDamage -= OnDamageDone;
    }

    private void OnDamageDone()
    {
        healthBar.value = Health;
    }

    public override void Attack()
    {
        base.Attack();

        if (enemyMonster && enemyMonster.IsDead())
            return;

        animator?.SetFloat(ATTACK_SPEED, AttackSpeed);
        animator?.SetTrigger(ATTACK_TRIGGER);

        if (enemyMonster)
            enemyMonster.Damage(Power);
    }

    public override void Die()
    {
        base.Die();

        animator?.SetTrigger(DEAD_TRIGGER);
        healthBar.gameObject.SetActive(false);
    }
}
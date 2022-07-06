using System.Collections;
using UnityEngine;

public class AlliedMonster : Monster
{
    public float baseAttackDelay;
    private Animator animator;
    private Monster enemyMonster;

    public const string ATTACK_TRIGGER = "Attack";

    public override void Init()
    {
        base.Init();

        enemyMonster = FindObjectOfType<EnemyMonster>();
        animator = GetComponentInChildren<Animator>();

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        while (!IsDead())
        {
            animator?.SetTrigger(ATTACK_TRIGGER);

            if (enemyMonster)
            {
                enemyMonster.Damage(Power);
            }

            yield return new WaitForSeconds(baseAttackDelay / AttackSpeed);
        }
    }
}
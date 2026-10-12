using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyHealthScript : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 10;

    protected int currentHealth;

    private EnemyDropsScript dropsScript;
    private NavMeshAgent agent;
    private bool isDead = false;

    private MoveTo moveTo;
    private Coroutine knockbackCoroutine;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        dropsScript = GetComponent<EnemyDropsScript>();
        agent = GetComponent<NavMeshAgent>();
        moveTo = GetComponent<MoveTo>();
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log(
            $"{gameObject.name} took {damage} damage. " +
            $"HP: {currentHealth}/{maxHealth}"
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public virtual void TakeDamage(
        int damage,
        Vector3 knockbackDirection,
        float knockbackSpeed,
        float knockbackDuration)
    {
        if (isDead)
            return;

        TakeDamage(damage);

        if (isDead || currentHealth <= 0 || moveTo == null)
            return;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            moveTo.Knockback(
                knockbackDirection,
                knockbackSpeed,
                knockbackDuration
            )
        );
    }

    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        if (dropsScript != null)
        {
            dropsScript.Drop();
        }

        Destroy(gameObject);
    }
}
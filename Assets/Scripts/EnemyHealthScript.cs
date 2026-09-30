using UnityEngine;

public abstract class EnemyHealthScript : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 10;

    protected int currentHealth;

    private EnemyDropsScript dropsScript;
    private bool isDead = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        dropsScript = GetComponent<EnemyDropsScript>();
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
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

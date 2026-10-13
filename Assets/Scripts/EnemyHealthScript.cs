using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages enemy health, damage feedback, knockback, and death.
/// </summary>
public abstract class EnemyHealthScript : MonoBehaviour
{
    // Health configuration.
    [Header("Health")]
    [SerializeField] protected int maxHealth = 10;

    // Controls the spawn position and appearance of floating damage numbers.
    [Header("Damage Numbers")]
    [SerializeField] private DamageNumberScript damageNumberPrefab;
    [SerializeField] private float damageNumberForwardOffset = 0.75f;
    [SerializeField] private float damageNumberHeight = 2f;
    [SerializeField] private Vector3 damageNumberAdditionalOffset = Vector3.zero;

    // Current health shared with derived enemy classes.
    protected int currentHealth;

    // References to additional enemy components.
    private EnemyDropsScript dropsScript;
    private WaveManager waveManager;
    private MoveTo moveTo;

    // Tracks death state and any active knockback coroutine.
    private bool isDead = false;
    private Coroutine knockbackCoroutine;

    // Initializes health and caches component references.
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        dropsScript = GetComponent<EnemyDropsScript>();
        waveManager = FindFirstObjectByType<WaveManager>();
        moveTo = GetComponent<MoveTo>();
    }

    // Applies damage and handles the resulting feedback or death.
    public virtual void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        ShowDamageNumber(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Spawns a damage number between the enemy and the player.
    private void ShowDamageNumber(int damage)
    {
        if (damageNumberPrefab == null)
            return;

        Camera playerCamera = Camera.main;

        Vector3 spawnPosition = transform.position + Vector3.up * damageNumberHeight;

        if (playerCamera != null)
        {
            Vector3 directionToPlayer = playerCamera.transform.position - transform.position;

            directionToPlayer.y = 0f;

            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                directionToPlayer.Normalize();

                spawnPosition += directionToPlayer * damageNumberForwardOffset;
            }
        }

        spawnPosition += damageNumberAdditionalOffset;

        DamageNumberScript number = Instantiate(damageNumberPrefab, spawnPosition, Quaternion.identity);

        number.Initialize(damage);
    }

    // Applies damage and interrupts enemy movement with knockback.
    public virtual void TakeDamage(int damage, Vector3 knockbackDirection, float knockbackSpeed, float knockbackDuration)
    {
        if (isDead)
            return;

        TakeDamage(damage);

        if (isDead || currentHealth <= 0 || moveTo == null)
            return;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(moveTo.Knockback(knockbackDirection, knockbackSpeed, knockbackDuration));
    }

    // Handles death, drops, and removal of the enemy.
    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (dropsScript != null)
        {
            dropsScript.Drop();
        }

        if (waveManager != null)
        {
            waveManager.EnemyDied();
        }
        Destroy(gameObject);
    }
}
using UnityEngine;

public class EnemyWallScript : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 50;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
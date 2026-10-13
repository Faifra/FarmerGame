using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {

        EnemyHealthScript health = other.GetComponentInParent<EnemyHealthScript>();

        if (health != null)
        {
            health.TakeDamage((int)damage);
        }

        Destroy(gameObject);
    }
}
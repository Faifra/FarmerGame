using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile hit: {collision.gameObject.name}");

        //Damage system here

        Destroy(gameObject);
    }
}

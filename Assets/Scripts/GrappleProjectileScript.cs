
using UnityEngine;

/// <summary>
/// Detects enemy hits and attaches the player's grapple.
/// </summary>
public class GrappleProjectileScript : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private GrappleControllerScript grappleController;
    private bool hasHit;

    public void Initialize(GrappleControllerScript controller)
    {
        grappleController = controller;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        EnemyHealthScript enemy = other.GetComponentInParent<EnemyHealthScript>();

        if (enemy == null)
            return;

        hasHit = true;

        if (grappleController != null)
            grappleController.AttachToEnemy(enemy);

        Destroy(gameObject);
    }
}
using UnityEngine;

public class MeleeWeaponScript : WeaponScript
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 0.75f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask damageLayers;

    public override void Attack()
    {
        Vector3 attackPosition = transform.position + transform.forward * attackRange;

        Collider[] hits = Physics.OverlapSphere(
            attackPosition,
            attackRadius,
            damageLayers
        );

        foreach (Collider hit in hits)
        {
            Debug.Log($"Melee hit: {hit.gameObject.name}");

            //Damage system here
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 attackPosition = transform.position + transform.forward * attackRange;

        Gizmos.DrawWireSphere(attackPosition, attackRadius);
    }
}
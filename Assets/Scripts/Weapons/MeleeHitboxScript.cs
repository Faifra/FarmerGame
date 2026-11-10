
using UnityEngine;

/// <summary>
/// Relays trigger contacts from the weapon hitbox to its melee weapon.
/// </summary>
public class MeleeHitboxScript : MonoBehaviour
{
    [SerializeField] private MeleeWeaponScript meleeWeapon;

    private void Awake()
    {
        if (meleeWeapon == null)
            meleeWeapon = GetComponentInParent<MeleeWeaponScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (meleeWeapon != null)
            meleeWeapon.TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (meleeWeapon != null)
            meleeWeapon.TryHit(other);
    }
}
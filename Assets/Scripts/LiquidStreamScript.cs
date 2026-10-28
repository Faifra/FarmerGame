using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles continuous liquid streams and periodic damage to enemies inside the stream.
/// </summary>
public class LiquidStreamScript : MonoBehaviour
{
    [Header("Stream")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private float range = 10f;
    [SerializeField] private float spreadAngle = 5f;
    [SerializeField] private float closeRangeRadius = 1f;

    [Header("Burst")]
    [SerializeField] private float burstDuration = 0.2f;

    [Header("Damage")]
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private float tickInterval = 0.2f;

    [Header("Visual Effect")]
    [SerializeField] private ParticleSystem streamEffect;

    // Tracks whether the stream is currently being fired.
    private bool isActive;
    private float tickTimer;

    // Tracks whether the burst is currently being fired.
    private float burstTimer;
    private bool burstActive;

    // Prevents enemies with multiple colliders from taking duplicate damage per tick.
    private readonly HashSet<EnemyHealthScript> hitEnemies = new HashSet<EnemyHealthScript>();

    public void SetStreamActive(bool active)
    {
        if (isActive == active && !burstActive)
            return;

        burstActive = false;
        isActive = active;
        tickTimer = 0f;

        if (streamEffect != null)
        {
            if (isActive)
                streamEffect.Play();
            else
                streamEffect.Stop();
        }
    }

    private void Update()
    {
        if (!isActive || muzzle == null)
            return;

        if (burstActive)
        {
            burstTimer += Time.deltaTime;

            if (burstTimer >= burstDuration)
            {
                burstActive = false;
                isActive = false;

                if (streamEffect != null)
                    streamEffect.Stop();

                return;
            }
        }

        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer %= Mathf.Max(tickInterval, 0.001f);
            ApplyDamageTick();
        }
    }

    // Damages each enemy currently inside the stream cone.
    private void ApplyDamageTick()
    {
        Collider[] hits = Physics.OverlapSphere(muzzle.position, range, Physics.AllLayers, QueryTriggerInteraction.Collide);

        hitEnemies.Clear();

        foreach (Collider hit in hits)
        {
            EnemyHealthScript health = hit.GetComponentInParent<EnemyHealthScript>();

            if (health == null || hitEnemies.Contains(health))
                continue;

            Vector3 targetPoint = hit.ClosestPoint(muzzle.position);
            Vector3 direction = targetPoint - muzzle.position;
            float distance = direction.magnitude;

            if (distance > range)
                continue;

            if (distance > closeRangeRadius)
            {
                float angle = Vector3.Angle(muzzle.forward, direction.normalized);

                if (angle > spreadAngle)
                    continue;
            }

            if (hitEnemies.Add(health))
            {
                health.TakeDamage(damagePerTick);
            }
        }
    }

    // Fires one short burst.
    public void FireBurst()
    {
        if (muzzle == null)
            return;

        burstActive = true;
        burstTimer = 0f;
        tickTimer = 0f;
        isActive = true;

        if (streamEffect != null)
        {
            streamEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            streamEffect.Play();
        }
    }

    private void OnDisable()
    {
        SetStreamActive(false);
    }
}
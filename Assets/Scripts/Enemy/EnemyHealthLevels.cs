using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthLevels : EnemyHealthScript
{
    private List<int> extraHealth = new();
    private List<int> extraMaxHealth = new();

    public int HealthLayerCount => extraHealth.Count + 1;

    public void ConfigureHealthLayers(int additionalLayers, int healthPerLayer)
    {
        extraHealth.Clear();
        extraMaxHealth.Clear();

        for (int i = 0; i < additionalLayers; i++)
        {
            extraMaxHealth.Add(healthPerLayer);
            extraHealth.Add(healthPerLayer);
        }
    }

    public int LayerMaxHealth(int layerIndex)
    {
        if (layerIndex == 0)
            return MaxHealth;

        return extraMaxHealth[layerIndex - 1];
    }

    public int LayerCurrentHealth(int layerIndex)
    {
        if (layerIndex == 0)
            return CurrentHealth();

        return extraHealth[layerIndex - 1];
    }

    public override void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        int finalDamage = Mathf.RoundToInt(damage * DamageMultiplier);

        for (int i = extraHealth.Count - 1; i >= 0; i--)
        {
            if (extraHealth[i] <= 0)
                continue;

            extraHealth[i] -= finalDamage;

            if (extraHealth[i] < 0)
            {
                finalDamage = -extraHealth[i];
                extraHealth[i] = 0;
            }
            else
            {
                finalDamage = 0;
            }

            if (finalDamage <= 0)
                break;
        }

        if (finalDamage > 0)
        {
            ApplyBaseHealthDamage(finalDamage);
            ShowDamageNumber(damage);
        }
        else
        {
            ShowDamageNumber(damage);
        }
    }

    public override void TakeDamage(int damage, Vector3 knockbackDirection, float knockbackSpeed, float knockbackDuration)
    {
        if (IsDead)
            return;

        TakeDamage(damage);

        if (IsDead || MoveToComponent == null)
            return;

        ApplyKnockback(knockbackDirection, knockbackSpeed, knockbackDuration);
    }

    public override void TakePercentageDamage(float percentage)
    {
        if (IsDead)
            return;

        int damage = Mathf.RoundToInt(MaxHealth * percentage);

        if (damage > 0)
            TakeDamage(damage);
    }

    public override void TakeDamageFromSprinkler(int damage, GameObject wallPrefab, float wallHealthMultiplier = 1f)
    {
        if (IsDead)
            return;

        int finalDamage = Mathf.RoundToInt(damage * DamageMultiplier);

        for (int i = extraHealth.Count - 1; i >= 0; i--)
        {
            if (extraHealth[i] <= 0)
                continue;

            extraHealth[i] -= finalDamage;

            if (extraHealth[i] < 0)
            {
                finalDamage = -extraHealth[i];
                extraHealth[i] = 0;
            }
            else
            {
                finalDamage = 0;
            }

            if (finalDamage <= 0)
                break;
        }

        if (finalDamage > 0)
        {
            ApplySprinklerBaseDamage(finalDamage, wallPrefab, wallHealthMultiplier);
            ShowDamageNumber(damage);
        }
        else
        {
            ShowDamageNumber(damage);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks upgrades purchased by the player during the game.
/// </summary>
public class UpgradeManagerScript : MonoBehaviour
{
    [Header("Weapon Upgrades")]
    [SerializeField] private BasicSprayUpgradeScript basicSprayUpgrade;
    [SerializeField] private ShotgunSprayUpgradeScript shotgunSprayUpgrade;
    [SerializeField] private PitchforkUpgradeScript pitchforkUpgrade;
    [SerializeField] private ScytheUpgradeScript scytheUpgrade;
    [SerializeField] private GrappleUpgradeScript grappleUpgrade;
    [SerializeField] private SniperUpgradeScript sniperUpgrade;
    [SerializeField] private WaterBalloonUpgradeScript waterBalloonUpgrade;
    [SerializeField] private MilkChurnUpgradeScript milkChurnUpgrade;
    [SerializeField] private RestorativeGasUpgradeScript restorativeGasUpgrade;

    [Header("Trap Upgrades")]
    [SerializeField] private LaunchTrapUpgradeScript launchTrapUpgrade;
    [SerializeField] private PullTrapUpgradeScript pullTrapUpgrade;
    [SerializeField] private BeehiveTrapUpgradeScript beehiveUpgrade;
    [SerializeField] private SprinklerTrapUpgradeScript sprinklerUpgrade;
    [SerializeField] private MagnifyingGlassTrapUpgradeScript magnifyingGlassTrapUpgrade;


    // Stores the IDs of upgrades purchased.
    private HashSet<string> purchasedUpgrades = new HashSet<string>();

    // Checks whether an upgrade has already been purchased.
    public bool HasUpgrade(string upgradeID)
    {
        return purchasedUpgrades.Contains(upgradeID);
    }

    // Registers an upgrade as purchased.
    public void RegisterUpgrade(string upgradeID)
    {
        if (string.IsNullOrEmpty(upgradeID))
            return;

        purchasedUpgrades.Add(upgradeID);
    }

    // Removes all purchased upgrades when the player dies.
    public void ResetUpgrades()
    {
        purchasedUpgrades.Clear();

        if (basicSprayUpgrade != null)
        {
            basicSprayUpgrade.ResetUpgrades();
        }

        if (shotgunSprayUpgrade != null)
        {
            shotgunSprayUpgrade.ResetUpgrades();
        }

        if (scytheUpgrade != null)
        {
            scytheUpgrade.ResetUpgrades();
        }

        if (pitchforkUpgrade != null)
        {
            pitchforkUpgrade.ResetUpgrades();
        }

        if (grappleUpgrade != null)
        {
            grappleUpgrade.ResetUpgrades();
        }

        if (sniperUpgrade != null)
        {
            sniperUpgrade.ResetUpgrades();
        }

        if (waterBalloonUpgrade != null)
        {
            waterBalloonUpgrade.ResetUpgrades();
        }

        if (milkChurnUpgrade != null)
        {
            milkChurnUpgrade.ResetUpgrades();
        }

        if (restorativeGasUpgrade != null)
        {
            restorativeGasUpgrade.ResetUpgrades();
        }

        if (launchTrapUpgrade != null)
        {
            launchTrapUpgrade.ResetUpgrades();
        }

        if (pullTrapUpgrade != null)
        {
            pullTrapUpgrade.ResetUpgrades();
        }

        if (beehiveUpgrade != null)
        {
            beehiveUpgrade.ResetUpgrades();
        }

        if (sprinklerUpgrade != null)
        {
            sprinklerUpgrade.ResetUpgrades();
        }

        if (magnifyingGlassTrapUpgrade != null)
        {
            magnifyingGlassTrapUpgrade.ResetUpgrades();
        }
    }
}
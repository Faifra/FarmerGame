
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks upgrades purchased during the current game and resets upgrade effects.
/// </summary>
public class UpgradeManagerScript : MonoBehaviour
{
    [Serializable]
    private class WeaponUpgrades
    {
        public BasicSprayUpgradeScript basicSpray;
        public ShotgunSprayUpgradeScript shotgunSpray;
        public PitchforkUpgradeScript pitchfork;
        public ScytheUpgradeScript scythe;
        public GrappleUpgradeScript grapple;
        public SniperUpgradeScript sniper;
        public WaterBalloonUpgradeScript waterBalloon;
        public MilkChurnUpgradeScript milkChurn;
        public RestorativeGasUpgradeScript restorativeGas;
    }

    [Serializable]
    private class TrapUpgrades
    {
        public LaunchTrapUpgradeScript launchTrap;
        public PullTrapUpgradeScript pullTrap;
        public BeehiveTrapUpgradeScript beehive;
        public SprinklerTrapUpgradeScript sprinkler;
        public MagnifyingGlassTrapUpgradeScript magnifyingGlass;
    }

    [Header("Weapon Upgrades")]
    [SerializeField] private WeaponUpgrades weapons = new WeaponUpgrades();

    [Header("Trap Upgrades")]
    [SerializeField] private TrapUpgrades traps = new TrapUpgrades();

    private readonly HashSet<string> purchasedUpgrades = new HashSet<string>();

    // Weapon upgrade accessors.
    public BasicSprayUpgradeScript BasicSpray => weapons.basicSpray;
    public ShotgunSprayUpgradeScript ShotgunSpray => weapons.shotgunSpray;
    public PitchforkUpgradeScript Pitchfork => weapons.pitchfork;
    public ScytheUpgradeScript Scythe => weapons.scythe;
    public GrappleUpgradeScript Grapple => weapons.grapple;
    public SniperUpgradeScript Sniper => weapons.sniper;
    public WaterBalloonUpgradeScript WaterBalloon => weapons.waterBalloon;
    public MilkChurnUpgradeScript MilkChurn => weapons.milkChurn;
    public RestorativeGasUpgradeScript RestorativeGas => weapons.restorativeGas;

    // Trap upgrade accessors.
    public LaunchTrapUpgradeScript LaunchTrap => traps.launchTrap;
    public PullTrapUpgradeScript PullTrap => traps.pullTrap;
    public BeehiveTrapUpgradeScript Beehive => traps.beehive;
    public SprinklerTrapUpgradeScript Sprinkler => traps.sprinkler;
    public MagnifyingGlassTrapUpgradeScript MagnifyingGlass => traps.magnifyingGlass;

    public bool HasUpgrade(string upgradeID)
    {
        return purchasedUpgrades.Contains(upgradeID);
    }

    public void RegisterUpgrade(string upgradeID)
    {
        if (!string.IsNullOrEmpty(upgradeID))
            purchasedUpgrades.Add(upgradeID);
    }

    /// <summary>
    /// Clears purchased upgrades and resets all assigned upgrade components.
    /// </summary>
    public void ResetUpgrades()
    {
        purchasedUpgrades.Clear();

        weapons.basicSpray?.ResetUpgrades();
        weapons.shotgunSpray?.ResetUpgrades();
        weapons.pitchfork?.ResetUpgrades();
        weapons.scythe?.ResetUpgrades();
        weapons.grapple?.ResetUpgrades();
        weapons.sniper?.ResetUpgrades();
        weapons.waterBalloon?.ResetUpgrades();
        weapons.milkChurn?.ResetUpgrades();
        weapons.restorativeGas?.ResetUpgrades();

        traps.launchTrap?.ResetUpgrades();
        traps.pullTrap?.ResetUpgrades();
        traps.beehive?.ResetUpgrades();
        traps.sprinkler?.ResetUpgrades();
        traps.magnifyingGlass?.ResetUpgrades();
    }
}
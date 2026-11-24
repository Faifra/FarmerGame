using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the upgrade menu, item selection, and purchases.
/// Upgrade component references are managed by UpgradeManagerScript.
/// </summary>
public class UpgradeMenuScript : MonoBehaviour
{
    // --------------------------------------------------
    // INSPECTOR DATA CLASSES
    // --------------------------------------------------

    [Serializable]
    private class TabUI
    {
        public Button purchaseWeaponsButton;
        public Button purchaseTrapsButton;
        public Button upgradeTrapsButton;
        public Button upgradeWeaponsButton;

        public GameObject purchaseWeaponsPanel;
        public GameObject purchaseTrapsPanel;
        public GameObject upgradeTrapsPanel;
        public GameObject upgradeWeaponsPanel;

        public Color selectedColor = new Color(183f / 255f, 121f / 255f, 54f / 255f);

        public Color normalColor = new Color(52f / 255f, 70f / 255f, 79f / 255f);
    }

    [Serializable]
    private class ItemUI
    {
        public Outline outline;
        public GameObject upgradePanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
    }

    [Serializable]
    private class PurchaseUI
    {
        public int cost;
        public Button button;
        public TextMeshProUGUI costText;
    }

    [Serializable]
    private class UpgradeGroupUI
    {
        public PurchaseUI first = new PurchaseUI();
        public PurchaseUI second = new PurchaseUI();
        public PurchaseUI third = new PurchaseUI();
    }

    [Serializable]
    private class WeaponPurchaseUI
    {
        public PurchaseUI basicSpray = new PurchaseUI();
        public PurchaseUI shotgunSpray = new PurchaseUI();
        public PurchaseUI pitchfork = new PurchaseUI();
        public PurchaseUI scythe = new PurchaseUI();
        public PurchaseUI grapple = new PurchaseUI();
        public PurchaseUI sniper = new PurchaseUI();
        public PurchaseUI waterBalloon = new PurchaseUI();
        public PurchaseUI milkChurn = new PurchaseUI();
        public PurchaseUI restorativeGas = new PurchaseUI();
    }

    [Serializable]
    private class TrapPurchaseUI
    {
        public PurchaseUI launch = new PurchaseUI();
        public PurchaseUI pull = new PurchaseUI();
        public PurchaseUI beehive = new PurchaseUI();
        public PurchaseUI magnifyingGlass = new PurchaseUI();
        public PurchaseUI sprinkler = new PurchaseUI();
    }

    private class UpgradeDefinition
    {
        public string ID;
        public int Cost;
        public Button Button;
        public Action Activate;

        public UpgradeDefinition(string id, PurchaseUI ui, Action activate)
        {
            ID = id;
            Cost = ui.cost;
            Button = ui.button;
            Activate = activate;
        }
    }

    private class ItemSelection
    {
        public Outline Outline;
        public GameObject Panel;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI DescriptionText;
        public string Name;
        public string Description;

        public ItemSelection(ItemUI ui, string name, string description)
        {
            Outline = ui.outline;
            Panel = ui.upgradePanel;
            NameText = ui.nameText;
            DescriptionText = ui.descriptionText;
            Name = name;
            Description = description;
        }
    }

    // --------------------------------------------------
    // HUD AND MENU
    // --------------------------------------------------

    [Header("HUD")]
    [SerializeField] private GameObject hotbarUI;
    [SerializeField] private GameObject ammoTextUI;

    [Header("Menu")]
    [SerializeField] private GameObject upgradeMenu;

    [Header("Tabs")]
    [SerializeField] private TabUI tabs = new TabUI();

    // --------------------------------------------------
    // PLAYER REFERENCES
    // --------------------------------------------------

    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;
    [SerializeField] private UpgradeManagerScript upgradeManager;

    // --------------------------------------------------
    // PURCHASES
    // --------------------------------------------------

    [Header("Trap Purchases")]
    [SerializeField] private TrapPurchaseUI trapPurchases = new TrapPurchaseUI();

    [Header("Weapon Purchases")]
    [SerializeField] private WeaponPurchaseUI weaponPurchases = new WeaponPurchaseUI();

    // --------------------------------------------------
    // ITEM SELECTION UI
    // --------------------------------------------------

    [Header("Weapon Selection UI")]
    [SerializeField] private ItemUI basicSpray = new ItemUI();
    [SerializeField] private ItemUI shotgunSpray = new ItemUI();
    [SerializeField] private ItemUI pitchfork = new ItemUI();
    [SerializeField] private ItemUI scythe = new ItemUI();
    [SerializeField] private ItemUI grapple = new ItemUI();
    [SerializeField] private ItemUI sniper = new ItemUI();
    [SerializeField] private ItemUI waterBalloon = new ItemUI();
    [SerializeField] private ItemUI milkChurn = new ItemUI();
    [SerializeField] private ItemUI restorativeGas = new ItemUI();

    [Header("Trap Selection UI")]
    [SerializeField] private ItemUI launchTrap = new ItemUI();
    [SerializeField] private ItemUI pullTrap = new ItemUI();
    [SerializeField] private ItemUI beehiveTrap = new ItemUI();
    [SerializeField] private ItemUI sprinklerTrap = new ItemUI();
    [SerializeField] private ItemUI magnifyingGlassTrap = new ItemUI();

    // --------------------------------------------------
    // WEAPON UPGRADE PURCHASE UI
    // --------------------------------------------------

    [Header("Basic Spray Upgrades")]
    [SerializeField] private UpgradeGroupUI basicSprayUpgrades = new UpgradeGroupUI();

    [Header("Shotgun Spray Upgrades")]
    [SerializeField] private UpgradeGroupUI shotgunSprayUpgrades = new UpgradeGroupUI();

    [Header("Pitchfork Upgrades")]
    [SerializeField] private UpgradeGroupUI pitchforkUpgrades = new UpgradeGroupUI();

    [Header("Scythe Upgrades")]
    [SerializeField] private UpgradeGroupUI scytheUpgrades = new UpgradeGroupUI();

    [Header("Grapple Upgrades")]
    [SerializeField] private UpgradeGroupUI grappleUpgrades = new UpgradeGroupUI();

    [Header("Sniper Upgrades")]
    [SerializeField] private UpgradeGroupUI sniperUpgrades = new UpgradeGroupUI();

    [Header("Water Balloon Upgrades")]
    [SerializeField] private UpgradeGroupUI waterBalloonUpgrades = new UpgradeGroupUI();

    [Header("Milk Churn Upgrades")]
    [SerializeField] private UpgradeGroupUI milkChurnUpgrades = new UpgradeGroupUI();

    [Header("Restorative Gas Upgrades")]
    [SerializeField] private UpgradeGroupUI restorativeGasUpgrades = new UpgradeGroupUI();

    // --------------------------------------------------
    // TRAP UPGRADE PURCHASE UI
    // --------------------------------------------------

    [Header("Launch Trap Upgrades")]
    [SerializeField] private UpgradeGroupUI launchTrapUpgrades = new UpgradeGroupUI();

    [Header("Pull Trap Upgrades")]
    [SerializeField] private UpgradeGroupUI pullTrapUpgrades = new UpgradeGroupUI();

    [Header("Beehive Trap Upgrades")]
    [SerializeField] private UpgradeGroupUI beehiveTrapUpgrades = new UpgradeGroupUI();

    [Header("Sprinkler Trap Upgrades")]
    [SerializeField] private UpgradeGroupUI sprinklerTrapUpgrades = new UpgradeGroupUI();

    [Header("Magnifying Glass Trap Upgrades")]
    [SerializeField] private UpgradeGroupUI magnifyingGlassTrapUpgrades = new UpgradeGroupUI();

    // --------------------------------------------------
    // UPGRADE IDS
    // --------------------------------------------------

    private const string ImprovedSprayID = "basic_spray_improved";
    private const string MaximumHealthDamageID = "basic_spray_bonus_damage";
    private const string KnockbackID = "basic_spray_knockback";

    private const string DoubleBurstID = "shotgun_spray_double_burst";
    private const string FasterReloadID = "shotgun_spray_faster_reload";
    private const string CloseRangeDamageID = "shotgun_spray_close_range_damage";

    private const string PitchforkDamageID = "pitchfork_bonus_damage";
    private const string PitchforkSlowdownID = "pitchfork_enemy_slowdown";
    private const string PitchforkDropsID = "pitchfork_bonus_drops";

    private const string ScytheRadiusID = "scythe_increased_radius";
    private const string ScytheAttackSpeedID = "scythe_increasing_attack_speed";
    private const string ScytheHealthOnKillID = "scythe_health_on_kill";

    private const string GrappleSpeedID = "grapple_speed";
    private const string GrappleEnemyVulnerabilityID = "grapple_enemy_vulnerability";
    private const string GrappleHealthAndSpeedOnKillID = "grapple_health_and_speed_on_kill";

    private const string SniperFasterReloadID = "sniper_faster_reload";
    private const string SniperExposeWeakpointsID = "sniper_expose_weakpoints";
    private const string SniperRicochetShotID = "sniper_ricochet_shot";

    private const string BalloonExpandedSplashID = "balloon_expanded_splash";
    private const string BalloonRubberizedBalloonID = "balloon_rubberized_balloon";
    private const string BallonCuncussiveAfterShockID = "balloon_concussive_aftershock";

    private const string MilkChurnFasterReloadID = "milk_churn_faster_reload";
    private const string MilkChurnLargerSplatterID = "milk_churn_larger_splatter";
    private const string MilkChurnUnstableMixtureID = "milk_churn_unstable_mixture";

    private const string RestorativeGasExpandedCanisterID = "restorative_gas_expanded_canister";
    private const string RestorativeGasFasterReloadID = "restorative_gas_faster_reload";
    private const string RestorativeGasLingeringTonicID = "restorative_gas_lingering_tonic";

    private const string LaunchTrapHighForceLaunchID = "launch_trap_high_force_launch";
    private const string LaunchTrapConcussiveExhaustID = "launch_trap_concussive_exhaust";
    private const string LaunchTrapDoubleTriggerID = "launch_trap_double_trigger";

    private const string PullTrapExpandedVacuumID = "pull_trap_expanded_vacuum";
    private const string PullTrapExtendedReachID = "pull_trap_extended_reach";
    private const string PullTrapForcefulLaunchID = "pull_trap_forceful_launch";

    private const string BeehiveTrapExpandedColonyID = "beehive_trap_expanded_colony";
    private const string BeehiveTrapFrenziedSwarmID = "beehive_trap_frenzied_swarm";
    private const string BeehiveTrapSwarmReplicationID = "beehive_trap_swarm_replication";

    private const string SprinklerTrapPressurizedSprayID = "sprinkler_trap_pressurized_spray";
    private const string SprinklerTrapRapidRotationID = "sprinkler_trap_rapid_rotation";
    private const string SprinklerTrapReinforcedBarriersID = "sprinkler_trap_reinforced_barriers";

    private const string MagnifyingGlassTrapFocusedSunlightID = "magnifying_glass_trap_focused_sunlight";
    private const string MagnifyingGlassTrapIntensifiedHeatID = "magnifying_glass_trap_intensified_heat";
    private const string MagnifyingGlassTrapRefractedBeamID = "magnifying_glass_trap_refracted_beam";

    // --------------------------------------------------
    // RUNTIME DATA
    // --------------------------------------------------

    private bool menuOpen;

    public bool IsMenuOpen => menuOpen;

    private UpgradeDefinition[] upgrades;
    private ItemSelection[] itemSelections;

    // --------------------------------------------------
    // INITIALIZATION
    // --------------------------------------------------

    private void Start()
    {
        InitializeUpgradeDefinitions();
        InitializeItemSelections();
        UpdateCostLabels();
        ShowPurchaseTrapsTab();
        UpdateButton();
    }

    private void InitializeUpgradeDefinitions()
    {
        if (upgradeManager == null)
        {
            upgrades = Array.Empty<UpgradeDefinition>();
            return;
        }

        upgrades = new UpgradeDefinition[]
        {
            // Basic Spray
            new UpgradeDefinition(ImprovedSprayID, basicSprayUpgrades.first, upgradeManager.BasicSpray != null ? upgradeManager.BasicSpray.ActivateImprovedSpray : null),

            new UpgradeDefinition(MaximumHealthDamageID, basicSprayUpgrades.second, upgradeManager.BasicSpray != null ? upgradeManager.BasicSpray.ActivateMaximumHealthDamage : null),

            new UpgradeDefinition(KnockbackID, basicSprayUpgrades.third, upgradeManager.BasicSpray != null ? upgradeManager.BasicSpray.ActivateKnockback : null),

            // Shotgun Spray
            new UpgradeDefinition(DoubleBurstID, shotgunSprayUpgrades.first, upgradeManager.ShotgunSpray != null ? upgradeManager.ShotgunSpray.ActivateDoubleBurst : null),

            new UpgradeDefinition(FasterReloadID, shotgunSprayUpgrades.second, upgradeManager.ShotgunSpray != null ? upgradeManager.ShotgunSpray.ActivateFasterReload : null),

            new UpgradeDefinition(CloseRangeDamageID, shotgunSprayUpgrades.third, upgradeManager.ShotgunSpray != null ? upgradeManager.ShotgunSpray.ActivateCloseRangeDamage : null),

            // Pitchfork
            new UpgradeDefinition(PitchforkDamageID, pitchforkUpgrades.first, upgradeManager.Pitchfork != null ? upgradeManager.Pitchfork.ActivateBonusDamage : null),

            new UpgradeDefinition(PitchforkSlowdownID, pitchforkUpgrades.second, upgradeManager.Pitchfork != null ? upgradeManager.Pitchfork.ActivateEnemySlowdown : null),

            new UpgradeDefinition(PitchforkDropsID, pitchforkUpgrades.third, upgradeManager.Pitchfork != null ? upgradeManager.Pitchfork.ActivateBonusResources : null),

            // Scythe
            new UpgradeDefinition(ScytheRadiusID, scytheUpgrades.first, upgradeManager.Scythe != null ? upgradeManager.Scythe.ActivateIncreasedAttackRadius : null),

            new UpgradeDefinition(ScytheAttackSpeedID, scytheUpgrades.second, upgradeManager.Scythe != null ? upgradeManager.Scythe.ActivateIncreasingAttackSpeed : null),

            new UpgradeDefinition(ScytheHealthOnKillID, scytheUpgrades.third, upgradeManager.Scythe != null ? upgradeManager.Scythe.ActivateHealthOnKill : null),

            // Grapple
            new UpgradeDefinition(GrappleSpeedID, grappleUpgrades.first, upgradeManager.Grapple != null ? upgradeManager.Grapple.ActivateReinforcedGraft : null),

            new UpgradeDefinition(GrappleEnemyVulnerabilityID, grappleUpgrades.second, upgradeManager.Grapple != null ? upgradeManager.Grapple.ActivateCorrosiveGraft : null),

            new UpgradeDefinition(GrappleHealthAndSpeedOnKillID, grappleUpgrades.third, upgradeManager.Grapple != null ? upgradeManager.Grapple.ActivateGrapplerMomentum : null),

            // Sniper
            new UpgradeDefinition(SniperFasterReloadID, sniperUpgrades.first, upgradeManager.Sniper != null ? upgradeManager.Sniper.ActivateFasterReload : null),

            new UpgradeDefinition(SniperExposeWeakpointsID, sniperUpgrades.second, upgradeManager.Sniper != null ? upgradeManager.Sniper.ActivateExposedWeakPoints : null),

            new UpgradeDefinition(SniperRicochetShotID, sniperUpgrades.third, upgradeManager.Sniper != null ? upgradeManager.Sniper.ActivateRicochetShot : null),

            // Water Balloon
            new UpgradeDefinition(BalloonExpandedSplashID, waterBalloonUpgrades.first, upgradeManager.WaterBalloon != null ? upgradeManager.WaterBalloon.ActivateExpandedSplash : null),

            new UpgradeDefinition(BalloonRubberizedBalloonID, waterBalloonUpgrades.second, upgradeManager.WaterBalloon != null ? upgradeManager.WaterBalloon.ActivateRubberizedBalloon : null),

            new UpgradeDefinition(BallonCuncussiveAfterShockID, waterBalloonUpgrades.third, upgradeManager.WaterBalloon != null ? upgradeManager.WaterBalloon.ActivateConcussiveAftershock : null),

            // Milk Churn
            new UpgradeDefinition(MilkChurnFasterReloadID, milkChurnUpgrades.first, upgradeManager.MilkChurn != null ? upgradeManager.MilkChurn.ActivateRapidChurning : null),

            new UpgradeDefinition(MilkChurnLargerSplatterID, milkChurnUpgrades.second, upgradeManager.MilkChurn != null ? upgradeManager.MilkChurn.ActivateLargerSplatter : null),

            new UpgradeDefinition(MilkChurnUnstableMixtureID, milkChurnUpgrades.third, upgradeManager.MilkChurn != null ? upgradeManager.MilkChurn.ActivateUnstableMixture : null),

            // Restorative Gas
            new UpgradeDefinition(RestorativeGasExpandedCanisterID, restorativeGasUpgrades.first, upgradeManager.RestorativeGas != null ? upgradeManager.RestorativeGas.ActivateExtendedCanister : null),

            new UpgradeDefinition(RestorativeGasFasterReloadID, restorativeGasUpgrades.second, upgradeManager.RestorativeGas != null ? upgradeManager.RestorativeGas.ActivateRapidRefill : null),

            new UpgradeDefinition(RestorativeGasLingeringTonicID, restorativeGasUpgrades.third, upgradeManager.RestorativeGas != null ? upgradeManager.RestorativeGas.ActivateLingeringTonic : null),

            // Launch Trap
            new UpgradeDefinition(LaunchTrapHighForceLaunchID, launchTrapUpgrades.first, upgradeManager.LaunchTrap != null ? upgradeManager.LaunchTrap.ActivateHighForceLaunch : null),

            new UpgradeDefinition(LaunchTrapConcussiveExhaustID, launchTrapUpgrades.second, upgradeManager.LaunchTrap != null ? upgradeManager.LaunchTrap.ActivateConcussiveBlast : null),

            new UpgradeDefinition(LaunchTrapDoubleTriggerID, launchTrapUpgrades.third, upgradeManager.LaunchTrap != null ? upgradeManager.LaunchTrap.ActivateDoubleTrigger : null),

            // Pull Trap
            new UpgradeDefinition(PullTrapExpandedVacuumID, pullTrapUpgrades.first, upgradeManager.PullTrap != null ? upgradeManager.PullTrap.ActivateExpandedVacuum : null),

            new UpgradeDefinition(PullTrapExtendedReachID, pullTrapUpgrades.second, upgradeManager.PullTrap != null ? upgradeManager.PullTrap.ActivateExtendedReach : null),

            new UpgradeDefinition(PullTrapForcefulLaunchID, pullTrapUpgrades.third, upgradeManager.PullTrap != null ? upgradeManager.PullTrap.ActivateForcefulLaunch : null),

            // Beehive Trap
            new UpgradeDefinition(BeehiveTrapExpandedColonyID, beehiveTrapUpgrades.first, upgradeManager.Beehive != null ? upgradeManager.Beehive.ActivateExpandedColony : null),

            new UpgradeDefinition(BeehiveTrapFrenziedSwarmID, beehiveTrapUpgrades.second, upgradeManager.Beehive != null ? upgradeManager.Beehive.ActivateFrenziedSwarm : null),

            new UpgradeDefinition(BeehiveTrapSwarmReplicationID, beehiveTrapUpgrades.third, upgradeManager.Beehive != null ? upgradeManager.Beehive.ActivateSwarmReplication : null),

            // Sprinkler Trap
            new UpgradeDefinition(SprinklerTrapPressurizedSprayID, sprinklerTrapUpgrades.first, upgradeManager.Sprinkler != null ? upgradeManager.Sprinkler.ActivatePressurizedSpray : null),

            new UpgradeDefinition(SprinklerTrapRapidRotationID, sprinklerTrapUpgrades.second, upgradeManager.Sprinkler != null ? upgradeManager.Sprinkler.ActivateRapidRotation : null),

            new UpgradeDefinition(SprinklerTrapReinforcedBarriersID, sprinklerTrapUpgrades.third, upgradeManager.Sprinkler != null ? upgradeManager.Sprinkler.ActivateReinforcedBarriers : null),

            // Magnifying Glass Trap
            new UpgradeDefinition(MagnifyingGlassTrapFocusedSunlightID, magnifyingGlassTrapUpgrades.first, upgradeManager.MagnifyingGlass != null ? upgradeManager.MagnifyingGlass.ActivateFocusedSunlight : null),

            new UpgradeDefinition(MagnifyingGlassTrapIntensifiedHeatID, magnifyingGlassTrapUpgrades.second, upgradeManager.MagnifyingGlass != null ? upgradeManager.MagnifyingGlass.ActivateIntensifiedHeat : null),

            new UpgradeDefinition(MagnifyingGlassTrapRefractedBeamID, magnifyingGlassTrapUpgrades.third, upgradeManager.MagnifyingGlass != null ? upgradeManager.MagnifyingGlass.ActivateRefractedBeam : null)
        };
    }

    private void InitializeItemSelections()
    {
        itemSelections = new ItemSelection[]
        {
            new ItemSelection(basicSpray, "Basic Spray", "A basic spray weapon that fires a continuous stream of liquid."),

            new ItemSelection(shotgunSpray, "Shotgun Spray", "A spray weapon that fires a burst of liquid."),

            new ItemSelection(pitchfork, "Pitchfork", "A sturdy pitchfork allowing for powerful thrusting attacks."),

            new ItemSelection(scythe, "Scythe", "A scythe allowing for wide sweeping swings."),

            new ItemSelection(grapple, "Grapple", "A grapple gun providing increased mobility and positioning."),

            new ItemSelection(sniper, "Sniper", "A precision weapon designed for long-range attacks."),

            new ItemSelection(waterBalloon, "Water Balloon Launcher", "A powerful weapon launching water balloon explosives."),

            new ItemSelection(milkChurn, "Milk Churn Launcher", "A launcher that fires damaging milk-based projectiles."),

            new ItemSelection(restorativeGas, "Restorative Gas Thrower", "A weapon that releases restorative gas."),

            new ItemSelection(launchTrap, "Launch Trap", "A trap that launches enemies away."),

            new ItemSelection(pullTrap, "Pull Trap", "A trap that pulls enemies toward it."),

            new ItemSelection(beehiveTrap, "Beehive Trap", "A trap that releases a swarm of bees."),

            new ItemSelection(sprinklerTrap, "Sprinkler Trap", "A trap that sprays enemies with pressurized liquid."),

            new ItemSelection(magnifyingGlassTrap, "Magnifying Glass Trap", "A trap that focuses sunlight to damage enemies.")
        };
    }

    // --------------------------------------------------
    // COST LABELS
    // --------------------------------------------------

    private void UpdateCostLabels()
    {
        SetCostText(weaponPurchases.basicSpray);
        SetCostText(weaponPurchases.shotgunSpray);
        SetCostText(weaponPurchases.pitchfork);
        SetCostText(weaponPurchases.scythe);
        SetCostText(weaponPurchases.grapple);
        SetCostText(weaponPurchases.sniper);
        SetCostText(weaponPurchases.waterBalloon);
        SetCostText(weaponPurchases.milkChurn);
        SetCostText(weaponPurchases.restorativeGas);

        SetCostText(trapPurchases.launch);
        SetCostText(trapPurchases.pull);
        SetCostText(trapPurchases.beehive);
        SetCostText(trapPurchases.magnifyingGlass);
        SetCostText(trapPurchases.sprinkler);

        SetCostText(basicSprayUpgrades);
        SetCostText(shotgunSprayUpgrades);
        SetCostText(pitchforkUpgrades);
        SetCostText(scytheUpgrades);
        SetCostText(grappleUpgrades);
        SetCostText(sniperUpgrades);
        SetCostText(waterBalloonUpgrades);
        SetCostText(milkChurnUpgrades);
        SetCostText(restorativeGasUpgrades);

        SetCostText(launchTrapUpgrades);
        SetCostText(pullTrapUpgrades);
        SetCostText(beehiveTrapUpgrades);
        SetCostText(sprinklerTrapUpgrades);
        SetCostText(magnifyingGlassTrapUpgrades);
    }

    private void SetCostText(PurchaseUI purchase)
    {
        if (purchase != null && purchase.costText != null)
            purchase.costText.text = purchase.cost.ToString();
    }

    private void SetCostText(UpgradeGroupUI group)
    {
        SetCostText(group.first);
        SetCostText(group.second);
        SetCostText(group.third);
    }

    // --------------------------------------------------
    // MENU OPEN / CLOSE
    // --------------------------------------------------

    public void OpenMenu()
    {
        if (menuOpen)
            return;

        menuOpen = true;
        SetMenuState(true);
        UpdateButton();
    }

    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        menuOpen = false;
        SetMenuState(false);
    }

    private void SetMenuState(bool isOpen)
    {
        if (upgradeMenu != null)
            upgradeMenu.SetActive(isOpen);

        if (hotbarUI != null)
            hotbarUI.SetActive(!isOpen);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(!isOpen);

        if (playerLook != null)
            playerLook.SetMenuOpen(isOpen);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(isOpen);
    }

    // --------------------------------------------------
    // TABS
    // --------------------------------------------------

    public void ShowPurchaseWeaponsTab()
    {
        SetActiveTab(tabs.purchaseWeaponsPanel, tabs.purchaseWeaponsButton);
    }

    public void ShowPurchaseTrapsTab()
    {
        SetActiveTab(tabs.purchaseTrapsPanel, tabs.purchaseTrapsButton);
    }

    public void ShowUpgradeTrapsTab()
    {
        SetActiveTab(tabs.upgradeTrapsPanel, tabs.upgradeTrapsButton);
    }

    public void ShowUpgradeWeaponsTab()
    {
        SetActiveTab(tabs.upgradeWeaponsPanel, tabs.upgradeWeaponsButton);
    }

    private void SetActiveTab(GameObject selectedPanel, Button selectedButton)
    {
        SetPanelActive(tabs.purchaseWeaponsPanel, false);
        SetPanelActive(tabs.purchaseTrapsPanel, false);
        SetPanelActive(tabs.upgradeTrapsPanel, false);
        SetPanelActive(tabs.upgradeWeaponsPanel, false);

        SetPanelActive(selectedPanel, true);

        SetTabColor(tabs.purchaseWeaponsButton, tabs.normalColor);
        SetTabColor(tabs.purchaseTrapsButton, tabs.normalColor);
        SetTabColor(tabs.upgradeTrapsButton, tabs.normalColor);
        SetTabColor(tabs.upgradeWeaponsButton, tabs.normalColor);

        SetTabColor(selectedButton, tabs.selectedColor);
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
            panel.SetActive(isActive);
    }

    private void SetTabColor(Button button, Color color)
    {
        if (button != null && button.image != null)
            button.image.color = color;
    }

    // --------------------------------------------------
    // WEAPON PURCHASES
    // --------------------------------------------------

    public void BuyBasicSpray() => TryPurchaseWeapon(weaponPurchases.basicSpray, 0);
    public void BuyShotgunSpray() => TryPurchaseWeapon(weaponPurchases.shotgunSpray, 1);
    public void BuyPitchfork() => TryPurchaseWeapon(weaponPurchases.pitchfork, 2);
    public void BuyScythe() => TryPurchaseWeapon(weaponPurchases.scythe, 3);
    public void BuyGrapple() => TryPurchaseWeapon(weaponPurchases.grapple, 4);
    public void BuySniper() => TryPurchaseWeapon(weaponPurchases.sniper, 5);
    public void BuyWaterBalloon() => TryPurchaseWeapon(weaponPurchases.waterBalloon, 6);
    public void BuyMilkChurn() => TryPurchaseWeapon(weaponPurchases.milkChurn, 7);
    public void BuyRestorativeGas() => TryPurchaseWeapon(weaponPurchases.restorativeGas, 8);

    private void TryPurchaseWeapon(PurchaseUI purchase, int weaponIndex)
    {
        if (purchase == null ||
            playerResources == null ||
            playerWeapons == null)
            return;

        if (!playerWeapons.CanPurchaseWeapon(weaponIndex))
            return;

        if (!playerResources.SpendDrops(purchase.cost))
            return;

        if (!playerWeapons.PurchaseWeapon(weaponIndex))
            return;

        UpdateButton();
    }

    // --------------------------------------------------
    // TRAP PURCHASES
    // --------------------------------------------------

    public void BuyLaunchTrap() => TryPurchaseTrap(trapPurchases.launch, playerWeapons != null ? (Action)playerWeapons.AddLaunchTrap : null);

    public void BuyPullTrap() => TryPurchaseTrap(trapPurchases.pull, playerWeapons != null ? (Action)playerWeapons.AddPullTrap : null);

    public void BuyBeehiveTrap() => TryPurchaseTrap(trapPurchases.beehive, playerWeapons != null ? (Action)playerWeapons.AddBeehiveTrap : null);

    public void BuyMagnifyingGlassTrap() => TryPurchaseTrap(trapPurchases.magnifyingGlass, playerWeapons != null ? (Action)playerWeapons.AddMagnifyingGlassTrap : null);

    public void BuySprinklerTrap() => TryPurchaseTrap(trapPurchases.sprinkler, playerWeapons != null ? (Action)playerWeapons.AddSprinklerTrap : null);

    private void TryPurchaseTrap(PurchaseUI purchase, Action addTrap)
    {
        if (purchase == null || playerResources == null || addTrap == null)
            return;

        if (!playerResources.SpendDrops(purchase.cost))
            return;

        addTrap.Invoke();
        UpdateButton();
    }

    // --------------------------------------------------
    // ITEM SELECTION
    // --------------------------------------------------

    private void SelectItem(int index)
    {
        if (itemSelections == null || index < 0 || index >= itemSelections.Length)
            return;

        CloseAllUpgradePanels();

        for (int i = 0; i < itemSelections.Length; i++)
        {
            if (itemSelections[i].Outline != null)
                itemSelections[i].Outline.enabled = i == index;
        }

        ItemSelection selected = itemSelections[index];

        SetPanelActive(selected.Panel, true);

        if (selected.NameText != null)
            selected.NameText.text = selected.Name;

        if (selected.DescriptionText != null)
            selected.DescriptionText.text = selected.Description;

        UpdateButton();
    }

    private void CloseAllUpgradePanels()
    {
        if (itemSelections == null)
            return;

        foreach (ItemSelection item in itemSelections)
        {
            if (item.Panel != null)
                item.Panel.SetActive(false);
        }
    }

    public void ShowBasicSprayUpgrades() => SelectItem(0);
    public void ShowShotgunSprayUpgrades() => SelectItem(1);
    public void ShowPitchforkUpgrades() => SelectItem(2);
    public void ShowScytheUpgrades() => SelectItem(3);
    public void ShowGrappleUpgrades() => SelectItem(4);
    public void ShowSniperUpgrades() => SelectItem(5);
    public void ShowWaterBallonLauncherUpgrades() => SelectItem(6);
    public void ShowMilkChurnLauncherUpgrades() => SelectItem(7);
    public void ShowRestorativeGasThrowerUpgrades() => SelectItem(8);
    public void ShowLaunchTrapUpgrades() => SelectItem(9);
    public void ShowPullTrapUpgrades() => SelectItem(10);
    public void ShowBeehiveTrapUpgrades() => SelectItem(11);
    public void ShowSprinklerTrapUpgrades() => SelectItem(12);
    public void ShowMagnifyingGlassTrapUpgrades() => SelectItem(13);

    // --------------------------------------------------
    // UPGRADE PURCHASE CALLBACKS
    // --------------------------------------------------

    private void PurchaseUpgrade(string upgradeID)
    {
        if (upgrades == null)
            return;

        foreach (UpgradeDefinition upgrade in upgrades)
        {
            if (upgrade.ID != upgradeID)
                continue;

            TryPurchaseUpgrade(upgrade.ID, upgrade.Cost, upgrade.Activate);

            return;
        }
    }

    public void BuyImprovedSpray() => PurchaseUpgrade(ImprovedSprayID);
    public void BuyMaximumHealthDamage() => PurchaseUpgrade(MaximumHealthDamageID);
    public void BuyKnockback() => PurchaseUpgrade(KnockbackID);

    public void BuyDoubleBurst() => PurchaseUpgrade(DoubleBurstID);
    public void BuyFasterReload() => PurchaseUpgrade(FasterReloadID);
    public void BuyCloseRangeDamage() => PurchaseUpgrade(CloseRangeDamageID);

    public void BuyPitchforkBonusDamage() => PurchaseUpgrade(PitchforkDamageID);
    public void BuyPitchforkEnemySlowdown() => PurchaseUpgrade(PitchforkSlowdownID);
    public void BuyPitchforkBonusDrops() => PurchaseUpgrade(PitchforkDropsID);

    public void BuyScytheRadius() => PurchaseUpgrade(ScytheRadiusID);
    public void BuyScytheAttackSpeed() => PurchaseUpgrade(ScytheAttackSpeedID);
    public void BuyScytheHealthOnKill() => PurchaseUpgrade(ScytheHealthOnKillID);

    public void BuyReinforcedGraft() => PurchaseUpgrade(GrappleSpeedID);
    public void BuyCorrosiveGraft() => PurchaseUpgrade(GrappleEnemyVulnerabilityID);
    public void BuyGrapplerMomentum() => PurchaseUpgrade(GrappleHealthAndSpeedOnKillID);

    public void BuySniperFasterReload() => PurchaseUpgrade(SniperFasterReloadID);
    public void BuySniperExposedWeakPoints() => PurchaseUpgrade(SniperExposeWeakpointsID);
    public void BuySniperRicochetShot() => PurchaseUpgrade(SniperRicochetShotID);

    public void BuyWaterBalloonExpandedSplash() => PurchaseUpgrade(BalloonExpandedSplashID);
    public void BuyWaterBalloonRubberizedBalloon() => PurchaseUpgrade(BalloonRubberizedBalloonID);
    public void BuyWaterBalloonConcussiveAftershock() => PurchaseUpgrade(BallonCuncussiveAfterShockID);

    public void BuyMilkChurnRapidChurnin() => PurchaseUpgrade(MilkChurnFasterReloadID);
    public void BuyMilkChurnLargerSplatter() => PurchaseUpgrade(MilkChurnLargerSplatterID);
    public void BuyMilkChurnUnstableMixture() => PurchaseUpgrade(MilkChurnUnstableMixtureID);

    public void BuyRestorativeGasExtendedCanister() => PurchaseUpgrade(RestorativeGasExpandedCanisterID);
    public void BuyRestorativeGasRapidRefill() => PurchaseUpgrade(RestorativeGasFasterReloadID);
    public void BuyRestorativeGasLingeringTonic() => PurchaseUpgrade(RestorativeGasLingeringTonicID);

    public void BuyLaunchTrapHighForceLaunch() => PurchaseUpgrade(LaunchTrapHighForceLaunchID);
    public void BuyLaunchTrapConcussiveBlast() => PurchaseUpgrade(LaunchTrapConcussiveExhaustID);
    public void BuyLaunchTrapDoubleTrigger() => PurchaseUpgrade(LaunchTrapDoubleTriggerID);

    public void BuyPullTrapExpandedVacuum() => PurchaseUpgrade(PullTrapExpandedVacuumID);
    public void BuyPullTrapExtendedReach() => PurchaseUpgrade(PullTrapExtendedReachID);
    public void BuyPullTrapForcefulLaunch() => PurchaseUpgrade(PullTrapForcefulLaunchID);

    public void BuyBeehiveTrapExpandedColony() => PurchaseUpgrade(BeehiveTrapExpandedColonyID);
    public void BuyBeehiveTrapFrenziedSwarm() => PurchaseUpgrade(BeehiveTrapFrenziedSwarmID);
    public void BuyBeehiveTrapSwarmReplication() => PurchaseUpgrade(BeehiveTrapSwarmReplicationID);

    public void BuySprinklerTrapPressurizedSpray() => PurchaseUpgrade(SprinklerTrapPressurizedSprayID);
    public void BuySprinklerTrapRapidRotation() => PurchaseUpgrade(SprinklerTrapRapidRotationID);
    public void BuySprinklerTrapReinforcedBarriers() => PurchaseUpgrade(SprinklerTrapReinforcedBarriersID);

    public void BuyMagnifyingGlassTrapFocusedSunlight() => PurchaseUpgrade(MagnifyingGlassTrapFocusedSunlightID);
    public void BuyMagnifyingGlassTrapIntensifiedHeat() => PurchaseUpgrade(MagnifyingGlassTrapIntensifiedHeatID);
    public void BuyMagnifyingGlassTrapRefractedBeam() => PurchaseUpgrade(MagnifyingGlassTrapRefractedBeamID);

    // --------------------------------------------------
    // SHARED PURCHASE LOGIC
    // --------------------------------------------------

    private void TryPurchaseUpgrade(string upgradeID, int cost, Action activateUpgrade)
    {
        if (playerResources == null || upgradeManager == null || activateUpgrade == null)
            return;

        if (upgradeManager.HasUpgrade(upgradeID))
            return;

        if (!playerResources.SpendDrops(cost))
            return;

        activateUpgrade.Invoke();
        upgradeManager.RegisterUpgrade(upgradeID);

        UpdateButton();
    }

    // --------------------------------------------------
    // BUTTON AVAILABILITY
    // --------------------------------------------------

    private void UpdateButton()
    {
        if (playerResources == null)
            return;

        int drops = playerResources.TotalDrops;

        UpdateWeaponButton(weaponPurchases.basicSpray, 0, drops);
        UpdateWeaponButton(weaponPurchases.shotgunSpray, 1, drops);
        UpdateWeaponButton(weaponPurchases.pitchfork, 2, drops);
        UpdateWeaponButton(weaponPurchases.scythe, 3, drops);
        UpdateWeaponButton(weaponPurchases.grapple, 4, drops);
        UpdateWeaponButton(weaponPurchases.sniper, 5, drops);
        UpdateWeaponButton(weaponPurchases.waterBalloon, 6, drops);
        UpdateWeaponButton(weaponPurchases.milkChurn, 7, drops);
        UpdateWeaponButton(weaponPurchases.restorativeGas, 8, drops);

        UpdateTrapButton(trapPurchases.launch, drops);
        UpdateTrapButton(trapPurchases.pull, drops);
        UpdateTrapButton(trapPurchases.beehive, drops);
        UpdateTrapButton(trapPurchases.magnifyingGlass, drops);
        UpdateTrapButton(trapPurchases.sprinkler, drops);

        if (upgrades == null)
            return;

        foreach (UpgradeDefinition upgrade in upgrades)
        {
            if (upgrade.Button == null)
                continue;

            upgrade.Button.interactable = upgradeManager != null && upgrade.Activate != null && !upgradeManager.HasUpgrade(upgrade.ID) && drops >= upgrade.Cost;
        }
    }

    private void UpdateWeaponButton(PurchaseUI purchase, int weaponIndex, int drops)
    {
        if (purchase == null || purchase.button == null)
            return;

        purchase.button.interactable = playerWeapons != null && playerWeapons.CanPurchaseWeapon(weaponIndex) && drops >= purchase.cost;
    }

    private void UpdateTrapButton(PurchaseUI purchase, int drops)
    {
        if (purchase != null && purchase.button != null)
            purchase.button.interactable = drops >= purchase.cost;
    }

    private void OnDestroy()
    {
        if (tabs.purchaseTrapsButton != null)
            tabs.purchaseTrapsButton.onClick.RemoveListener(ShowPurchaseTrapsTab);

        if (tabs.upgradeTrapsButton != null)
            tabs.upgradeTrapsButton.onClick.RemoveListener(ShowUpgradeTrapsTab);

        if (tabs.upgradeWeaponsButton != null)
            tabs.upgradeWeaponsButton.onClick.RemoveListener(ShowUpgradeWeaponsTab);
    }
}
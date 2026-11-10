using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using NUnit.Framework;

/// <summary>
/// Manages the upgrade menu and purchases traps using player resources.
/// </summary>
public class UpgradeMenuScript : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private GameObject hotbarUI;
    [SerializeField] private GameObject ammoTextUI;

    // Reference to the upgrade menu UI.
    [Header("Menu")]
    [SerializeField] private GameObject upgradeMenu;

    [Header("Tabs")]
    [SerializeField] private Button purchaseTrapsTabButton;
    [SerializeField] private Button upgradeTrapsTabButton;
    [SerializeField] private Button upgradeWeaponsTabButton;

    [SerializeField] private GameObject purchaseTrapsPanel;
    [SerializeField] private GameObject upgradeTrapsPanel;
    [SerializeField] private GameObject upgradeWeaponsPanel;

    [Header("Tab Colors")]
    [SerializeField] private Color selectedTabColor = new Color(183f / 255f, 121f / 255f, 54f / 255f);
    [SerializeField] private Color normalTabColor = new Color(52f / 255f, 70f / 255f, 79f / 255f);

    // Player components affected by opening and using the menu.
    [Header("Player")]
    [SerializeField] private PlayerResourceScript playerResources;
    [SerializeField] private PlayerLookScript playerLook;
    [SerializeField] private PlayerWeaponsScript playerWeapons;
    [SerializeField] private UpgradeManagerScript upgradeManager;

    // Launch trap purchase settings and UI references.
    [Header("Launch Trap Purchase")]
    [SerializeField] private int launchTrapCost = 10;
    [SerializeField] private Button launchTrapButton;
    [SerializeField] private TextMeshProUGUI launchTrapCostText;

    // Pull trap purchase settings and UI references.
    [Header("Pull Trap Purchase")]
    [SerializeField] private int pullTrapCost = 15;
    [SerializeField] private Button pullTrapButton;
    [SerializeField] private TextMeshProUGUI pullTrapCostText;

    // Beehive trap purchase settings and UI references.
    [Header("Beehive Trap Purchase")]
    [SerializeField] private int beehiveTrapCost = 10;
    [SerializeField] private Button beehiveTrapButton;
    [SerializeField] private TextMeshProUGUI beehiveTrapCostText;

    // Magnifying glass trap purchase settings and UI references.
    [Header("Magnifying Glass Trap Purchase")]
    [SerializeField] private int magnifyingGlassTrapCost = 10;
    [SerializeField] private Button magnifyingGlassTrapButton;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapCostText;

    // Magnifying glass trap purchase settings and UI references.
    [Header("Sprinkler Trap Purchase")]
    [SerializeField] private int sprinklerTrapCost = 10;
    [SerializeField] private Button sprinklerTrapButton;
    [SerializeField] private TextMeshProUGUI sprinklerTrapCostText;

    [Header("Basic Spray Selection")]
    [SerializeField] private GameObject basicSprayUpgradePanel;
    [SerializeField] private TextMeshProUGUI basicSprayNameText;
    [SerializeField] private TextMeshProUGUI basicSprayDescriptionText;

    [Header("Shotgun Spray Selection")]
    [SerializeField] private GameObject shotgunSprayUpgradePanel;
    [SerializeField] private TextMeshProUGUI shotgunSprayNameText;
    [SerializeField] private TextMeshProUGUI shotgunSprayDescriptionText;

    [Header("Pitchfork Selection")]
    [SerializeField] private GameObject pitchforkUpgradePanel;
    [SerializeField] private TextMeshProUGUI pitchforkNameText;
    [SerializeField] private TextMeshProUGUI pitchforkDescriptionText;

    [Header("Scythe Selection")]
    [SerializeField] private GameObject scytheUpgradePanel;
    [SerializeField] private TextMeshProUGUI scytheNameText;
    [SerializeField] private TextMeshProUGUI scytheDescriptionText;

    [Header("Grapple Selection")]
    [SerializeField] private GameObject grappleUpgradePanel;
    [SerializeField] private TextMeshProUGUI grappleNameText;
    [SerializeField] private TextMeshProUGUI grappleDescriptionText;

    [Header("Sniper Selection")]
    [SerializeField] private GameObject sniperUpgradePanel;
    [SerializeField] private TextMeshProUGUI sniperNameText;
    [SerializeField] private TextMeshProUGUI sniperDescriptionText;

    [Header("Water Balloon Launcher Selection")]
    [SerializeField] private GameObject waterBalloonUpgradePanel;
    [SerializeField] private TextMeshProUGUI waterBalloonNameText;
    [SerializeField] private TextMeshProUGUI waterBalloonDescriptionText;

    [Header("Milk Churn Launcher Selection")]
    [SerializeField] private GameObject milkChurnUpgradePanel;
    [SerializeField] private TextMeshProUGUI milkChurnNameText;
    [SerializeField] private TextMeshProUGUI milkChurnDescriptionText;

    [Header("Restorative Gas Thrower Selection")]
    [SerializeField] private GameObject restorativeGasUpgradePanel;
    [SerializeField] private TextMeshProUGUI restorativeGasNameText;
    [SerializeField] private TextMeshProUGUI restorativeGasDescriptionText;

    [Header("Basic Spray - Improved Spray")]
    [SerializeField] private int improvedSprayCost = 10;
    [SerializeField] private Button improvedSprayButton;
    [SerializeField] private TextMeshProUGUI improvedSprayCostText;

    [Header("Basic Spray - Maximum Health Damage")]
    [SerializeField] private int maximumHealthDamageCost = 15;
    [SerializeField] private Button maximumHealthDamageButton;
    [SerializeField] private TextMeshProUGUI maximumHealthDamageCostText;

    [Header("Basic Spray - Knockback")]
    [SerializeField] private int knockbackCost = 20;
    [SerializeField] private Button knockbackButton;
    [SerializeField] private TextMeshProUGUI knockbackCostText;

    [Header("Shotgun Spray - Double Burst")]
    [SerializeField] private int doubleBurstCost = 15;
    [SerializeField] private Button doubleBurstButton;
    [SerializeField] private TextMeshProUGUI doubleBurstCostText;

    [Header("Shotgun Spray - Faster Reload")]
    [SerializeField] private int fasterReloadCost = 10;
    [SerializeField] private Button fasterReloadButton;
    [SerializeField] private TextMeshProUGUI fasterReloadCostText;

    [Header("Shotgun Spray - Close Range Damage")]
    [SerializeField] private int closeRangeDamageCost = 15;
    [SerializeField] private Button closeRangeDamageButton;
    [SerializeField] private TextMeshProUGUI closeRangeDamageCostText;

    [Header("Pitchfork - Bonus Damage")]
    [SerializeField] private int pitchforkBonusDamageCost = 15;
    [SerializeField] private Button pitchforkBonusDamageButton;
    [SerializeField] private TextMeshProUGUI pitchforkBonusDamageCostText;

    [Header("Pitchfork - Enemy Slowdown")]
    [SerializeField] private int pitchforkEnemySlowdownCost = 10;
    [SerializeField] private Button pitchforkEnemySlowdownButton;
    [SerializeField] private TextMeshProUGUI pitchforkEnemySlowdownCostText;

    [Header("Pitchfork - Bonus Drops")]
    [SerializeField] private int pitchforkBonusDropsCost = 15;
    [SerializeField] private Button pitchforkBonusDropsButton;
    [SerializeField] private TextMeshProUGUI pitchforkBonusDropsCostText;

    [Header("Scythe - Increased Attack Radius")]
    [SerializeField] private int scytheRadiusCost = 10;
    [SerializeField] private Button scytheRadiusButton;
    [SerializeField] private TextMeshProUGUI scytheRadiusCostText;

    [Header("Scythe - Increasing Attack Speed")]
    [SerializeField] private int scytheAttackSpeedCost = 15;
    [SerializeField] private Button scytheAttackSpeedButton;
    [SerializeField] private TextMeshProUGUI scytheAttackSpeedCostText;

    [Header("Scythe - Health on Kill")]
    [SerializeField] private int scytheHealthOnKillCost = 20;
    [SerializeField] private Button scytheHealthOnKillButton;
    [SerializeField] private TextMeshProUGUI scytheHealthOnKillCostText;

    [Header("Grapple - Grapple speed and cooldown")]
    [SerializeField] private int grappleSpeedCost = 10;
    [SerializeField] private Button grappleSpeedButton;
    [SerializeField] private TextMeshProUGUI grappleSpeedCostText;

    [Header("Grapple - Increased enemy vulnerablility")]
    [SerializeField] private int grappleEnemyVulnerabilityCost = 15;
    [SerializeField] private Button grappleEnemyVulnerabilityButton;
    [SerializeField] private TextMeshProUGUI grappleEnemyVulnerabilityCostText;

    [Header("Grapple - Health & Speed on Kill")]
    [SerializeField] private int grappleHealthAndSpeedOnKillCost = 20;
    [SerializeField] private Button grappleHealthAndSpeedOnKillButton;
    [SerializeField] private TextMeshProUGUI grappleHealthAndSpeedOnKillCostText;

    [Header("Sniper - Faster Reload")]
    [SerializeField] private int sniperFasterReloadCost = 10;
    [SerializeField] private Button sniperFasterReloadButton;
    [SerializeField] private TextMeshProUGUI sniperFasterReloadCostText;

    [Header("Sniper - Expose Weakpoints")]
    [SerializeField] private int sniperExposeWeakpointsCost = 30;
    [SerializeField] private Button sniperExposeWeakpointsButton;
    [SerializeField] private TextMeshProUGUI sniperExposeWeakpointsCostText;

    [Header("Sniper - Ricochet Shot")]
    [SerializeField] private int sniperRicochetShotCost = 25;
    [SerializeField] private Button sniperRicochetShotButton;
    [SerializeField] private TextMeshProUGUI sniperRicochetShotCostText;

    [Header("Water Balloon Launcher - Expanded Splash")]
    [SerializeField] private int balloonExpandedSplashCost = 15;
    [SerializeField] private Button balloonExpandedSplashButton;
    [SerializeField] private TextMeshProUGUI balloonExpandedSplashCostText;

    [Header("Water Balloon Launcher - Rubberized Balloon")]
    [SerializeField] private int balloonRubberizedBalloonCost = 25;
    [SerializeField] private Button balloonRubberizedBalloonButton;
    [SerializeField] private TextMeshProUGUI balloonRubberizedBalloonCostText;

    [Header("Water Balloon Launcher - Concussive Aftershock")]
    [SerializeField] private int balloonConcussiveAftershockCost = 25;
    [SerializeField] private Button balloonConcussiveAftershockButton;
    [SerializeField] private TextMeshProUGUI balloonConcussiveAftershockCostText;

    [Header("Milk Churn Launcher - Faster Reload")]
    [SerializeField] private int milkChurnFasterReloadCost = 10;
    [SerializeField] private Button milkChurnFasterReloadButton;
    [SerializeField] private TextMeshProUGUI milkChurnFasterReloadCostText;

    [Header("Milk Churn Launcher - Larger Splatter")]
    [SerializeField] private int milkChurnLargerSplatterCost = 20;
    [SerializeField] private Button milkChurnLargerSplatterButton;
    [SerializeField] private TextMeshProUGUI milkChurnLargerSplatterCostText;

    [Header("Milk Churn Launcher - Unstable Mixture")]
    [SerializeField] private int milkChurnUnstableMixtureCost = 25;
    [SerializeField] private Button milkChurnUnstableMixtureButton;
    [SerializeField] private TextMeshProUGUI milkChurnUnstableMixtureCostText;

    [Header("Restorative Gas Thrower - Expanded Canister")]
    [SerializeField] private int restorativeGasExpandedCanisterCost = 25;
    [SerializeField] private Button restorativeGasExpandedCanisterButton;
    [SerializeField] private TextMeshProUGUI restorativeGasExpandedCanisterCostText;

    [Header("Restorative Gas Thrower - Faster Reload")]
    [SerializeField] private int restorativeGasFasterReloadCost = 15;
    [SerializeField] private Button restorativeGasFasterReloadButton;
    [SerializeField] private TextMeshProUGUI restorativeGasFasterReloadCostText;

    [Header("Restorative Gas Thrower - Lingering Tonic")]
    [SerializeField] private int restorativeGasLingeringTonicCost = 25;
    [SerializeField] private Button restorativeGasLingeringTonicButton;
    [SerializeField] private TextMeshProUGUI restorativeGasLingeringTonicCostText;

    [Header("Launch Trap Selection")]
    [SerializeField] private GameObject launchTrapUpgradePanel;
    [SerializeField] private TextMeshProUGUI launchTrapNameText;
    [SerializeField] private TextMeshProUGUI launchTrapDescriptionText;

    [Header("Pull Trap Selection")]
    [SerializeField] private GameObject pullTrapUpgradePanel;
    [SerializeField] private TextMeshProUGUI pullTrapNameText;
    [SerializeField] private TextMeshProUGUI pullTrapDescriptionText;

    [Header("Beehive Trap Selection")]
    [SerializeField] private GameObject beehiveTrapUpgradePanel;
    [SerializeField] private TextMeshProUGUI beehiveTrapNameText;
    [SerializeField] private TextMeshProUGUI beehiveTrapDescriptionText;

    [Header("Sprinkler Trap Selection")]
    [SerializeField] private GameObject sprinklerTrapUpgradePanel;
    [SerializeField] private TextMeshProUGUI sprinklerTrapNameText;
    [SerializeField] private TextMeshProUGUI sprinklerTrapDescriptionText;

    [Header("Magnifying Glass Trap Selection")]
    [SerializeField] private GameObject magnifyingGlassTrapUpgradePanel;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapNameText;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapDescriptionText;

    [Header("Launch Trap - High Force Launch")]
    [SerializeField] private int launchTrapHighForceLaunchCost = 15;
    [SerializeField] private Button launchTrapHighForceLaunchButton;
    [SerializeField] private TextMeshProUGUI launchTrapHighForceLaunchCostText;

    [Header("Launch Trap - Concussive Exhaust")]
    [SerializeField] private int launchTrapConcussiveExhaustCost = 20;
    [SerializeField] private Button launchTrapConcussiveExhaustButton;
    [SerializeField] private TextMeshProUGUI launchTrapConcussiveExhaustCostText;

    [Header("Launch Trap - Double Trigger")]
    [SerializeField] private int launchTrapDoubleTriggerCost = 30;
    [SerializeField] private Button launchTrapDoubleTriggerButton;
    [SerializeField] private TextMeshProUGUI launchTrapDoubleTriggerCostText;

    [Header("Pull Trap - Expanded Vacuum")]
    [SerializeField] private int pullTrapExpandedVacuumCost = 30;
    [SerializeField] private Button pullTrapExpandedVacuumButton;
    [SerializeField] private TextMeshProUGUI pullTrapExpandedVacuumCostText;

    [Header("Pull Trap - Extended Reach")]
    [SerializeField] private int pullTrapExtendedReachCost = 15;
    [SerializeField] private Button pullTrapExtendedReachButton;
    [SerializeField] private TextMeshProUGUI pullTrapExtendedReachCostText;

    [Header("Pull Trap - Forceful Launch")]
    [SerializeField] private int pullTrapForcefulLaunchCost = 25;
    [SerializeField] private Button pullTrapForcefulLaunchButton;
    [SerializeField] private TextMeshProUGUI pullTrapForcefulLaunchCostText;

    [Header("Beehive Trap - Expanded Colony")]
    [SerializeField] private int beehiveTrapExpandedColonyCost = 25;
    [SerializeField] private Button beehiveTrapExpandedColonyButton;
    [SerializeField] private TextMeshProUGUI beehiveTrapExpandedColonyCostText;

    [Header("Beehive Trap - Frenzied Swarm")]
    [SerializeField] private int beehiveTrapFrenziedSwarmCost = 15;
    [SerializeField] private Button beehiveTrapFrenziedSwarmButton;
    [SerializeField] private TextMeshProUGUI beehiveTrapFrenziedSwarmCostText;

    [Header("Beehive Trap - Swarm Replication")]
    [SerializeField] private int beehiveTrapSwarmReplicationCost = 25;
    [SerializeField] private Button beehiveTrapSwarmReplicationButton;
    [SerializeField] private TextMeshProUGUI beehiveTrapSwarmReplicationCostText;

    [Header("Sprinkler Trap - Pressurized Spray")]
    [SerializeField] private int sprinklerTrapPressurizedSprayCost = 15;
    [SerializeField] private Button sprinklerTrapPressurizedSprayButton;
    [SerializeField] private TextMeshProUGUI sprinklerTrapPressurizedSprayCostText;

    [Header("Sprinkler Trap - Rapid Rotation")]
    [SerializeField] private int sprinklerTrapRapidRotationCost = 20;
    [SerializeField] private Button sprinklerTrapRapidRotationButton;
    [SerializeField] private TextMeshProUGUI sprinklerTrapRapidRotationCostText;

    [Header("Sprinkler Trap - Reinforced Barriers")]
    [SerializeField] private int sprinklerTrapReinforcedBarriersCost = 20;
    [SerializeField] private Button sprinklerTrapReinforcedBarriersButton;
    [SerializeField] private TextMeshProUGUI sprinklerTrapReinforcedBarriersCostText;

    [Header("Magnifying Glass Trap - Focused Sunlight")]
    [SerializeField] private int magnifyingGlassTrapFocusedSunlightCost = 25;
    [SerializeField] private Button magnifyingGlassTrapFocusedSunlightButton;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapFocusedSunlightCostText;

    [Header("Magnifying Glass Trap - Intensified Heat")]
    [SerializeField] private int magnifyingGlassTrapIntensifiedHeatCost = 20;
    [SerializeField] private Button magnifyingGlassTrapIntensifiedHeatButton;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapIntensifiedHeatCostText;

    [Header("Magnifying Glass Trap - Refracted Beam")]
    [SerializeField] private int magnifyingGlassTrapRefractedBeamCost = 25;
    [SerializeField] private Button magnifyingGlassTrapRefractedBeamButton;
    [SerializeField] private TextMeshProUGUI magnifyingGlassTrapRefractedBeamCostText;

    [Header("Upgrade Scripts")]
    [SerializeField] private BasicSprayUpgradeScript basicSprayUpgrade;
    [SerializeField] private ShotgunSprayUpgradeScript shotgunSprayUpgrade;
    [SerializeField] private PitchforkUpgradeScript pitchforkUpgrade;
    [SerializeField] private ScytheUpgradeScript scytheUpgrade;
    [SerializeField] private GrappleUpgradeScript grappleUpgrade;
    [SerializeField] private SniperUpgradeScript sniperUpgrade;
    [SerializeField] private WaterBalloonUpgradeScript waterBalloonUpgrade;
    [SerializeField] private RestorativeGasUpgradeScript restorativeGasUpgrade;
    [SerializeField] private MilkChurnUpgradeScript milkChurnUpgrade;
    [SerializeField] private LaunchTrapUpgradeScript launchTrapUpgrade;
    [SerializeField] private PullTrapUpgradeScript pullTrapUpgrade;
    [SerializeField] private BeehiveTrapUpgradeScript beehiveTrapUpgrade;
    [SerializeField] private SprinklerTrapUpgradeScript sprinklerUpgrade;
    [SerializeField] private MagnifyingGlassTrapUpgradeScript magnifyingGlassTrapUpgrade;

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

    private const string GrappleSpeedID = "gapple_speed";
    private const string GrappleEnemyVulnerabilityID = "grapple_enemy_vulnerability";
    private const string GrappleHealthAndSpeedOnKillID = "grapple_health_and_speed_on_kill";

    private const string SniperFasterReloadID = "sniper_faster_reload";
    private const string SniperExposeWeakpointsID = "sniper_expose_weakpoints";
    private const string SniperRicochetShotID = "sniper_ricochet_shot";

    private const string BalloonExpandedSplashID = "balloon_expanded_splash";
    private const string BalloonRubberizedBalloonID = "balloon_rubberized_balloon";
    private const string BallonCuncussiveAfterShockID = "ballon_concussive_aftershock";

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

    // Tracks whether the upgrade menu is currently open.
    private bool menuOpen = false;

    // Allows other scripts to check the menu state.
    public bool IsMenuOpen => menuOpen;

    // Initializes the menu and displays trap prices.
    private void Start()
    {
        upgradeMenu.SetActive(false);

        if (launchTrapCostText != null)
        {
            launchTrapCostText.text = launchTrapCost.ToString();
        }

        if (pullTrapCostText != null)
        {
            pullTrapCostText.text = pullTrapCost.ToString();
        }

        if (beehiveTrapCostText != null)
        {
            beehiveTrapCostText.text = beehiveTrapCost.ToString();
        }

        if (magnifyingGlassTrapCostText != null)
        {
            magnifyingGlassTrapCostText.text = magnifyingGlassTrapCost.ToString();
        }

        if (sprinklerTrapCostText != null)
        {
            sprinklerTrapCostText.text = sprinklerTrapCost.ToString();
        }

        if (improvedSprayCostText != null)
            improvedSprayCostText.text = improvedSprayCost.ToString();

        if (maximumHealthDamageCostText != null)
            maximumHealthDamageCostText.text = maximumHealthDamageCost.ToString();

        if (knockbackCostText != null)
            knockbackCostText.text = knockbackCost.ToString();

        if (doubleBurstCostText != null)
            doubleBurstCostText.text = doubleBurstCost.ToString();

        if (fasterReloadCostText != null)
            fasterReloadCostText.text = fasterReloadCost.ToString();

        if (closeRangeDamageCostText != null)
            closeRangeDamageCostText.text = closeRangeDamageCost.ToString();

        if (pitchforkBonusDamageCostText != null)
            pitchforkBonusDamageCostText.text = pitchforkBonusDamageCost.ToString();

        if (pitchforkEnemySlowdownCostText != null)
            pitchforkEnemySlowdownCostText.text = pitchforkEnemySlowdownCost.ToString();

        if (pitchforkBonusDropsCostText != null)
            pitchforkBonusDropsCostText.text = pitchforkBonusDropsCost.ToString();

        if (scytheRadiusCostText != null)
            scytheRadiusCostText.text = scytheRadiusCost.ToString();

        if (scytheAttackSpeedCostText != null)
            scytheAttackSpeedCostText.text = scytheAttackSpeedCost.ToString();

        if (scytheHealthOnKillCostText != null)
            scytheHealthOnKillCostText.text = scytheHealthOnKillCost.ToString();

        if (grappleSpeedCostText != null)
            grappleSpeedCostText.text = grappleSpeedCost.ToString();

        if (grappleEnemyVulnerabilityCostText != null)
            grappleEnemyVulnerabilityCostText.text = grappleEnemyVulnerabilityCost.ToString();

        if (grappleHealthAndSpeedOnKillCostText != null)
            grappleHealthAndSpeedOnKillCostText.text = grappleHealthAndSpeedOnKillCost.ToString();

        if (sniperFasterReloadCostText != null)
            sniperFasterReloadCostText.text = sniperFasterReloadCost.ToString();

        if (sniperExposeWeakpointsCostText != null)
            sniperExposeWeakpointsCostText.text = sniperExposeWeakpointsCost.ToString();

        if (sniperRicochetShotCostText != null)
            sniperRicochetShotCostText.text = sniperRicochetShotCost.ToString();

        if (balloonExpandedSplashCostText != null)
            balloonExpandedSplashCostText.text = balloonExpandedSplashCost.ToString();

        if (balloonRubberizedBalloonCostText != null)
            balloonRubberizedBalloonCostText.text = balloonRubberizedBalloonCost.ToString();

        if (balloonConcussiveAftershockCostText != null)
            balloonConcussiveAftershockCostText.text = balloonConcussiveAftershockCost.ToString();

        if (milkChurnFasterReloadCostText != null)
            milkChurnFasterReloadCostText.text = milkChurnFasterReloadCost.ToString();

        if (milkChurnLargerSplatterCostText != null)
            milkChurnLargerSplatterCostText.text = milkChurnLargerSplatterCost.ToString();

        if (milkChurnUnstableMixtureCostText != null)
            milkChurnUnstableMixtureCostText.text = milkChurnUnstableMixtureCost.ToString();

        if (restorativeGasExpandedCanisterCostText != null)
            restorativeGasExpandedCanisterCostText.text = restorativeGasExpandedCanisterCost.ToString();

        if (restorativeGasFasterReloadCostText != null)
            restorativeGasFasterReloadCostText.text = restorativeGasFasterReloadCost.ToString();

        if (restorativeGasLingeringTonicCostText != null)
            restorativeGasLingeringTonicCostText.text = restorativeGasLingeringTonicCost.ToString();

        if (launchTrapHighForceLaunchCostText != null)
            launchTrapHighForceLaunchCostText.text = launchTrapHighForceLaunchCost.ToString();

        if (launchTrapConcussiveExhaustCostText != null)
            launchTrapConcussiveExhaustCostText.text = launchTrapConcussiveExhaustCost.ToString();

        if (launchTrapDoubleTriggerCostText != null)
            launchTrapDoubleTriggerCostText.text = launchTrapDoubleTriggerCost.ToString();

        if (pullTrapExpandedVacuumCostText != null)
            pullTrapExpandedVacuumCostText.text = pullTrapExpandedVacuumCost.ToString();

        if (pullTrapExtendedReachCostText != null)
            pullTrapExtendedReachCostText.text = pullTrapExtendedReachCost.ToString();

        if (pullTrapForcefulLaunchCostText != null)
            pullTrapForcefulLaunchCostText.text = pullTrapForcefulLaunchCost.ToString();

        if (beehiveTrapExpandedColonyCostText != null)
            beehiveTrapExpandedColonyCostText.text = beehiveTrapExpandedColonyCost.ToString();

        if (beehiveTrapFrenziedSwarmCostText != null)
            beehiveTrapFrenziedSwarmCostText.text = beehiveTrapFrenziedSwarmCost.ToString();

        if (beehiveTrapSwarmReplicationCostText != null)
            beehiveTrapSwarmReplicationCostText.text = beehiveTrapSwarmReplicationCost.ToString();

        if (sprinklerTrapPressurizedSprayCostText != null)
            sprinklerTrapPressurizedSprayCostText.text = sprinklerTrapPressurizedSprayCost.ToString();

        if (sprinklerTrapRapidRotationCostText != null)
            sprinklerTrapRapidRotationCostText.text = sprinklerTrapRapidRotationCost.ToString();

        if (sprinklerTrapReinforcedBarriersCostText != null)
            sprinklerTrapReinforcedBarriersCostText.text = sprinklerTrapReinforcedBarriersCost.ToString();

        if (magnifyingGlassTrapFocusedSunlightCostText != null)
            magnifyingGlassTrapFocusedSunlightCostText.text = magnifyingGlassTrapFocusedSunlightCost.ToString();

        if (magnifyingGlassTrapIntensifiedHeatCostText != null)
            magnifyingGlassTrapIntensifiedHeatCostText.text = magnifyingGlassTrapIntensifiedHeatCost.ToString();

        if (magnifyingGlassTrapRefractedBeamCostText != null)
            magnifyingGlassTrapRefractedBeamCostText.text = magnifyingGlassTrapRefractedBeamCost.ToString();

        if (basicSprayUpgradePanel != null)
            basicSprayUpgradePanel.SetActive(false);

        if (shotgunSprayUpgradePanel != null)
            shotgunSprayUpgradePanel.SetActive(false);

        if (pitchforkUpgradePanel != null)
            pitchforkUpgradePanel.SetActive(false);

        if (scytheUpgradePanel != null)
            scytheUpgradePanel.SetActive(false);

        if (grappleUpgradePanel != null)
            grappleUpgradePanel.SetActive(false);

        if (sniperUpgradePanel != null)
            sniperUpgradePanel.SetActive(false);

        if (waterBalloonUpgradePanel != null)
            waterBalloonUpgradePanel.SetActive(false);

        if (milkChurnUpgradePanel != null)
            milkChurnUpgradePanel.SetActive(false);

        if (restorativeGasUpgradePanel != null)
            restorativeGasUpgradePanel.SetActive(false);

        if (launchTrapUpgradePanel != null)
            launchTrapUpgradePanel.SetActive(false);

        if (pullTrapUpgradePanel != null)
            pullTrapUpgradePanel.SetActive(false);

        if (beehiveTrapUpgradePanel != null)
            beehiveTrapUpgradePanel.SetActive(false);

        if (sprinklerTrapUpgradePanel != null)
            sprinklerTrapUpgradePanel.SetActive(false);

        if (magnifyingGlassTrapUpgradePanel != null)
            magnifyingGlassTrapUpgradePanel.SetActive(false);

        UpdateButton();

        ShowPurchaseTrapsTab();
    }

    // Keeps purchase button availability in sync with player resources.
    private void Update()
    {
        if (menuOpen)
        {
            UpdateButton();
        }
    }

    //------------------------------------------------------------------------
    // MENU OPEN / CLOSE
    // -----------------------------------------------------------------------

    // Opens the menu and notifies player controls.
    public void OpenMenu()
    {
        if (menuOpen)
            return;

        menuOpen = true;
        upgradeMenu.SetActive(true);

        if (hotbarUI != null)
            hotbarUI.SetActive(false);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(false);

        if (playerLook != null)
            playerLook.SetMenuOpen(true);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(true);

    }

    // Closes the menu and restores player controls.
    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        menuOpen = false;
        upgradeMenu.SetActive(false);

        if (hotbarUI != null)
            hotbarUI.SetActive(true);

        if (ammoTextUI != null)
            ammoTextUI.SetActive(true);

        if (playerLook != null)
            playerLook.SetMenuOpen(false);

        if (playerWeapons != null)
            playerWeapons.SetMenuOpen(false);

    }

    //------------------------------------------------------------------------
    // TABS
    // -----------------------------------------------------------------------

    public void ShowPurchaseTrapsTab()
    {
        SetActiveTab(purchaseTrapsPanel, purchaseTrapsTabButton);
    }

    public void ShowUpgradeTrapsTab()
    {
        SetActiveTab(upgradeTrapsPanel, upgradeTrapsTabButton);
    }

    public void ShowUpgradeWeaponsTab()
    {
        SetActiveTab(upgradeWeaponsPanel, upgradeWeaponsTabButton);
    }

    private void SetActiveTab(GameObject selectedPanel, Button selectedButton)
    {
        purchaseTrapsPanel.SetActive(false);
        upgradeTrapsPanel.SetActive(false);
        upgradeWeaponsPanel.SetActive(false);

        selectedPanel.SetActive(true);

        purchaseTrapsTabButton.image.color = normalTabColor;
        upgradeTrapsTabButton.image.color = normalTabColor;
        upgradeWeaponsTabButton.image.color = normalTabColor;

        selectedButton.image.color = selectedTabColor;
    }

    //------------------------------------------------------------------------
    // TRAP PURCHASING
    // -----------------------------------------------------------------------

    // Purchases a launch trap if the player can afford it.
    public void BuyLaunchTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchased = playerResources.SpendDrops(launchTrapCost);

        if (!purchased)
        {
            return;
        }

        playerWeapons.AddLaunchTrap();

        UpdateButton();
    }

    // Purchases a pull trap if the player can afford it.
    public void BuyPullTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchased = playerResources.SpendDrops(pullTrapCost);

        if (!purchased)
        {
            return;
        }

        playerWeapons.AddPullTrap();

        UpdateButton();
    }

    // Purchases a beehive trap if the player can afford it.
    public void BuyBeehiveTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchased = playerResources.SpendDrops(beehiveTrapCost);

        if (!purchased)
        {
            return;
        }

        playerWeapons.AddBeehiveTrap();

        UpdateButton();
    }

    // Purchases a magnifying glass trap if the player can afford it.
    public void BuyMagnifyingGlassTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchase = playerResources.SpendDrops(magnifyingGlassTrapCost);

        if (!purchase)
        {
            return;
        }

        playerWeapons.AddMagnifyingGlassTrap();

        UpdateButton();
    }

    // Purchases a magnifying glass trap if the player can afford it.
    public void BuySprinklerTrap()
    {
        if (playerResources == null)
            return;

        if (playerWeapons == null)
            return;

        bool purchase = playerResources.SpendDrops(sprinklerTrapCost);

        if (!purchase)
        {
            return;
        }

        playerWeapons.AddSprinklerTrap();

        UpdateButton();
    }

    //------------------------------------------------------------------------
    // ITEM SELECTION
    // -----------------------------------------------------------------------

    private void CloseAllUpgradePanels()
    {
        if (basicSprayUpgradePanel != null)
            basicSprayUpgradePanel.SetActive(false);

        if (shotgunSprayUpgradePanel != null)
            shotgunSprayUpgradePanel.SetActive(false);

        if (pitchforkUpgradePanel != null)
            pitchforkUpgradePanel.SetActive(false);

        if (scytheUpgradePanel != null)
            scytheUpgradePanel.SetActive(false);

        if (grappleUpgradePanel != null)
            grappleUpgradePanel.SetActive(false);

        if (sniperUpgradePanel != null)
            sniperUpgradePanel.SetActive(false);

        if (waterBalloonUpgradePanel != null)
            waterBalloonUpgradePanel.SetActive(false);

        if (milkChurnUpgradePanel != null)
            milkChurnUpgradePanel.SetActive(false);

        if (restorativeGasUpgradePanel != null)
            restorativeGasUpgradePanel.SetActive(false);

        if (launchTrapUpgradePanel != null)
            launchTrapUpgradePanel.SetActive(false);

        if (pullTrapUpgradePanel != null)
            pullTrapUpgradePanel.SetActive(false);

        if (beehiveTrapUpgradePanel != null)
            beehiveTrapUpgradePanel.SetActive(false);

        if (sprinklerTrapUpgradePanel != null)
            sprinklerTrapUpgradePanel.SetActive(false);

        if (magnifyingGlassTrapUpgradePanel != null)
            magnifyingGlassTrapUpgradePanel.SetActive(false);
    }

    public void ShowBasicSprayUpgrades()
    {
        CloseAllUpgradePanels();

        if (basicSprayUpgradePanel != null)
            basicSprayUpgradePanel.SetActive(true);

        if (basicSprayNameText != null)
            basicSprayNameText.text = "Basic Spray";

        if (basicSprayDescriptionText != null)
            basicSprayDescriptionText.text = "A basic spray weapon that fires a continuous stream of liquid.";

        UpdateButton();
    }

    public void ShowShotgunSprayUpgrades()
    {
        CloseAllUpgradePanels();

        if (shotgunSprayUpgradePanel != null)
            shotgunSprayUpgradePanel.SetActive(true);

        if (shotgunSprayNameText != null)
            shotgunSprayNameText.text = "Shotgun Spray";

        if (shotgunSprayDescriptionText != null)
            shotgunSprayDescriptionText.text = "A  spray weapon that fires a burst of liquid.";

        UpdateButton();
    }

    public void ShowPitchforkUpgrades()
    {
        CloseAllUpgradePanels();

        if (pitchforkUpgradePanel != null)
            pitchforkUpgradePanel.SetActive(true);

        if (pitchforkNameText != null)
            pitchforkNameText.text = "Pitchfork";

        if (pitchforkDescriptionText != null)
            pitchforkDescriptionText.text = "A sturdy pitchfork allowing for powerful thrusting attacks.";

        UpdateButton();
    }

    public void ShowScytheUpgrades()
    {
        CloseAllUpgradePanels();

        if (scytheUpgradePanel != null)
            scytheUpgradePanel.SetActive(true);

        if (scytheNameText != null)
            scytheNameText.text = "Scythe";

        if (scytheDescriptionText != null)
            scytheDescriptionText.text = "A scythe allowing for wide sweeping swings.";

        UpdateButton();
    }

    public void ShowGrappleUpgrades()
    {
        CloseAllUpgradePanels();

        if (grappleUpgradePanel != null)
            grappleUpgradePanel.SetActive(true);

        if (grappleNameText != null)
            grappleNameText.text = "Grapple";

        if (grappleDescriptionText != null)
            grappleDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowSniperUpgrades()
    {
        CloseAllUpgradePanels();

        if (sniperUpgradePanel != null)
            sniperUpgradePanel.SetActive(true);

        if (sniperNameText != null)
            sniperNameText.text = "Sniper";

        if (sniperDescriptionText != null)
            sniperDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowWaterBallonLauncherUpgrades()
    {
        CloseAllUpgradePanels();

        if (waterBalloonUpgradePanel != null)
            waterBalloonUpgradePanel.SetActive(true);

        if (waterBalloonNameText != null)
            waterBalloonNameText.text = "Water Ballon Launcher";

        if (waterBalloonDescriptionText != null)
            waterBalloonDescriptionText.text = "A powerful weapon launching water balloon explosives.";

        UpdateButton();
    }

    public void ShowMilkChurnLauncherUpgrades()
    {
        CloseAllUpgradePanels();

        if (milkChurnUpgradePanel != null)
            milkChurnUpgradePanel.SetActive(true);

        if (milkChurnNameText != null)
            milkChurnNameText.text = "Milk Churn Launcher";

        if (milkChurnDescriptionText != null)
            milkChurnDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowRestorativeGasThrowerUpgrades()
    {
        CloseAllUpgradePanels();

        if (restorativeGasUpgradePanel != null)
            restorativeGasUpgradePanel.SetActive(true);

        if (restorativeGasNameText != null)
            restorativeGasNameText.text = "Restorative Gas Thrower";

        if (restorativeGasDescriptionText != null)
            restorativeGasDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowLaunchTrapUpgrades()
    {
        CloseAllUpgradePanels();

        if (launchTrapUpgradePanel != null)
            launchTrapUpgradePanel.SetActive(true);

        if (launchTrapNameText != null)
            launchTrapNameText.text = "Launch Trap";

        if (launchTrapDescriptionText != null)
            launchTrapDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowPullTrapUpgrades()
    {
        CloseAllUpgradePanels();

        if (pullTrapUpgradePanel != null)
            pullTrapUpgradePanel.SetActive(true);

        if (pullTrapNameText != null)
            pullTrapNameText.text = "Pull Trap";

        if (pullTrapDescriptionText != null)
            pullTrapDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowBeehiveTrapUpgrades()
    {
        CloseAllUpgradePanels();

        if (beehiveTrapUpgradePanel != null)
            beehiveTrapUpgradePanel.SetActive(true);

        if (beehiveTrapNameText != null)
            beehiveTrapNameText.text = "Beehive Trap";

        if (beehiveTrapDescriptionText != null)
            beehiveTrapDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowSprinklerTrapUpgrades()
    {
        CloseAllUpgradePanels();

        if (sprinklerTrapUpgradePanel != null)
            sprinklerTrapUpgradePanel.SetActive(true);

        if (sprinklerTrapNameText != null)
            sprinklerTrapNameText.text = "Sprinkler Trap";

        if (sprinklerTrapDescriptionText != null)
            sprinklerTrapDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    public void ShowMagnifyingGlassTrapUpgrades()
    {
        CloseAllUpgradePanels();

        if (magnifyingGlassTrapUpgradePanel != null)
            magnifyingGlassTrapUpgradePanel.SetActive(true);

        if (magnifyingGlassTrapNameText != null)
            magnifyingGlassTrapNameText.text = "Magnifying Glass Trap";

        if (magnifyingGlassTrapDescriptionText != null)
            magnifyingGlassTrapDescriptionText.text = "A grapple gun providing for increased mobility, positioning, and damage amplification.";

        UpdateButton();
    }

    //------------------------------------------------------------------------
    // WEAPON UPGRADE PURCHASES
    // -----------------------------------------------------------------------

    public void BuyImprovedSpray()
    {
        TryPurchaseUpgrade(ImprovedSprayID, improvedSprayCost, basicSprayUpgrade != null ? basicSprayUpgrade.ActivateImprovedSpray : null);
    }

    public void BuyMaximumHealthDamage()
    {
        TryPurchaseUpgrade(MaximumHealthDamageID, maximumHealthDamageCost, basicSprayUpgrade != null ? basicSprayUpgrade.ActivateMaximumHealthDamage : null);
    }

    public void BuyKnockback()
    {
        TryPurchaseUpgrade(KnockbackID, knockbackCost, basicSprayUpgrade != null ? basicSprayUpgrade.ActivateKnockback : null);
    }

    public void BuyDoubleBurst()
    {

        TryPurchaseUpgrade(DoubleBurstID, doubleBurstCost, shotgunSprayUpgrade != null ? shotgunSprayUpgrade.ActivateDoubleBurst : null);
    }

    public void BuyFasterReload()
    {

        TryPurchaseUpgrade(FasterReloadID, fasterReloadCost, shotgunSprayUpgrade != null ? shotgunSprayUpgrade.ActivateFasterReload : null);
    }

    public void BuyCloseRangeDamage()
    {
        TryPurchaseUpgrade(CloseRangeDamageID, closeRangeDamageCost, shotgunSprayUpgrade != null ? shotgunSprayUpgrade.ActivateCloseRangeDamage : null);
    }

    public void BuyPitchforkBonusDamage()
    {
        TryPurchaseUpgrade(PitchforkDamageID, pitchforkBonusDamageCost, pitchforkUpgrade != null ? pitchforkUpgrade.ActivateBonusDamage : null);
    }

    public void BuyPitchforkEnemySlowdown()
    {
        TryPurchaseUpgrade(PitchforkSlowdownID, pitchforkEnemySlowdownCost, pitchforkUpgrade != null ? pitchforkUpgrade.ActivateEnemySlowdown : null);
    }

    public void BuyPitchforkBonusDrops()
    {
        TryPurchaseUpgrade(PitchforkDropsID, pitchforkBonusDropsCost, pitchforkUpgrade != null ? pitchforkUpgrade.ActivateBonusResources : null);
    }

    public void BuyScytheRadius()
    {
        TryPurchaseUpgrade(ScytheRadiusID, scytheRadiusCost, scytheUpgrade != null ? scytheUpgrade.ActivateIncreasedAttackRadius: null);
    }

    public void BuyScytheAttackSpeed()
    {
        TryPurchaseUpgrade(ScytheAttackSpeedID, scytheAttackSpeedCost, scytheUpgrade != null ? scytheUpgrade.ActivateIncreasingAttackSpeed : null);
    }

    public void BuyScytheHealthOnKill()
    {
        TryPurchaseUpgrade(ScytheHealthOnKillID, scytheHealthOnKillCost, scytheUpgrade != null ? scytheUpgrade.ActivateHealthOnKill : null);
    }

    public void BuyReinforcedGraft()
    {
        TryPurchaseUpgrade(GrappleSpeedID, grappleSpeedCost, grappleUpgrade != null ? grappleUpgrade.ActivateReinforcedGraft : null);
    }

    public void BuyCorrosiveGraft()
    {
        TryPurchaseUpgrade(GrappleEnemyVulnerabilityID, grappleEnemyVulnerabilityCost, grappleUpgrade != null ? grappleUpgrade.ActivateCorrosiveGraft : null);
    }

    public void BuyGrapplerMomentum()
    {
        TryPurchaseUpgrade(GrappleHealthAndSpeedOnKillID, grappleHealthAndSpeedOnKillCost, grappleUpgrade != null ? grappleUpgrade.ActivateGrapplerMomentum : null);
    }

    public void BuySniperFasterReload()
    {
        TryPurchaseUpgrade(SniperFasterReloadID, sniperFasterReloadCost, sniperUpgrade != null ? sniperUpgrade.ActivateFasterReload : null);
    }

    public void BuySniperExposedWeakPoints()
    {
        TryPurchaseUpgrade(SniperExposeWeakpointsID, sniperExposeWeakpointsCost, sniperUpgrade != null ? sniperUpgrade.ActivateExposedWeakPoints : null);
    }

    public void BuySniperRicochetShot()
    {
        TryPurchaseUpgrade(SniperRicochetShotID, sniperRicochetShotCost, sniperUpgrade != null ? sniperUpgrade.ActivateRicochetShot : null);
    }

    public void BuyWaterBalloonExpandedSplash()
    {
        TryPurchaseUpgrade(BalloonExpandedSplashID, balloonExpandedSplashCost, waterBalloonUpgrade != null ? waterBalloonUpgrade.ActivateExpandedSplash : null);
    }

    public void BuyWaterBalloonRubberizedBalloon()
    {
        TryPurchaseUpgrade(BalloonRubberizedBalloonID, balloonRubberizedBalloonCost, waterBalloonUpgrade != null ? waterBalloonUpgrade.ActivateRubberizedBalloon : null);
    }

    public void BuyWaterBalloonConcussiveAftershock()
    {
        TryPurchaseUpgrade(BallonCuncussiveAfterShockID, balloonConcussiveAftershockCost, waterBalloonUpgrade != null ? waterBalloonUpgrade.ActivateConcussiveAftershock : null);
    }

    public void BuyMilkChurnRapidChurnin()
    {
        TryPurchaseUpgrade(MilkChurnFasterReloadID, milkChurnFasterReloadCost, milkChurnUpgrade != null ? milkChurnUpgrade.ActivateRapidChurning : null);
    }

    public void BuyMilkChurnLargerSplatter()
    {
        TryPurchaseUpgrade(MilkChurnLargerSplatterID, milkChurnLargerSplatterCost, milkChurnUpgrade != null ? milkChurnUpgrade.ActivateLargerSplatter : null);
    }

    public void BuyMilkChurnUnstableMixture()
    {
        TryPurchaseUpgrade(MilkChurnUnstableMixtureID, milkChurnUnstableMixtureCost, milkChurnUpgrade != null ? milkChurnUpgrade.ActivateUnstableMixture : null);
    }

    public void BuyRestorativeGasExtendedCanister()
    {
        TryPurchaseUpgrade(RestorativeGasExpandedCanisterID, restorativeGasExpandedCanisterCost, restorativeGasUpgrade != null ? restorativeGasUpgrade.ActivateExtendedCanister : null);
    }

    public void BuyRestorativeGasRapidRefill()
    {
        TryPurchaseUpgrade(RestorativeGasFasterReloadID, restorativeGasFasterReloadCost, restorativeGasUpgrade != null ? restorativeGasUpgrade.ActivateRapidRefill : null);
    }

    public void BuyRestorativeGasLingeringTonic()
    {
        TryPurchaseUpgrade(RestorativeGasLingeringTonicID, restorativeGasLingeringTonicCost, restorativeGasUpgrade != null ? restorativeGasUpgrade.ActivateLingeringTonic : null);
    }

    public void BuyLaunchTrapHighForceLaunch()
    {
        TryPurchaseUpgrade(LaunchTrapHighForceLaunchID, launchTrapHighForceLaunchCost, launchTrapUpgrade != null ? launchTrapUpgrade.ActivateHighForceLaunch : null);
    }

    public void BuyLaunchTrapConcussiveBlast()
    {
        TryPurchaseUpgrade(LaunchTrapConcussiveExhaustID, launchTrapConcussiveExhaustCost, launchTrapUpgrade != null ? launchTrapUpgrade.ActivateConcussiveBlast : null);
    }

    public void BuyLaunchTrapDoubleTrigger()
    {
        TryPurchaseUpgrade(LaunchTrapDoubleTriggerID, launchTrapDoubleTriggerCost, launchTrapUpgrade != null ? launchTrapUpgrade.ActivateDoubleTrigger : null);
    }

    public void BuyPullTrapExpandedVacuum()
    {
        TryPurchaseUpgrade(PullTrapExpandedVacuumID, pullTrapExpandedVacuumCost, pullTrapUpgrade != null ? pullTrapUpgrade.ActivateExpandedVacuum : null);
    }

    public void BuyPullTrapExtendedReach()
    {
        TryPurchaseUpgrade(PullTrapExtendedReachID, pullTrapExtendedReachCost, pullTrapUpgrade != null ? pullTrapUpgrade.ActivateExtendedReach : null);
    }

    public void BuyPullTrapForcefulLaunch()
    {
        TryPurchaseUpgrade(PullTrapForcefulLaunchID, pullTrapForcefulLaunchCost, pullTrapUpgrade != null ? pullTrapUpgrade.ActivateForcefulLaunch : null);
    }

    public void BuyBeehiveTrapExpandedColony()
    {
        TryPurchaseUpgrade(BeehiveTrapExpandedColonyID, beehiveTrapExpandedColonyCost, beehiveTrapUpgrade != null ? beehiveTrapUpgrade.ActivateExpandedColony : null);
    }

    public void BuyBeehiveTrapFrenziedSwarm()
    {
        TryPurchaseUpgrade(BeehiveTrapFrenziedSwarmID, beehiveTrapFrenziedSwarmCost, beehiveTrapUpgrade != null ? beehiveTrapUpgrade.ActivateFrenziedSwarm : null);
    }

    public void BuyBeehiveTrapSwarmReplication()
    {
        TryPurchaseUpgrade(BeehiveTrapSwarmReplicationID, beehiveTrapSwarmReplicationCost, beehiveTrapUpgrade != null ? beehiveTrapUpgrade.ActivateSwarmReplication : null);
    }

    public void BuySprinklerTrapPressurizedSpray()
    {
        TryPurchaseUpgrade(SprinklerTrapPressurizedSprayID, sprinklerTrapPressurizedSprayCost, sprinklerUpgrade != null ? sprinklerUpgrade.ActivatePressurizedSpray : null);
    }

    public void BuySprinklerTrapRapidRotation()
    {
        TryPurchaseUpgrade(SprinklerTrapRapidRotationID, sprinklerTrapRapidRotationCost, sprinklerUpgrade != null ? sprinklerUpgrade.ActivateRapidRotation : null);
    }

    public void BuySprinklerTrapReinforcedBarriers()
    {
        TryPurchaseUpgrade(SprinklerTrapReinforcedBarriersID, sprinklerTrapReinforcedBarriersCost, sprinklerUpgrade != null ? sprinklerUpgrade.ActivateReinforcedBarriers : null);
    }

    public void BuyMagnifyingGlassTrapFocusedSunlight()
    {
        TryPurchaseUpgrade(MagnifyingGlassTrapFocusedSunlightID, magnifyingGlassTrapFocusedSunlightCost, magnifyingGlassTrapUpgrade != null ? magnifyingGlassTrapUpgrade.ActivateFocusedSunlight : null);
    }

    public void BuyMagnifyingGlassTrapIntensifiedHeat()
    {
        TryPurchaseUpgrade(MagnifyingGlassTrapIntensifiedHeatID, magnifyingGlassTrapIntensifiedHeatCost, magnifyingGlassTrapUpgrade != null ? magnifyingGlassTrapUpgrade.ActivateIntensifiedHeat : null);
    }

    public void BuyMagnifyingGlassTrapRefractedBeam()
    {
        TryPurchaseUpgrade(MagnifyingGlassTrapRefractedBeamID, magnifyingGlassTrapRefractedBeamCost, magnifyingGlassTrapUpgrade != null ? magnifyingGlassTrapUpgrade.ActivateRefractedBeam : null);
    }

    // Checks affordability, prevents duplicate purchases, deducts drops and activates the purchased upgrade.
    private void TryPurchaseUpgrade(string upgradeID, int cost, Action activateUpgrade)
    {
        if (playerResources == null || upgradeManager == null || activateUpgrade == null)
        {
            return;
        }

        if (upgradeManager.HasUpgrade(upgradeID))
            return;

        if (!playerResources.SpendDrops(cost))
            return;

        activateUpgrade.Invoke();

        upgradeManager.RegisterUpgrade(upgradeID);

        UpdateButton();
    }

    // Updates purchase button availability based on current resources.
    private void UpdateButton()
    {
        if (playerResources == null)
            return;

        int drops = playerResources.TotalDrops;

        if (launchTrapButton != null)
            launchTrapButton.interactable = playerResources.TotalDrops >= launchTrapCost;

        if (pullTrapButton != null)
            pullTrapButton.interactable = playerResources.TotalDrops >= pullTrapCost;

        if (beehiveTrapButton != null)
            beehiveTrapButton.interactable = playerResources.TotalDrops >= beehiveTrapCost;

        if (magnifyingGlassTrapButton != null)
            magnifyingGlassTrapButton.interactable = playerResources.TotalDrops >= magnifyingGlassTrapCost;

        if (sprinklerTrapButton != null)
            sprinklerTrapButton.interactable = playerResources.TotalDrops >= sprinklerTrapCost;

        if (improvedSprayButton != null)
        {
            improvedSprayButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(ImprovedSprayID) && drops >= improvedSprayCost;
        }

        if (maximumHealthDamageButton != null)
        {
            maximumHealthDamageButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MaximumHealthDamageID) && drops >= maximumHealthDamageCost;
        }

        if (knockbackButton != null)
        {
            knockbackButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(KnockbackID) && drops >= knockbackCost;
        }

        if (doubleBurstButton != null)
        {
            doubleBurstButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(DoubleBurstID) && drops >= doubleBurstCost;
        }

        if (fasterReloadButton != null)
        {
            fasterReloadButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(FasterReloadID) && drops >= fasterReloadCost;
        }

        if (closeRangeDamageButton != null)
        {
            closeRangeDamageButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(CloseRangeDamageID) && drops >= closeRangeDamageCost;
        }

        if (pitchforkBonusDamageButton != null)
        {
            pitchforkBonusDamageButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PitchforkDamageID) && drops >= pitchforkBonusDamageCost;
        }

        if (pitchforkEnemySlowdownButton != null)
        {
            pitchforkEnemySlowdownButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PitchforkSlowdownID) && drops >= pitchforkEnemySlowdownCost;
        }

        if (pitchforkBonusDropsButton != null)
        {
            pitchforkBonusDropsButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PitchforkDropsID) && drops >= pitchforkBonusDropsCost;
        }

        if (scytheRadiusButton != null)
        {
            scytheRadiusButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(ScytheRadiusID) && drops >= scytheRadiusCost;
        }

        if (scytheAttackSpeedButton != null)
        {
            scytheAttackSpeedButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(ScytheAttackSpeedID) && drops >= scytheAttackSpeedCost;
        }

        if (scytheHealthOnKillButton != null)
        {
            scytheHealthOnKillButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(ScytheHealthOnKillID) && drops >= scytheHealthOnKillCost;
        }

        if (grappleSpeedButton != null)
        {
            grappleSpeedButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(GrappleSpeedID) && drops >= grappleSpeedCost;
        }

        if (grappleEnemyVulnerabilityButton != null)
        {
            grappleEnemyVulnerabilityButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(GrappleEnemyVulnerabilityID) && drops >= grappleEnemyVulnerabilityCost;
        }

        if (grappleHealthAndSpeedOnKillButton != null)
        {
            grappleHealthAndSpeedOnKillButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(GrappleHealthAndSpeedOnKillID) && drops >= grappleHealthAndSpeedOnKillCost;
        }

        if (sniperFasterReloadButton != null)
        {
            sniperFasterReloadButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SniperFasterReloadID) && drops >= sniperFasterReloadCost;
        }

        if (sniperExposeWeakpointsButton != null)
        {
            sniperExposeWeakpointsButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SniperExposeWeakpointsID) && drops >= sniperExposeWeakpointsCost;
        }

        if (sniperRicochetShotButton != null)
        {
            sniperRicochetShotButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SniperRicochetShotID) && drops >= sniperRicochetShotCost;
        }

        if (balloonExpandedSplashButton != null)
        {
            balloonExpandedSplashButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BalloonExpandedSplashID) && drops >= balloonExpandedSplashCost;
        }

        if (balloonRubberizedBalloonButton != null)
        {
            balloonRubberizedBalloonButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BalloonRubberizedBalloonID) && drops >= balloonRubberizedBalloonCost;
        }

        if (balloonConcussiveAftershockButton != null)
        {
            balloonConcussiveAftershockButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BallonCuncussiveAfterShockID) && drops >= balloonConcussiveAftershockCost;
        }

        if (milkChurnFasterReloadButton != null)
        {
            milkChurnFasterReloadButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MilkChurnFasterReloadID) && drops >= milkChurnFasterReloadCost;
        }

        if (milkChurnLargerSplatterButton != null)
        {
            milkChurnLargerSplatterButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MilkChurnLargerSplatterID) && drops >= milkChurnLargerSplatterCost;
        }

        if (milkChurnUnstableMixtureButton != null)
        {
            milkChurnUnstableMixtureButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MilkChurnUnstableMixtureID) && drops >= milkChurnUnstableMixtureCost;
        }

        if (restorativeGasExpandedCanisterButton != null)
        {
            restorativeGasExpandedCanisterButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(RestorativeGasExpandedCanisterID) && drops >= restorativeGasExpandedCanisterCost;
        }

        if (restorativeGasFasterReloadButton != null)
        {
            restorativeGasFasterReloadButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(RestorativeGasFasterReloadID) && drops >= restorativeGasFasterReloadCost;
        }

        if (restorativeGasLingeringTonicButton != null)
        {
            restorativeGasLingeringTonicButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(RestorativeGasLingeringTonicID) && drops >= restorativeGasLingeringTonicCost;
        }

        if (launchTrapHighForceLaunchButton != null)
        {
            launchTrapHighForceLaunchButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(LaunchTrapHighForceLaunchID) && drops >= launchTrapHighForceLaunchCost;
        }

        if (launchTrapConcussiveExhaustButton != null)
        {
            launchTrapConcussiveExhaustButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(LaunchTrapConcussiveExhaustID) && drops >= launchTrapConcussiveExhaustCost;
        }

        if (launchTrapDoubleTriggerButton != null)
        {
            launchTrapDoubleTriggerButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(LaunchTrapDoubleTriggerID) && drops >= launchTrapDoubleTriggerCost;
        }

        if (pullTrapExpandedVacuumButton != null)
        {
            pullTrapExpandedVacuumButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PullTrapExpandedVacuumID) && drops >= pullTrapExpandedVacuumCost;
        }

        if (pullTrapExtendedReachButton != null)
        {
            pullTrapExtendedReachButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PullTrapExtendedReachID) && drops >= pullTrapExtendedReachCost;
        }

        if (pullTrapForcefulLaunchButton != null)
        {
            pullTrapForcefulLaunchButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(PullTrapForcefulLaunchID) && drops >= pullTrapForcefulLaunchCost;
        }

        if (beehiveTrapExpandedColonyButton != null)
        {
            beehiveTrapExpandedColonyButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BeehiveTrapExpandedColonyID) && drops >= beehiveTrapExpandedColonyCost;
        }

        if (beehiveTrapFrenziedSwarmButton != null)
        {
            beehiveTrapFrenziedSwarmButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BeehiveTrapFrenziedSwarmID) && drops >= beehiveTrapFrenziedSwarmCost;
        }

        if (beehiveTrapSwarmReplicationButton != null)
        {
            beehiveTrapSwarmReplicationButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(BeehiveTrapSwarmReplicationID) && drops >= beehiveTrapSwarmReplicationCost;
        }

        if (sprinklerTrapPressurizedSprayButton != null)
        {
            sprinklerTrapPressurizedSprayButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SprinklerTrapPressurizedSprayID) && drops >= sprinklerTrapPressurizedSprayCost;
        }

        if (sprinklerTrapRapidRotationButton != null)
        {
            sprinklerTrapRapidRotationButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SprinklerTrapRapidRotationID) && drops >= sprinklerTrapRapidRotationCost;
        }

        if (sprinklerTrapReinforcedBarriersButton != null)
        {
            sprinklerTrapReinforcedBarriersButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(SprinklerTrapReinforcedBarriersID) && drops >= sprinklerTrapReinforcedBarriersCost;
        }

        if (magnifyingGlassTrapFocusedSunlightButton != null)
        {
            magnifyingGlassTrapFocusedSunlightButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MagnifyingGlassTrapFocusedSunlightID) && drops >= magnifyingGlassTrapFocusedSunlightCost;
        }

        if (magnifyingGlassTrapIntensifiedHeatButton != null)
        {
            magnifyingGlassTrapIntensifiedHeatButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MagnifyingGlassTrapIntensifiedHeatID) && drops >= magnifyingGlassTrapIntensifiedHeatCost;
        }

        if (magnifyingGlassTrapRefractedBeamButton != null)
        {
            magnifyingGlassTrapRefractedBeamButton.interactable = upgradeManager != null && !upgradeManager.HasUpgrade(MagnifyingGlassTrapRefractedBeamID) && drops >= magnifyingGlassTrapRefractedBeamCost;
        }
    }
}
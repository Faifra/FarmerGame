using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int startingHP = 100;
    
    [Header("Stun Options")]
    [SerializeField] private static bool isStunned = false;
    [SerializeField] private static bool immuneToStun = false;
    [SerializeField] private static float stunCooldownTimer;
    [SerializeField] private static float stunCooldown;
    [SerializeField] private static float stunTimer;
    [SerializeField] private static float stunDuration;

    [Header("PauseMenu")]
    [SerializeField] private GameObject pauseMenu;

    private static int playerHP;
    private static int maximumHP;
    private static float playerStamina;
    private static float maxStamina;

    private UpgradeManagerScript upgradeManager;
    private Coroutine healthRegenerationCoroutine;

    public static GameManagerScript instance { get; private set; }
    public static int MaxHP => maximumHP;
    public bool IsStunned => isStunned;
    
    private PlayerMovementScript playerMovementScript;
    private float previousMoveForce;

    private InputAction pauseAction;
    private bool togglePauseMenu;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        upgradeManager = FindFirstObjectByType<UpgradeManagerScript>();

        maximumHP = Mathf.Max(1, maxHP);
        playerHP = Mathf.Clamp(startingHP, 0, maximumHP);

        isStunned = false;
        immuneToStun = false;

        playerMovementScript = FindFirstObjectByType<PlayerMovementScript>();
    }

    void Start()
    {
        pauseAction = InputSystem.actions.FindAction("PauseGame");
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            togglePauseMenu = !togglePauseMenu;
        }

        if(playerHP <= 0) // Reload level when we die
        {
            if (upgradeManager != null)
                upgradeManager.ResetUpgrades();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isStunned)
        {
            stunTimer += Time.deltaTime;

            if (stunTimer >= stunDuration)
            {
                playerMovementScript.MoveForce = previousMoveForce;
                isStunned = false;
                stunTimer = 0f;
            }
        }

        if (immuneToStun)
        {
            stunCooldownTimer += Time.deltaTime;

            if (stunCooldownTimer >= stunCooldown)
            {
                immuneToStun = false;
                stunCooldownTimer = 0f;
            }
        }

        if (togglePauseMenu)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

    }


    public static void SetHP(int newhp)
    {
        playerHP = Mathf.Clamp(newhp, 0, maximumHP);
    }

    public static int GetHP()
    {
        return playerHP;
    }

    public static void RestoreHP(int amount)
    {
        playerHP = Mathf.Min(playerHP + amount, maximumHP);
    }

    public static void SetStamina(float newsp)
    {
        playerStamina = newsp;
    }

    public static float GetStamina()
    {
        return playerStamina;
    }

    public static void SetMaxStamina(float newsp)
    {
        maxStamina = newsp;
    }

    public static float GetMaxStamina()
    {
        return maxStamina;
    }

    public void ApplyTemporaryHealthRegeneration(int healthPerSecond, float duration)
    {
        if (healthRegenerationCoroutine != null)
            StopCoroutine(healthRegenerationCoroutine);

        healthRegenerationCoroutine = StartCoroutine(HealthRegenerationRoutine(healthPerSecond, duration));
    }

    public void ApplyStun(float duration, float cooldown)
    {
        if (isStunned || immuneToStun)
            return;

        
        isStunned = true;
        immuneToStun = true;

        stunDuration = duration;
        stunCooldown = cooldown;

        stunTimer = 0f;
        stunCooldownTimer = 0f;

        playerMovementScript.IsDashing = false;
        playerMovementScript.ShouldJump = false;
        previousMoveForce = playerMovementScript.MoveForce;
        playerMovementScript.MoveForce = 0f;
    }

    private IEnumerator HealthRegenerationRoutine(int healthPerSecond, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1f);

            elapsedTime += 1f;

            RestoreHP(healthPerSecond);
        }

        healthRegenerationCoroutine = null;
    }


    // Helper methods
    public static void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

        #else
            Application.Quit();
            
        #endif
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame()
    {
        if(pauseMenu == null)
            return;

        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if(pauseMenu == null)
            return;

        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public void TogglePauseMenu(bool b)
    {
        togglePauseMenu = b;
    }

}

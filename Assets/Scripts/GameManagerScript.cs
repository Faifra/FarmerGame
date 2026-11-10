using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int startingHP = 100;

    private static int playerHP;
    private static int maximumHP;

    private static float playerStamina;
    private static float maxStamina;

    public static GameManagerScript instance { get; private set; }

    private UpgradeManagerScript upgradeManager;

    private Coroutine healthRegenerationCoroutine;
    public static int MaxHP => maximumHP;

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

    void Update()
    {
        if(playerHP <= 0) // Reload level when we die
        {
            if (upgradeManager != null)
                upgradeManager.ResetUpgrades();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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

}

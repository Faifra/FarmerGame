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

    public static GameManagerScript instance { get; private set; }

    public static int MaxHP => maximumHP;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

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

    void Update()
    {
        if(playerHP <= 0) // Reload level when we die
        {
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

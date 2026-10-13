using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private static int playerHP;
    private static float playerStamina;
    
    public static GameManagerScript instance { get; private set; }

    private void Awake() 
    { 

        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }
    
    public static void SetHP(int newhp)
    {
        playerHP = newhp;
    }

    public static int GetHP()
    {
        return playerHP;
    }

    public static void SetStamina(float newsp)
    {
        playerStamina = newsp;
    }

    public static float GetStamina()
    {
        return playerStamina;
    }

    void Start()
    {
        playerHP = 100; // Test variable
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

}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{

    private static int playerHP;

    // 
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

}

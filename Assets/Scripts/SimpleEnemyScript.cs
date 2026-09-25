using UnityEngine;

public class SimpleEnemyScript : MonoBehaviour
{

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerScript.SetHP(GameManagerScript.GetHP() -1);
            Debug.Log(GameManagerScript.GetHP());
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStaminaScript : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        slider.value = GameManagerScript.GetStamina();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealthScript : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        slider.minValue = 0;
        slider.maxValue = GameManagerScript.MaxHP;
    }

    void Update()
    {
        slider.value = GameManagerScript.GetHP();
    }
}
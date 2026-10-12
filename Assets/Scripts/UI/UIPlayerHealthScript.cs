using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealthScript : MonoBehaviour
{

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        slider.value = GameManagerScript.GetHP();
    }
}

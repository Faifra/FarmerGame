using UnityEngine;

public class SkyManager : MonoBehaviour
{
        public float skySpeed = 0.5f;


    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);
    }
}

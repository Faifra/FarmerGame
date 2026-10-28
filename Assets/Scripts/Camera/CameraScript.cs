using Unity.Cinemachine;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private static CinemachineCamera Ccamera;

    void Start()
    {
        Ccamera = GetComponent<CinemachineCamera>();
    }

    public static void SetCameraFOV(float fov)
    {
        Ccamera.Lens.FieldOfView = fov;
    }

    public static void SetCameraDutch(float dutch)
    {
        Ccamera.Lens.Dutch = dutch;
    }

}

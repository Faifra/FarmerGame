using UnityEngine;
using Unity.Cinemachine;

public class PlayerLookScript : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField]
    private float mouseSensitivty = 1f;

    [SerializeField]
    private float mouseAcceleration = 0f;

    [SerializeField]
    private float mouseDecceleration = 0f;

    [Header("Components")]
    [SerializeField]
    private CinemachineInputAxisController cameraInputAxisController;
    
    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField]
    private Transform playerStandingPosition;
    
    [SerializeField]
    private Transform playerCrouchPosition;
    // ==========================================//

    private Camera playerCamera;

    // ==========================================//
    void Start()
    {
        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==========================================//
    
    void Update()
    {
        foreach(var c in cameraInputAxisController.Controllers)
        {
            c.Driver.AccelTime = mouseAcceleration;
            c.Driver.DecelTime = mouseDecceleration;
            if(c.Name == "Look X (Pan)")
            {
                c.Input.Gain = mouseSensitivty;

            }
            if(c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = -mouseSensitivty;
            }
        }
        
        if (PlayerMovementScript.shouldCrouch)
        {
            cinemachineCamera.Follow = playerCrouchPosition;
        }
        else
        {
            cinemachineCamera.Follow = playerStandingPosition;
        }
    }

    // ==========================================//

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, playerCamera.transform.eulerAngles.y, 0);
    }
}

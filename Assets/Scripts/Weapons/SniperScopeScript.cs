using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

/// <summary>
/// Handles aiming and zooming for the sniper weapon.
/// </summary>
public class SniperScopeScript : MonoBehaviour
{
    [Header("Scope Settings")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float zoomFieldOfView = 20f;
    [SerializeField] private float zoomSpeed = 10f;

    private InputAction aimAction;
    private float normalFieldOfView;

    private void Awake()
    {
        aimAction = InputSystem.actions.FindAction("Aim");

        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (cinemachineCamera != null)
            normalFieldOfView = cinemachineCamera.Lens.FieldOfView;
    }

    private void Update()
    {
        if (aimAction == null)
            return;

        bool isAiming = aimAction.IsPressed();

        WeakPointScript.SetScoped(isAiming);

        if (cinemachineCamera == null)
            return;

        float targetFieldOfView = aimAction.IsPressed() ? zoomFieldOfView : normalFieldOfView;

        var lens = cinemachineCamera.Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFieldOfView, zoomSpeed * Time.deltaTime);

        cinemachineCamera.Lens = lens;
    }

    private void OnDisable()
    {
        WeakPointScript.SetScoped(false);

        if (cinemachineCamera == null)
            return;

        var lens = cinemachineCamera.Lens;
        lens.FieldOfView = normalFieldOfView;
        cinemachineCamera.Lens = lens;
    }
}
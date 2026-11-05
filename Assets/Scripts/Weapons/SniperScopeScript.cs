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
        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (cinemachineCamera == null)
        {
            return;
        }

        normalFieldOfView = cinemachineCamera.Lens.FieldOfView;

        aimAction = InputSystem.actions.FindAction("Aim");
    }

    private void Update()
    {
        if (cinemachineCamera == null || aimAction == null)
            return;

        float targetFieldOfView = aimAction.IsPressed() ? zoomFieldOfView : normalFieldOfView;

        var lens = cinemachineCamera.Lens;
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFieldOfView, zoomSpeed * Time.deltaTime);

        cinemachineCamera.Lens = lens;
    }

    private void OnDisable()
    {
        if (cinemachineCamera == null)
            return;

        var lens = cinemachineCamera.Lens;
        lens.FieldOfView = normalFieldOfView;
        cinemachineCamera.Lens = lens;
    }
}
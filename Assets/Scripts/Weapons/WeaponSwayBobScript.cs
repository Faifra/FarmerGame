using UnityEngine.InputSystem;
using Unity.Mathematics;
using UnityEngine;

public class WeaponSwayBobScript : MonoBehaviour
{

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    void Update()
    {
        // get mouse input
        float mouseX = InputSystem.actions.FindAction("Look").ReadValue<Vector2>().normalized.x * multiplier;
        float mouseY = InputSystem.actions.FindAction("Look").ReadValue<Vector2>().normalized.y * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

}

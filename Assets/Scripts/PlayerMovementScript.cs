using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{

    [Header("Move Values")]
    [SerializeField]
    private float moveForce = 1.5f; 

    [SerializeField]
    private float jumpForce = 5.0f;

    [Space, SerializeField]
    private bool useTogglableSprint;

    [SerializeField]
    private bool useTogglableCrouch;

    [Space, SerializeField]
    private float sprintMultiplier = 2f;

    [SerializeField]
    private float slideSpeedMultiplier = 1.2f;

    [Header("Control Values")]
    [SerializeField]
    private float fallOffTime = 1f;

    [SerializeField]
    private float airControl = 0.3f;

    [SerializeField]
    private float groundedAirControl = 1f;

    // ==========================================//

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    private Vector3 finalMoveVector;
    private bool shouldJump;
    private bool grounded;
    private bool shouldSprint;
    public static bool shouldCrouch;
    private bool shouldSlide;

    private Rigidbody playerRigidBody;

    static public float jumpTimer = 0;

    // ==========================================//

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");
    }

    // ==========================================//

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        finalMoveVector = transform.forward * moveValue.y + transform.right * moveValue.x;
        grounded = PlayerCollisionScript.isGrounded();

        jumpTimer += Time.deltaTime;

        if (jumpAction.WasPressedThisFrame() && (grounded || jumpTimer < fallOffTime))
        {
            shouldJump = true;
        }

        if (!useTogglableSprint)
        {
            if (sprintAction.IsPressed()) shouldSprint = true;
            else shouldSprint = false;
        }
        else
        {
            if (sprintAction.WasPressedThisFrame()) shouldSprint = !shouldSprint;
        }

        if (useTogglableCrouch)
        {
            if (crouchAction.WasPressedThisFrame())
            {
                shouldCrouch = !shouldCrouch;
            }
        }
        else
        {
            if(crouchAction.IsPressed()) shouldCrouch = true;
            else shouldCrouch = false;
        }

    }

    // ==========================================//

    private void FixedUpdate()
    {

        Vector3 velocity = playerRigidBody.linearVelocity;
        Vector3 targetVelocity = finalMoveVector.normalized * moveForce;
        targetVelocity *= !shouldSprint ? 1 : sprintMultiplier;
        targetVelocity *= !shouldSlide ? 1 : slideSpeedMultiplier;
        Vector3 appliedVelocity = new Vector3(targetVelocity.x - velocity.x, 0, targetVelocity.z - velocity.z);

        float moveControlMultiplier = grounded ? groundedAirControl : airControl;

        playerRigidBody.AddForce(appliedVelocity * moveControlMultiplier, ForceMode.VelocityChange);

        if (shouldJump)
        {
            playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            shouldJump = false;
        }
    }

    public void ApplyMagnetBoots()
    {
        // debug
        Debug.Log("We picked up a power up!");
    }

    public void ApplyGravityBoots()
    {
        
    }
}

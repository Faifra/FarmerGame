using System.Collections;
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

    [Header("Stamina"), SerializeField]
    private int maxStamina = 100;

    [SerializeField]
    private int staminaRemovalRate = 1;

    [SerializeField]
    private int staminaRecoveryRate = 1;

    [Header("Control Values")]
    [SerializeField]
    private float fallOffTime = 1f;

    [SerializeField]
    private float airControl = 0.3f;

    [SerializeField]
    private float groundedAirControl = 1f;

    [Header("Debug")]
    [SerializeField]
    private int maxJumps = 1;

    [SerializeField]
    private int jumpsLeft;

    [SerializeField]
    private int playerStamina;

    // ==========================================//

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction dashAction;

    private Vector3 finalMoveVector;
    private bool shouldJump;
    private bool grounded;
    private bool shouldSprint;
    public static bool shouldCrouch;
    private bool shouldSlide;

    private Rigidbody playerRigidBody;

    static public float jumpTimer = 0;

    [Header("Dash")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1.0f;

    [Tooltip("How fast the speed dissapears after dashing, higher = faster dissapearance")]
    [SerializeField] private float dashSpeedChangeFactor = 25f;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;

    private bool keepMomentum;
    private float dashMomentumSpeed;
    private Coroutine dashMomentumRoutine;

    // ==========================================//

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        dashAction = InputSystem.actions.FindAction("Dash");

        jumpsLeft = maxJumps;
        playerStamina = maxStamina;
    }

    // ==========================================//

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        finalMoveVector = transform.forward * moveValue.y + transform.right * moveValue.x;
        grounded = PlayerCollisionScript.isGrounded();

        jumpTimer += Time.deltaTime;

        if (grounded && playerRigidBody.linearVelocity.y <= 0)
        {
            jumpsLeft = maxJumps;
        }

        if (jumpAction.WasPressedThisFrame() && jumpsLeft > 0)
        {
            shouldJump = true;
        }

        // Sprint
        if (!useTogglableSprint)
        {
            shouldSprint = sprintAction.IsPressed();
        }
        else
        {
            if (sprintAction.WasPressedThisFrame()) shouldSprint = !shouldSprint;
        }

        // Crouch
        if (useTogglableCrouch)
        {
            if (crouchAction.WasPressedThisFrame())
            {
                shouldCrouch = !shouldCrouch;
            }
        }
        else
        {
            shouldCrouch = crouchAction.IsPressed();
        }

        // Dash cooldown
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Dash input
        if (dashAction.WasPressedThisFrame() && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash(moveValue);
        }

        // Dash timer
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                StartDashMomentum();
            }
        }
    }

    // ==========================================//

    private void FixedUpdate()
    {
        if (isDashing)
        {
            ApplyDash();
            return;
        }

        Vector3 velocity = playerRigidBody.linearVelocity;
        Vector3 targetVelocity = finalMoveVector.normalized * moveForce;

        // Stamina
        if (shouldSprint)
        {
            if (playerStamina <= 0)
            {
                shouldSprint = false;
            }
            else
            {
                playerStamina -= staminaRemovalRate;
            }
        }
        else if (!shouldSprint && playerStamina < maxStamina)
        {
            playerStamina += staminaRecoveryRate;
        }

        targetVelocity *= !shouldSprint ? 1 : sprintMultiplier;
        targetVelocity *= !shouldSlide ? 1 : slideSpeedMultiplier;

        if (keepMomentum)
        {
            targetVelocity = dashDirection * dashMomentumSpeed;
        }

        Vector3 appliedVelocity = new Vector3(targetVelocity.x - velocity.x, 0, targetVelocity.z - velocity.z);

        float moveControlMultiplier = grounded ? groundedAirControl : airControl;

        playerRigidBody.AddForce(appliedVelocity * moveControlMultiplier, ForceMode.VelocityChange);

        if (shouldJump)
        {
            playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, 0, playerRigidBody.linearVelocity.z);
            playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            shouldJump = false;
            jumpsLeft--;
        }
    }

    // ==========================================

    private void StartDash(Vector2 moveValue)
    {
        // If no input, dash forward
        Vector3 inputDir = transform.forward * moveValue.y + transform.right * moveValue.x;
        if (inputDir.sqrMagnitude > 0.01f)
            dashDirection = inputDir.normalized;
        else
            dashDirection = transform.forward;

        // Cancel any momentum left over from a previous dash
        StopDashMomentum();

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Reset velocity to make dash consistent
        playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, 0f, playerRigidBody.linearVelocity.z);
    }

    private void ApplyDash()
    {
        Vector3 dashVelocity = dashDirection * dashForce;
        Vector3 current = playerRigidBody.linearVelocity;
        Vector3 change = new Vector3(dashVelocity.x - current.x, 0f, dashVelocity.z - current.z);

        playerRigidBody.AddForce(change, ForceMode.VelocityChange);
    }

    private void StartDashMomentum()
    {
        StopDashMomentum();
        dashMomentumRoutine = StartCoroutine(DashMomentumRoutine());
    }

    private void StopDashMomentum()
    {
        if (dashMomentumRoutine != null)
        {
            StopCoroutine(dashMomentumRoutine);
            dashMomentumRoutine = null;
        }
        keepMomentum = false;
    }

    private IEnumerator DashMomentumRoutine()
    {
        // Smoothly lerp from the dash speed down to the normal move speed
        keepMomentum = true;
        dashMomentumSpeed = dashForce;

        float time = 0f;
        float difference = Mathf.Abs(dashForce - GetNormalMoveSpeed());

        while (time < difference)
        {
            dashMomentumSpeed = Mathf.Lerp(dashForce, GetNormalMoveSpeed(), time / difference);
            time += Time.deltaTime * dashSpeedChangeFactor;

            yield return null;
        }

        keepMomentum = false;
        dashMomentumRoutine = null;
    }

    private float GetNormalMoveSpeed()
    {
        float speed = moveForce;
        speed *= !shouldSprint ? 1 : sprintMultiplier;
        speed *= !shouldSlide ? 1 : slideSpeedMultiplier;
        return speed;
    }

    // ==========================================//

    public void ApplyPowerUp(PowerUpSO powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.SuperMegaJump:
                maxJumps++;
                break;
        }
    }
}
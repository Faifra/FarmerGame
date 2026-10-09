//using UnityEngine;

//public class PlayerAirDash : MonoBehaviour
//{
//    [Header("References")]
//    public Transform orientation;
//    public Transform playerCam;
//    private Rigidbody rb;
//    private PlayerMovementScript pm;

//    [Header("Dashing")]
//    public float dashForce;
//    public float dashUpwardForce;
//    public float dashDurtion;

//    [Header("Cooldown")]
//    public float dashCooldown;
//    private float dashCooldownTimer;

//    [Header("Input")]
//    public KeyCode dashKey = KeyCode.G;

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        pm = GetComponent<PlayerMovementScript>();
//    }

//    private void Update()
//    {
//        if (Input.GetKey(dashKey) && dashCooldownTimer <= 0)
//        {
//            Dash();
//            dashCooldownTimer = dashCooldown;
//        }
//        else
//        {
//            dashCooldownTimer -= Time.deltaTime;
//        }
//    }

//    private void Dash()
//    {
//        Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;

//        rb.AddForce(forceToApply, ForceMode.Impulse);

//        Invoke(nameof(ResetDash), dashDurtion);
//    }

//    private void ResetDash()
//    {

//    }
//}

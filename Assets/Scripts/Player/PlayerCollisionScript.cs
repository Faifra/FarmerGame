using Unity.Mathematics;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{

    [Header("Raycast Settings")]
    [SerializeField]
    private static float groundCheckLength = 1.1f;

    [SerializeField]
    private CapsuleCollider standingCollider;

    [SerializeField]
    private CapsuleCollider crouchingCollider;

    [Header("Wall Run Settings")]
    [SerializeField] private float dutchSmoothTime = 0.2f;
    [SerializeField] private float dutchAngle = 25f;

    [Header("Debug")]
    [SerializeField] private bool toggleCameraDutch;
    [SerializeField]
    private static bool toggleDebug;

    // ==========================================//

    private static Vector3 playerPosition;
    private static bool playerCollidingWithWall = false;

    public static bool playerWallRunning;
    
    private float currentDutch;
    private float dutchVelocity;
    private float targetDutch;

    // ==========================================//

    void Start()
    {
        playerPosition = gameObject.transform.position;
    }

    void Update()
    {
        playerPosition = gameObject.transform.position;

        currentDutch = Mathf.SmoothDampAngle(currentDutch, targetDutch, ref dutchVelocity, dutchSmoothTime);

        CameraScript.SetCameraDutch(currentDutch);
    }

    void FixedUpdate()
    {
        if (PlayerMovementScript.shouldCrouch)
        {
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;
        }
        else
        {
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
        }
    }

    // ==========================================//

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("WallRun")) // If we don't wall run
        {
            if(collision.GetContact(0).normal != Vector3.up && collision.GetContact(0).normal != Vector3.down) // and we aren't on the ground
            {
                playerCollidingWithWall = true;
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("WallRun") || !toggleCameraDutch)
        {
            targetDutch = 0;
        }
        else
        {
            Vector3 normal = collision.GetContact(0).normal;

            if(Mathf.Abs(normal.y) > 0.9f)
                return;

            playerWallRunning = true;

            if (!isGrounded())
            {

                Vector3 cross = Vector3.Cross(normal, transform.rotation.eulerAngles);

                if(cross.x > 0 && cross.z < 0 || collision.GetContact(0).normal == Vector3.left) // left: 1,0,-1
                {
                    targetDutch = cross.magnitude >= 270 || cross.magnitude < 90 ? dutchAngle : -dutchAngle;
                }
                else if(cross.x < 0 && cross.z > 0 || collision.GetContact(0).normal == Vector3.right) // right -1,0,1
                {
                    targetDutch = cross.magnitude >= 90 && cross.magnitude < 270 ? dutchAngle : -dutchAngle;
                }
                else if (cross.x < 0 && cross.z < 0 || collision.GetContact(0).normal == Vector3.forward) // front: -1,0,-1
                {
                    targetDutch = cross.magnitude >= 180 && cross.magnitude < 360 ? -dutchAngle : dutchAngle;
                }
                else if (cross.x > 0 && cross.z > 0 || collision.GetContact(0).normal == Vector3.back) // back: 1,0,1
                {
                    targetDutch = cross.magnitude >= 180 ? dutchAngle : -dutchAngle;
                }
            }
            else
            {
                targetDutch = 0;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("WallRun"))
        {
            playerWallRunning = false;
            CameraScript.SetCameraDutch(0);
        }
        else
        {
            playerCollidingWithWall = false;
        }
    }

    // ==========================================//

    public static bool isCollidingWithWall()
    {
        return playerCollidingWithWall;
    }

    public static bool isGrounded()
    {
        RaycastHit hit;
        int layerMask = ~(1 << 3);
        
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, groundCheckLength, layerMask))
        {
            if(toggleDebug)
                Debug.DrawRay(playerPosition, -hit.normal * hit.distance, Color.red); 

            PlayerMovementScript.jumpTimer = 0;
            playerCollidingWithWall = false;
            return true;
        }
        else
        {
            if(toggleDebug)
                Debug.DrawRay(playerPosition, Vector3.down * groundCheckLength, Color.white); 
            
            return false;
        }

    }
}

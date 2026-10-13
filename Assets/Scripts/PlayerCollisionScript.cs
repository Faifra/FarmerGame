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

    [Header("Debug")]
    [SerializeField]
    private static bool toggleDebug;

    // ==========================================//

    private Rigidbody playerRigidBody;
    private static Vector3 playerPosition;
    private static bool playerCollidingWithWall = false;

    public static bool playerWallRunning;

    // ==========================================//

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerPosition = gameObject.transform.position;
    }

    void Update()
    {
        playerPosition = gameObject.transform.position;
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
        if (collision.gameObject.CompareTag("WallRun"))
        {
            if(collision.GetContact(0).normal != Vector3.up && collision.GetContact(0).normal != Vector3.down)
            {
                playerWallRunning = true;
                
                Vector3 cross =  Vector3.Cross(collision.GetContact(0).normal, transform.rotation.eulerAngles);

                if(!isGrounded())
                {
                    if(cross.x > 0 && cross.z < 0 || collision.GetContact(0).normal == Vector3.left) // left: 1,0,-1
                    {
                        CameraScript.SetCameraDutch(cross.magnitude >= 270 || cross.magnitude < 90 ? 25 : -25); // apply depending on the angle --  a bit buggy as of rn
                    }
                    else if(cross.x < 0 && cross.z > 0 || collision.GetContact(0).normal == Vector3.right) // right -1,0,1
                    {
                        CameraScript.SetCameraDutch(cross.magnitude >= 90 && cross.magnitude < 270 ? 25 : -25); 
                    }
                    else if (cross.x < 0 && cross.z < 0 || collision.GetContact(0).normal == Vector3.forward) // front: -1,0,-1
                    {
                        CameraScript.SetCameraDutch(cross.magnitude >= 180 && cross.magnitude < 360 ? -25 : 25); 
                    }
                    else if (cross.x > 0 && cross.z > 0 || collision.GetContact(0).normal == Vector3.back) // back: 1,0,1
                    {
                        CameraScript.SetCameraDutch(cross.magnitude >= 180 ? 25 : -25);
                    }
                }
                else
                {
                    CameraScript.SetCameraDutch(0);
                }


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

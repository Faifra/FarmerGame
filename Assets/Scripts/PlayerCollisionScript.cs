using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{

    [Header("Raycast Settings")]
    [SerializeField]
    private static float groundCheckLength = 1.5f;

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

    // ==========================================//

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("WallRun"))
        {
            if(collision.GetContact(0).normal != Vector3.up && collision.GetContact(0).normal != Vector3.down)
            {
                playerRigidBody.linearVelocity = new Vector3(playerRigidBody.linearVelocity.x, 0, playerRigidBody.linearVelocity.z);
            }
        }
    }

    // ==========================================//

    public static bool isGrounded()
    {
        RaycastHit hit;
        int layerMask = ~(1 << 3);
        
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, groundCheckLength, layerMask))
        {
            if(toggleDebug)
                Debug.DrawRay(playerPosition, -hit.normal * hit.distance, Color.red); 

            PlayerMovementScript.jumpTimer = 0;
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

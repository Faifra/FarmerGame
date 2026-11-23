using UnityEngine;

public class SaladAI : MonoBehaviour
{
    private enum MovementState
    {
        Chasing,
        Hovering
    }
    [Header("References")]
    [SerializeField] private GameObject leafPrefab;

    //----------------------------------------------------------------
    private Transform player;
    private MovementState movementState;
    private Vector3 previousPlayerPos;
    private Vector3 playerVelocity;
    //----------------------------------------------------------------

    [Header("Movement Settings")]
    [SerializeField] private float targetAltitude = 15f; //target altitude above GROUND, will probably always be above player
    [SerializeField] private float altitudeBuffer = 2f; //+- buffer for altitude to prevent jittery movement
    [SerializeField] private float moveSpeed = 5f; //should be faster  than player
    [SerializeField] private float rangeToHover = 15f; //distance from player to start hovering

    //UNUSED, WILL ADD LATER
    [Header("Wobble Settings // unused until QA")]
    [SerializeField] private float wobbleSpeed = 0.5f; //how fast wobble up/down/sideways while moving
    [SerializeField] private float wobbleAmount = 0.5f; //how much wobble up/down/sideways while moving

    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 12f; //orbit radius around the player

    [Header("Leaf Drop Settings")]
    [SerializeField] private float predictionTime = 1f; //determines how many seconds "into the future" the salad will try to predict.
    [SerializeField] private float throwForce = 10f;
    //if playervelocity is 5, it will try to throw salad 5 units in front of player. 

    //----------------------------------------------------------------
    private float orbitAngle = 0f;
    //----------------------------------------------------------------

    //would be cool to fly in a circle around the player (above player still) to induce a sense of stress for the player. 
    //should "wobble"

    [Header("Attack Settings")]
    private float dropLeafTimer = 0f; 
    private float dropLeafCooldown = 5f;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        movementState = MovementState.Chasing;
        previousPlayerPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementState)
        {
            case MovementState.Chasing:
                ChasePlayer();
                break;
            case MovementState.Hovering:
                Hover();
                break;
        }

        //track player movement, for dropleaf method
        playerVelocity = (player.position - previousPlayerPos) / Time.deltaTime; //get playervelocity using position from most recent update
        previousPlayerPos = player.position; //get player position for most recent update 
    }

    private void ChasePlayer()
    {
        Vector3 direction = player.position - transform.position; //creates vector from salad to player.
        direction.y = 0f; //removes the vertical difference on the vector.

        float distanceToPlayer = direction.magnitude; //calculates the length of our now horizontal vector.

        if (distanceToPlayer <= rangeToHover) //compare distance to player
        {
            movementState = MovementState.Hovering; //change state if "true"
            return;
        }
        else //if not "true", keep updating movement to chase player.
        {
            //horizontal movement stuff
            direction.Normalize(); //normalize movement
            transform.position += direction * moveSpeed * Time.deltaTime; //udpate movement

            //handle vertical movement
            float desiredY = player.position.y + targetAltitude; //desired altitude above player
            float altitudeDifference = desiredY - transform.position.y; //get altitude difference

            if (Mathf.Abs(altitudeDifference) > altitudeBuffer) //create absolute value of acceptable altitude, move up or down if greater than buffer
            {
                float altitudeDirection = Mathf.Sign(altitudeDifference); //get direction to move in (up or down)
                transform.position += new Vector3(0f, altitudeDirection * moveSpeed * Time.deltaTime, 0f); //update vertical movement
            }
        }
    }

    private void Hover()
    {
        float x = Mathf.Cos(orbitAngle) * orbitRadius;
        float z = Mathf.Sin(orbitAngle) * orbitRadius;

        Vector3 targetPosition = player.position + new Vector3(x, targetAltitude, z);
        orbitAngle += moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        dropLeafTimer += Time.deltaTime;

        if (dropLeafTimer >= dropLeafCooldown)
        {
            DropLeaf();
            dropLeafTimer = 0f;
        }
    }

    private void DropLeaf()
    {
        Vector3 predictedPosition = player.position + playerVelocity * predictionTime;
        Vector3 throwDirection = (predictedPosition - transform.position).normalized;

        GameObject leaf = Instantiate(leafPrefab, transform.position, Quaternion.identity);
        leaf.SetActive(true);

        Rigidbody rb = leaf.GetComponent<Rigidbody>();
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse); //instantly move the leaf towards the predicted player position.
        dropLeafTimer = 0f;
    }
    
}

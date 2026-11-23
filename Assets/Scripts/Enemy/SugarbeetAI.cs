using UnityEngine;

public class SugarbeetAI : MonoBehaviour
{
    [SerializeField] private float targetAltitude = 10f; //should hover at this altitude above ground, there is a threshold to prevent bugs
    [SerializeField] float altitudeThreshold = 3f; //threshold for hovering to prevent bugs
    [SerializeField] float flyingSpeed = 4f; //speed of flying

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float chargeCooldown = 3f;
    [SerializeField] private int chargeDamage = 5;
    private bool isCharging = false;

    [Header("Flying Movement")]
    [SerializeField] private float bobbingHeight = 0.5f;
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float zigzagDistance = 1.5f;
    [SerializeField] private float zigzagSpeed = 2f;
    [SerializeField] private float zigzagDirectionChangeSpeed = 1f;

    private float flyingTime;
    private float zigzagOffset;
    private float zigzagDirection = 1f;

    [Header("Charge Rotation")]
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float windupSpinSpeed = 1000f;

    private Quaternion normalRotation;
    private bool returningToNormalRotation = false;

    //CHARGE GREJER
    private float chargeTimer = 0f;
    private float windupTimer = 0f;
    private float windupDuration = 3f; //duration to windup before charge
    private Vector3 chargeTarget;

    [SerializeField] private LayerMask groundLayer; 

    [SerializeField] private BehaviourState currentState = BehaviourState.Ascending;
    private Transform player;

    private enum BehaviourState
    {
        Ascending,
        Hovering,
        Windup,
        Charging,
        Cooldown
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        normalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case BehaviourState.Ascending:
                Ascend();
                break;
            case BehaviourState.Hovering:
                Hover();
                break;
            case BehaviourState.Windup:
                Windup();
                break;
            case BehaviourState.Charging:
                Charge();
                break;
            case BehaviourState.Cooldown:
                ReCharge();
                break;
        }
    }

    void Ascend()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            float altitude = hit.point.y + targetAltitude;

            Vector3 targetPos = new Vector3(transform.position.x, altitude, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, flyingSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - altitude) <= altitudeThreshold)
            {
                currentState = BehaviourState.Hovering;
            }
        }
    }

    void Hover()
    {
        flyingTime += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        zigzagOffset += zigzagDirection * zigzagSpeed * Time.deltaTime;

        if (zigzagOffset >= zigzagDistance)
        {
            zigzagOffset = zigzagDistance;
            zigzagDirection = -1f;
        }
        else if (zigzagOffset <= -zigzagDistance)
        {
            zigzagOffset = -zigzagDistance;
            zigzagDirection = 1f;
        }

        if (Random.value < zigzagDirectionChangeSpeed * Time.deltaTime)
        {
            zigzagDirection *= -1f;
        }

        Vector3 sideways = Vector3.Cross(Vector3.up, direction).normalized;

        Vector3 movement = Vector3.zero;

        if (distanceToPlayer > attackRange)
        {
            movement += direction * flyingSpeed;
        }

        movement += sideways * zigzagOffset;

        float bobbing = Mathf.Sin(flyingTime * bobbingSpeed) * bobbingHeight;

        Vector3 targetPosition = transform.position + movement * Time.deltaTime;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundLayer))
        {
            float desiredAltitude = hit.point.y + targetAltitude + bobbing;

            targetPosition.y = Mathf.MoveTowards(transform.position.y, desiredAltitude, flyingSpeed * Time.deltaTime);
        }

        transform.position = targetPosition;

        if (distanceToPlayer <= attackRange) //if within range to start attack, switch to windup state. 
            //beet should ignore the attack range considering the windup, would be too easy for the player to reset the state by simply moving out of range.
            //which would make the beet useless. windup state doesnt check for a range requirement, so attack should happen regardless.
        {
            currentState = BehaviourState.Windup;
            windupTimer = 0f;
        }
    }

    private Quaternion GetPlayerFacingRotation()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        if (directionToPlayer.sqrMagnitude < 0.001f)
            return transform.rotation;

        Vector3 currentBottomDirection = transform.rotation * Vector3.down;

        return Quaternion.FromToRotation(currentBottomDirection, directionToPlayer.normalized) * transform.rotation;
    }

    private void Windup()
    {
        Debug.Log("windup has started");
        windupTimer += Time.deltaTime;

        Quaternion targetRotation = GetPlayerFacingRotation();

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        transform.Rotate(Vector3.up, windupSpinSpeed * Time.deltaTime, Space.Self);

        if (windupTimer >= windupDuration)
        {
            Debug.Log("windup ended, charging now");

            transform.rotation = GetPlayerFacingRotation();

            chargeTarget = player.position;

            currentState = BehaviourState.Charging;
            windupTimer = 0f;
            isCharging = true;
        }
    }

    private void Charge()
    {
        transform.position = Vector3.MoveTowards(transform.position, chargeTarget, chargeSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, chargeTarget) <= 0.1f)
        {
            returningToNormalRotation = true;

            currentState = BehaviourState.Cooldown;
            chargeTimer = 0f;
            isCharging = false;
        }
    }

    private void ReCharge()
    {
        if (returningToNormalRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, normalRotation, turnSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, normalRotation) <= 0.5f)
            {
                transform.rotation = normalRotation;
                returningToNormalRotation = false;
            }
        }

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeCooldown)
        {
            chargeTimer = 0f;
            currentState = BehaviourState.Ascending;
            flyingTime = 0f;
            zigzagOffset = 0f;
            zigzagDirection = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManagerScript.SetHP(GameManagerScript.GetHP() - chargeDamage);
            Debug.Log(GameManagerScript.GetHP());
        }
    }
}

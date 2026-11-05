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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            transform.position += direction * flyingSpeed * Time.deltaTime;
        }

        if (distanceToPlayer <= attackRange) //if within range to start attack, switch to windup state. 
            //beet should ignore the attack range considering the windup, would be too easy for the player to reset the state by simply moving out of range.
            //which would make the beet useless. windup state doesnt check for a range requirement, so attack should happen regardless.
        {
            currentState = BehaviourState.Windup;
        }
    }

    private void Windup()
    {
        Debug.Log("windup has started");
        windupTimer += Time.deltaTime;

        if (windupTimer >= windupDuration)
        {
            Debug.Log("windup ended, charging now");
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
            currentState = BehaviourState.Cooldown;
            chargeTimer = 0f;
            isCharging = false;
        }
    }

    private void ReCharge()
    {
        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeCooldown)
        {
            chargeTimer = 0f;
            currentState = BehaviourState.Ascending;
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

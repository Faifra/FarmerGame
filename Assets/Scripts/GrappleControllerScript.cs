
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

/// <summary>
/// Manages grappling, pulling, tether duration, and the post-switch damage window.
/// </summary>
public class GrappleControllerScript : MonoBehaviour
{
    [Header("Pulling")]
    [SerializeField] private float playerPullSpeed = 18f;
    [SerializeField] private float enemyPullSpeed = 12f;
    [SerializeField] private float maxTetherDuration = 3f;

    [Header("Damage Window")]
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float vulnerabilityDuration = 0.75f;

    [Header("Visuals")]
    [SerializeField] private LineRenderer tetherLine;

    private InputAction attackAction;
    private InputAction pullEnemyAction;

    private Rigidbody playerBody;
    private PlayerMovementScript playerMovement;

    private EnemyHealthScript tetheredEnemy;
    private Rigidbody enemyBody;
    private NavMeshAgent enemyAgent;

    private float tetherTimer;
    private bool tethered;
    private bool pullingEnemy;
    private bool enemyWasAgentEnabled;

    private GrapplingWeaponScript grappleWeapon;
    public bool IsTethered => tethered;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovementScript>();

        attackAction = InputSystem.actions.FindAction("Grapple Pull Player");
        pullEnemyAction = InputSystem.actions.FindAction("Grapple Pull Enemy");

        if (tetherLine != null)
            tetherLine.enabled = false;
    }

    private void Update()
    {
        if (!tethered)
            return;

        tetherTimer += Time.deltaTime;

        if (tetherTimer >= maxTetherDuration)
        {
            EndTether(false);
            return;
        }

        if (attackAction != null && attackAction.IsPressed())
        {
            pullingEnemy = false;
            playerMovement.SetGrappleMovement(true);
        }
        else if (pullEnemyAction != null && pullEnemyAction.IsPressed())
        {
            pullingEnemy = true;
            playerMovement.SetGrappleMovement(false);
        }
        else
        {
            pullingEnemy = false;
            playerMovement.SetGrappleMovement(false);
        }

        UpdateTetherLine();
    }

    private void FixedUpdate()
    {
        if (!tethered || tetheredEnemy == null)
            return;

        if (pullingEnemy)
        {
            PullEnemy();
        }
        else if (attackAction != null && attackAction.IsPressed())
        {
            PullPlayer();
        }
    }

    public void AttachToEnemy(EnemyHealthScript enemy)
    {
        if (enemy == null || playerBody == null)
            return;

        if (tethered)
            EndTether(false);

        tetheredEnemy = enemy;
        enemyBody = enemy.GetComponent<Rigidbody>();
        enemyAgent = enemy.GetComponent<NavMeshAgent>();

        grappleWeapon = FindFirstObjectByType<GrapplingWeaponScript>();

        enemyWasAgentEnabled = enemyAgent != null && enemyAgent.enabled;

        tetherTimer = 0f;
        tethered = true;
        pullingEnemy = false;

        if (tetherLine != null)
        {
            tetherLine.enabled = true;
            tetherLine.positionCount = 2;
        }
    }

    private void PullPlayer()
    {
        Vector3 direction = tetheredEnemy.transform.position - transform.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Vector3 pullVelocity = direction.normalized * playerPullSpeed;
        Vector3 sidewaysVelocity = playerMovement.GetGrappleSidewaysVelocity();

        playerBody.linearVelocity = pullVelocity + sidewaysVelocity;
    }

    private void PullEnemy()
    {
        Vector3 direction = transform.position - tetheredEnemy.transform.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        if (enemyAgent != null && enemyAgent.enabled && enemyAgent.isOnNavMesh)
        {
            enemyAgent.velocity = direction * enemyPullSpeed;
            enemyAgent.Move(direction * enemyPullSpeed * Time.fixedDeltaTime);
        }
        else if (enemyBody != null && !enemyBody.isKinematic)
        {
            enemyBody.linearVelocity = direction * enemyPullSpeed;
        }
        else
        {
            tetheredEnemy.transform.position += direction * enemyPullSpeed * Time.fixedDeltaTime;
        }
    }

    private void UpdateTetherLine()
    {
        if (tetherLine == null || tetheredEnemy == null)
            return;

        tetherLine.SetPosition(0, transform.position);
        tetherLine.SetPosition(1, tetheredEnemy.transform.position);
    }

    public void OnWeaponSwitch()
    {
        if (!tethered)
            return;

        Vector3 pullDirection = Vector3.zero;

        if (tetheredEnemy != null)
        {
            pullDirection = (tetheredEnemy.transform.position - transform.position).normalized;
        }

        EndTether(true);

        if (playerBody != null && pullDirection.sqrMagnitude > 0f)
        {
            Vector3 velocity = playerBody.linearVelocity;
            float towardEnemySpeed = Vector3.Dot(velocity, pullDirection);

            if (towardEnemySpeed > 0f)
            {
                velocity -= pullDirection * towardEnemySpeed;
                playerBody.linearVelocity = velocity;
            }
        }

        if (playerMovement != null)
            playerMovement.SetGrappleMovement(false);
    }

    private void EndTether(bool switchedWeapon)
    {
        if (!tethered)
            return;

        EnemyHealthScript previousEnemy = tetheredEnemy;

        tethered = false;
        pullingEnemy = false;
        tetheredEnemy = null;

        Debug.Log("Grapple tether ended");

        if (playerMovement != null)
            playerMovement.SetGrappleMovement(false);

        if (enemyAgent != null && enemyAgent.enabled && enemyAgent.isOnNavMesh)
        {
            enemyAgent.isStopped = false;
        }

        if (tetherLine != null)
            tetherLine.enabled = false;

        if (switchedWeapon && previousEnemy != null)
        {
            previousEnemy.ApplyTemporaryDamageMultiplier(damageMultiplier, vulnerabilityDuration);
        }

        enemyBody = null;
        enemyAgent = null;
    }

    private void OnDisable()
    {
        if (tethered)
            EndTether(false);
    }


    private void LateUpdate()
    {
        if (!tethered || grappleWeapon == null)
            return;

        if (!grappleWeapon.gameObject.activeInHierarchy)
            OnWeaponSwitch();
    }
}
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [Header("Home Area")]
    public Transform homeCenter;
    public float roamRadius = 10f;
    public float roamDelay = 3f;

    [Header("Detection & Behavior")]
    public float detectionRadius = 15f;
    public float disturbanceThreshold = 50f;
    public float stopDistanceToPlayer = 2f;        // Stop chase when this close
    public float chaseCooldown = 5f;               // Cooldown after disengaging
    public float chaseSpeed = 3.5f;
    public float roamSpeed = 2f;

    [Header("Wander Pause Settings")]
    public bool pauseAtWanderPoint = true;        // Whether to pause after wandering
    public float pauseDuration = 2f;              // How long to pause before moving again

    private NavMeshAgent agent;
    private float roamTimer;
    private bool isChasingPlayer;
    private float cooldownTimer;
    private bool inCooldown;
    private bool isPausedAtWanderPoint = false;
    private float pauseTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        roamTimer = roamDelay;
        NPCManager.Instance.RegisterNPC(this);
    }

    private void Update()
    {
        if (NPCManager.Instance.Player == null)
            return;

        float disturbance = DisturbanceManager.Instance.disturbanceValue;
        float distanceToPlayer = Vector3.Distance(transform.position, NPCManager.Instance.Player.position);

        if (inCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                inCooldown = false;
            }
            return; // Don't do anything else during cooldown
        }

        if (isChasingPlayer)
        {
            if (distanceToPlayer <= stopDistanceToPlayer)
            {
                ResetEngagementWithCooldown();  // Too close, disengage
                return;
            }

            TryChasePlayer();  // Continue chasing
        }
        else
        {
            if (disturbance >= disturbanceThreshold && distanceToPlayer <= detectionRadius)
            {
                TryChasePlayer();
            }
            else
            {
                if (pauseAtWanderPoint && isPausedAtWanderPoint)
                {
                    pauseTimer -= Time.deltaTime;
                    if (pauseTimer <= 0f)
                    {
                        isPausedAtWanderPoint = false;
                        roamTimer = roamDelay; // Resume wander delay
                    }
                }
                else
                {
                    roamTimer += Time.deltaTime;
                    if (roamTimer >= roamDelay && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    {
                        Wander();
                        roamTimer = 0f;
                    }
                }
            }
        }
    }

    private void TryChasePlayer()
    {
        if (agent == null || NPCManager.Instance.Player == null)
            return;

        isChasingPlayer = true;
        agent.speed = chaseSpeed;

        Vector3 playerPos = NPCManager.Instance.Player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(playerPos, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning($"[Forrest Watcher NPC] NavMesh.SamplePosition failed within 10 units. Falling back to raw player position.");
            agent.SetDestination(playerPos);
        }
    }

    private void Wander()
    {
        if (agent == null || homeCenter == null)
            return;

        isChasingPlayer = false;
        agent.speed = roamSpeed;

        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection.y = 0f; // Ensure flat movement
        Vector3 targetPosition = homeCenter.position + randomDirection;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(targetPosition, out navHit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);

            if (pauseAtWanderPoint)
            {
                isPausedAtWanderPoint = true;
                pauseTimer = pauseDuration;
            }
        }
        else
        {
            Debug.LogWarning($"[NPC] Wander: No valid NavMesh position found near {targetPosition}.");
        }
    }

    public void ResetEngagement()
    {
        isChasingPlayer = false;
        agent.speed = roamSpeed;
        roamTimer = roamDelay;
    }

    private void ResetEngagementWithCooldown()
    {
        ResetEngagement();
        inCooldown = true;
        cooldownTimer = chaseCooldown;
    }
}

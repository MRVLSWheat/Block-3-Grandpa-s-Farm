using UnityEngine;

public class AnimalProximityBehaviour : MonoBehaviour
{
    public float firstStageDistance = 10f;
    public float secondStageDistance = 5f;
    public float minRunDistance = 5f;
    public float maxRunDistance = 15f;
    public float runAwaySpeed = 5f;
    public float fleeAngleVariation = 45f; // Max random angle (in degrees) from the direct flee direction

    private Transform player;
    private Animator animator;
    private bool isLookingAtPlayer = false;
    private bool isRunningAway = false; // Flag to track if the animal is running

    private Vector3? runTarget = null;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure your player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (isRunningAway)
        {
            // Continue running until we reach the target
            RunAwayFromPlayer();
        }
        else if (distanceToPlayer <= secondStageDistance)
        {
            StartRunningAway();
        }
        else if (distanceToPlayer <= firstStageDistance)
        {
            LookAtPlayer();
            runTarget = null;
        }
        else
        {
            ResetBehavior();
            runTarget = null;
        }
    }

    private void LookAtPlayer()
    {
        if (!isLookingAtPlayer)
        {
            isLookingAtPlayer = true;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void StartRunningAway()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        // Pick a random direction to run
        Vector3 awayDirection = (transform.position - player.position).normalized;
        float randomAngle = Random.Range(-fleeAngleVariation, fleeAngleVariation);
        awayDirection = Quaternion.Euler(0, randomAngle, 0) * awayDirection;

        float randomDistance = Random.Range(minRunDistance, maxRunDistance);
        runTarget = transform.position + awayDirection * randomDistance;

        isRunningAway = true; // Start the running away behavior
    }

    private void RunAwayFromPlayer()
    {
        if (runTarget == null) return;

        Vector3 moveDirection = (runTarget.Value - transform.position).normalized;
        transform.position += moveDirection * runAwaySpeed * Time.deltaTime;

        // Check if we've reached the target distance
        if (Vector3.Distance(transform.position, runTarget.Value) < 0.5f)
        {
            // Stop running once we’ve reached the target
            isRunningAway = false;

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    private void ResetBehavior()
    {
        isLookingAtPlayer = false;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }
}

using UnityEngine;

public class AnimalProximityBehaviour : MonoBehaviour
{
    public float firstStageDistance = 10f;
    public float secondStageDistance = 5f;
    public float minRunDistance = 5f;
    public float maxRunDistance = 15f;
    public float runAwaySpeed = 5f;
    public float approachSpeed = 2f; // Speed for approach behavior after feeding
    public float fleeAngleVariation = 45f;
    public float followTime = 5f; // Time to follow the player after feeding
    public float minFollowDistance = 1f; // Minimum distance to maintain from the player

    private Transform player;
    private Animator animator;
    private bool isLookingAtPlayer = false;
    private bool isRunningAway = false;
    private bool isFollowingPlayer = false; // Tracks if the animal is following the player

    private Vector3? runTarget = null;
    private float followTimer = 0f; // Timer to track follow duration

    private Renderer rend;

    void Start()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure your player has the 'Player' tag.");
        }

        SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral pink at start
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If following the player, handle follow behavior
        if (isFollowingPlayer)
        {
            FollowPlayer();
            SetColor(Color.green); // Turn green while following

            // Decrease follow timer and stop following when the timer runs out
            followTimer -= Time.deltaTime;
            if (followTimer <= 0f)
            {
                isFollowingPlayer = false;
                SetColor(new Color(1f, 0.4f, 0.7f)); // Revert to neutral (pink)
            }
        }
        else
        {
            // Handle different states based on the player's distance
            if (isRunningAway)
            {
                RunAwayFromPlayer();
                SetColor(Color.red); // Stage 2 (running)
            }
            else if (distanceToPlayer <= secondStageDistance)
            {
                StartRunningAway();
                SetColor(Color.red); // Stage 2 (starting to run)
            }
            else if (distanceToPlayer <= firstStageDistance)
            {
                LookAtPlayer();
                runTarget = null;
                SetColor(Color.yellow); // Stage 1 (alert)

                // Check for feed input when in stage 1
                if (Input.GetKeyDown(KeyCode.F)) // Change this to your preferred button
                {
                    StartApproachingPlayer();
                }
            }
            else
            {
                ResetBehavior();
                runTarget = null;
                SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral (pink)
            }
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

        Vector3 awayDirection = (transform.position - player.position).normalized;
        float randomAngle = Random.Range(-fleeAngleVariation, fleeAngleVariation);
        awayDirection = Quaternion.Euler(0, randomAngle, 0) * awayDirection;

        float randomDistance = Random.Range(minRunDistance, maxRunDistance);
        runTarget = transform.position + awayDirection * randomDistance;

        isRunningAway = true;
    }

    private void RunAwayFromPlayer()
    {
        if (runTarget == null) return;

        Vector3 moveDirection = (runTarget.Value - transform.position).normalized;
        transform.position += moveDirection * runAwaySpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, runTarget.Value) < 0.5f)
        {
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

    private void StartApproachingPlayer()
    {
        // Start moving towards the player and start the follow behavior
        isFollowingPlayer = true;
        followTimer = followTime; // Reset follow timer
    }

    private void FollowPlayer()
    {
        // Move towards the player but maintain a minimum distance
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only move towards the player if we are further than the minimum follow distance
        if (distanceToPlayer > minFollowDistance)
        {
            transform.position += directionToPlayer * approachSpeed * Time.deltaTime; // Use approachSpeed here
        }
    }

    private void SetColor(Color color)
    {
        if (rend != null && rend.material.color != color)
        {
            rend.material.color = color;
        }
    }
}

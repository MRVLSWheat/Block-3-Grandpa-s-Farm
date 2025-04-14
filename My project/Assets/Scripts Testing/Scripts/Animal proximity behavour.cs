using UnityEngine;

public class AnimalProximityBehaviour : MonoBehaviour
{
    public float firstStageDistance = 10f;
    public float secondStageDistance = 5f;
    public float chaseSpeed = 5f;
    public float fleeAngleVariation = 45f;
    public float minChaseTime = 2f;
    public float maxChaseTime = 6f;

    private Transform player;
    private Animator animator;
    private bool isLookingAtPlayer = false;
    private bool isChasing = false;
    private float chaseTimer = 0f;

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

        if (isChasing)
        {
            ChasePlayer();
            SetColor(Color.red); // Chasing color
            chaseTimer -= Time.deltaTime;

            if (chaseTimer <= 0f)
            {
                isChasing = false;
                ResetBehavior();
                SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral
            }
        }
        else if (distanceToPlayer <= secondStageDistance)
        {
            StartChasing();
            SetColor(Color.red); // Starting to chase
        }
        else if (distanceToPlayer <= firstStageDistance)
        {
            LookAtPlayer();
            SetColor(Color.yellow); // Stage 1 (alert)
        }
        else
        {
            ResetBehavior();
            SetColor(new Color(1f, 0.4f, 0.7f)); // Neutral
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

    private void StartChasing()
    {
        isChasing = true;
        chaseTimer = Random.Range(minChaseTime, maxChaseTime);

        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;

        // Rotate toward the player while chasing
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void ResetBehavior()
    {
        isLookingAtPlayer = false;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
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

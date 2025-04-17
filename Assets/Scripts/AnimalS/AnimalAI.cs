using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float waitTime = 3f;

    private NavMeshAgent agent;
    private Vector3 centerPoint;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        centerPoint = transform.position; // Set initial spawn point as center
        PickNewDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                PickNewDestination();
                timer = 0;
            }
        }
    }

    void PickNewDestination()
    {
        Vector3 newPos = GetRandomPointInCircle();
        agent.SetDestination(newPos);
    }

    Vector3 GetRandomPointInCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        Vector3 finalPosition = new Vector3(centerPoint.x + randomPoint.x, transform.position.y, centerPoint.z + randomPoint.y);
        return finalPosition;
    }
}

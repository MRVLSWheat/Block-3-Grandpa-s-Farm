using UnityEngine;

public class HostileAnimal : MonoBehaviour
{
    // Distance for teleport trigger
    public float detectionRange = 5f;

    private GameObject player;
    private GameObject respawnPoint;
    private PlayerMovement playerMovement;  // Reference to the player's movement script (if applicable)

    public float spawnRadius = 5f;  // Radius around respawn point for random spawn
    public LayerMask groundLayer;  // Layer for ground detection

    void Start()
    {
        // Automatically find the player by its tag
        player = GameObject.FindWithTag("Player");

        // Automatically find the Respawn object by name or tag
        respawnPoint = GameObject.Find("RespawnPoint");

        // Check if the player has a movement script attached (if applicable)
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();  // Assuming the script is called PlayerMovement
        }

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Ensure the player has the 'Player' tag.");
        }

        if (respawnPoint == null)
        {
            Debug.LogError("Respawn Point not found in the scene. Ensure the RespawnPoint object exists.");
        }
    }

    void Update()
    {
        // Only check if player and respawn point are available
        if (player != null && respawnPoint != null)
        {
            // Calculate the distance between the player and the hostile animal
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            // If the player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // Teleport the player instantly to a random position around the respawn point
                Vector3 randomPosition = GetRandomSpawnPosition();
                player.transform.position = randomPosition;
                Debug.Log("Player has been teleported instantly to a random spawn position around the respawn point.");
            }
        }
    }

    // Function to get a random spawn position on the ground around the respawn point
    Vector3 GetRandomSpawnPosition()
    {
        // Try generating a valid spawn position for a maximum number of attempts
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // Generate a random position around the respawn point within the specified radius
            Vector3 randomPos = respawnPoint.transform.position + Random.insideUnitSphere * spawnRadius;

            // Cast a ray down to find the ground position
            RaycastHit hit;
            if (Physics.Raycast(randomPos + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                // Ensure the player is placed slightly above the ground to avoid spawning inside it
                return hit.point + Vector3.up * 1f;  // 1f is the offset to ensure the player is not below ground
            }

            // If raycast fails, keep trying up to the maximum attempts
        }

        // If no valid position was found, return the original respawn point as a fallback
        Debug.LogWarning("No valid spawn position found, falling back to respawn point.");
        return respawnPoint.transform.position;
    }
}

using UnityEngine;

public class BookPlane : MonoBehaviour
{
    public GameObject planePrefab;  // assign your prefab in the Inspector
    public Transform spawnPoint;    // optional: where to spawn it

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnPlane();
        }
    }

    void SpawnPlane()
    {
        Vector3 position = spawnPoint ? spawnPoint.position : Vector3.zero;
        Instantiate(planePrefab, position, Quaternion.identity);
        Debug.Log("Plane spawned at: " + position);
    }
}

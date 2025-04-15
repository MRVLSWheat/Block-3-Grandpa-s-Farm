using UnityEngine;

public class BookPlane : MonoBehaviour
{
    public GameObject planePrefab;      // Assign your prefab in the Inspector
    public GameObject spawnedPlane;     // To hold the spawned instance
    public Transform cameraTransform;   // Assign the first person camera in the Inspector
    public Vector3 offsetFromCamera = new Vector3(0f, -0.2f, 1.5f); // Adjust as needed
    public Vector3 rotationOffset = new Vector3(0f, 180f, 90f);     // Fix sideways rotation
    public bool bookOpened = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!bookOpened)
            {
                SpawnPlane();
                bookOpened = true;
            }
            else
            {
                DeletePlane();
                bookOpened = false;
            }
        }
    }

    void SpawnPlane()
    {
        // Instantiate and parent to camera
        spawnedPlane = Instantiate(planePrefab);
        spawnedPlane.transform.SetParent(cameraTransform);

        // Set position relative to camera
        spawnedPlane.transform.localPosition = offsetFromCamera;

        // Set rotation to face player correctly
        spawnedPlane.transform.localRotation = Quaternion.Euler(rotationOffset);
    }

    void DeletePlane()
    {
        if (spawnedPlane != null)
        {
            Destroy(spawnedPlane);
        }
    }
}
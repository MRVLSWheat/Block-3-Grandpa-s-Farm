using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BookPlane : MonoBehaviour
{
    public GameObject planePrefab;  // assign your prefab in the Inspector
    public GameObject PrefabTwo;
    public Transform spawnPoint;    // optional: where to spawn it
    public bool bookopened = false;
    private int spawnDistance = 3;
    private Vector3 position; // Changed from int to Vector3 to fix the error

    void Update()
    {
        position = new Vector3(transform.position.x + spawnDistance, transform.position.y, transform.position.z);
        if (Input.GetKeyDown(KeyCode.B) && bookopened == false)
        {
            SpawnPlane();
            bookopened = true;
        }
        else if (Input.GetKeyDown(KeyCode.B) && bookopened == true)
        {
            DeletePlane();
            bookopened = false;
        }
    }

    void SpawnPlane()
    {
        Quaternion yOnlyRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        Vector3 position = spawnPoint ? spawnPoint.position : Vector3.zero; // Fixed: position is now a Vector3
        PrefabTwo = Instantiate(planePrefab, position, yOnlyRotation); // Fixed: position is now a Vector3
    }

    void DeletePlane()
    {
        Destroy(PrefabTwo);
    }
}

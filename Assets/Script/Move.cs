using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5f; // Increased speed value to compensate for Time.deltaTime

    void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection).normalized;

        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
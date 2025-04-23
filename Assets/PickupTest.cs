using UnityEngine;

public class PickupTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.Play("PickUp"); // Use the exact name of the animation state
        }
    }
}

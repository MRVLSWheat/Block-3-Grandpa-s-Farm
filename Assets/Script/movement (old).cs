using UnityEngine.Animations;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float speed = 1.0f;
    public Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(xDirection / speed, 0.0f, zDirection / speed);

        transform.position += moveDirection;
        //anim.SetFloat("MoveSpace", moveDirection.magnitude);
        if (moveDirection.magnitude > 0)
        {
            anim.SetFloat("MoveSpaceInt", moveDirection.magnitude);
        }
        else
        {
            anim.SetFloat("MoveSpaceInt", 0f); // Stop animation
        }

    }
}
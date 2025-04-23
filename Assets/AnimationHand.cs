using UnityEngine;

public class AnimationHand : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (anim != null)
            {
                anim.SetTrigger("PickUpTrigger");
                anim.Play("PickUpTrigger", 0, 0f);
            }
        }
    }
}
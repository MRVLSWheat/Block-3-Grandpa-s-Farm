using UnityEngine;
using TMPro;

public class castRay : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public TextMeshProUGUI tooltipText;
    private Transform player;
    public float objectRange = 1f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 20))
        {
            if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Flower"))
            {

                float distanceToHit = Vector3.Distance(hit.transform.position, transform.position);

                if (distanceToHit <= objectRange)
                {
                    tooltipText.text = "Press E";
                    tooltipText.gameObject.SetActive(true);
                    return;
                }
            }
            else { tooltipText.gameObject.SetActive(false); }
        }
        else

        {
            tooltipText.gameObject.SetActive(false);
        }

    }
}
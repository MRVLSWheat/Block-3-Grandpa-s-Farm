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

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Flower"))
            {
                float distanceToHit = Vector3.Distance(hit.transform.position, player.position);

                if (distanceToHit <= objectRange)
                {
                    tooltipText.text = "Press E";
                    tooltipText.gameObject.SetActive(true);
                    return;
                }
            }
        }
        tooltipText.gameObject.SetActive(false);

    }
}
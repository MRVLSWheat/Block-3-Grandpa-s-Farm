using UnityEngine;

public class castRay : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    private bool pressingE = false;
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
                    pressingE = true;
                    return;
                }
            }
        }
        pressingE = false;
    }

    private void OnGUI()
    {
        if (pressingE)
        {
            GUIStyle e = new GUIStyle();
            e.fontSize = 20;
            GUI.Label(new Rect(630, 20, 300, 100), "Text here :D", e);
        }
    }
}
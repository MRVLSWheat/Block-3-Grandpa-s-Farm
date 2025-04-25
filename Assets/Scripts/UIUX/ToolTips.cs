using UnityEngine;

public class castRay : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    private bool pressingE = false;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Only proceed if the object has a tag of "NPC" or "Flowers"
            if (hit.transform.CompareTag("NPC") || hit.transform.CompareTag("Flower") /*|| hit.transform.CompareTag("Animals")*/)
            {
                pressingE = true;
            }
            else
            {
                pressingE = false;
            }
        }
    }

    private void OnGUI()
    {
        if (pressingE == true)
        {
            GUIStyle e = new GUIStyle();
            e.fontSize = 20;
            GUI.Label(new Rect(630, 20, 300, 100), "Text here :D", e);
        }
    }
}
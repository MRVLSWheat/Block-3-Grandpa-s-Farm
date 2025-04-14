using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class Grandpa : MonoBehaviour
{
    private int Random;
    public GameObject GrandpaPrais1;
    public GameObject GrandpaPrais2;
    public GameObject GrandpaNoPrais1;
    public GameObject GrandpaNoPrais2;
    private int GrandpaSwap = 0;
    private int GrandpaSwap2 = 0;
    private int GrandpaNoSwap = 0;


    public void Praised()
    {
  
        if (UnityEngine.Random.Range(1, 100) < 50)
        {
            GrandpaPrais1.SetActive(false);
            GrandpaPrais1.SetActive(true);
            GrandpaSwap = 1;
        }
        else
        {
            GrandpaPrais2.SetActive(false);
            GrandpaPrais2.SetActive(true);
            GrandpaSwap = 2;
        }
    }

    public void NoPraised()
    {
 
        if (UnityEngine.Random.Range(1, 100) < 50)
        {
            GrandpaNoPrais1.SetActive(false);
            GrandpaNoPrais1.SetActive(true);
            GrandpaNoSwap = 1;
        }
        else
        {
            GrandpaNoPrais2.SetActive(false);
            GrandpaNoPrais2.SetActive(true);
            GrandpaNoSwap = 2;
        }
    }

    void OnGUI()
    {
        GUIStyle s = new GUIStyle();
        s.fontSize = 50;
        if (GrandpaSwap == 1)
        {
            GUI.Label(new Rect(350, 350, 1800, 180), "Good job Kid", s);
            GrandpaSwap2++;
        }
        else if (GrandpaSwap == 2)
        {
            GUI.Label(new Rect(400, 350, 800, 80), "Well done", s);
            GrandpaSwap2++;
        }
        else if (GrandpaNoSwap == 1)
        {
            GUI.Label(new Rect(370, 350, 1800, 180), "Not good", s);
            GrandpaSwap2++;
        }
        else if (GrandpaNoSwap == 2)
        {
            GUI.Label(new Rect(400, 350, 800, 80), "Too bad", s);
            GrandpaSwap2++;
        }
    }

    private void Update()
    {
        if (GrandpaSwap2 > 0)
        {
            if (GrandpaSwap2 > 300)
            {
                GrandpaSwap2 = 0;
                GrandpaSwap = 0;
                GrandpaNoSwap = 0;
            }
        }
    }

}

    
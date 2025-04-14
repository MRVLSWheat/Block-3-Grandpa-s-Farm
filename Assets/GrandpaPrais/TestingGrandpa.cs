using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class TestingGrandpa : MonoBehaviour
{
    public Grandpa grandpa;
    void Update()
    {
        if (Input.GetKeyUp("g"))
        {
            grandpa.Praised();
        }

        if (Input.GetKeyUp("h"))
        {
            grandpa.NoPraised();
        }
    }
}



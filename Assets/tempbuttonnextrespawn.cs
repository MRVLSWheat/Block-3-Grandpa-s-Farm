using FlowerSpawner;
using Unity.VisualScripting;
using UnityEngine;

public class tempbuttonnextrespawn : MonoBehaviour
{
    public Flowerspawning Respawningflower;
    public void OnMouseDown()
    {
            Respawningflower.RespawnFlowers(); 
        Debug.Log("Respawning flowers");
    }
}

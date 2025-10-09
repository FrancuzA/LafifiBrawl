using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnUnits : MonoBehaviour
{

    public List<GameObject> UnitsToSpawn;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    void Start()
    {
        Dependencies.Instance.RegisterDependency(this);
        DeployUnits();
    }


    public void DeployUnits()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(UnitsToSpawn[0].name, spawnPosition,Quaternion.identity);
           
    }

    
}

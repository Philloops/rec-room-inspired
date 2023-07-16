using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    GameObject genericVRPlayerPrefab;

    public Vector3 spawnPos;

    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(genericVRPlayerPrefab.name, spawnPos, Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}

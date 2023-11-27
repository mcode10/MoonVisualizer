using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class deployObjectButton : MonoBehaviour
{
    public GameObject obstacle;
    public void WhenButtonClicked () 
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 playerDirection = GameObject.FindGameObjectWithTag("Player").transform.forward;
        Quaternion playerRotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;
        float spawnDistance = 10;

        Vector3 spawnPosition = playerPosition + playerDirection*spawnDistance;
        Instantiate(obstacle, spawnPosition, playerRotation);
    }
}

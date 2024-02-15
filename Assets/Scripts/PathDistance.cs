using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathDistance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pathDistanceTXT;

    RoverMove roverMove;

    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
    }

    // Update is called once per frame
    void Update()
    {
        pathDistanceTXT.text = roverMove.distanceString + "m";
    }
}

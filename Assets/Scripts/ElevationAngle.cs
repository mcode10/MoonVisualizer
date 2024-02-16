using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevationAngle : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI elevationAngleTXT;

    RoverMove roverMove;
    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
    }

    // Update is called once per frame
    void Update()
    {
        elevationAngleTXT.text = roverMove.elevationAngleString;
    }
}
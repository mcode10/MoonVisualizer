using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AzimuthAngleTXT : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI azimuthAngleTXT;

    RoverMove roverMove;
    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();

    }
    // Update is called once per frame
    void Update()
    {
        azimuthAngleTXT.text = roverMove.azimuthAngleString;
    }

}

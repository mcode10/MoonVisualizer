using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slope : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI slopeTXT;


    RoverMove roverMove;
    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
    }

    // Update is called once per frame
    void Update()
    {
        slopeTXT.text = roverMove.slopeAngleString;
    }
}

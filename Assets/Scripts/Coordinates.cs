using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coordinates : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coordinatesTXT;

    RoverMove roverMove;
    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
    }

    // Update is called once per frame
    void Update()
    {
        coordinatesTXT.text = roverMove.coordinates;
    }
}

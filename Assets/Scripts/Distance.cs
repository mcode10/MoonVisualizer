using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Distance : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI distanceTXT;

    RoverMove roverMove;
    // Start is called before the first frame update
    void Start()
    {
        roverMove = FindObjectOfType<RoverMove>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceTXT.text = roverMove.distanceString;
    }
}

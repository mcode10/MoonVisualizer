using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoverMove : MonoBehaviour
{
    // The Lunar radius.
    const float lunarRadius = 1737.4f;

    // Longitudes and latitudes to translate back to.
    const float minLatitude = -88.44359886830375f;
    const float maxLatitude = -89.17347901603343f;
    const float minLongitude = 107.5300786165378f;
    const float maxLongitude = 141.8371251025832f;

    // Reference latitude and longitude for Earth.
    const float earthLatitude = -6f;
    const float earthLongitude = 20f;
    const float earthLatitudeInRadians = earthLatitude * Mathf.Deg2Rad;
    const float earthLongitudeInRadians = earthLongitude * Mathf.Deg2Rad;

    // The corresponding cartesian coordinates.
    const float earthX = 1623.678285f;
    const float earthY = 590.9705659f;
    const float earthZ = -181.6077521f;
    Vector3 earth = new Vector3(earthX, earthY, earthZ);

    // Distance to calculate slope for elevation angle calculations.
    const float slopeDistance = 0.1f;

    DestinationCube point;
    // NavMesh component variables
    NavMeshAgent agent;
    NavMeshPath path;
    public Vector3 destination;

    // Speed value for slider.

    // Initialize the SetWaypoints object.
    SetWaypoints setWaypoints;

    public float speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        point = FindObjectOfType<DestinationCube>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        // These two being in Update() makes pathfinding dynamic (shouldn't
        // get stuck on corners)
        agent.CalculatePath(destination, path);
        agent.SetPath(path);

        // Get the rover and waypoints components.
        RoverMove roverMove = FindObjectOfType<RoverMove>();
        setWaypoints = GetComponent<SetWaypoints>();

        Debug.Log("About to FindWaypoints().");
        List<Vector3> waypoints = setWaypoints.FindWaypoints(path);
        Debug.Log($"Found {waypoints.Count} waypoints.");
        setWaypoints.CreateWaypoints(waypoints);
    }

    void OnGUI()
    {
        speed = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), speed, 0.0f, 50.0f);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().speed = speed;
        // uncomment for dynamic pathfinding
        // agent.CalculatePath(destination, path);
        // agent.SetPath(path);
    }

}

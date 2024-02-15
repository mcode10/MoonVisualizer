using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoverMove : MonoBehaviour
{
    public float startDistanceFromBottom = 0.2f;   // Should probably be higher than skin width
    public float sphereCastRadius = 0.25f;
    public float sphereCastDistance = 0.75f;       // How far spherecast moves down from origin point
    public float raycastLength = 0.75f;
    public Vector3 rayOriginOffset1 = new Vector3(-0.2f, 0f, 0.16f);
    public Vector3 rayOriginOffset2 = new Vector3(0.2f, 0f, -0.16f);
    public float groundSlopeAngle = 0f;            // Angle of the slope in degrees
    public Vector3 groundSlopeDir = Vector3.zero;  // The calculated slope as a vector
    public bool showDebug = false;                  // Show debug gizmos and lines
    public LayerMask castingMask;

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
    public Vector3 destination = new Vector3(0f, 0f, 0f);

    public string azimuthAngleString;
    public string slopeAngleString;
    public string elevationAngleString;
    public string coordinates;
    public float slope;
    public float distance;
    public string distanceString;
    // Speed value for slider.

    // Initialize the SetWaypoints object.
    SetWaypoints setWaypoints;
    public float speed = 10.0f;

    int w;
    float sliderSpeed;
    float beforeSpeed;
    bool atWaypoint;
    public List<Vector3> waypoints;
    // Start is called before the first frame update
    void Start()
    {
        point = FindObjectOfType<DestinationCube>();
        destination = point.GetComponent<Transform>().position;
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        // These two being in Update() makes pathfinding dynamic (shouldn't
        // get stuck on corners)
        agent.CalculatePath(destination, path);
        agent.SetPath(path);

        // Get the rover and waypoints components.
        RoverMove roverMove = FindObjectOfType<RoverMove>();
        setWaypoints = GetComponent<SetWaypoints>();

        waypoints = setWaypoints.FindWaypoints(path);
        setWaypoints.CreateWaypoints(waypoints);

        (float latitude, float longitude) = setWaypoints.FindLatitudeLongitudeOfUnityPoint(transform.position);
        float azimuthAngleFloat = setWaypoints.AzimuthToEarth(latitude, longitude);
        azimuthAngleString = azimuthAngleFloat.ToString();
        coordinates = $"Latitude: {latitude}, longitude: {longitude}";
        float elevationAngleRadians = setWaypoints.ElevationAngleToEarth(transform.position);
        elevationAngleString = (elevationAngleRadians * Mathf.Rad2Deg).ToString();
        SlopeAtPoint(transform.position);
        slopeAngleString = groundSlopeAngle.ToString();
        w = 0;
        atWaypoint = false;
        distanceString = setWaypoints.CalculatePathDistanceInMeters(path.corners).ToString();
    }

    void OnGUI()
    {
        sliderSpeed = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), sliderSpeed, 0.0f, 50.0f);
    }

    // Update is called once per frame
    void Update()
    {
        (float latitude, float longitude) = setWaypoints.FindLatitudeLongitudeOfUnityPoint(transform.position);
        float azimuthAngleFloat = setWaypoints.AzimuthToEarth(latitude, longitude);
        azimuthAngleString = azimuthAngleFloat.ToString();
        coordinates = $"Latitude: {latitude}, longitude: {longitude}";
        float elevationAngleRadians = setWaypoints.ElevationAngleToEarth(transform.position);
        elevationAngleString = (elevationAngleRadians * Mathf.Rad2Deg).ToString();
        SlopeAtPoint(transform.position);
        slopeAngleString = setWaypoints.SlopeOfTerrain(transform.position).ToString();
        if ((Vector3.Distance(waypoints[w], transform.position) <= 10) && !atWaypoint)
        {
            beforeSpeed = agent.speed;
            agent.speed = 20.0f;
            atWaypoint = true;
            Debug.Log(speed);
        }
        else if ((Vector3.Distance(waypoints[w], transform.position) > 10) && atWaypoint)
        {
            agent.speed = 11f;
            w++;
            atWaypoint = false;
        }
        // uncomment for dynamic pathfinding
        // agent.CalculatePath(destination, path);
        // agent.SetPath(path);
    }
    public void SlopeAtPoint(Vector3 origin)
    {
        // Out hit point from our cast(s)
        RaycastHit hit;

        // SPHERECAST
        // "Casts a sphere along a ray and returns detailed information on what was hit."
        if (Physics.SphereCast(origin, sphereCastRadius, Vector3.down, out hit, sphereCastDistance, castingMask))
        {
            // Angle of our slope (between these two vectors).
            // A hit normal is at a 90 degree angle from the surface that is collided with (at the point of collision).
            // e.g. On a flat surface, both vectors are facing straight up, so the angle is 0.
            groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Find the vector that represents our slope as well.
            //  temp: basically, finds vector moving across hit surface
            Vector3 temp = Vector3.Cross(hit.normal, Vector3.down);
            //  Now use this vector and the hit normal, to find the other vector moving up and down the hit surface
            groundSlopeDir = Vector3.Cross(temp, hit.normal);
        }

        // Now that's all fine and dandy, but on edges, corners, etc, we get angle values that we don't want.
        // To correct for this, let's do some raycasts. You could do more raycasts, and check for more
        // edge cases here. There are lots of situations that could pop up, so test and see what gives you trouble.
        RaycastHit slopeHit1;
        RaycastHit slopeHit2;

        // FIRST RAYCAST
        if (Physics.Raycast(origin + rayOriginOffset1, Vector3.down, out slopeHit1, raycastLength))
        {
            // Debug line to first hit point
            if (showDebug) { Debug.DrawLine(origin + rayOriginOffset1, slopeHit1.point, Color.red); }
            // Get angle of slope on hit normal
            float angleOne = Vector3.Angle(slopeHit1.normal, Vector3.up);

            // 2ND RAYCAST
            if (Physics.Raycast(origin + rayOriginOffset2, Vector3.down, out slopeHit2, raycastLength))
            {
                // Debug line to second hit point
                if (showDebug) { Debug.DrawLine(origin + rayOriginOffset2, slopeHit2.point, Color.red); }
                // Get angle of slope of these two hit points.
                float angleTwo = Vector3.Angle(slopeHit2.normal, Vector3.up);
                // 3 collision points: Take the MEDIAN by sorting array and grabbing middle.
                float[] tempArray = new float[] { groundSlopeAngle, angleOne, angleTwo };
                Array.Sort(tempArray);
                groundSlopeAngle = tempArray[1];
            }
            else
            {
                // 2 collision points (sphere and first raycast): AVERAGE the two
                float average = (groundSlopeAngle + angleOne) / 2;
                groundSlopeAngle = average;
            }
        }
    }


}

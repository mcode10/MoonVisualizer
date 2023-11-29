using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWaypoints : MonoBehaviour
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

    // The corresponding cartersian coordinates.
    const float earthX = 1623.678285f;
    const float earthY = 590.9705659f;
    const float earthZ = -181.6077521f;
    Vector3 earth = new Vector3(earthX, earthY, earthZ);

    // Distance to calculate slope for elevation angle calculations.
    const float slopeDistance = 0.1f;

    // Initialize the terrain as a global object to allow access from multiple
    // functions.
    Terrain terrain;
    TerrainData terrainData;


    void Start()
    {
        // Get the necessary Unity Game Objects.
        UnityEngine.AI.NavMeshAgent navmesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        UnityEngine.AI.NavMeshPath path = navmesh.path;
        // Replace with FindObjectOfType<Terrain>() at some point.
        terrain = FindObjectOfType<Terrain>();
        terrainData = terrain.terrainData;

        Vector3 startPoint;
        List<Vector3> waypoints = FindWaypoints(path);

        CreateWaypoints(waypoints);
        Debug.Log(waypoints);
    }

    void Update() { }

    List<Vector3> FindWaypoints(UnityEngine.AI.NavMeshPath path)
    {
        Vector3[] pathCorners = path.corners;
        float totalPathDistance = CalculatePathDistance(pathCorners);
        float segmentLength = totalPathDistance / 11f;
        List<Vector3> waypoints = new List<Vector3>();

        List<float> segments = new List<float>();

        for (int segmentNumber = 0; segmentNumber < 10; segmentNumber++)
        {
            Debug.Log("Started new segment calculations.");
            Debug.Log(segmentNumber);
            bool foundWaypoint = false;
            int cornerRecursion = 0;
            (Vector3 nearestCorner, Vector3 fartherCorner) = FindNearestCorner(segmentNumber, segmentLength, pathCorners, cornerRecursion);

            while (!foundWaypoint)
            {
                if (PointIsViableWaypoint(nearestCorner))
                {
                    Vector3 waypoint = nearestCorner;
                    waypoints.Add(waypoint);
                    foundWaypoint = true;
                }
                else
                {

                    if (PointIsViableWaypoint(fartherCorner))
                    {
                        Vector3 waypoint = fartherCorner;
                        waypoints.Add(waypoint);
                        foundWaypoint = true;
                    }
                }

                cornerRecursion++;
            }
        }

        if (cornerRecursion > 5)
        {
            break;
        }
        return waypoints;
    }

    float CalculatePathDistance(Vector3[] pathCorners)
    {
        float pathDistance = 0f;
        bool initialPoint = true;
        Vector3 previousPoint = pathCorners[0];
        foreach (Vector3 corner in pathCorners)
        {
            if (initialPoint == true)
            {
                previousPoint = corner;
                initialPoint = false;
            }
            else
            {
                float segmentDistance = Vector3.Distance(previousPoint, corner);
                pathDistance = pathDistance + segmentDistance;
                previousPoint = corner;
            }
        }

        return pathDistance;
    }

    (Vector3 nearestCorner, Vector3 fartherCorner) FindNearestCorner(int segmentNumber, float segmentLength, Vector3[] pathCorners, int recursion = 0)
    {
        int averageCornersPerSegment = (int)MathF.Floor(pathCorners.Length / 11);
        float pointDistance = segmentNumber * segmentLength;
        float totalDistance = 0f;
        int cornerIndex = 0;
        // Find the corner that is just past the distance we are looking for.
        while (totalDistance < pointDistance)
        {
            if (cornerIndex != 0)
            {
                totalDistance = totalDistance + Vector3.Distance(pathCorners[cornerIndex - 1], pathCorners[cornerIndex]);
            }
            cornerIndex++;
        }


        // Set that and the corner just ahead of the distance we are looking for as the corners to compare.
        Debug.Log(pathCorners.Length);
        Debug.Log(cornerIndex);
        Vector3 secondCorner = pathCorners[cornerIndex - 1 - recursion];
        Vector3 firstCorner = pathCorners[cornerIndex + 1 + recursion];

        // Find the midpoint of the target segment.
        float midX = (secondCorner.x + firstCorner.x) / 2;
        float midY = (secondCorner.y + firstCorner.y) / 2;
        float midZ = (secondCorner.z + firstCorner.z) / 2;
        Vector3 segmentMidpoint = new Vector3(midX, midY, midZ);

        // Determine which corner is closer.
        if (Vector3.Distance(segmentMidpoint, secondCorner) < Vector3.Distance(segmentMidpoint, firstCorner))
        {
            return (secondCorner, firstCorner);
        }
        else
        {
            return (firstCorner, secondCorner);
        }
    }

    bool PointIsViableWaypoint(Vector3 point)
    {
        (float latitude, float longitude) = FindLatitudeLongitudeOfUnityPoint(point);

        Vector3 cartesianPoint = CartesianConversion(latitude, longitude);

        float azimuthToEarth = AzimuthToEarth(latitude, longitude);
        float elevationAngleToEarth = ElevationAngleToEarth(cartesianPoint);
        float elevationAngleOfTerrain = ElevationAngleOfTerrain(cartesianPoint, azimuthToEarth);

        if (elevationAngleOfTerrain < elevationAngleToEarth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    (float latitude, float longitude) FindLatitudeLongitudeOfUnityPoint(Vector3 point)
    {
        float unityX = point.x;
        float unityY = point.y;

        float interpolatedX = Mathf.InverseLerp(0, 2499, unityX);
        float interpolatedY = Mathf.InverseLerp(0, 2499, unityY);

        float latitude = Mathf.Lerp(minLatitude, maxLatitude, interpolatedX);
        float longitude = Mathf.Lerp(minLongitude, maxLongitude, interpolatedY);

        return (latitude, longitude);
    }

    Vector3 CartesianConversion(float latitude, float longitude)
    {
        float x = lunarRadius * Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad);
        float y = lunarRadius * Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad);
        float z = lunarRadius * Mathf.Sin(latitude * Mathf.Deg2Rad);

        return new Vector3(x, y, z);
    }

    float AzimuthToEarth(float latitude, float longitude)
    {
        float latitudeInRadians = DegreesToRadians(latitude);
        float longitudeInRadians = DegreesToRadians(longitude);
        float x = (Mathf.Sin(longitudeInRadians - earthLongitudeInRadians) * Mathf.Cos(earthLatitudeInRadians));
        float y = ((Mathf.Cos(latitudeInRadians) * Mathf.Sin(earthLatitudeInRadians)) - (Mathf.Sin(latitudeInRadians) * Mathf.Cos(earthLatitudeInRadians) * Mathf.Cos(earthLongitudeInRadians - longitudeInRadians)));
        float azimuthAngle = Mathf.Atan2(y, x);

        return azimuthAngle;
    }

    float DegreesToRadians(float degreeValue)
    {
        return degreeValue * Mathf.PI / 180f;
    }

    float ElevationAngleOfTerrain(Vector3 point, float azimuthAngle)
    {
        float xDistanceForSlope = (slopeDistance * Mathf.Cos(azimuthAngle));
        float yDistanceForSlope = (slopeDistance * Mathf.Sin(azimuthAngle));

        Vector3 slopeSamplePoint = PointFromClosestTerrain(new Vector3(point.x + xDistanceForSlope, 0f, point.z + yDistanceForSlope));
        float heightForSlope = point.y - slopeSamplePoint.y;

        float elevationAngle = Mathf.Atan(heightForSlope / slopeDistance);

        return elevationAngle;
    }

    float ElevationAngleToEarth(Vector3 point)
    {
        float xPointToEarth = point.x - earth.x;
        float yPointToEarth = point.y - earth.y;
        float zPointToEarth = point.z - earth.z;

        float earthLatitudeRadians = earthLatitude * Mathf.Deg2Rad;
        float earthLongitudeRadians = earthLongitude * Mathf.Deg2Rad;

        float range = Mathf.Sqrt(Mathf.Pow(xPointToEarth, 2f) + Mathf.Pow(yPointToEarth, 2f) + Mathf.Pow(zPointToEarth, 2f));
        float rz1 = xPointToEarth * Mathf.Cos(earthLatitudeRadians) * Mathf.Cos(earthLongitudeRadians);
        float rz2 = yPointToEarth * Mathf.Cos(earthLatitudeRadians) * Mathf.Sin(earthLongitudeRadians);
        float rz3 = zPointToEarth * Mathf.Sin(earthLongitudeRadians);
        float rz = rz1 + rz2 + rz3;

        return Mathf.Asin(rz / range);
    }

    void CreateWaypoints(List<Vector3> waypoints)
    {
        foreach (Vector3 waypoint in waypoints)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = waypoint;
        }
    }

    Vector3 PointFromClosestTerrain(Vector3 position)
    {
        Terrain terrain = GetClosestCurrentTerrain(position);

        position.y = terrain.SampleHeight(position);

        return position;
    }

    Terrain GetClosestCurrentTerrain(Vector3 position)
    {
        //Get all terrain
        Terrain[] terrains = Terrain.activeTerrains;

        //Make sure that terrains length is ok
        if (terrains.Length == 0)
            return null;

        //If just one, return that one terrain
        if (terrains.Length == 1)
            return terrains[0];

        //Get the closest one to the player
        float lowDist = (terrains[0].GetPosition() - position).sqrMagnitude;
        var terrainIndex = 0;

        for (int i = 1; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            Vector3 terrainPos = terrain.GetPosition();

            //Find the distance and check if it is lower than the last one then store it
            var dist = (terrainPos - position).sqrMagnitude;
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return terrains[terrainIndex];
    }
}

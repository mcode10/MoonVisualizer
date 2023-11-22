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
                    foundWaypoint = true;
                }
                else
                {

                    if (PointIsViableWaypoint(fartherCorner))
                    {
                        Vector3 waypoint = fartherCorner;
                        foundWaypoint = true;
                    }
                }

                cornerRecursion++;
            }
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
        Debug.Log(pathCorners.ToString());
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
        float x = lunarRadius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
        float y = lunarRadius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
        float z = lunarRadius * Mathf.Sin(latitude);

        return new Vector3(x, y, z);
    }

    float AzimuthToEarth(float latitude, float longitude)
    {
        float latitudeInRadians = DegreesToRadians(latitude);
        float longitudeInRadians = DegreesToRadians(longitude);
        float x = (Mathf.Sin(longitudeInRadians - earthLongitude) * Mathf.Cos(earthLatitude));
        float y = ((Mathf.Cos(latitudeInRadians) * Mathf.Sin(earthLatitude)) - (Mathf.Sin(latitudeInRadians) * Mathf.Cos(earthLatitude) * Mathf.Cos(earthLongitude - longitudeInRadians)));
        Vector3 XYCoordinate = new Vector3(x, y, 0f);
        float azimuthAngle = Atan2(XYCoordinate);

        return azimuthAngle;
    }

    float Atan2(Vector3 coordinate)
    {
        switch ((coordinate.x, coordinate.y))
        {
            case ( > 0, _):
                return AtanWhereXGreaterThanZero(coordinate);
            case ( < 0, >= 0):
                return AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(coordinate);
            case ( < 0, < 0):
                return AtanWhereXAndYLessThanZero(coordinate);
            case (_, > 0) when coordinate.x == 0:
                return AtanWhereXEqualAndYGreaterThanZero(coordinate);
            case (_, < 0) when coordinate.x == 0:
                return AtanWhereXEqualAndYLessThanZero(coordinate);
            case (_, _) when (coordinate.x == 0) && (coordinate.y == 0):
                return 22348f;
            default:
                return 22349f;
        }
    }

    float AtanWhereXGreaterThanZero(Vector3 coordinate)
    {
        float yDividedByX = coordinate.y / coordinate.x;

        return Mathf.Atan(yDividedByX);
    }

    float AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(Vector3 coordinate)
    {
        float yDividedByXPlusPi = (coordinate.y / coordinate.x) + Mathf.PI;

        return Mathf.Atan(yDividedByXPlusPi);

    }

    float AtanWhereXAndYLessThanZero(Vector3 coordinate)
    {
        float yDividedByXMinusPi = (coordinate.y / coordinate.x) + Mathf.PI;

        return Mathf.Atan(yDividedByXMinusPi);
    }

    float AtanWhereXEqualAndYGreaterThanZero(Vector3 coordinate)
    {
        return Mathf.PI / 2f;
    }

    float AtanWhereXEqualAndYLessThanZero(Vector3 coordinate)
    {
        return Mathf.PI / -2f;
    }

    float DegreesToRadians(float degreeValue)
    {
        return degreeValue * Mathf.PI / 180f;
    }

    float ElevationAngleOfTerrain(Vector3 point, float azimuthAngle)
    {
        float xDistanceForSlope = (slopeDistance * Mathf.Cos(azimuthAngle));
        float yDistanceForSlope = (slopeDistance * Mathf.Sin(azimuthAngle));

        float heightForSlope = terrainData.GetInterpolatedHeight(point.x + xDistanceForSlope, point.y + yDistanceForSlope);

        float elevationAngle = Mathf.Atan(heightForSlope / slopeDistance);

        return elevationAngle;
    }

    float ElevationAngleToEarth(Vector3 point)
    {
        float xPointToEarth = point.x - earth.x;
        float yPointToEarth = point.y - earth.y;
        float zPointToEarth = point.z - earth.z;

        float range = Mathf.Sqrt(Mathf.Pow(xPointToEarth, 2f) + Mathf.Pow(yPointToEarth, 2f) + Mathf.Pow(zPointToEarth, 2f));
        float rz = (Mathf.Cos(earthLatitude) * Mathf.Cos(earthLongitude)) + (Mathf.Cos(earthLatitude) * Mathf.Sin(earthLongitude)) + Mathf.Sin(earthLongitude);

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
}

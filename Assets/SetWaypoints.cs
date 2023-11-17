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
    const Vector3 earth = new Vector3(earthX, earthY, earthZ);

    // Distance to calculate slope for elevation angle calculations.
    const float slopeDistance = 0.1f;

    // Initialize the terrain as a global object to allow access from multiple
    // functions.
    Terrain terrain;
    TerrainData terrainData;


    void Start()
    {
        // Get the necessary Unity Game Objects.
        NavMeshAgent navmesh = something.GetComponent<UnityEngine.AI.NavMeshAgent>();
        NavMeshPath path = navmesh.path;
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        Vector3 startPoint;
        Vector3[] waypoints = FindWaypoints(path);

        SetWaypoints(waypoints);
    }

    void Update() { }

    Vector3[] FindWaypoints(UnityEngine.AI.NavMeshPath path)
    {
        Vector3[] pathCorners = path.corners;
        float totalPathDistance = CalculatePathDistance(pathCorners);
        float segmentLength = totalPathDistance / 11f;
        Vector3[] waypoints = new Vector3[];

        List<float> segments = new List<float>();

        for (int segmentNumber = 0; segmentNumber < 10; segmentNumber++)
        {
            (Vector3 nearestCorner, Vector3 fartherCorner) = FindNearestCorner(segmentNumber, segmentLength, pathCorners);
            if (PointIsViableWaypoint(nearestCorner))
            {
                waypoint.Add(nearestCorner);
            }
            else
            {
                if (PointIsViableWaypoint(fartherCorner))
                {
                    waypoint.Add(fartherCorner);
                }
                else
                {
                    nearestMidpoint = FindNearestMidpoint(nearestCorner);
                    Vector3 trueWaypoint;
                    while (!PointIsViableWaypoint(nearestCorner))
                    {
                        // Code to increment midpoint to search viable waypoint.
                    }
                }
            }
        }

        return waypoints;
    }

    float CalculatePathDistance(Vector3[] pathCorners)
    {
        float pathDistance = 0f;
        bool initialPoint = true;
        Vector3 previousPoint;
        foreach (Vector3 corner in pathCorners)
        {
            if (initialPoint == true)
            {
                float segmentDistance = Vector3.distance(previousPoint, corner);
                pathDistance = pathDistance + segmentDistance;
                previousPoint = corner;
            }
            else
            {
                previousPoint = corner;
                initialPoint = false;
            }
        }

        return pathDistance;
    }

    (Vector3 nearestCorner, Vector3 fartherCorner) FindNearestCorner(int segmentNumber, float segmentLength, Vector3[] pathCorners, int recursion = 0)
    {
        int averageCornersPerSegment = (int)MathF.Floor(pathCorners.Length / 11);
        float pointDistance = segmentNumber * segmentLength;
        float totalDistance = 0;
        int cornerIndex = 0;
        // Find the corner that is just past the distance we are looking for.
        while (totalDistance < pointDistance)
        {
            if (cornerIndex == 0)
            {
                totalDistance = startPoint + Vector3.Distance(startPoint, pathCorners[cornerIndex]);
            }
            else
            {
                totalDistance = totalDistance + Vector3.Distance(pathCorners[cornerIndex], pathCorners[cornerIndex + 1]);
            }
            cornerIndex++;
        }


        // Set that and the corner just ahead of the distance we are looking for as the corners to compare.
        Vector3 secondCorner = pathCorners[cornerIndex - recursion];
        Vector3 firstCorner = pathCorners[cornerIndex + 1 + recursion];

        // Find the midpoint of the target segment.
        float midX = (secondCorner.x + firstCorner.x) / 2;
        float midY = (secondCorner.y + firstCorner.y) / 2;
        float midZ = (secondCorner.z + firstCorner.z) / 2;
        Vector3 segmentMidpoint = new Vector3(midX, midY, midZ);

        // Initialize the point that will be returned.
        Vector3 idealCorner;

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
        float elevationAngleToEarth = ElevationAngleToEarth(azimuthToEarth);
        float elevationAngleOfTerrain = ElevationAngleOfTerrain(azimuthToEarth);

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

        float interpolatedX = Mathf.InverseLerp(0, 2499, x);
        float interpolatedY = Mathf.InverseLerp(0, 2499, y);

        float latitude = Mathf.Lerp(minLatitude, maxLatitude, interpolatedX);
        float longitude = Mathf.Lerp(minLongitude, maxLongtitude, interpolatedX);

        return (latitude, longitude);
    }

    Vector3 CartesianConversion(float latitude, float longitude)
    {
        x = lunarRadius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
        y = lunarRadius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
        z = lunarRadius * Mathf.Sin(latitude);

        return new Vector3(x, y, z);
    }

    float AzimuthToEarth(float latitude, float longitude)
    {
        float latitudeInRadians = DegreesToRadians(latitude);
        float longitudeInRadians = DegreesToRadians(longitude);
        float x = (Mathf.Sin(longitudeInRadians - earthLongitude) * Mathf.Cos(earthLatitude));
        float y = ((Mathf.Cos(latitudeInRadians) * Mathf.Sin(earthLatitude)) - (Mathf.Sin(latitudeInRadians) * Mathf.Cos(earthLatitude) * Mathf.Cos(earthLongitude - longitudeInRadians)));
        float azimuthAngle = Atan2(x, y);

        return azimuthAngle;
    }

    float Atan2(Vector3 coordinate)
    {
        float atan2_result = coordinate switch
        {
            (coordinate.x > 0) => AtanWhereXGreaterThanZero(coordinate),
            (coordinate.x < 0) && (coordinate.y >= 0) => AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(coordinate),
            (coordinate.x < 0) && (coordinate.y < 0) => AtanWhereXAndYLessThanZero(coordinate),
            (coordinate.x == 0) && (coordinate.y > 0) => AtanWhereXEqualAndYGreaterThanZero(coordinate),
            (coordinate.x == 0) && (coordinate.y < 0) => AtanWhereXEqualAndYLessThanZero(coordinate),
            (coordinate.x == 0) && (coordinate.y == 0) => throw new ArgumentException("Invalid input for atan calculation")
        };

        return atan2_result;
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

        float heightForSlope = terrainData.GetHeights(point.x + xDistanceForSlope, point.y + yDistanceForSlope);

        float elevationAngle = Mathf.Atan(heightForSlope / slopeDistance);

        return elevationAngle;
    }

    float ElevationAngleToEarth(Vector3 point)
    {
        float xPointToEarth = point.x - earth.x;
        float yPointToEarth = point.y - earth.y;
        float zPointToEarth = point.z - earth.z;

        float range = Mathf.Sqrt((xPointToEarth ^ 2) + (yPointToEarth ^ 2) + (zPointToEarth ^ 2));
        float rz = (Mathf.Cos(earthLatitude) * Mathf.Cos(earthLongitude)) + (Mathf.Cos(earthLatitude) * Mathf.Sin(earthLongitude)) + Mathf.Sin(earthLongitude);

        return Mathf.Asin(rz / range);
    }
    void SetWaypoints(Vector3[] waypoints) { }
}

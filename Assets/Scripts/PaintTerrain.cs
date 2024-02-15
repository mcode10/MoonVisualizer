using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // used for Sum of array

public class PaintTerrain : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
    }
    public SplatHeights[] splatHeights;
    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain;
        terrain = GetComponent<Terrain>();
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        // Declare constants for earth and lunar values
        const float lunarRadius = 1737.4f;
         const float earthX = 1623.678285f;
         const float earthY = 590.9705659f;
         const float earthZ = -181.6077521f;
         const float earthLatitude = -6f;
         const float earthLongitude = 20f;
         const float minLatitude = -88.44359886830375f;
         const float maxLatitude = -89.17347901603343f;
         const float minLongitude = 107.5300786165378f;
         const float maxLongitude = 141.8371251025832f;
         const float slopeDistance = 0.1f;
         Vector3 earth = new Vector3(earthX, earthY, earthZ);
        // DONT DELETE THIS: Loops through points to find elevation angle at each point
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                
                float terrainElevation = GetElevationAngle(new Vector3(x, y));
            }
        }
     


    float GetElevationAngle(Vector3 point)
    {
        (float latitude, float longitude) = FindLatitudeLongitudeOfUnityPoint(point);

        Vector3 cartesianPoint = CartesianConversion(latitude, longitude);

        float azimuthToEarth = AzimuthToEarth(latitude, longitude);
        float elevationAngleToEarth = ElevationAngleToEarth(cartesianPoint);
        float elevationAngleOfTerrain = ElevationAngleOfTerrain(cartesianPoint, azimuthToEarth);

        return elevationAngleOfTerrain;
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
        float azimuthAngle = Mathf.Atan2(y, x);

        return azimuthAngle;
    }

    float DegreesToRadians(float degreeValue)
    {
        return degreeValue * Mathf.PI / 180f;
    }

    float ElevationAngleOfTerrain(Vector3 point, float azimuthAngle)
    {
        float xDistanceForSlope = slopeDistance * Mathf.Cos(azimuthAngle);
        float yDistanceForSlope = slopeDistance * Mathf.Sin(azimuthAngle);

        Vector3 slopeSamplePoint = PointFromClosestTerrain(new Vector3(point.x + xDistanceForSlope, 0f, z: point.z
                                                                                                           + yDistanceForSlope));
        float heightForSlope = point.y - slopeSamplePoint.y;

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

    // Sets the terrain data
    terrainData.SetAlphamaps(0, 0, splatmapData);

        



    }

}

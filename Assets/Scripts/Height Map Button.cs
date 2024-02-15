using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapButton : MonoBehaviour
{
    // Declare constants for earth and lunar values
        
   public void OnClick()
    {
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
         const float earthLatitudeInRadians = earthLatitude * Mathf.Deg2Rad;
        const float earthLongitudeInRadians = earthLongitude * Mathf.Deg2Rad;
         Vector3 earth = new Vector3(earthX, earthY, earthZ);
        // Get an array of all active terrains
        Terrain[] terrains = Terrain.activeTerrains;

        // Iterate through each terrain
        for (int i = 0; i < terrains.Length; i++)
        {
            // Get the TerrainData for the current terrain
            TerrainData terrainData = terrains[i].terrainData;
            
            
            // Get the existing alphamaps
            float[, ,] alphamaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

            // Modify the alphamaps as needed
            ModifyAlphamaps(alphamaps);

            // Set the modified alphamaps back to the terrain
            terrainData.SetAlphamaps(0, 0, alphamaps);
            void ModifyAlphamaps(float[, ,] alphamaps)
    {
        // Iterate through each point in the alphamaps
        for (int y = 0; y < alphamaps.GetLength(1); y++)
        {
            for (int x = 0; x < alphamaps.GetLength(0); x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)alphamaps.GetLength(1);
                float x_01 = (float)x/(float)alphamaps.GetLength(0);
                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01,x_01);
                float terrainwidth = terrainData.size.x;
                float terrainlength = terrainData.size.z;
                float terrainpositionx = x_01 * terrainwidth;
                float terrainpositionz = y_01 * terrainlength;
                Vector3 worldposition = new Vector3(terrainpositionx, 0, terrainpositionz);
                float steepness = terrainData.GetSteepness(y_01,x_01);
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution),Mathf.RoundToInt(x_01 * terrainData.heightmapResolution) );

                // Modify the alphamap values for each layer
                for (int layer = 0; layer < alphamaps.GetLength(2); layer++)
                {
                    // Example: Set all alphamap values to 0 in the first layer
                    alphamaps[x, y, layer] = (layer == 0) ? 0.3f : 

                    
                    alphamaps[x, y, layer] = (layer == 1) ?  Mathf.Clamp01(height / terrainData.size.y): 
                    alphamaps[x, y, layer] = (layer == 2) ?  0f : 
                    alphamaps[x, y, layer] = (layer == 3) ?  0f : 
                    alphamaps[x, y, layer] = (layer == 4) ?  0f : 0; 
                    // For Azimuth map:
                    // Change Mathf.Clamp01(GetElevationAngle(worldposition)) to Mathf.Clamp01(GetAzimuthAngle(worldposition))
                    // For height map:
                    // Change Mathf.Clamp01(GetElevationAngle(worldposition)) to Mathf.Clamp01(height / terrainData.size.y)
                    // For slope map:
                    // Change Mathf.Clamp01(GetElevationAngle(worldposition)) to Mathf.Clamp01(steepness*steepness/(terrainData.heightmapResolution/10.0f))
                }
            }
        }
        float GetElevationAngle(Vector3 point)
    {
        Debug.Log($"Testing viability for point: {point.x}, {point.y}, {point.z}");
        (float latitude, float longitude) = FindLatitudeLongitudeOfUnityPoint(point);
        Debug.Log($"{latitude}, {longitude}");

        Vector3 cartesianPoint = CartesianConversion(latitude, longitude);
        Debug.Log($"{cartesianPoint.x}, {cartesianPoint.y}, {cartesianPoint.z}");

        float azimuthToEarth = AzimuthToEarth(latitude, longitude);
        float elevationAngleToEarth = ElevationAngleToEarth(cartesianPoint);
        Debug.Log($"{azimuthToEarth * Mathf.Rad2Deg}, {elevationAngleToEarth * Mathf.Rad2Deg}, {{elevationAngleOfTerrain * Mathf.Rad2Deg}}");
        return elevationAngleToEarth;
    }

    float GetAzimuthAngle (Vector3 point)
    {
        Debug.Log($"Testing viability for point: {point.x}, {point.y}, {point.z}");
        (float latitude, float longitude) = FindLatitudeLongitudeOfUnityPoint(point);
        Debug.Log($"{latitude}, {longitude}");

        Vector3 cartesianPoint = CartesianConversion(latitude, longitude);
        Debug.Log($"{cartesianPoint.x}, {cartesianPoint.y}, {cartesianPoint.z}");

        float azimuthToEarth = AzimuthToEarth(latitude, longitude);
        float elevationAngleToEarth = ElevationAngleToEarth(cartesianPoint);
        Debug.Log($"{azimuthToEarth * Mathf.Rad2Deg}, {elevationAngleToEarth * Mathf.Rad2Deg}, {{elevationAngleOfTerrain * Mathf.Rad2Deg}}");
        return azimuthToEarth;
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
        float x = Mathf.Sin(earthLongitudeInRadians - longitudeInRadians) * Mathf.Cos(earthLatitudeInRadians);
        float y = (Mathf.Cos(latitudeInRadians) * Mathf.Sin(earthLatitudeInRadians)) - (Mathf.Sin(latitudeInRadians) * Mathf.Cos(earthLatitudeInRadians) * Mathf.Cos(earthLongitudeInRadians - longitudeInRadians));
        float azimuthAngle = Mathf.Atan2(y, x);

        return azimuthAngle;
    }

    float DegreesToRadians(float degreeValue)
    {
        return degreeValue * Mathf.PI / 180f;
    }

    
    float ElevationAngleToEarth(Vector3 point)
    {
        Vector3 horizontalVectorToEarth = new Vector3(earth.x - point.x, 0f, earth.z - point.z);
        float horizontalDistanceToEarth = horizontalVectorToEarth.magnitude;
        float verticalDistanceToEarth = earth.y - point.y;
        float finalAngle = Mathf.Atan2(verticalDistanceToEarth, horizontalDistanceToEarth);

        return finalAngle;
    }

    }
        }
    }

    
}

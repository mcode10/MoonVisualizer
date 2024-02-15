using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MakeTerrainMapEA : MonoBehaviour
{
 
       
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
         const float earthLatitudeInRadians = earthLatitude * Mathf.Deg2Rad;
        const float earthLongitudeInRadians = earthLongitude * Mathf.Deg2Rad;
         Vector3 earth = new Vector3(earthX, earthY, earthZ);
         
    void Start () {
        
        Terrain[] terrains = Terrain.activeTerrains;

        for (int r = 0; r < terrains.Length; r++)
        {
            Debug.Log(terrains[r]);
        }
        


        for (int i = 0; i < terrains.Length; i++)
        {

            TerrainData terrain = terrains[i].terrainData;
        

         // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[, ,] splatmapData = new float[terrain.alphamapWidth, terrain.alphamapHeight, terrain.alphamapLayers];
        

        // DONT DELETE THIS: Loops through points to find elevation angle at each point
        for (int y = 0; y < terrain.alphamapHeight; y++)
            {
            for (int x = 0; x < terrain.alphamapWidth; x++)
             {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)terrain.alphamapHeight;
                float x_01 = (float)x/(float)terrain.alphamapWidth;
                
                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrain.GetInterpolatedNormal(y_01,x_01);
                float terrainwidth = terrain.size.x;
                float terrainlength = terrain.size.z;
                float terrainpositionx = x_01 * terrainwidth;
                float terrainpositionz = y_01 * terrainlength;
                 
                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrain.alphamapLayers];
                Debug.Log(splatWeights);
                
                // Assigns base texture 
                splatWeights[0] = 0.5f;
                Debug.Log(splatWeights);
                // Assign elevation angles to a SplatMap texture
                 
                splatWeights[1] = GetElevationAngle(CartesianConversion(terrainpositionx, terrainpositionz));
                 Debug.Log(GetElevationAngle(CartesianConversion(terrainpositionx, terrainpositionz)));
                 Debug.Log(splatWeights[1]);
                
                
                float z = splatWeights.Sum();
                  Debug.Log(z);
                // Loop through each terrain texture
                for(int p = 0; p < terrain.alphamapLayers; p++){
                    if (x < terrain.alphamapWidth && y < terrain.alphamapHeight && p < terrain.alphamapLayers) {
                    
                    
                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                     

                    // Normalize so that sum of all texture weights = 1
                    if (z != 0)
                    splatWeights[p] /= z;
                     

                    // Assign this point to the splatmap array
                    splatmapData[x, y, p] = splatWeights[p];
                     Debug.Log(splatWeights[p]);
                    
                    }
                }
            }
            
        }
        terrain.SetAlphamaps(0, 0, splatmapData);
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
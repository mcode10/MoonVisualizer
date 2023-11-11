using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainDataImporter : MonoBehaviour
{
    // Constants for normalizing the height.
    const float minHeight = -418.9165344238281f;
    const float maxHeight = 1648.679809570313f;

    // Terrain size constant.
    const int terrainSize = 3200;

    // Start is called before the first frame update
    void Start()
    {
        List<List<float>> points = ReadCSV();
        points = ScaleData(points);
        Debug.Log("Started conversion to array.");
        float[,] pointArray = ConvertScaledDataTo2DArray(points);
        Debug.Log("Finished conversion to array.");

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = SetHeights(pointArray, terrain.terrainData);

        Debug.Log("Complete?");
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<List<float>> ReadCSV()
    {
        List<List<float>> points = new List<List<float>>();
        string path = "/home/namun/Documents/Unity/MoonVisualizer/Assets/LatitudeLongitudeHeight.csv";
        int count = 0;
        using (StreamReader sr = new StreamReader(path))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] splitLines = line.Split(',');

                float latitude = float.Parse(splitLines[0]);
                float longitude = float.Parse(splitLines[1]);

                float height = float.Parse(splitLines[2]);
                float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, height);
                if (count == 0)
                {
                    Debug.Log($"{normalizedHeight}");
                }

                List<float> point = new List<float>();

                point.Insert(0, latitude);
                point.Insert(1, longitude);
                point.Insert(2, normalizedHeight);

                points.Insert(count, point);
                count++;
            }
        }

        return points;
    }

    List<List<float>> ScaleData(List<List<float>> points)
    {
        int index = 0;
        List<List<float>> scaledPoints = new List<List<float>>();
        foreach (List<float> point in points)
        {
            float scalingConstant = 2f;

            float latitude = point[0] * scalingConstant;
            float longitude = point[1] * scalingConstant;
            float height = point[2];

            // points.Remove(point);
            List<float> scaledPoint = new List<float>();

            scaledPoint.Insert(0, latitude);
            scaledPoint.Insert(1, longitude);
            scaledPoint.Insert(2, height);

            scaledPoints.Insert(index, scaledPoint);
            index++;
        }

        return scaledPoints;
    }


    float[,] ConvertScaledDataTo2DArray(List<List<float>> points)
    {
        float[,] heights = new float[3200, 3200];
        // int xIndex = 0;
        // int yIndex = 0;
        int mainIndex = 0;
        // foreach (List<float> point in points)
        // {
        //     heights[xIndex, yIndex] = point[2];

        // if (++yIndex >= 3200)
        // {
        //     yIndex = 0;
        //     if (++xIndex >= 3200)
        //     {
        //         xIndex = 0;
        //     }
        //     else
        //     {
        //         xIndex++;
        //     }
        // }
        // else
        // {
        //     yIndex++;
        //     xIndex = 0;
        // }

        // }
        for (int x = 0; x < 3200; x++)
        {
            for (int y = 0; y < 3200; y++)
            {
                List<float> point = points[mainIndex];
                heights[x, y] = point[2];
                mainIndex++;
            }
        }


        return heights;
    }

    TerrainData SetHeights(float[,] points, TerrainData terrainData)
    {
        terrainData.heightmapResolution = 3200;
        Debug.Log(terrainData.heightmapResolution);
        terrainData.size = new Vector3(terrainSize, 500, terrainSize);
        terrainData.SetHeights(0, 0, points);
        return terrainData;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainDataImporter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<List<float>> points = ReadCSV();
        points = ScaleData(points);
        float[,] pointArray = ConvertScaledDataTo2DArray(points);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = SetHeights(pointArray, terrain.terrainData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<List<float>> ReadCSV()
    {
        List<List<float>> points = new List<List<float>>();
        string path = "/LatitudeLongitudeHeight.csv";
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
                List<float> point = new List<float>();

                point.Insert(0, latitude);
                point.Insert(1, longitude);
                point.Insert(2, height);

                points.Insert(count, point);
                count += count;

                if (count == 0)
                {
                    Console.WriteLine(point);
                }
            }
        }

        return points;
    }

    List<List<float>> ScaleData(List<List<float>> points)
    {
        int index = 0;
        foreach (List<float> point in points)
        {
            float scalingConstant = 5f;

            float latitude = point[0] * scalingConstant;
            float longitude = point[1] * scalingConstant;
            float height = point[2] * scalingConstant;

            points.Remove(point);
            List<float> scaledPoint = new List<float>();

            scaledPoint.Insert(0, latitude);
            scaledPoint.Insert(1, longitude);
            scaledPoint.Insert(2, height);

            points.Insert(index, scaledPoint);
            index++;
        }

        return points;
    }


    float[,] ConvertScaledDataTo2DArray(List<List<float>> points)
    {
        float[,] heights = new float[points.Count, points.Count];
        foreach (List<float> point in points)
        {
            heights[(int)Math.Round(point[0]), (int)Math.Round(point[1])] = point[0];
        }

        return heights;
    }

    TerrainData SetHeights(float[,] points, TerrainData terrainData)
    {
        terrainData.SetHeights(0, 0, points);
        return terrainData;
    }
}

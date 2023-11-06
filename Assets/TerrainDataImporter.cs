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
        Console.WriteLine(points);

    }

    // Update is called once per frame
    void Update()
    {

    }

    List<List<float>> ReadCSV()
    {
        List<List<float>> points = new List<List<float>>();
        string path = @"./LatitudeLongitudeHeight.csv";
        int count = 0;
        using (StreamReader sr = new StreamReader(path))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] splitLines = line.Split(',');

                float latitude = float.Parse(splitLines[0]);
                float longitude = float.Parse(splitLines[1]);
                float height = float.Parse(splitLines[3]);
                List<float> point = new List<float>();

                point.Insert(0, latitude);
                point.Insert(1, longitude);
                point.Insert(2, height);

                points.Insert(count, point);
                count += count;
            }
        }
    }

}

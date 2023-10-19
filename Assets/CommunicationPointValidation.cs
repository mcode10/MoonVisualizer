using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Coordinate
{
    Coordinate(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override string ToString() => $"[{X}, {Y}, {Z}]";
}

public struct LongitudeLatitude
{
    LongitudeLatitude(float longitude, float latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }

    public float Longitude { get; set; }
    public float Latitude { get; set; }

    public override string ToString() => $"[{Longitude}, {Latitude}]";
}

public class CommunicationPointValidation : MonoBehaviour
{
    // The Lunar radius.
    const float lunarRadius = 1737.4;

    // These are DUMMY values.
    const float earthLongitude = 232.23;
    const float earthLatitude = 232.23;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    Float AzimuthAngleToEarth(LongitudeLatitude longitudeLatitude)
    {
        float longitude = LongitudeLatitude.Longitude;
        float latitude = LongitudeLatitude.Latitude;

        Float azimuthAngle = Atan2(MathF.Sin());
    }

    bool IsPointValid(Coordinate coordinate)
    {

        return false;
    }

    Float Atan2(Coordinate coordinate)
    {
        switch (coordinate)
        {
            case coordinate.X > 0:
                float atan2_result = AtanWhereXGreaterThanZero(point);
                break;
            case (coordinate.X < 0) && (coordinate.Y >= 0):
                float atan2_result = AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(coordinate);
                break;
            case (coordinate.X < 0) && (coordinate.Y < 0):
                float atan2_result = AtanWhereXAndYLessThanZero(coordinate);
                break;
            case (coordinate.X == 0) && (coordinate.Y > 0):
                float atan2_result = AtanWhereXEqualAndYGreaterThanZero(coordinate);
                break;
            case (coordinate.X == 0) && (coordinate.Y < 0):
                float atan2_result = AtanWhereXEqualAndYLessThanZero(coordinate);
                break;
            case (coordinate.X == 0) && (coordinate.Y == 0):
                throw new ArgumentException("Invalid input for atan calculation", coordinate);
                break;
            default:
                throw new ArgumentException("Bad inputs", coordinate);
                break;
        };

        return atan2_result;
    }

    float AtanWhereXGreaterThanZero(Coordinate coordinate)
    {
        float yDividedByX = coordinate.Y / coordinate.X;

        return MathF.Atan(yDividedByX);
    }

    float AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(Coordinate coordinate)
    {
        float yDividedByXPlusPi = (coordinate.Y / coordinate.X) + Math.Pi;

        return MathF.Atan(yDividedByXPlusPi);

    }

    float AtanWhereXAndYLessThanZero(Coordinate coordinate)
    {
        float yDividedByXMinusPi = (coordinate.Y / coordinate.X) + Math.Pi;

        return MathF.Atan(yDividedByXMinusPi);
    }

    float AtanWhereXEqualAndYGreaterThanZero(Coordinate coordinate)
    {
        return Math.Pi / 2;
    }

    float AtanWhereXEqualAndYLessThanZero(Coordinate coordinate)
    {
        return Math.Pi / -2;
    }

    Coordinate ConvertLatitudeAndLongitudeToCoordinate(float terrainHeight, float longitude, float latitude)
    {
        float radius = lunarRadius + terrainHeight;
        float x = radius * MathF.Cos(latitude) * MathF.Cos(longitude);
        float y = radius * MathF.Cos(latitude) * MathF.Sin(longitude);
        float z = radius * MathF.Sin(latitude);

        return new Point(x, y, z);
    }

    // LongitudeLatitude ConvertCoordinateToLongitudeLatitude() {}
    // A function skeleton that may need to be filled out.

}

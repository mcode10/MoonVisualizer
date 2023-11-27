using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// public struct Coordinate
// {
//     Coordinate(float x, float y, float z)
//     {
//         X = x;
//         Y = y;
//         Z = z;
//     }

//     public float X { get; set; }
//     public float Y { get; set; }
//     public float Z { get; set; }

//     public override string ToString() => $"[{X}, {Y}, {Z}]";
// }

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
    const float lunarRadius = 1737.4f;

    // These are DUMMY values.
    const float earthLongitude = 232.23f;
    const float earthLatitude = 232.23f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // float AzimuthAngleToEarth(LongitudeLatitude longitudeLatitude)
    // {
    //     float longitude = LongitudeLatitude.Longitude;
    //     float latitude = LongitudeLatitude.Latitude;

    //     float azimuthAngle = Atan2(MathF.Sin());
    // }

    bool IsPointValid(Vector3 coordinate)
    {

        return false;
    }

    // float Atan2(Vector3 coordinate)
    // {
    //     float atan2_result = coordinate switch
    //     {
    //         (coordinate.x > 0) => AtanWhereXGreaterThanZero(coordinate),
    //         (coordinate.x < 0) && (coordinate.y >= 0) => AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(coordinate),
    //         (coordinate.x < 0) && (coordinate.y < 0) => AtanWhereXAndYLessThanZero(coordinate),
    //         (coordinate.x == 0) && (coordinate.y > 0) => AtanWhereXEqualAndYGreaterThanZero(coordinate),
    //         (coordinate.x == 0) && (coordinate.y < 0) => AtanWhereXEqualAndYLessThanZero(coordinate),
    //         (coordinate.x == 0) && (coordinate.y == 0) => throw new ArgumentException("Invalid input for atan calculation"),
    //         _ => throw new ArgumentException("Invalid input for atan calculation"),
    //     };

    //     return atan2_result;
    // }

    float AtanWhereXGreaterThanZero(Vector3 coordinate)
    {
        float yDividedByX = coordinate.y / coordinate.x;

        return MathF.Atan(yDividedByX);
    }

    float AtanWhereXLessThanZeroAndYGreaterThanOrEqualToZero(Vector3 coordinate)
    {
        float yDividedByXPlusPi = (coordinate.y / coordinate.x) + MathF.PI;

        return MathF.Atan(yDividedByXPlusPi);

    }

    float AtanWhereXAndYLessThanZero(Vector3 coordinate)
    {
        float yDividedByXMinusPi = (coordinate.y / coordinate.x) + MathF.PI;

        return MathF.Atan(yDividedByXMinusPi);
    }

    float AtanWhereXEqualAndYGreaterThanZero(Vector3 coordinate)
    {
        return MathF.PI / 2f;
    }

    float AtanWhereXEqualAndYLessThanZero(Vector3 coordinate)
    {
        return MathF.PI / -2f;
    }

    Vector3 ConvertLatitudeAndLongitudeToCoordinate(float terrainHeight, float longitude, float latitude)
    {
        float radius = lunarRadius + terrainHeight;
        float x = radius * MathF.Cos(latitude) * MathF.Cos(longitude);
        float y = radius * MathF.Cos(latitude) * MathF.Sin(longitude);
        float z = radius * MathF.Sin(latitude);

        return new Vector3(x, y, z);
    }

    // LongitudeLatitude ConvertCoordinateToLongitudeLatitude() {}
    // A function skeleton that may need to be filled out.

}

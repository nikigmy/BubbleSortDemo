using Unity.Mathematics;
using UnityEngine;

public static class CubeHelpers
{
    public static void CalculatePosition(int index, ref float3 result, int cols = 100, int offset = 2)
    {
        result.z = (int)(index / cols);
        result.x = index - (result.z * cols);
        if ((int)result.z % 2 == 1)
        {
            result.x = cols - result.x - 1;
        }
        result.x *= offset;
        result.z *= -offset;
    }

    public static void GetColors(int length, Color startColor, float intervalR, float intervalG, float intervalB, ref Color[] colors)
    {
        for (int i = 0; i < length; i++)
        {
            colors[i] = new Color(startColor.r + intervalR * i, startColor.g + intervalG * i, startColor.b + intervalB * i);
        }
    }
    public static void GetColorsRedGreen(int length, ref Color[] colors)
    {
        float ratio;
        for (int i = 0; i < length; i++)
        {
            ratio = (float)i / length;
            colors[i] = new Color(2 * (1-ratio), 2 * ratio, 0);
        }
    }
}
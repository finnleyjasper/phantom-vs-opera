using UnityEngine;

public static class ToneMapper
{
    public static int minTone = 1;
    public static int maxTone = 10;

    public static float minY = 0f;
    public static float maxY = 10f;

    public static float GetYPosition(int tone)
    {
        float t = (float)(tone - minTone) / (maxTone - minTone);
        return Mathf.Lerp(minY, maxY, t);
    }
}
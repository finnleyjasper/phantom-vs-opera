using UnityEngine;

public static class PitchMapper
{
    public static int minTone = 1;
    public static int maxTone = 10;

    // lanes
    private static float[] laneZ = { -4f, -2f, 0f, 2f, 4f };

    public static float GetZPosition(int tone)
    {
        // Normalize tone → 0 to 1
        float t = (float)(tone - minTone) / (maxTone - minTone);

        // Map to lane index
        int index = Mathf.RoundToInt(t * (laneZ.Length - 1));

        return laneZ[index];
    }
}

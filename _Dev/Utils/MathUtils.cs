using Godot;

public static class MathUtils
{
    public static float SqrDistance(Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).LengthSquared();
    }

    public static float Lerp(float a, float b, float t)
    {
        return (1.0f - t) * a + b * t;
    }

    public static float InvLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }

    public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = InvLerp(iMin, iMax, v);
        return Lerp(oMin, oMax, t);
    }
}

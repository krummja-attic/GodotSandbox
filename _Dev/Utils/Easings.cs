using Godot;

public static class Easings
{
    public static class Back
    {
        public static float EaseIn(float t, float b, float c, float d, float s = 1.70158f)
        {
            t = t / d;
            return c * t * t * ((s + 1) * t - s) + b;
        }

        public static float EaseOut(float t, float b, float c, float d, float s = 1.70158f)
        {
            t = t / d - 1;
            return c * (t * t * ((s + 1) * t + s) + 1) + b;
        }

        public static float EaseInOut(float t, float b, float c, float d, float s = 1.70158f)
        {
            t = t / d * 2;
            s = s * 1.525f;
            if (t < 1)
                return c / 2 * (t * t * ((s + 1) * t - s)) + b;
            t = t - 2;
            return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
        }
    }
}

public static class EasingsV3
{
    public static class Back
    {
        public static Vector3 EaseIn(float t, Vector3 b, Vector3 c, float d, float s = 1.70158f)
        {
            return new Vector3(
                Easings.Back.EaseIn(t, b.x, c.x, d, s),
                Easings.Back.EaseIn(t, b.y, c.y, d, s),
                Easings.Back.EaseIn(t, b.z, c.z, d, s)
            );
        }
        
        public static Vector3 EaseOut(float t, Vector3 b, Vector3 c, float d, float s = 1.70158f)
        {
            return new Vector3(
                Easings.Back.EaseOut(t, b.x, c.x, d, s),
                Easings.Back.EaseOut(t, b.y, c.y, d, s),
                Easings.Back.EaseOut(t, b.z, c.z, d, s)
            );
        }
        
        public static Vector3 EaseInOut(float t, Vector3 b, Vector3 c, float d, float s = 1.70158f)
        {
            return new Vector3(
                Easings.Back.EaseInOut(t, b.x, c.x, d, s),
                Easings.Back.EaseInOut(t, b.y, c.y, d, s),
                Easings.Back.EaseInOut(t, b.z, c.z, d, s)
            );
        }
    }
}

using System;
using Godot;

public static class SpatialUtils
{
    public static Vector3 PlaneTransform(Transform transform)
    {
        return new Vector3(transform.origin.x, 0f, transform.origin.z);
    }

    public static Vector3 PlaneVector(Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }
    
    public static Vector3 PlaneVector(Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    public static Vector2 VectorClamp(Vector2 vector, float min, float max)
    {
        float x = Mathf.Clamp(vector.x, min, max);
        float y = Mathf.Clamp(vector.y, min, max);
        return new Vector2(x, y);
    }
    
    public static Vector3 VectorClamp(Vector3 vector, float min, float max)
    {
        float x = Mathf.Clamp(vector.x, min, max);
        float y = Mathf.Clamp(vector.y, min, max);
        float z = Mathf.Clamp(vector.z, min, max);
        return new Vector3(x, y, z);
    }
    
    public static Vector3[] ComputeSubvectors(Vector3 start, Vector3 end, int steps)
    {
        Vector3 totalVector = end - start;
        
        GD.Print("Total Vector        " + totalVector);
        GD.Print("-----------------------------------------");
        
        float xComp = totalVector.x / steps;
        float zComp = totalVector.z / steps;

        int count = Mathf.CeilToInt(Mathf.Max(totalVector.x / xComp, totalVector.z / zComp));
        Vector3[] segments = new Vector3[count];
        
        Vector3 nextPos = start;
        
        for (int i = 0; i < count; i++)
        {
            segments[i] = new Vector3(xComp, 0f, zComp);
            GD.Print("Next Segment        " + segments[i]);
        }
        
        return segments;
    }
}

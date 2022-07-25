using System;
using Godot;

public class StarGenerator : Spatial
{
    [Export(PropertyHint.Range, "100,100000,100")]
    public int StarCount = 100;

    [Export]
    public int MinRadius = 3;
    
    [Export]
    public int MaxRadius = 100;
    
    [Export(PropertyHint.File)]
    public string StarScene;
    
    private PackedScene starRes;

    private Random _random;

    public override void _Ready()
    {
        starRes = ResourceLoader.Load<PackedScene>(StarScene);
        Generate();
    }

    private void Generate()
    {
        for (int i = 0; i < StarCount; i++)
        {
            _random = new Random(i);
            Spatial star = starRes.Instance<Spatial>();
            Vector3 position = GetPoint(MinRadius, MaxRadius);
            star.Transform = new Transform(Basis.Identity, position);
            AddChild(star);
        }
    }

    private Vector3 GetPoint(int minRadius, int maxRadius)
    {
        float d = 1.0f;
        float x, y, z;

        int scale = _random.Next(minRadius, maxRadius);

        do
        {
            x = (float) _random.NextDouble() * 2.0f - 1.0f;
            y = (float) _random.NextDouble() * 2.0f - 1.0f;
            z = (float) _random.NextDouble() * 2.0f - 1.0f;
            d = x * x + y * y + z * z;
        } 
        while (d > 1.0);
        
        return new Vector3(x * scale, y  * (scale / 10), z * scale);
    }
}

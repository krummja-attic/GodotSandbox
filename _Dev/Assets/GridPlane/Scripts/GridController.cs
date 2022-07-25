using Godot;

public class GridController : Spatial
{
    [Export(PropertyHint.Range, "0.01, 1000")]
    public float Snap;

    private Spatial Target;
    
    private float Distance;
    
    public override void _Ready()
    {
        Target = GetNode<Spatial>("/root/Root/CameraRig");
        Distance = this.Transform.origin.DistanceTo(Target.Transform.origin);
    }

    public override void _Process(float delta)
    {
        if (Snap < 0.01f) Snap = 0.01f;

        Vector3 position = new Vector3(
            Mathf.Round(Target.Transform.origin.x / Snap) * Snap,
            Target.Transform.origin.y,
            Mathf.Round(Target.Transform.origin.z / Snap) * Snap
        );

        Transform t = Transform;
        t.origin = position;
        Transform = t;
    }
}

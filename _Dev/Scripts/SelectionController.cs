using Godot;

public class SelectionController : Spatial
{
    private const int PATH_IDX = 0;
    private const int PATH_FOLLOW_IDX = 0;
    
    private Spatial CameraRig;
    
    private Spatial SpatialSelection;

    [Signal]
    public delegate void SetTraversalTarget(Vector3 target);
    
    public override void _Ready()
    {
        CameraRig = GetNode<Spatial>("/root/Root/CameraRig");
        Connect(nameof(SetTraversalTarget), CameraRig, "OnSetTraversalTarget");
    }

    public void OnSetSelection(Spatial selected)
    {
        SpatialSelection = selected;
        EmitSignal(nameof(SetTraversalTarget), selected.Transform.origin);
    }
}

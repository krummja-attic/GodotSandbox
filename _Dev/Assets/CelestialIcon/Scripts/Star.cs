using Godot;
using System;

public class Star : Spatial
{
    private const float UNIT = 0.1f;

    private const float RADIUS = 100f;
    
    private Area Selection;

    private Sprite3D Icon;

    private Sprite3D Line;

    private Sprite3D Intercept;

    private Spatial CameraRig;

    private Spatial Grid;

    private Spatial SelectionController;

    [Signal]
    public delegate void SetSelection(Spatial selectedObject); 
    
    public override void _Ready()
    {
        Selection = GetNode<Area>("Area");
        Selection.Connect("input_event", this, nameof(OnSelected));

        SelectionController = GetNode<Spatial>("/root/Root/SelectionController");
        Connect(nameof(SetSelection), SelectionController, "OnSetSelection");

        Icon = GetNode<Sprite3D>("Icon");
        Line = GetNode<Sprite3D>("Line");
        Intercept = GetNode<Sprite3D>("Intercept");
        CameraRig = GetNode<Spatial>("/root/Root/CameraRig");
        Grid = GetNode<Spatial>("/root/Root/GridController");
        
        SetInterceptParameters();
    }

    public override void _Process(float delta)
    {
        SetDistanceParameters();
        SetInterceptParameters();
    }

    private void SetDistanceParameters()
    {
        // Get the unit distance between the icon and the camera
        float d = MathUtils.SqrDistance(Transform.origin, CameraRig.Transform.origin);
        
        // Convert 0 to RADIUS distance range to 0f and 1f range
        float lerp = MathUtils.Remap(d, 0f, RADIUS, 0f, 1f);
        
        // Lerp opacity
        Color colIcon = Icon.Modulate;
        Color colLine = Line.Modulate;
        Color colInte = Intercept.Modulate;

        colIcon.a = Mathf.Clamp(MathUtils.InvLerp(0.1f, 1f, lerp), 0.1f, 1f);
        colLine.a = Mathf.Clamp(MathUtils.InvLerp(0f, 1f, lerp), 0f, 1f);
        colInte.a = Mathf.Clamp(MathUtils.InvLerp(0f, 1f, lerp), 0f, 1f);

        Icon.Modulate = colIcon;
        Line.Modulate = colLine;
        Intercept.Modulate = colInte;
    }
    
    private void SetInterceptParameters()
    {
        float x = Transform.origin.x;
        float z = Transform.origin.z;
        float height = Transform.origin.y - Grid.Transform.origin.y;

        Line.Translation = new Vector3(0, -height, 0);
        
        if (height > 0)
            Line.Offset = new Vector2(0, 500);
        else
            Line.Offset = new Vector2(0, -500);
        
        Line.Scale = new Vector3(0.1f, height * UNIT, 0.1f);
        Intercept.Translation = new Vector3(0, -height, 0);
    }
    
    private void OnSelected(Node camera, InputEvent @input, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@input is InputEventMouseButton @mouseEvent)
        {
            switch ((ButtonList) @mouseEvent.ButtonIndex)
            {
                case ButtonList.Left:
                {
                    if (@mouseEvent.Pressed)
                        EmitSignal(nameof(SetSelection), this);
                    break;
                }
            }
        }
    }
}

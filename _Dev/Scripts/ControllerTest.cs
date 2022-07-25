using Godot;
using System;

public class ControllerTest : Spatial
{
    // Controller Parameters
    [Export]
    public float MaxImpulse = 5f;

    [Export]
    public float Acceleration = 10f;

    [Export]
    public float Damping = 15f;
    
    // Node Tree References
    private Spatial Root;
    private Camera Camera;
    private Position3D Target;

    // Mouse Input Vectors
    private Vector2 PanStart;
    private Vector2 PanDelta;
    private Vector2 PanStop;
    
    // Velocity Vectors
    private Vector3 Velocity;
    
    // Position Vectors
    private Vector3 LastPosition;
    private Vector3 TargetPosition;

    // Input Flags
    private bool IsPanning;
    
    // Motion Variables
    private float Impulse;

    private Vector3 Origin
    {
        get
        {
            return this.Transform.origin;
        }
        
        set
        {
            Transform transform = this.Transform;
            transform.origin = value;
            this.Transform = transform;
        }
    }
    
    public override void _Ready()
    {
        Root = GetParent<Spatial>();
        Camera = GetChild<Camera>((int) ChildIndices.CAMERA);
        Target = GetChild<Position3D>((int) ChildIndices.TARGET);
        
        Camera.LookAt(Target.Transform.origin, Vector3.Up);

        LastPosition = Origin;
    }

    public override void _Input(InputEvent @inputEvent)
    {
        if (@inputEvent is InputEventMouseButton @buttonEvent)
        {
            switch ((ButtonList) @buttonEvent.ButtonIndex)
            {
                case ButtonList.Left:
                {
                    if (!IsPanning && @buttonEvent.Pressed)
                    {
                        PanStart = @buttonEvent.Position;
                        IsPanning = true;
                    }

                    if (IsPanning && !@buttonEvent.Pressed)
                    {
                        PanStop = @buttonEvent.Position;
                        IsPanning = false;
                    }

                    break;
                }
            }
        }

        if (@inputEvent is InputEventMouseMotion @mouseMotion)
        {
            // Use this for non-continuous input, e.g. for camera rotation, 
            // where you don't want to constantly add the mouse delta as an
            // impulse to the rotation, but instead want to do an absolute
            // transform in sync with the mouse movement.
        }
    }

    public override void _Process(float delta)
    {
        GetMousePanInput();
        UpdateVelocityStep(delta);
        UpdatePositionStep(delta);
    }

    private void GetMousePanInput()
    {
        if (!IsPanning) return;

        PanStop = GetViewport().GetMousePosition();
        PanDelta = (PanStart - PanStop) / 50f;
        PanDelta = SpatialUtils.VectorClamp(PanDelta, -10f, 10f);
        TargetPosition = SpatialUtils.PlaneVector(PanDelta) * -1.0f;
    }
    
    private void UpdateVelocityStep(float delta)
    {
        Velocity = (Origin - LastPosition) / delta;
        LastPosition = Origin;
    }

    private void UpdatePositionStep(float delta)
    {
        if (TargetPosition.Length() > 0.1f)
        {
            Impulse = Mathf.Lerp(Impulse, MaxImpulse, delta * Acceleration);
            Translate(TargetPosition * Impulse * delta);
        }
        
        else
        {
            Velocity = Velocity.LinearInterpolate(Vector3.Zero, delta * Damping);
            Origin += Velocity * delta;
        }
        
        TargetPosition = Vector3.Zero;
    }

    private enum ChildIndices
    {
        CAMERA = 0,
        TARGET = 1,
    }
}


using Godot;
using System;


public enum ChildIndices
{
    CAMERA = 0,
    TARGET = 1,
}

public class CameraController : Spatial
{
    [Export(PropertyHint.Range, "1,10")]
    public float MaxSpeed = 5f;
    
    [Export(PropertyHint.Range, "5,15")]
    public float Acceleration = 10f;

    [Export(PropertyHint.Range, "1,20")]
    public float Damping = 15f;

    [Export]
    public float StepSize = 2f;
    
    [Export(PropertyHint.Range, "1,10")]
    public float ZoomDamping = 7.5f;
    
    [Export]
    public float MinHeight = 5f;
    
    [Export]
    public float MaxHeight = 50f;
    
    [Export(PropertyHint.Range, "1,10")]
    public float ZoomSpeed = 2f;

    [Export(PropertyHint.Range, "0, 1")]
    public float MaxRotationSpeed = 0.1f;
    
    [Export(PropertyHint.Range, "0,10")]
    public float RotationDamping = 5f;

    [Export(PropertyHint.Range, "1, 10")]
    public float HeightSpeed = 2f;
    
    [Export(PropertyHint.Range, "0, 10")]
    public float HeightDamping = 2f;

    private Node Root;
    private Camera Camera;
    private Position3D Target;
    private Label PositionLabel;
    private Label VelocityLabel;
    
    private Quat TargetRotation;

    private Vector3[] VectorSteps;
    
    private Vector3 HorizontalInput;
    private Vector3 HorizontalVelocity;
    private Vector3 LastPosition;
    private Vector3 DeltaPosition;
    private Vector3 TargetPosition;
    
    private Vector2 StartDrag;
    private Vector2 EndDrag;
    
    private float Speed;
    private float TargetZoomHeight;
    private float TargetRigHeight;

    private bool IsPanning;
    private bool IsDragging;
    public bool IsTranslating;
    public bool IsCtrlHeld;
    public bool IsShiftHeld;
    
    public override void _Ready()
    {
        Root = GetNode<Node>("/root/Root");
        Camera = GetNode<Camera>("Camera");
        // PositionLabel = GetNode<Label>("/root/Root/GUI/Debug/Position");
        // VelocityLabel = GetNode<Label>("/root/Root/GUI/Debug/Velocity");

        Target = GetNode<Position3D>("Target");
        Camera.LookAt(Target.Transform.origin, Vector3.Up);

        LastPosition = Transform.origin;
        TargetRotation = Transform.basis.Quat();
        TargetZoomHeight = Camera.GlobalTransform.origin.y;
    }

    public override void _Process(float delta)
    {
        // String xPos = Transform.origin.x.ToString("n2");
        // String yPos = Transform.origin.y.ToString("n2");
        // String zPos = Transform.origin.z.ToString("n2");
        // PositionLabel.Text = xPos + ", " + yPos + ", " + zPos;
        //
        // String xVel = HorizontalVelocity.x.ToString("n2");
        // String zVel = HorizontalVelocity.z.ToString("n2");
        // VelocityLabel.Text = xVel + ", " + zVel;
        
        // WASD Movement
        GetKeyboardInput(delta);
        
        // LMB + Drag Movement
        GetMousePanInput(delta);
        
        if (!IsTranslating)
        {
            // Rig Velocity Update
            UpdateHorizontalVelocity(delta);
            
            // Rig Position Update
            UpdateHorizontalPosition(delta);
        }
        
        if (IsTranslating)
        {
            Transform t = Transform;
            t.origin = TargetPosition;
            Transform = t;

            LastPosition = TargetPosition;
            IsTranslating = false;
        }
        
        // Rig Rotation Update
        UpdateCameraRotation(delta);

        // Camera Height Update
        UpdateCameraHeight(delta);
        
        // Rig Height Update
        UpdateRigHeight(delta);
    }

    public override void _Input(InputEvent @inputEvent)
    {
        if (@inputEvent is InputEventWithModifiers @modEvent)
        {
            IsCtrlHeld = @modEvent.Control;
            IsShiftHeld = @modEvent.Shift;
        }
        
        if (@inputEvent is InputEventMouseButton @mouseEvent)
        {
            switch ((ButtonList) @mouseEvent.ButtonIndex)
            {
                case ButtonList.Left:
                {
                    if (!IsPanning && @mouseEvent.Pressed)
                    {
                        StartDrag = @mouseEvent.Position;
                        IsPanning = true;
                    }
                    
                    if (IsPanning && !@mouseEvent.Pressed)
                    {
                        IsPanning = false;
                    }
                    
                    break;
                }
                
                case ButtonList.Right:
                {
                    if (!IsDragging && @mouseEvent.Pressed)
                        IsDragging = true;
                    if (IsDragging && !@mouseEvent.Pressed)
                        IsDragging = false;
                    break;
                }
            }
            
            float wheelDelta = 0;
            
            if (@mouseEvent.IsPressed())
            {
                switch ((ButtonList) @mouseEvent.ButtonIndex)
                {
                    case ButtonList.WheelDown:
                    {
                        wheelDelta = (@mouseEvent.Factor / 10f);
                        if (IsCtrlHeld)
                        {
                            SetRigHeight(wheelDelta);
                        }
                        else
                        {
                            SetCameraHeight(wheelDelta);
                        }
                        break;
                    }

                    case ButtonList.WheelUp:
                    {
                        wheelDelta = -(@mouseEvent.Factor / 10f);
                        if (IsCtrlHeld)
                        {
                            SetRigHeight(wheelDelta);
                        }
                        else
                        {
                            SetCameraHeight(wheelDelta);
                        }
                        break;
                    }
                }
            }
        }

        if (@inputEvent is InputEventMouseMotion @motionEvent)
        {
            if (IsDragging)
            {
                float value = @motionEvent.Relative.x * -1;

                if (Mathf.Abs(value) > 0.1f)
                {
                    Quat basis = Transform.basis.Quat();
                    float yEuler = Transform.basis.GetEuler().y;
                    float target = (value * MaxRotationSpeed + yEuler);
                    TargetRotation = new Quat(Vector3.Up, target);
                }
            }
        }
    }

    public void OnSetTraversalTarget(Vector3 target)
    {
        // TODO Figure out why traversal keeps resetting to the prior Y level.
        IsTranslating = true;
        TargetPosition = SpatialUtils.PlaneVector(target);
        // SetRigHeight(target.y);
    }
    
    /// <summary>
    /// Set DeltaPosition from keyboard inputs.
    /// 
    /// DeltaPosition is used during the UpdateHorizontalPosition step in our
    /// Update function.
    /// </summary>
    /// 
    /// <param name="delta">Frame delta time</param>
    private void GetKeyboardInput(float delta)
    {
        Vector3 keyboardInput = new Vector3(0, 0, 0);
        
        // TODO Set Q and E for rotation as well
        
        if (Input.IsActionPressed("forward"))
            keyboardInput.z = -1;
        if (Input.IsActionPressed("left"))
            keyboardInput.x = -1;
        if (Input.IsActionPressed("back"))
            keyboardInput.z = 1;
        if (Input.IsActionPressed("right"))
            keyboardInput.x = 1;

        keyboardInput = keyboardInput.Normalized();

        if (keyboardInput.LengthSquared() > 0.1f)
        {
            DeltaPosition += keyboardInput * 5f;
        }
    }

    /// <summary>
    /// Set DeltaPosition based on the current mouse drag delta.
    ///
    /// DeltaPosition is used during the UpdateHorizontalPosition step in our
    /// Update function.
    /// </summary>
    /// 
    /// <param name="delta">Frame delta time</param>
    private void GetMousePanInput(float delta)
    {
        if (!IsPanning) return;

        EndDrag = GetViewport().GetMousePosition();
        
        Vector2 deltaDrag = StartDrag - EndDrag;
        Vector2 scaledDrag = deltaDrag / new Vector2(100f, 100f);
        DeltaPosition = SpatialUtils.PlaneVector(scaledDrag) * -1.0f;
    }

    /// <summary>
    /// Set HorizontalVelocity by getting the position delta between this frame
    /// and the prior frame.
    ///
    /// HorizontalVelocity is used during the UpdateHorizontalPosition step
    /// of our Update function.
    /// </summary>
    /// 
    /// <param name="delta">Frame delta time</param>
    private void UpdateHorizontalVelocity(float delta)
    {
        HorizontalVelocity = (GlobalTransform.origin - LastPosition) / delta;
        // HorizontalVelocity.y = 0;
        LastPosition = GlobalTransform.origin;
    }
    
    private void UpdateHorizontalPosition(float delta)
    {
        if (DeltaPosition.LengthSquared() > 0.1f)
        {
            Speed = Mathf.Lerp(Speed, MaxSpeed, delta * Acceleration);
            Translate(DeltaPosition * Speed * delta);
        }
        
        else
        {
            HorizontalVelocity = HorizontalVelocity.LinearInterpolate(
                Vector3.Zero, delta * Damping
            );
            
            Transform transform = this.Transform;
            transform.origin += HorizontalVelocity * delta;
            this.Transform = transform;
        }

        DeltaPosition = Vector3.Zero;
    }

    private void UpdateCameraRotation(float delta)
    {
        Quat start = Transform.basis.Quat().Normalized();
        Quat end = TargetRotation.Normalized();
        Quat result = start.Slerp(end, delta * RotationDamping);
        
        Transform transform = Transform;
        transform.basis = new Basis(result);
        Transform = transform;
    }

    private void UpdateCameraHeight(float delta)
    {
        float start = Camera.Transform.origin.y;
        float yPos = MathUtils.Lerp(start, TargetZoomHeight, delta * ZoomSpeed * ZoomDamping);
        
        Vector3 end = new Vector3(
            Camera.Transform.origin.x,
            yPos,
            Camera.Transform.origin.z
        );
        
        Transform transform = Camera.Transform;
        transform.origin = end;
        Camera.Transform = transform;
        
        Camera.LookAt(Target.GlobalTransform.origin, Vector3.Up);
    }

    private void UpdateRigHeight(float delta)
    {
        float start = Transform.origin.y;
        float yPos = MathUtils.Lerp(start, TargetRigHeight, delta * HeightSpeed * HeightDamping);

        Vector3 end = new Vector3(
            Transform.origin.x,
            yPos,
            Transform.origin.z
        );

        Transform transform = Transform;
        transform.origin = end;
        Transform = transform;
    }
    
    private void SetCameraHeight(float value)
    {
        TargetZoomHeight = Camera.Transform.origin.y + value * StepSize;
        if (TargetZoomHeight < MinHeight) TargetZoomHeight = MinHeight;
        if (TargetZoomHeight > MaxHeight) TargetZoomHeight = MaxHeight;
    }
    
    private void SetRigHeight(float value)
    {
        TargetRigHeight = Transform.origin.y + value * StepSize;
    }
}

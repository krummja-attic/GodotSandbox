using Godot;

public class Root : Node2D
{
    private ColorRect colorRect;
    
    public override void _Ready()
    {
        NodePath path = "CanvasLayer/ColorRect";
        colorRect = GetNode<ColorRect>(path);
    }

    public override void _Process(float delta)
    {
        Vector2 mousePosition = GetViewport().GetMousePosition();
        mousePosition.x /= GetViewportRect().Size.x;
        mousePosition.y /= GetViewportRect().Size.y;
        colorRect.Material.Set("shader_param/target", mousePosition);
    }
}

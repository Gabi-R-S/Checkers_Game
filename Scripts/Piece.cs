using Godot;
using System;

public partial class Piece : Area3D
{
    public uint ID { get; set; }
    public Vector2I BoardPosition { get; set; }
    public Player OwnerPlayer { get; set; }

    bool hovered;

    public event Action<uint> Hover;
    public event Action<uint> Unhover;
    public event Action<uint> Clicked;


    public override void _Ready()
    {
        var entered = () => OnMouseEntered();

        var exited = () => OnMouseExited();

        InputEventEventHandler clicked = (a, @event, c, d, e) =>
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && hovered)
            {
                OnChildClicked();
            }
        };

        MouseEntered += entered;
        MouseExited += exited;
        InputEvent += clicked;
        TreeExited += () =>
            {
                MouseEntered -= entered;
                MouseExited -= exited;
                InputEvent -= clicked;
            };

    }

    void OnMouseEntered()
    {
        if (!hovered)
        {
            hovered = true;
            Hover?.Invoke(ID);
        }
    }
    void OnMouseExited()
    {
        if (hovered)
        {
            hovered = false;
            Unhover?.Invoke(ID);
        }
    }
    void OnChildClicked()
    {
        Clicked?.Invoke(ID);
    }
}

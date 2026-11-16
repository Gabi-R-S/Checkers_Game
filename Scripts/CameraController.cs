using Godot;
using System;

public partial class CameraController : Camera3D
{
    [Export]
    BoardController controller;

    [Export]
    bool invertHorizontalRotation = false;
    [Export]
    float minRadius = 1.0f;
    [Export]
    float radiusScaleFactor = 1.1f;

    [Export]
    float movementSpeed = 1.1f;

    Vector3 target;
    float maxRadius;
    float currentRadius;
    void RepositionCamera()
    {
        target = controller.Centre;


        currentRadius = controller.BoardRadius;
        maxRadius = 1.5f * currentRadius;


        Position = new Vector3(Mathf.Cos(currentRadius), 2, Mathf.Sin(currentRadius)).Normalized() * currentRadius;

        LookAt(target);
    }


    public override void _Ready()
    {
        RepositionCamera();
        controller.BoardChanged += RepositionCamera;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            bool isMouseDown = Input.IsMouseButtonPressed(MouseButton.Right);

            if (isMouseDown)
            {
                var relativeMovement = mouseMotion.Relative;
                var difference = Position - target;

                difference = difference.Rotated(invertHorizontalRotation ? Vector3.Up : Vector3.Down, relativeMovement.X / 1000f * movementSpeed);
                difference = difference.Rotated(difference.Cross(Vector3.Up).Normalized(), relativeMovement.Y / 1000f * movementSpeed);

                if (difference.X == 0 & difference.Y == 0)
                {
                    difference.X = 0.0001f;
                }

                Position = target + difference.Normalized() * currentRadius;
                LookAt(target);
            }

        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp || mouseButton.ButtonIndex == MouseButton.WheelDown)
            {

                currentRadius = mouseButton.ButtonIndex == MouseButton.WheelUp ? Mathf.Clamp(currentRadius / radiusScaleFactor, minRadius, maxRadius) : currentRadius = Mathf.Clamp(currentRadius * radiusScaleFactor, minRadius, maxRadius);

                var difference = Position - target;

                Position = target + difference.Normalized() * currentRadius;
                LookAt(target);
            }
            if (mouseButton.ButtonIndex == MouseButton.WheelLeft || mouseButton.ButtonIndex == MouseButton.WheelRight)
            {
                var relativeMovement = mouseButton.ButtonIndex == MouseButton.WheelLeft? 1 : -1;
                var difference = Position - target;

                difference = difference.Rotated(invertHorizontalRotation ? Vector3.Up : Vector3.Down, relativeMovement / 1000f * movementSpeed);

                Position = target + difference.Normalized() * currentRadius;
                LookAt(target);                
            }

        }
    }




}

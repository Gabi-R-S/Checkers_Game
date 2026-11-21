using Godot;
using System;

public partial class CameraController : Camera3D
{
    const float MAX_HEIGHT = 10000f;
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
    [Export]
    float heightVariation = 10.0f;
    float height = 1.0f;
    Vector3 Target
    {
        get => target + new Vector3(0, height, 0);
    }
    float maxRadius;
    float currentRadius;
    void RepositionCamera()
    {
        target = controller.Centre;


        currentRadius = controller.BoardRadius*1.5f;
        maxRadius = 2.5f * currentRadius;


        Position = new Vector3(0, currentRadius*0.8f, currentRadius).Normalized() * currentRadius;

        LookAt(Target);
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
                var difference = Position - Target;

                difference = difference.Rotated(invertHorizontalRotation ? Vector3.Up : Vector3.Down, relativeMovement.X / 1000f * movementSpeed);
                difference = difference.Rotated(difference.Cross(Vector3.Up).Normalized(), relativeMovement.Y / 1000f * movementSpeed);

                if (difference.X == 0 & difference.Y == 0)
                {
                    difference.X = 0.0001f;
                }

                Position = Target + difference.Normalized() * currentRadius;
                LookAt(Target);
            }

        }
        else if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp || mouseButton.ButtonIndex == MouseButton.WheelDown)
            {

                currentRadius = mouseButton.ButtonIndex == MouseButton.WheelUp ? Mathf.Clamp(currentRadius / radiusScaleFactor, minRadius, maxRadius) : currentRadius = Mathf.Clamp(currentRadius * radiusScaleFactor, minRadius, maxRadius);

                var difference = Position - Target;

                Position = Target + difference.Normalized() * currentRadius;
                LookAt(Target);
            }
            if (mouseButton.ButtonIndex == MouseButton.WheelLeft || mouseButton.ButtonIndex == MouseButton.WheelRight)
            {
                var relativeMovement = mouseButton.ButtonIndex == MouseButton.WheelLeft ? 1 : -1;
                var difference = Position - Target;

                difference = difference.Rotated(invertHorizontalRotation ? Vector3.Up : Vector3.Down, relativeMovement / 1000f * movementSpeed);

                Position = Target + difference.Normalized() * currentRadius;
                LookAt(Target);
            }

        }
    }


    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Z))
        {
            height -= heightVariation * (float)delta;
            if (height < 0)
            {
                height = 0;
                var difference = Position - Target;

                Position = Target + difference.Normalized() * currentRadius;
                LookAt(Target);
            }
            else
            {
                Position -= new Vector3(0, heightVariation * (float)delta, 0);
            }
        }
        if (Input.IsKeyPressed(Key.X))
        {
            height += heightVariation * (float)delta;
            if (height > MAX_HEIGHT)
            {
                height = MAX_HEIGHT;
                var difference = Position - Target;

                Position = Target + difference.Normalized() * currentRadius;
                LookAt(Target);
            }
            else
            {
                Position += new Vector3(0, heightVariation * (float)delta, 0);
            }


        }

    }



}

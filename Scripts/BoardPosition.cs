using Godot;
using System;
using System.Collections.Generic;
using static Godot.CollisionObject3D;
public partial class BoardPosition : Node3D, IMaterialChanger
{
    public void SetMaterial(MaterialChangerMaterial material)
    {
        foreach (var step in steps)
        {
            var materialChanger = step.FindChild("Mesh") as MeshMaterialChanger;
            if (materialChanger != null)
            {
                materialChanger.SetMaterial(material);
            }
        }
    }

    const uint MAX_HEIGHT_STEPS = 10000;
    [Export]
    public uint X;
    [Export]
    public uint Y;


    [Export]
    PackedScene heightStepPrefab;
    [Export]
    float heightPerStep;
    [Export]
    float pieceAppearDuration = 0.3f;
    uint heightSteps = 0;
    Stack<Area3D> steps = new();

    public event Action<uint, uint> Hover;
    public event Action<uint, uint> Unhover;
    public event Action<uint, uint> Clicked;

    HashSet<Node3D> currentHovered = new HashSet<Node3D>();

    Action increaseCallback;
    public void IncreaseHeightSteps(Action callback)
    {
        if (heightSteps < MAX_HEIGHT_STEPS)
        {
            increaseCallback = callback;
            HeightSteps++;
        }
    }
    public void DecreaseHeightSteps()
    {
        if (heightSteps > 0)
        {
            HeightSteps--;
        }
    }

    public void Appear(Node3D target, float duration = 0.5f, Action onCompleted = null)
    {
        target.Scale = new Vector3(0.001f,0.001f,0.001f);

        Tween tween = target.CreateTween();

        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.Out);

        tween.TweenProperty(target, "scale", Vector3.One, duration);

        if (onCompleted != null)
        { 
            tween.TweenCallback(Callable.From(onCompleted)); 
        }
    }

    public uint HeightSteps
    {
        get => heightSteps;
        set
        {
            if (value < heightSteps)
            {
                while (heightSteps > value)
                {
                    var child = steps.Pop();
                    currentHovered.Remove(child);
                    RemoveChild(child);
                    child.QueueFree();
                    heightSteps--;
                }
            }
            else if (value > heightSteps)
            {
                while (heightSteps < value)
                {
                    var child = heightStepPrefab.Instantiate<Area3D>();
                    child.Position = new Vector3(0, heightPerStep * heightSteps, 0);

                    AddChild(child);
                    steps.Push(child);

                    var entered = () => OnMouseEntered(child);
                    var exited = () => OnMouseExited(child);
                    InputEventEventHandler clicked = (a, @event, c, d, e) =>
                    {
                        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && currentHovered.Count > 0)
                        {
                            OnChildClicked();
                        }
                    };

                    child.MouseEntered += entered;
                    child.MouseExited += exited;
                    child.InputEvent += clicked;
                    child.TreeExited += () =>
                        {
                            child.MouseEntered -= entered;
                            child.MouseExited -= exited;
                            child.InputEvent -= clicked;
                        };

                    heightSteps++;
                    Appear(child, pieceAppearDuration, increaseCallback);
                    increaseCallback = null;
                }
            }
        }
    }

    void OnMouseEntered(Node3D node)
    {
        if (currentHovered.Add(node))
        {
            Hover?.Invoke(X, Y);
        }
    }
    void OnMouseExited(Node3D node)
    {
        if (currentHovered.Remove(node))
        {
            Unhover?.Invoke(X, Y);
        }
    }
    void OnChildClicked()
    {
        Clicked?.Invoke(X, Y);
    }

}

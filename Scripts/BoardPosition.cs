using Godot;
using System;
using System.Collections.Generic;
using static Godot.CollisionObject3D;
public partial class BoardPosition : Node3D
{
    const uint MAX_HEIGHT_STEPS = 10000;
    [Export]
    uint x;
    [Export]
    uint y;

    [Export]
    PackedScene heightStepPrefab;
    [Export]
    float heightPerStep;

    uint heightSteps = 0;
    Stack<Area3D> steps = new ();

    public event Action<uint,uint> OnHover;
    public event Action<uint,uint> OnUnhover;
    public event Action<uint,uint> OnClicked;

    HashSet<Node3D> currentHovered = new HashSet<Node3D>();

    public void IncreaseHeightSteps()
    {
        if(heightSteps < MAX_HEIGHT_STEPS)
        {
            HeightSteps++;
        }
    }
    public void DecreaseHeightSteps()
    {
        if(heightSteps > 0)
        {
            HeightSteps--;
        }
    }
    uint HeightSteps
    {
        get => heightSteps;
        set
        {
            if(value < heightSteps)
            {
                while (heightSteps > value)
                {
                    var child = steps.Pop();
                    currentHovered.Remove(child);
                    RemoveChild(child);
                    heightSteps--; 
                }
            } else if(value > heightSteps)
            {
                while (heightSteps < value)
                {
                    var child = heightStepPrefab.Instantiate<Area3D>();
                    child.Position = new Vector3(0,heightPerStep * heightSteps,0);
                    
                    AddChild(child);
                    steps.Push(child);
                    
                    var entered = () => OnMouseEntered(child);
                    var exited = () => OnMouseExited(child); 
                    InputEventEventHandler clicked = (a,@event,c,d,e) => {
                        if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && currentHovered.Count > 0)
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
                }
            }
        }
    }


    public void SetHoverCallbacks(Action<uint,uint> onHover, Action<uint,uint> onUnhover)
    {
        this.OnHover = onHover;
        this.OnUnhover = onUnhover;
    }
    void OnMouseEntered(Node3D node)
    {
        if(currentHovered.Add(node))
        {
            OnHover?.Invoke(x,y);
        }
    }
    void OnMouseExited(Node3D node)
    {
        if(currentHovered.Remove(node))
        {
            OnUnhover?.Invoke(x,y);
        }
    }
    void OnChildClicked()
    {
        OnClicked?.Invoke(x,y);
    }

}

using Godot;
using System;
using System.Collections.Generic;
using static Godot.CollisionObject3D;
public partial class BoardPosition : Node3D, IMaterialChanger
{
   public void SetMaterial(MaterialChangerMaterial material)
    {
        foreach(var step in steps)
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

    uint heightSteps = 0;
    Stack<Area3D> steps = new ();

    public event Action<uint,uint> Hover;
    public event Action<uint,uint> Unhover;
    public event Action<uint,uint> Clicked;

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
    public uint HeightSteps
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

    void OnMouseEntered(Node3D node)
    {
        if(currentHovered.Add(node))
        {
            Hover?.Invoke(X,Y);
        }
    }
    void OnMouseExited(Node3D node)
    {
        if(currentHovered.Remove(node))
        {
            Unhover?.Invoke(X,Y);
        }
    }
    void OnChildClicked()
    {
        Clicked?.Invoke(X,Y);
    }

}

using Godot;
using System;

public partial class PositionMaterialChanger : MeshInstance3D
{
    [Export]
    Material evenMaterial;
    [Export]
    Material oddMaterial;
    
    Material normalMaterial;

    [Export]
    Material highlightedMaterial;

    void BecomeNormal(uint x=0, uint y= 0)
    {
        SetSurfaceOverrideMaterial(0,normalMaterial);
        
    }
    void BecomeHighlighted(uint x=0, uint y= 0)
    {
        SetSurfaceOverrideMaterial(0,highlightedMaterial);
        
    }
    public override void _Ready()
    {
        BoardPosition parent = GetParent().GetParent<BoardPosition>();
        
        normalMaterial = (parent.X + parent.Y) % 2 == 0? evenMaterial:oddMaterial; 
        BecomeNormal();

        parent.Hover+=BecomeHighlighted;
        parent.Unhover+=BecomeNormal;
    }

}

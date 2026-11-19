using Godot;
using System;

public partial class PositionMaterialChanger : MeshMaterialChanger
{


    [Export]
    Material evenMaterial;
    [Export]
    Material oddMaterial;
    


    public override void _Ready()
    {
        BoardPosition parent = GetParent().GetParent<BoardPosition>();
        
        NormalMaterial = (parent.X + parent.Y) % 2 == 0? evenMaterial:oddMaterial; 
        SetMaterial(MaterialChangerMaterial.NORMAL);
    }

}

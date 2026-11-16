using Godot;
using System;

public partial class PieceMaterialChanger : MeshInstance3D
{
    [Export]
    Material player1Material;
    [Export]
    Material player2Material;
    
    Material normalMaterial;

    [Export]
    Material highlightedMaterial;

    void BecomeNormal(uint x=0)
    {
        SetSurfaceOverrideMaterial(0,normalMaterial);
        
    }
    void BecomeHighlighted(uint x=0)
    {
        SetSurfaceOverrideMaterial(0,highlightedMaterial);
        
    }
    public override void _Ready()
    {
        Piece parent = GetParent<Piece>();
        
        normalMaterial = parent.OwnerPlayer == Player.PLAYER1 ? player1Material: player2Material;

        BecomeNormal();

        parent.Hover+=BecomeHighlighted;
        parent.Unhover+=BecomeNormal;
    }


}

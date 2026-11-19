using Godot;
using System;

public partial class PieceMaterialChanger : MeshMaterialChanger
{

    [Export]
    Material player1Material;
    [Export]
    Material player2Material;
    
    public override void _Ready()
    {
        Piece parent = GetParent<Piece>();
        
        NormalMaterial = parent.OwnerPlayer == Player.PLAYER1 ? player1Material: player2Material;

        SetMaterial(MaterialChangerMaterial.NORMAL);
        
    }


}

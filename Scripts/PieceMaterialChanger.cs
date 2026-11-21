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
        if (parent.OwnerPlayer == Player.PLAYER1)
        {
            NormalMaterial = player1Material;
        }
        else
        {
            NormalMaterial = player2Material;
         }
        SetMaterial(MaterialChangerMaterial.NORMAL);

    }


}

using Godot;
using System;

public partial class Piece : Area3D
{
    public uint ID {get; set;}
    public Vector2I BoardPosition {get; set;}
    public Player OwnerPlayer {get; set;}
}

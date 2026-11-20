

using Godot;

class Move:TurnCommand
{
int PieceID {get; set;}
Vector2I Origin {get; set;}
Vector2I Target {get; set;}
}
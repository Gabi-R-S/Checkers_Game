

using Godot;

class Kill : TurnCommand
{
int PieceID {get; set;}
int TargetID {get; set;}
Vector2I Origin {get; set;}
Vector2I Target {get; set;}
}
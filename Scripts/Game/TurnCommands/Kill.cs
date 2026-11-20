

using Godot;

class Kill : TurnCommand
{
    public uint PieceID {get; private set;}
    public uint TargetID {get; private set;}
    public Kill(uint piece, uint target)
    {
        PieceID=piece;
        TargetID=target;
    }
}
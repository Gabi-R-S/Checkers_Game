

using Godot;

class Move : TurnCommand
{
    public uint PieceID { get; private set; }
    public uint X { get; private set; }
    public uint Y { get; private set; }
    public Move(uint pieceID, uint x, uint y)
    {
        PieceID = pieceID;
        X = x;
        Y = y;
    }
}
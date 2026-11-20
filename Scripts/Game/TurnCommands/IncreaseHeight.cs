

using Godot;

class IncreaseHeight : TurnCommand
{
    public uint X { get; private set; }
    public uint Y { get; private set; }

    public IncreaseHeight(uint x, uint y)
    {
        X = x;

        Y = y;

    }
}
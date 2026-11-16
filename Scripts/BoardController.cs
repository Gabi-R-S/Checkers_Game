using Godot;
using System;

public partial class BoardController : Node3D
{

    [Export]
    PackedScene boardPositionPrefab;


    [Export]
    public Vector2I Size { get; private set; }

    [Export]
    float positionSpacing = 1.0f;

    public event Action<uint,uint> PositionHover;
    public event Action<uint,uint> PositionUnhover;
    public event Action<uint,uint> PositionClicked;

    public event Action BoardChanged; 


    public Vector3 Centre { 
        get=>new Vector3(0,0,0);
        }
    
    public float BoardRadius {
            get=> Math.Max(Size.X,Size.Y) * (float) Math.Sqrt(2);
        }

    void OnPositionHover(uint x, uint y)
    {
        PositionHover?.Invoke(x,y);
    }
    void OnPositionUnhover(uint x, uint y)
    {
        PositionUnhover?.Invoke(x,y);
    }
    void OnPositionClicked(uint x, uint y)
    {
        PositionClicked?.Invoke(x,y);
    }
    Vector3 GetPosition(int x, int y)
    {
        return new Vector3((x - (Size.X - 1 ) / 2.0f) * positionSpacing, 0, (y - ( Size.Y - 1) / 2.0f) * positionSpacing);
    }

    public override void _Ready()
    {
        for (int x = 0; x < Size.X; x++)
        {
            for (int y = 0; y < Size.Y; y++)
            {
                var position = boardPositionPrefab.Instantiate<BoardPosition>();
                position.Position = GetPosition(x, y);
                position.X = (uint)x;
                position.Y = (uint)y;
                position.HeightSteps = 1;

                position.Hover +=  OnPositionHover;
                position.Unhover += OnPositionUnhover;
                position.Clicked +=  OnPositionClicked;

                AddChild(position);
            }
        }
        BoardChanged?.Invoke();
    }
}

using Godot;
using System;
using System.Collections.Generic;

public partial class BoardController : Node3D
{

    [Export]
    PackedScene boardPositionPrefab;


    [Export]
    PackedScene piecePrefab;


    [Export]
    public Vector2I Size { get; private set; }

    [Export]
    float positionSpacing = 1.0f;
    [Export]
    float pieceHeight = 1.0f;
    [Export]
    uint numberOfRowsWithPieces = 0;

    public event Action<uint, uint> PositionHover;
    public event Action<uint, uint> PositionUnhover;
    public event Action<uint, uint> PositionClicked;

    public event Action BoardChanged;


    BoardPosition[,] positions;


    public Vector3 Centre
    {
        get => new Vector3(0, 0, 0);
    }

    public float BoardRadius
    {
        get => Math.Max(Size.X, Size.Y) * (float)Math.Sqrt(2);
    }

    void OnPositionHover(uint x, uint y)
    {
        PositionHover?.Invoke(x, y);
    }
    void OnPositionUnhover(uint x, uint y)
    {
        PositionUnhover?.Invoke(x, y);
    }
    void OnPositionClicked(uint x, uint y)
    {
        PositionClicked?.Invoke(x, y);
    }
    Vector3 GetPosition(int x, int y)
    {
        return new Vector3((x - (Size.X - 1) / 2.0f) * positionSpacing,
            0,
            (y - (Size.Y - 1) / 2.0f) * positionSpacing);
    }

    Vector3 GetPiecePosition(int x, int y)
    {
        int height = x > 0 && y > 0 && x < Size.X && y < Size.Y ? (int)positions[x, y].HeightSteps : 1;
        return new Vector3((x - (Size.X - 1) / 2.0f) * positionSpacing,
            (height - 0.5f) * pieceHeight,
            (y - (Size.Y - 1) / 2.0f) * positionSpacing);
    }

    void CreateBoard()
    {
        positions = new BoardPosition[Size.X, Size.Y];
        for (int x = 0; x < Size.X; x++)
        {
            for (int y = 0; y < Size.Y; y++)
            {
                var position = boardPositionPrefab.Instantiate<BoardPosition>();
                position.Position = GetPosition(x, y);
                position.X = (uint)x;
                position.Y = (uint)y;
                position.HeightSteps = 1;

                position.Hover += OnPositionHover;
                position.Unhover += OnPositionUnhover;
                position.Clicked += OnPositionClicked;

                AddChild(position);
                positions[x, y] = position;
            }
        }
        BoardChanged?.Invoke();

    }


    void PlacePieces()
    {
        for (int i = 0; i < numberOfRowsWithPieces && i < Size.Y / 2; i++)
        {
            for (int x = i % 2; x < Size.X; x += 2)
            {
                var piece1 = piecePrefab.Instantiate<Area3D>();
                var piece2 = piecePrefab.Instantiate<Area3D>();

                AddChild(piece1);
                AddChild(piece2);
                piece1.Position = GetPiecePosition(x, i);
                piece2.Position = GetPiecePosition(x, Size.Y - 1 - i);

            }
        }
    }

    public override void _Ready()
    {
        CreateBoard();
        PlacePieces();
    }



}

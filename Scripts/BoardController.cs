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
    float positionSpacing = 1.0f;
    [Export]
    float pieceHeight = 1.0f;


    Board board;
    public Board Board{ get => board; set
        {
            foreach(var child in GetChildren()){
                RemoveChild(child);   
            }
            
            board = value;

            CreateBoard();
            PlacePieces();
            
            BoardChanged();
        }
    }

    public event Action<uint, uint> PositionHover;
    public event Action<uint, uint> PositionUnhover;
    public event Action<uint, uint> PositionClicked;
    public event Action<uint> PieceHover;
    public event Action<uint> PieceUnhover;
    public event Action<uint> PieceClicked;
    public event Action BoardChanged;
    BoardPosition[,] positions;
    Dictionary<uint, Piece> pieces;

    public Piece GetPieceNode(uint id)
    {
        if (!pieces.ContainsKey(id))
        {
            return null;
        }
        return pieces[id];
    } 

    public Vector3 Centre
    {
        get => new Vector3(0, 0, 0);
    }

    public float BoardRadius
    {
        get => board == null ? 1 : Math.Max(board.Size.X, board.Size.Y) * (float)Math.Sqrt(2);
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
        return new Vector3((x - (board.Size.X - 1) / 2.0f) * positionSpacing,
            0,
            (y - (board.Size.Y - 1) / 2.0f) * positionSpacing);
    }

    Vector3 GetPiecePosition(int x, int y)
    {
        int height = x > 0 && y > 0 && x < board.Size.X && y < board.Size.Y ? (int)positions[x, y].HeightSteps : 1;
        return new Vector3((x - (board.Size.X - 1) / 2.0f) * positionSpacing,
            (height - 0.5f) * pieceHeight,
            (y - (board.Size.Y - 1) / 2.0f) * positionSpacing);
    }

    void CreateBoard()
    {
        positions = new BoardPosition[board.Size.X, board.Size.Y];
        for (int x = 0; x < board.Size.X; x++)
        {
            for (int y = 0; y < board.Size.Y; y++)
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
        pieces= new ();
        foreach (var (pID,(x,y,p)) in board.Pieces)
        {
            var piece = piecePrefab.Instantiate<Piece>();

            piece.ID = pID;
            piece.Board = board;

            piece.Position= GetPiecePosition((int)x,(int) y);
            AddChild(piece);
            pieces.Add(pID,piece);             
        }
    }




}

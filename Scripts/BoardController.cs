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
    float Positionspacing = 1.0f;
    [Export]
    float pieceHeight = 1.0f;


    Board board;
    public Board Board
    {
        get => board; set
        {
            foreach (var child in GetChildren())
            {
                RemoveChild(child);
            }

            board = value;

            if (board != null)
            {
                CreateBoard();
                PlacePieces();
            }

            BoardChanged?.Invoke();
        }
    }

    public event Action<uint, uint> PositionHover;
    public event Action<uint, uint> PositionUnhover;
    public event Action<uint, uint> PositionClicked;
    public event Action<uint> PieceHover;
    public event Action<uint> PieceUnhover;
    public event Action<uint> PieceClicked;
    public event Action BoardChanged;
    public BoardPosition[,] Positions { get; private set; }
    public Dictionary<uint, Piece> Pieces { get; private set; }

    public Piece GetPieceNode(uint id)
    {
        if (!Pieces.ContainsKey(id))
        {
            return null;
        }
        return Pieces[id];
    }

    public BoardPosition GetBoardPositionNode(uint x, uint y)
    {
        if (x >= board.Size.X || y >= board.Size.Y)
        {
            return null;
        }

        return Positions[x, y];
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


    void OnPieceHover(uint id)
    {
        PieceHover?.Invoke(id);
    }
    void OnPieceUnhover(uint id)
    {
        PieceUnhover?.Invoke(id);
    }
    void OnPieceClicked(uint id)
    {
        PieceClicked?.Invoke(id);
    }


    Vector3 GetPosition(int x, int y)
    {
        return new Vector3((x - (board.Size.X - 1) / 2.0f) * Positionspacing,
            0,
            (y - (board.Size.Y - 1) / 2.0f) * Positionspacing);
    }

    Vector3 GetPiecePosition(uint x, uint y)
    {
        int height = x > 0 && y > 0 && x < board.Size.X && y < board.Size.Y ? (int)Positions[x, y].HeightSteps : 1;
        return new Vector3((x - (board.Size.X - 1) / 2.0f) * Positionspacing,
            (height - 0.5f) * pieceHeight,
            (y - (board.Size.Y - 1) / 2.0f) * Positionspacing);
    }

    void CreateBoard()
    {
        Positions = new BoardPosition[board.Size.X, board.Size.Y];
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
                Positions[x, y] = position;
            }
        }
        BoardChanged?.Invoke();

    }


    void PlacePieces()
    {
        Pieces = new();
        foreach (var (pID, (x, y, p)) in board.Pieces)
        {
            var piece = piecePrefab.Instantiate<Piece>();

            piece.ID = pID;
            piece.Board = board;

            piece.Hover += OnPieceHover;
            piece.Unhover += OnPieceUnhover;
            piece.Clicked += OnPieceClicked;

            piece.Position = GetPiecePosition(x, y);
            AddChild(piece);
            Pieces.Add(pID, piece);
        }
    }


    public void Perform(TurnCommand command, Action callback)
    {
        if (command is IncreaseHeight heightCommand)
        {
            board.Apply(command);

            GetBoardPositionNode(heightCommand.X, heightCommand.Y)?.IncreaseHeightSteps();
            callback?.Invoke();
        }
        else if (command is Move move)
        {
            board.Apply(command);

            var node = GetPieceNode(move.PieceID);
            if (node != null)
            {
                node.Position = GetPiecePosition(move.X, move.Y);
            }

            callback?.Invoke();
        }
        else if (command is Kill kill)
        {
            var node = GetPieceNode(kill.TargetID);
            var target = node.BoardPosition;
            board.Apply(command);


            if (node != null)
            {
                RemoveChild(node);
            }


            node = GetPieceNode(kill.PieceID);

            if (node != null)
            {
                node.Position = GetPiecePosition((uint)target.X, (uint)target.Y);
            }

            callback.Invoke();
        }

    }

}

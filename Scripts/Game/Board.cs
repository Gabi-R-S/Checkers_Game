using System;
using System.Collections.Generic;
using Godot;

public class Board
{
    public Board(uint w, uint h)
    {
        this.w = w;
        this.h = h;
        heights = new uint[w, h];

        for (int x = 0; x < w; x++)
        {

            for (int y = 0; y < h; y++)
            {
                heights[x, y] = 1;
            }

        }

    }
    uint w, h;
    uint[,] heights;
    public uint GetHeightAt(uint x, uint y)
    {
        if (x >= w || y >= h)
        {
            return 0;
        }
        return heights[x, y];
    }
    public void IncreaseHeightAt(uint x, uint y)
    {
        if (x >= w || y >= h)
        {
            return;
        }

        heights[x, y]++;
    }



    public bool IsThereAPieceInPosition(uint x, uint y)
    {

        if (x >= w || y >= h)
        {
            return false;
        }

        foreach (var (pID, (px, py, p)) in Pieces)
        {
            if (x == px && y == py)
            {
                return true;
            }
        }

        return false;
    }

    public Dictionary<uint, (uint, uint, Player)> Pieces { get; private set; } = new Dictionary<uint, (uint, uint, Player)>();

    public bool ContainsPieceAt(uint x, uint y)
    {
        foreach (var (X, Y, p) in Pieces.Values)
        {
            if (x == X && y == Y)
            {
                return true;
            }
        }

        return false;
    }
    public bool IsPieceOfType(uint id, Player type)
    {
        if (!Pieces.ContainsKey(id))
        {
            return false;
        }
        return Pieces[id].Item3 == type;
    }

    public void MovePiece(uint id, uint x, uint y)
    {
        Pieces[id] = (x, y, Pieces[id].Item3);
    }
    public void KillPiece(uint id)
    {
        Pieces.Remove(id);
    }
    public Vector2I Size { get => new Vector2I((int)w, (int)h); }



    bool IsInsideBoard(uint x, uint y)
    {
        return x < w && y < h;
    }

    public bool CanMoveTo(uint piece, uint x, uint y)
    {
        if (!Pieces.ContainsKey(piece))
        {
            return false;
        }

        (var px, var py, var p) = Pieces[piece];
        return IsInsideBoard(x, y) && (Math.Abs(x - px) + Math.Abs(y - py) == 1) && !IsThereAPieceInPosition(x, y) && heights[px, py] - heights[x, y] == 1;
    }

    public bool CanKill(uint piece, uint target)
    {
        if (!Pieces.ContainsKey(piece) || !Pieces.ContainsKey(target))
        {
            return false;
        }

        (var px, var py, var pp) = Pieces[piece];
        (var tx, var ty, var tp) = Pieces[piece];
        return pp != tp && (Math.Abs(tx - px) + Math.Abs(ty - py) == 1) && heights[px, py] - heights[tx, ty] == 1;
    }

    public List<TurnCommand> GetAllValidCommands(Player player)
    {
        List<TurnCommand> commands = new List<TurnCommand>();

        for (uint x = 0; x < w; x++)
        {

            for (uint y = 0; y < h; y++)
            {
                if (IsThereAPieceInPosition(x, y))
                {
                    commands.Add(new IncreaseHeight(x, y));
                }
            }

        }

        foreach (var (p, data) in Pieces)
        {

            if (data.Item3 == player)
            {
                if (CanMoveTo(p, data.Item1 - 1, data.Item2))
                {
                    commands.Add(new Move(p, data.Item1 - 1, data.Item2));
                }

                if (CanMoveTo(p, data.Item1 + 1, data.Item2))
                {
                    commands.Add(new Move(p, data.Item1 + 1, data.Item2));
                }

                if (CanMoveTo(p, data.Item1, data.Item2 + 1))
                {
                    commands.Add(new Move(p, data.Item1, data.Item2 + 1));
                }

                if (CanMoveTo(p, data.Item1, data.Item2 - 1))
                {
                    commands.Add(new Move(p, data.Item1, data.Item2 - 1));
                }

                foreach (var (p2, data2) in Pieces)
                {
                    if (CanKill(p, p2))
                    {
                        commands.Add(new Kill(p, p2));
                    }
                }
            }
        }
        return commands;
    }


    public Board Clone()
    {
        var newBoard = new Board(w, h);


        for (int x = 0; x < w; x++)
        {

            for (int y = 0; y < h; y++)
            {
                newBoard.heights[x, y] = heights[x, y];
            }

        }

        foreach (var (pID, (px, py, p)) in Pieces)
        {
            newBoard.Pieces.Add(pID, (px, py, p));
        }

        return newBoard;
    }
}
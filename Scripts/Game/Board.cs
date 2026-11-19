using System;
using System.Collections.Generic;
using Godot;

public class Board {
    public Board(uint w, uint h)
    {
        this.w = w;
        this.h = h;
        heights = new uint[w,h];

        for (int x = 0; x < w; x++)
        {
            
            for (int y = 0; y < h; y++)
            {
                heights[x,y] = 1;
            }

        }
        
    }
    uint w, h;
    uint [,] heights; 
    public uint GetHeightAt(uint x, uint y)
    {
        if (x >= w || y >= h)
        {
            return 0;
        }
        return heights[x,y];    
    }


    public bool IsThereAPieceInPosition(uint x, uint y)
    {
        
        if (x >= w || y >= h)
        {
            return false;
        }

        foreach (var (pID,(px, py, p)) in Pieces)
        {
            if (x == px && y == py)
            {
                return true;
            }
        }    

        return false;
    }

    public Dictionary<uint, (uint, uint, Player)> Pieces {get; private set;}= new Dictionary<uint, (uint, uint, Player)>();

    public bool ContainsPieceAt(uint x, uint y)
    {
        foreach(var (X,Y,p) in Pieces.Values)
        {
            if(x == X && y == Y)
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

    public Vector2I Size{get=> new Vector2I((int)w,(int)h);}
}
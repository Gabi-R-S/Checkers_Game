using Godot;
using System;

public partial class Master : Node3D
{
    [Export]
    public Vector2I Size { get; private set; }

    [Export]
    uint numberOfRowsWithPieces = 0;


    Board board;
    Board CreateBoard()
    {
        Board board = new Board((uint)Size.X,(uint)Size.Y);

        uint pID=0;
        for (uint i = 0; i < numberOfRowsWithPieces && i < board.Size.Y / 2; i++)
        {
            for (uint x = i % 2; x < board.Size.X; x += 2)
            {
                board.Pieces.Add(pID,(x, i, Player.PLAYER1));
                pID ++;
                board.Pieces.Add(pID,(x, (uint)(board.Size.Y - 1 - i), Player.PLAYER1));
                pID ++;     
            }
        }
        
        return board;

    }

    public override void _Ready()
    {
        board = CreateBoard();
        var controller = GetChildOrNull<BoardController>(0);
        if(controller != null)
        {
            controller.Board = board;
        }
    }

   
}

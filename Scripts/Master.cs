using Godot;
using System;

public partial class Master : Node3D
{
    [Export]
    public TurnControllerInitialiser Initialiser {get; set;}

    TurnController Player1{get;set;}
    TurnController Player2{get;set;}

    Player nextPlayer;


    [Export]
    public Vector2I Size { get; private set; }

    [Export]
    uint numberOfRowsWithPieces = 0;
    Board board;


    void NewTurn()
    {
        if(nextPlayer == Player.PLAYER1)
        {
            nextPlayer = Player.PLAYER2;
            Player1?.StartTurn(NewTurn);
        }
        else
        {
            nextPlayer = Player.PLAYER1;
            Player2?.StartTurn(NewTurn);
        }
    }


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
                
                board.Pieces.Add(pID,(x, (uint)(board.Size.Y - 1 - i), Player.PLAYER2));
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

        (Player1, Player2) = Initialiser.Initialise(this);

        NewTurn();
    }

   
}

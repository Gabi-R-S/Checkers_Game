using Godot;
using System;

public partial class PlayerTurnController : Node3D
{

    enum State
    {
        NONE,
        SELECTING_ACTION
    }


    [Export]
    BoardController boardController;

    Action currentTurnCallback = null;
    Player currentPlayer;

    [Export]
    int maxTurnEnergy = 0;
    int turnEnergy = 0;

    State state;


    public void StartTurn(Player player, Action callback = null)
    {
        if (state != State.NONE)
        {
            throw new Exception("tried to start turn with turn still in progress");
        }

        state = State.SELECTING_ACTION;
        currentPlayer = player;
        currentTurnCallback = callback;
        turnEnergy = maxTurnEnergy;


    }

    void OnPositionHover(uint x, uint y)
    {

    }


    void OnPositionUnhover(uint x, uint y)
    {

    }

    void OnPositionClicked(uint x, uint y)
    {

    }




    void OnPieceHover(uint id)
    {
        if (state == State.SELECTING_ACTION)
        {
            if (boardController.Board.IsPieceOfType(id, currentPlayer))
            {
                var node = boardController.GetPieceNode(id);

                if (node != null)
                {
                    MeshMaterialChanger changer = node.FindChild("Mesh") as MeshMaterialChanger;

                    if(changer != null)
                    {
                        changer.SetMaterial(MaterialChangerMaterial.HOVERED);
                    }
                }                
            }
        }
    }


    void OnPieceUnhover(uint id)
    {

        if (state == State.SELECTING_ACTION)
        {
            if (boardController.Board.IsPieceOfType(id, currentPlayer))
            {
                var node = boardController.GetPieceNode(id);

                if (node != null)
                {
                    MeshMaterialChanger changer = node.FindChild("Mesh") as MeshMaterialChanger;

                    if(changer != null)
                    {
                        changer.SetMaterial(MaterialChangerMaterial.NORMAL);
                    }
                }                
            }
        }
    }

    void OnPieceClicked(uint id)
    {

    }

    public override void _Ready()
    {
        if (boardController == null)
        {
            throw new Exception("ERROR: Board Controller Required");
        }

        boardController.PositionHover += OnPositionHover;
        boardController.PositionUnhover += OnPositionUnhover;
        boardController.PositionClicked += OnPositionClicked;

        boardController.PieceHover += OnPieceHover;
        boardController.PieceUnhover += OnPieceUnhover;
        boardController.PieceClicked += OnPieceClicked;
    }



}

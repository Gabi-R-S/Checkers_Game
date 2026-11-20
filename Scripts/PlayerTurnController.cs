using Godot;
using System;

public partial class PlayerTurnController : Node3D
{

    enum State
    {
        NONE,
        SELECTING_ACTION,
        PIECE_SELECTED
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
        if (state == State.SELECTING_ACTION)
        {
            if (!boardController.Board.ContainsPieceAt(x, y))
            {
                var node = boardController.GetBoardPositionNode(x, y) as IMaterialChanger;

                if (node != null)
                {
                    node.SetMaterial(MaterialChangerMaterial.HOVERED);
                }

            }
        }
    }


    void OnPositionUnhover(uint x, uint y)
    {

        if (state == State.SELECTING_ACTION)
        {

            var node = boardController.GetBoardPositionNode(x, y) as IMaterialChanger;

            if (node != null)
            {
                node.SetMaterial(MaterialChangerMaterial.NORMAL);
            }

        }
    }

    void OnPositionClicked(uint x, uint y)
    {
        if (state == State.SELECTING_ACTION)
        {
            if (!boardController.Board.ContainsPieceAt(x, y))
            {
                TurnCommand command = new IncreaseHeight(x, y);
                boardController.Perform(command, OnCommandFinished);
            }
        }
    }

    void OnCommandFinished()
    {
        turnEnergy--;
        if (turnEnergy == 0)
        {
            state = State.NONE;
            currentTurnCallback?.Invoke();
        }

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
                    IMaterialChanger changer = node.FindChild("Mesh") as IMaterialChanger;

                    if (changer != null)
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
                    IMaterialChanger changer = node.FindChild("Mesh") as IMaterialChanger;

                    if (changer != null)
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

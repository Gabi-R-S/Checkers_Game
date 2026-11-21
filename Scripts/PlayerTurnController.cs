using Godot;
using System;

public partial class PlayerTurnController : Node3D
{
    public event Action TurnStart;
    public event Action TurnEnd;
    public event Action<int> EnergyChanged;

    enum State
    {
        NONE,
        SELECTING_ACTION,
        PIECE_SELECTED,
        ANIMATING
    }


    [Export]
    BoardController boardController;

    Action currentTurnCallback = null;
    Player currentPlayer;

    int turnEnergy = 0;

    State state = State.NONE;

    Piece selectedPiece;

    public void StartTurn(Player player, Action callback = null)
    {
        if (state != State.NONE)
        {
            throw new Exception("tried to start turn with turn still in progress");
        }

        TurnStart?.Invoke();

        state = State.SELECTING_ACTION;
        currentPlayer = player;
        currentTurnCallback = callback;
        turnEnergy = Globals.MAX_TURN_ENERGY;

        // Reset any previous piece selection
        selectedPiece = null;


    }

    void OnPositionHover(uint x, uint y)
    {
        if (state == State.SELECTING_ACTION)
        {
            if (!boardController.Board.IsThereAPieceInPosition(x, y))
            {
                var node = boardController.GetBoardPositionNode(x, y) as IMaterialChanger;

                if (node != null)
                {
                    node.SetMaterial(MaterialChangerMaterial.HOVERED);
                }

            }
        }
        else if (state == State.PIECE_SELECTED)
        {

            if (CanMoveTo(x, y))
            {
                var node = boardController.GetBoardPositionNode(x, y);


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

            node?.SetMaterial(MaterialChangerMaterial.NORMAL);

        }
        else if (state == State.PIECE_SELECTED)
        {

            if (CanMoveTo(x, y))
            {
                var node = boardController.GetBoardPositionNode(x, y);

                node?.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
            }


        }

    }

    void OnPositionClicked(uint x, uint y)
    {
        if (state == State.SELECTING_ACTION)
        {
            if (!boardController.Board.IsThereAPieceInPosition(x, y))
            {
                TurnCommand command = new IncreaseHeight(x, y);
                IssueCommand(command);
            }
        }
        else if (state == State.PIECE_SELECTED)
        {
            if (CanMoveTo(x, y))
            {
                Move command = new Move(selectedPiece.ID, x, y);
                IssueCommand(command);
            }
        }
    }

    void ReturnMaterialsToNormal()
    {
        foreach (var piece in boardController.Pieces.Values)
        {
            IMaterialChanger changer = piece.FindChild("Mesh") as IMaterialChanger;

            if (changer != null)
            {
                changer.SetMaterial(MaterialChangerMaterial.NORMAL);
            }

        }
        foreach (var position in boardController.Positions)
        {
            position.SetMaterial(MaterialChangerMaterial.NORMAL);
        }

    }
    void IssueCommand(TurnCommand command)
    {
        ReturnMaterialsToNormal();
        state = State.ANIMATING;
        boardController.Perform(command, OnCommandFinished);
    }


    public void EndTurn()
    {
        if (state != State.NONE)
        {
            TurnEnd?.Invoke();
            state = State.NONE;
            currentTurnCallback?.Invoke();
        }
    }

    void OnCommandFinished()
    {
        turnEnergy--;
        EnergyChanged?.Invoke(turnEnergy);
        if (turnEnergy == 0)
        {
            EndTurn();
        }
        else
        {
            state = State.SELECTING_ACTION;
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
        else if (state == State.PIECE_SELECTED)
        {

            var node = boardController.GetPieceNode(id);


            if (node != null)
            {
                if (CanKill(node))
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
        else if (state == State.PIECE_SELECTED)
        {
            var node = boardController.GetPieceNode(id);


            if (node != null)
            {
                if (CanKill(node))
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

    bool CanKill(Piece piece)
    {
        return boardController.Board.CanKill(selectedPiece.ID, piece.ID);
    }

    bool CanMoveTo(uint x, uint y)
    {
        return boardController.Board.CanMoveTo(selectedPiece.ID, x, y);
    }
    void HighlightSurroundingPositions()
    {
        var pos = selectedPiece.BoardPosition;

        if (CanMoveTo((uint)(pos.X - 1), (uint)(pos.Y)))
        {
            boardController.GetBoardPositionNode((uint)(pos.X - 1), (uint)(pos.Y))?.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
        }

        if (CanMoveTo((uint)(pos.X + 1), (uint)(pos.Y)))
        {
            boardController.GetBoardPositionNode((uint)(pos.X + 1), (uint)(pos.Y))?.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
        }

        if (CanMoveTo((uint)(pos.X), (uint)(pos.Y + 1)))
        {
            boardController.GetBoardPositionNode((uint)(pos.X), (uint)(pos.Y + 1))?.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
        }

        if (CanMoveTo((uint)(pos.X), (uint)(pos.Y - 1)))
        {
            boardController.GetBoardPositionNode((uint)(pos.X), (uint)(pos.Y - 1))?.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
        }

        foreach (var piece in boardController.Pieces.Values)
        {
            if (CanKill(piece))
            {
                IMaterialChanger changer = piece.FindChild("Mesh") as IMaterialChanger;

                if (changer != null)
                {
                    changer.SetMaterial(MaterialChangerMaterial.HIGHLIGHTED);
                }
            }

        }
    }
    void OnPieceClicked(uint id)
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
                        changer.SetMaterial(MaterialChangerMaterial.SELECTED);
                    }
                    state = State.PIECE_SELECTED;
                    selectedPiece = node;
                    HighlightSurroundingPositions();
                }
            }
        }
        else if (state == State.PIECE_SELECTED)
        {
            var node = boardController.GetPieceNode(id);


            if (node != null)
            {
                if (CanKill(node))
                {
                    IssueCommand(new Kill(selectedPiece.ID, id));
                }
                else if (node == selectedPiece)
                {
                    ReturnMaterialsToNormal();
                    state = State.SELECTING_ACTION;
                }
            }

        }
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

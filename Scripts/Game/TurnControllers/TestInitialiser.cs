using Godot;
using System;

public partial class TestInitialiser: TurnControllerInitialiser
{
    public override (TurnController,TurnController) Initialise(Master master)
    {
        HandControlToPlayerTurnController p1 = new HandControlToPlayerTurnController(master,Player.PLAYER1);
        HandControlToPlayerTurnController p2 = new HandControlToPlayerTurnController(master,Player.PLAYER2);

        return (p1,p2);
    }

}
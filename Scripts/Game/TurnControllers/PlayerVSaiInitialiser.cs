using Godot;
using System;

public partial class PlayerVSaiInitialiser : TurnControllerInitialiser
{
    [Export]
    public string AItype {get; set;}= "easy";
    public override (TurnController, TurnController) Initialise(Master master)
    {
        IGameAI gameAI;
        if (AItype == "easy")
        {
            gameAI = new RandomAI();
        }
        else
        {
            gameAI = new RandomAI();
        }

        HandControlToPlayerTurnController p1 = new HandControlToPlayerTurnController(master, Player.PLAYER1);
        var p2 = new AIController(master, Player.PLAYER2, gameAI);

        return (p1, p2);
    }

}
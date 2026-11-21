using Godot;
using System;

public class HandControlToPlayerTurnController:TurnController
{
    public PlayerTurnController TurnControllerNode {get; set;}

    public HandControlToPlayerTurnController(Master master, Player owner): base(owner)
    {
        TurnControllerNode = master.FindChild("PlayerTurnController") as PlayerTurnController;
    }

    public override void StartTurn(Action callback = null)
    {
        if (TurnControllerNode != null)
        {
            TurnControllerNode.StartTurn(Player, callback);
        }
        else
        {
            callback();
        }
    }
}
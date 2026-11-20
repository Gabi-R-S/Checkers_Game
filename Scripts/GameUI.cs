using Godot;
using System;

public partial class GameUI : Node2D
{
    [Export]
    Master master;

    PlayerTurnController controller;
    public override void _Ready()
    {
        controller = master.FindChild("PlayerTurnController") as PlayerTurnController;

    }

}

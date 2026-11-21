using Godot;
using System;

public partial class GameUI : Node2D
{
    [Export]
    Master master;

    PlayerTurnController controller;

    [Export]
    Node2D heart1;
    [Export]
    Node2D heart2;
    [Export]
    Node2D heart3;

    public override void _Ready()
    {
        Hide();
        controller = master.FindChild("PlayerTurnController") as PlayerTurnController;

        controller.TurnStart+=()=>{
            Show();
            heart1.Show();
            heart2.Show();
            heart3.Show();
        };
        controller.TurnEnd+=Hide;

        controller.EnergyChanged += (e) =>
        {
            switch (e)
            {
                case 2:
                heart3.Hide();
                break;

                case 1:
                heart2.Hide();
                break;

                case 0:
                heart1.Hide();
                break;
                
                default:
                break;
            }

        };


        var button = FindChild("SkipTurn") as Button;
        if (button != null)
        {
            var turnController = master.FindChild("PlayerTurnController") as PlayerTurnController;
            button.Pressed += turnController.EndTurn;
            
        }
    }

}

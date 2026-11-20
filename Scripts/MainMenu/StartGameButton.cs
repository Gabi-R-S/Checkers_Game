using Godot;
using System;

public partial class StartGameButton : Button
{
    [Export]
    string AItype = "easy";

    [Export]
    Master master;

    public override void _Ready()
    {
        Pressed += () =>
        {
            IGameAI ai;

            if (AItype == "easy")
            {
                ai = new RandomAI();
            }
            else if(AItype == "normal")
            {
                ai = new RuleBasedAI();
            }
            else if(AItype == "hard")
            {
                ai = new MinMaxAI();
            }
            else
            {
                ai = new RandomAI();    
            }
            
            master.Player1 = new HandControlToPlayerTurnController(master,Player.PLAYER1);
            master.Player2 = new AIController(master,Player.PLAYER1,ai);
            master.StartGame();


            var parent = GetParent<Node2D>();
            parent.Hide();
            
            parent.GetParent().AddChild(master);
            parent.GetParent().RemoveChild(parent);


        };
    }


}

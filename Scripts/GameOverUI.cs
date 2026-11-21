using Godot;
using System;

public partial class GameOverUI : Node2D
{
    [Export]
    Master master;

    [Export]
    Button returnButton;
    [Export]
    Label youWin;
    [Export]
    Label gameOver;
    void ShowEndGameMenu(Player player)
    {
        returnButton?.Show();
        if(player == Player.PLAYER1)
        {
            youWin?.Show();
        }
        else
        {
            gameOver?.Show();
        }
    }
    public override void _Ready()
    {
        if (master != null)
        {
            master.GameShouldEnd +=ShowEndGameMenu; 
        }
        if (returnButton != null)
        {
            returnButton.Pressed += () =>
            {
                returnButton.Hide();
                youWin.Hide();
                gameOver.Hide();
            };
        }
    }

}

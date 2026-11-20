using Godot;
using System;

public abstract class TurnController
{
    public Player Player{get; private set;}
    public abstract void StartTurn(Action callback = null);

    public TurnController(Player player)
    {
        Player = player;
    }
}
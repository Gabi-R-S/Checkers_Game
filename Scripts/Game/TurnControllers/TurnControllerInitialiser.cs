using Godot;
using System;

public abstract partial class TurnControllerInitialiser: Resource
{
    public abstract (TurnController,TurnController) Initialise(Master master);
}
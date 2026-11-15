using Godot;
using System;

public partial class Increase : Button
{
    [Export]
    BoardPosition target;
    public override void _Ready()
    {
      ButtonDown += () => {
        target?.IncreaseHeightSteps();
      };
    }
}

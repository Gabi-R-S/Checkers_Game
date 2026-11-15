using Godot;
using System;

public partial class Decrease : Button
{
    [Export]
    BoardPosition target;
    public override void _Ready()
    {
      ButtonDown += () => {
        target?.DecreaseHeightSteps();
      };
    }
}

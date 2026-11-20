using Godot;
using System;

public partial class StartGameButton : Button
{
    [Export]
    string AItype = "easy";

    [Export]
    PackedScene gameMasterNode;

    public override void _Ready()
    {
        Pressed += () =>
        {
            var master = gameMasterNode.Instantiate<Master>();

            var aiInitialiser = new PlayerVSaiInitialiser();

            aiInitialiser.AItype = AItype;

            master.Initialiser = aiInitialiser;

            
            var parent = GetParent<Node2D>();

            parent.GetParent().AddChild(master);

            parent.GetParent().RemoveChild(parent);

            
        };
    }


}

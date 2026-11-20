using Godot;
using System;

public class AIController : TurnController
{
    public BoardController BoardController { get; set; }
    IGameAI ai;

    public AIController(Master master, Player owner, IGameAI ai) : base(owner)
    {
        this.ai = ai;
        BoardController = master.FindChild("BoardController") as BoardController;
    }

    public override void StartTurn(Action callback = null)
    {
        if (BoardController != null && ai != null)
        {
            var commands = ai.GetCommands(Player, BoardController.Board);

            void PlayInitialCommand()
            {
                if (commands.Count > 0)
                {

                    var initialCommand = commands[0];
                    commands.RemoveAt(0);

                    BoardController.Perform(initialCommand, PlayInitialCommand);

                }
                else
                {
                    callback?.Invoke();
                }
            }


            PlayInitialCommand();

        }
        else
        {
            callback();
        }
    }
}
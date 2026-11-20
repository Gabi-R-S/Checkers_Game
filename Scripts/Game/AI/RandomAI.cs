
using System.Collections.Generic;

public class RandomAI : IGameAI
{
    public List<TurnCommand> GetCommands(Player player, Board board)
    {
        var commands = new List<TurnCommand>();

        Board localBoard = board.Clone();

        for (int i = 0; i < Globals.MAX_TURN_ENERGY; i++)
        {
            var validCommands = localBoard.GetAllValidCommands(player);
            if (validCommands.Count == 0)
            {

                break;
            }

            var ind = Globals.RNG.NextInt64() % validCommands.Count;

            var command = validCommands[(int)ind];
            commands.Add(command);

            localBoard.Apply(command);
        }

        return commands;
    }

}
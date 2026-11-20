
using System.Collections.Generic;

public interface IGameAI
{
    List<TurnCommand> GetCommands(Player player, Board board);

}
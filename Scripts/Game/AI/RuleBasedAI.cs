
using System;
using System.Collections.Generic;

public class RuleBasedAI : IGameAI
{


public void FavourKills(
    Player player,
    Board board,
    Dictionary<TurnCommand, float> commands,
    int movesLeft)
{
    foreach (var command in commands.Keys)
    {
        if (command is Kill kill)
        {
            float bonus = 100f;

            // Reward killing enemies that threaten our pieces.
            if (IsThreatened(board, kill.TargetID, player))
                bonus += 20f;

            // Reward killing enemies that are close to our pieces.
            var (tx, ty, _) = board.Pieces[kill.TargetID];

            foreach (var (id, data) in board.Pieces)
            {
                if (data.Item3 == player)
                {
                    float distance = Distance(
                        tx, ty, data.Item1, data.Item2);

                    if (distance <= 2)
                        bonus += 5f;
                }
            }

            commands[command] += bonus;
        }
    }
}



public void FavourMoveTowardsEnemyPieces(
    Player player,
    Board board,
    Dictionary<TurnCommand, float> commands,
    int movesLeft)
{
    foreach (var command in commands.Keys)
    {
        if (command is not Move move)
            continue;

        var (oldX, oldY, _) = board.Pieces[move.PieceID];

        float oldDistance = float.MaxValue;
        float newDistance = float.MaxValue;

        foreach (var (id, data) in board.Pieces)
        {
            if (data.Item3 == player)
                continue;

            float oldD = Distance(
                oldX, oldY, data.Item1, data.Item2);

            float newD = Distance(
                move.X, move.Y, data.Item1, data.Item2);

            oldDistance = Math.Min(oldDistance, oldD);
            newDistance = Math.Min(newDistance, newD);
        }

        if (oldDistance == float.MaxValue)
            continue;

        if (newDistance < oldDistance)
        {
            commands[command] +=
                (oldDistance - newDistance) * 5f;
        }

        // Prefer higher terrain when approaching enemies.
        uint oldHeight = board.GetHeightAt(oldX, oldY);
        uint newHeight = board.GetHeightAt(move.X, move.Y);

        if (newHeight > oldHeight)
            commands[command] += 3f;
    }
}


   
public void AvoidBeingKilled(
    Player player,
    Board board,
    Dictionary<TurnCommand, float> commands,
    int movesLeft)
{
    foreach (var command in commands.Keys)
    {
        if (command is not Move move)
            continue;

        var (oldX, oldY, _) = board.Pieces[move.PieceID];

        float oldThreats = CountThreats(
            board, move.PieceID, GetEnemy(player));

        // Simulate the move.
        Board simulated = board.Clone();
        simulated.Apply(move);

        float newThreats = CountThreats(
            simulated, move.PieceID, GetEnemy(player));

        if (newThreats > oldThreats)
        {
            commands[command] -= 50f * newThreats;
        }
        else if (newThreats < oldThreats)
        {
            commands[command] += 30f * (oldThreats - newThreats);
        }

        // Additional penalty for moving into a position
        // where multiple enemies can attack us.
        if (newThreats > 1)
            commands[command] -= 20f;
    }
}


  

public void TryBlockEnemyPieceMovement(
    Player player,
    Board board,
    Dictionary<TurnCommand, float> commands,
    int movesLeft)
{
    Player enemy = GetEnemy(player);

    int oldMobility = CountEnemyMoves(board, enemy);

    foreach (var command in commands.Keys)
    {
        Board simulated = board.Clone();
        simulated.Apply(command);

        int newMobility = CountEnemyMoves(simulated, enemy);

        int blocked = oldMobility - newMobility;

        if (blocked > 0)
        {
            commands[command] += blocked * 10f;
        }

        // Especially valuable if the opponent has no moves.
        if (newMobility == 0)
        {
            commands[command] += 1000f;
        }
    }
}


    public TurnCommand ChooseCommand(Player player, Board board, List<TurnCommand> commands, int movesLeft)
    {
        Dictionary<TurnCommand, float> values = new Dictionary<TurnCommand, float>();

        foreach (var command in commands)
        {
            values.Add(command, 1.0f);
        }

        FavourKills(player,board,values,movesLeft);
        FavourMoveTowardsEnemyPieces(player,board,values,movesLeft);
        AvoidBeingKilled(player,board,values,movesLeft);
        TryBlockEnemyPieceMovement(player,board,values,movesLeft);



        float finalValue = 0f;
        TurnCommand finalCommand = null;
        foreach (var (command, value) in values)
        {
            if (value > finalValue)
            {
                finalCommand = command;
                finalValue = value;
            }
        }
        return finalCommand;
    }

    public override List<TurnCommand> GetCommands(Player player, Board board)
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

            var command = ChooseCommand(player, localBoard, commands, Globals.MAX_TURN_ENERGY - i);
            if (command == null)
            {
                break;
            }

            commands.Add(command);
            localBoard.Apply(command);
        }

        return commands;
    }

}
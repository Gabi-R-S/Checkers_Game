
using System.Collections.Generic;
using System;
public abstract class IGameAI
{
    public abstract List<TurnCommand> GetCommands(Player player, Board board);




    protected int CountEnemyMoves(Board board, Player enemy)
    {
        int count = 0;

        foreach (var (id, data) in board.Pieces)
        {
            if (data.Item3 != enemy)
                continue;

            uint x = data.Item1;
            uint y = data.Item2;

            if (x > 0 && board.CanMoveTo(id, x - 1, y))
                count++;

            if (board.CanMoveTo(id, x + 1, y))
                count++;

            if (y > 0 && board.CanMoveTo(id, x, y - 1))
                count++;

            if (board.CanMoveTo(id, x, y + 1))
                count++;
        }

        return count;
    }
    protected float Distance(uint x1, uint y1, uint x2, uint y2)
    {
        return Math.Abs((int)x1 - (int)x2) +
               Math.Abs((int)y1 - (int)y2);
    }

    protected bool IsThreatened(Board board, uint pieceID, Player enemy)
    {
        if (!board.Pieces.ContainsKey(pieceID))
            return false;

        var (x, y, _) = board.Pieces[pieceID];

        foreach (var (id, data) in board.Pieces)
        {
            if (data.Item3 == enemy && board.CanKill(id, pieceID))
                return true;
        }

        return false;
    }

    protected float CountThreats(Board board, uint pieceID, Player enemy)
    {
        float threats = 0;

        foreach (var (id, data) in board.Pieces)
        {
            if (data.Item3 == enemy && board.CanKill(id, pieceID))
                threats++;
        }

        return threats;
    }

    protected Player GetEnemy(Player player)
    {
        return player == Player.PLAYER1
            ? Player.PLAYER2
            : Player.PLAYER1;
    }
}
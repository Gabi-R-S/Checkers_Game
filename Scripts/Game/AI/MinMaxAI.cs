
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MinMaxAI : IGameAI
{
    private const float WIN_VALUE = 10000f;
    private const float INF = float.MaxValue;

    public int MaxDepth { get; set; } = 3;

    private enum BoundType
    {
        Exact,
        Lower,
        Upper
    }

    private class TTEntry
    {
        public float Value;
        public int Depth;
        public BoundType Bound;
    }

    private Dictionary<string, TTEntry> transpositionTable =
        new Dictionary<string, TTEntry>();


    // --------------------------------------------------
    // BOARD EVALUATION
    // --------------------------------------------------

    public float Value(Player player, Board board)
    {
        Player enemy = GetEnemy(player);

        var ownPieces = board.Pieces
            .Where(p => p.Value.Item3 == player).ToList();

        var enemyPieces = board.Pieces
            .Where(p => p.Value.Item3 == enemy).ToList();

        if (ownPieces.Count == 0)
            return -WIN_VALUE;

        if (enemyPieces.Count == 0)
            return WIN_VALUE;

        float score = 0;

        // Material advantage.
        score += ownPieces.Count * 100f;
        score -= enemyPieces.Count * 100f;

        // Mobility advantage.
        score += CountEnemyMoves(board, enemy) * -3f;
        score += CountEnemyMoves(board, player) * 3f;

        // Threats against individual pieces.
        foreach (var (id, data) in ownPieces)
        {
            int threats = (int)CountThreats(
                board, id, enemy);

            score -= threats * 25f;
        }

        foreach (var (id, data) in enemyPieces)
        {
            int threats = (int)CountThreats(
                board, id, player);

            score += threats * 25f;
        }

        return score;
    }


    // --------------------------------------------------
    // TERMINAL STATE
    // --------------------------------------------------

    private bool IsTerminal(
        Board board,
        Player player,
        out float score)
    {
        score = 0;

        if (!board.Pieces.Any(p => p.Value.Item3 == player))
        {
            score = -WIN_VALUE;
            return true;
        }

        Player enemy = GetEnemy(player);

        if (!board.Pieces.Any(p => p.Value.Item3 == enemy))
        {
            score = WIN_VALUE;
            return true;
        }

        if (board.GetAllValidCommands(player).Count == 0)
        {
            score = -WIN_VALUE;
            return true;
        }

        if (board.GetAllValidCommands(enemy).Count == 0)
        {
            score = WIN_VALUE;
            return true;
        }

        return false;
    }


    // --------------------------------------------------
    // TRANSPOSITION TABLE
    // --------------------------------------------------

    private string GetBoardKey(
        Board board,
        Player player,
        int movesLeft)
    {
        StringBuilder key = new StringBuilder();

        var size = board.Size;

        key.Append(player);
        key.Append('|');
        key.Append(movesLeft);
        key.Append('|');

        for (uint x = 0; x < size.X; x++)
        {
            for (uint y = 0; y < size.Y; y++)
            {
                key.Append(board.GetHeightAt(x, y));
                key.Append(',');
            }
        }

        key.Append('|');

        foreach (var piece in board.Pieces.OrderBy(p => p.Key))
        {
            key.Append(piece.Key);
            key.Append(':');
            key.Append(piece.Value.Item1);
            key.Append(',');
            key.Append(piece.Value.Item2);
            key.Append(',');
            key.Append(piece.Value.Item3);
            key.Append(';');
        }

        return key.ToString();
    }


    // --------------------------------------------------
    // MOVE ORDERING
    // --------------------------------------------------

    private List<TurnCommand> OrderCommands(
        List<TurnCommand> commands)
    {
        return commands
            .OrderByDescending(command =>
            {
                if (command is Kill)
                    return 3;

                if (command is Move)
                    return 2;

                if (command is IncreaseHeight)
                    return 1;

                return 0;
            })
            .ToList();
    }


    // --------------------------------------------------
    // MINIMAX + ALPHA-BETA
    // --------------------------------------------------

    private float Search(
        Board board,
        Player currentPlayer,
        Player maximizingPlayer,
        int movesLeft,
        int depth,
        float alpha,
        float beta)
    {
        // A player has finished their turn.
        if (movesLeft <= 0)
        {
            currentPlayer = GetEnemy(currentPlayer);
            movesLeft = Globals.MAX_TURN_ENERGY;
            depth--;
        }

        // Terminal states.
        if (IsTerminal(board, currentPlayer, out float terminalScore))
        {
            return currentPlayer == maximizingPlayer
                ? terminalScore
                : -terminalScore;
        }

        // Search depth exhausted.
        if (depth <= 0)
            return Value(maximizingPlayer, board);

        string key = GetBoardKey(
            board, currentPlayer, movesLeft);

        float originalAlpha = alpha;
        float originalBeta = beta;

        // Transposition table lookup.
        if (transpositionTable.TryGetValue(key, out TTEntry entry))
        {
            if (entry.Depth >= depth)
            {
                if (entry.Bound == BoundType.Exact)
                    return entry.Value;

                if (entry.Bound == BoundType.Lower)
                    alpha = Math.Max(alpha, entry.Value);

                if (entry.Bound == BoundType.Upper)
                    beta = Math.Min(beta, entry.Value);

                if (alpha >= beta)
                    return entry.Value;
            }
        }

        var commands = OrderCommands(
            board.GetAllValidCommands(currentPlayer));

        bool maximizing = currentPlayer == maximizingPlayer;

        float bestValue = maximizing ? -INF : INF;

        foreach (var command in commands)
        {
            Board child = board.Clone();
            child.Apply(command);

            float value = Search(
                child,
                currentPlayer,
                maximizingPlayer,
                movesLeft - 1,
                depth,
                alpha,
                beta);

            if (maximizing)
            {
                bestValue = Math.Max(bestValue, value);
                alpha = Math.Max(alpha, bestValue);
            }
            else
            {
                bestValue = Math.Min(bestValue, value);
                beta = Math.Min(beta, bestValue);
            }

            // Alpha-beta pruning.
            if (alpha >= beta)
                break;
        }

        // Store result in transposition table.
        BoundType bound;

        if (bestValue <= originalAlpha)
            bound = BoundType.Upper;
        else if (bestValue >= originalBeta)
            bound = BoundType.Lower;
        else
            bound = BoundType.Exact;

        transpositionTable[key] = new TTEntry
        {
            Value = bestValue,
            Depth = depth,
            Bound = bound
        };

        return bestValue;
    }


    // --------------------------------------------------
    // COMMAND SELECTION
    // --------------------------------------------------

    private TurnCommand FindBestCommand(
        Player player,
        Board board,
        List<TurnCommand> commands,
        int movesLeft,
        int depth)
    {
        TurnCommand bestCommand = null;
        float bestValue = -INF;

        float alpha = -INF;

        foreach (var command in OrderCommands(commands))
        {
            Board child = board.Clone();
            child.Apply(command);

            float value = Search(
                child,
                player,
                player,
                movesLeft - 1,
                depth,
                alpha,
                INF);

            if (value > bestValue)
            {
                bestValue = value;
                bestCommand = command;
            }

            alpha = Math.Max(alpha, bestValue);
        }

        return bestCommand;
    }


    // --------------------------------------------------
    // GET COMMANDS
    // --------------------------------------------------

    public override List<TurnCommand> GetCommands(
        Player player,
        Board board)
    {
        transpositionTable.Clear();

        List<TurnCommand> commands = new List<TurnCommand>();

        Board localBoard = board.Clone();

        for (int i = 0; i < Globals.MAX_TURN_ENERGY; i++)
        {
            var validCommands =
                localBoard.GetAllValidCommands(player);

            if (validCommands.Count == 0)
                break;

            TurnCommand command = FindBestCommand(
                player,
                localBoard,
                validCommands,
                Globals.MAX_TURN_ENERGY - i,
                MaxDepth);

            if (command == null)
                break;

            commands.Add(command);
            localBoard.Apply(command);
        }

        return commands;
    }
}

using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.ComputerOpponent;
using System;
using System.Diagnostics;

namespace Cecs475.BoardGames.Chess.Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
         ChessBoard b = new ChessBoard();
         IGameAi ai = new MinimaxAi(3);
            int MAX_MOVES = 6;
         Stopwatch watch = new Stopwatch();
         watch.Start();
            for (int i = 0; i < MAX_MOVES; i++)
            {
                var move = ai.FindBestMove(b);
                b.ApplyMove(move as ChessMove);

            }
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds} ms total");
          
        }
    }
}

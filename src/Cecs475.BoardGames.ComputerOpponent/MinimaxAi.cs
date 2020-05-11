using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.ComputerOpponent {
	internal struct MinimaxBestMove {
		public long Weight { get; set; }
		public IGameMove Move { get; set; }
	}

	public class MinimaxAi : IGameAi {
		private int mMaxDepth;
		public MinimaxAi(int maxDepth) {
			mMaxDepth = maxDepth;
		}

		public IGameMove FindBestMove(IGameBoard b) {
			return FindBestMove(b,
				true ? long.MinValue : long.MaxValue,
				true ? long.MaxValue : long.MinValue,
				mMaxDepth,b.CurrentPlayer==1).Move;
		}
		//private async Task<MinimaxBestMove> dummy(IGameBoard board, long alpha, long beta, int depthLeft, bool isMaximizing)
		//{
		//	return Task.Run(FindBestMove(board, alpha, beta, depthLeft, isMaximizing));
		//}

		private static MinimaxBestMove FindBestMove(IGameBoard board, long alpha, long beta, int depthLeft, bool isMaximizing)
		{
			if (depthLeft == 0 || board.IsFinished)
				return new MinimaxBestMove() { Weight = board.BoardWeight, Move = null };

			IGameMove bestMove = null;
			foreach (var move in board.GetPossibleMoves())
			{
				board.ApplyMove(move);
				var w = FindBestMove(board, alpha, beta, depthLeft - 1, !isMaximizing);
				board.UndoLastMove();

				if (isMaximizing && w.Weight > alpha)
				{
					bestMove = move;
					alpha = w.Weight;
					if (beta <= alpha)
						return new MinimaxBestMove() { Weight = beta, Move = bestMove };
				}
				else if (!isMaximizing && w.Weight < beta)
				{

					bestMove = move;
					beta = w.Weight;
					if (beta <= alpha)
						return new MinimaxBestMove() { Weight = alpha, Move = bestMove };
				}
			}
			return new MinimaxBestMove() { Weight = isMaximizing ? alpha : beta, Move = bestMove };
			//return minimax(b, depthLeft, b.CurrentPlayer == 1);
		}


		//private static MinimaxBestMove  minimax( IGameBoard board,int depth, bool isMaximizing)
		//{

		//	if (depth==0|| board.IsFinished)
		//		return new MinimaxBestMove() { Weight = board.BoardWeight,  Move= null };

		//	long bestWeight = (board.CurrentPlayer == 1) ? long.MinValue : long.MaxValue;
		//	IGameMove bestMove = null;

		//	foreach(var move in board.GetPossibleMoves())
		//	{
		//		board.ApplyMove(move);
		//		var w = minimax(board, depth - 1, !isMaximizing);
		//		board.UndoLastMove();
		//		if (isMaximizing && w.Weight>bestWeight)
		//		{
		//			bestWeight = w.Weight;
		//			bestMove = move;
		//		}
		//		else if (!isMaximizing&& w.Weight < bestWeight)
		//		{
		//			bestWeight = w.Weight;
		//			bestMove = move;
		//		}
		//	}
		//	return new MinimaxBestMove() { Weight = bestWeight, Move = bestMove };


		//}

	}
}

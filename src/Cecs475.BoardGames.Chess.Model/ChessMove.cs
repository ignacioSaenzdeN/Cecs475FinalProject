using System;
using System.Collections.Generic;
using Cecs475.BoardGames.Model;

namespace Cecs475.BoardGames.Chess.Model
{
    /// <summary>
    /// Represents a single move to be applied to a chess board.
    /// </summary>
    public class ChessMove : IGameMove, IEquatable<ChessMove>
    {
        // You can add additional fields, properties, and methods as you find
        // them necessary, but you cannot MODIFY any of the existing implementations.

        /// <summary>
        /// The starting position of the move.
        /// </summary>
        public BoardPosition StartPosition { get; }

        /// <summary>
        /// The ending position of the move.
        /// </summary>
        public BoardPosition EndPosition { get; }

        /// <summary>
        /// The type of move being applied.
        /// </summary>
        public ChessMoveType MoveType { get; }

        // You must set this property when applying a move.
        public int Player { get; set; }

        public ChessPieceType promoteTo { get; set; }
        /// <summary>
        /// Constructs a ChessMove that moves a piece from one position to another
        /// </summary>
        /// <param name="start">the starting position of the piece to move</param>
        /// <param name="end">the position where the piece will end up</param>
        /// <param name="moveType">the type of move represented</param>
        public ChessMove(BoardPosition start, BoardPosition end, ChessMoveType moveType = ChessMoveType.Normal)
        {
            StartPosition = start;
            EndPosition = end;
            MoveType = moveType;
            if (moveType.Equals(ChessMoveType.PawnPromote))
                promoteTo = ChessPieceType.Empty;
        }
        public ChessMove(BoardPosition start, BoardPosition end, ChessPieceType _promoteto, ChessMoveType moveType = ChessMoveType.PawnPromote)
        {
            StartPosition = start;
            EndPosition = end;
            MoveType = moveType;
            promoteTo = _promoteto;
        }


        // TODO: You must write this method.
        public virtual bool Equals(ChessMove other)
        {
            // Most chess moves are equal to each other if they have the same start and end position.
            // PawnPromote moves must also be promoting to the same piece type.
            if ((this.StartPosition.Equals(other.StartPosition) && this.EndPosition.Equals(other.EndPosition)) && this.promoteTo.Equals(other.promoteTo))
                return true;
/*            else if (this.StartPosition.Equals(other.StartPosition) && this.EndPosition.Equals(other.EndPosition) && this.promoteTo.Equals(other.promoteTo))
                return true;*/
            return false;
            //throw new NotImplementedException("You are responsible for implementing this method.");
        }



        // Equality methods.
        bool IEquatable<IGameMove>.Equals(IGameMove other)
        {
            ChessMove m = other as ChessMove;
            return this.Equals(m);
        }

        public override bool Equals(object other)
        {
            return Equals(other as ChessMove);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StartPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EndPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)MoveType;
                return hashCode;
            }
        }

        public static IEnumerable<BoardPosition> GetMoves4Piece(BoardPosition pos, ChessBoard board)
        {
            ChessPiece piece = board.GetPieceAtPosition(pos);
            IEnumerable<BoardPosition> attack_positions = new List<BoardPosition>();
            if (piece.PieceType.Equals(ChessPieceType.Pawn))
                return GetPawnMoves(pos, board);
            else if (piece.PieceType.Equals(ChessPieceType.Rook))
                return GetRookMoves(pos, board);
            else if (piece.PieceType.Equals(ChessPieceType.Knight))
                return GetKnightMoves(pos, board);
            else if (piece.PieceType.Equals(ChessPieceType.Bishop))
                return GetBishopMoves(pos, board);
            else if (piece.PieceType.Equals(ChessPieceType.Queen))
                return GetQueenMoves(pos, board);
            else if (piece.PieceType.Equals(ChessPieceType.King))
                return GetKingMoves(pos, board);
            return attack_positions;
        }

        public static IEnumerable<BoardPosition> GetPawnMoves(BoardPosition pawn_pos, ChessBoard board)
        {
            List<BoardPosition> pawn_moves = new List<BoardPosition>();
            //ChessPiece pawn = board.GetPieceAtPosition(pawn_pos);
            /*
            //front
            BoardPosition one_front = new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col);
            if (ChessBoard.PositionInBounds(one_front))
                pawn_moves.Add(one_front);
            */
            //front right
            int player = board.GetPlayerAtPosition(pawn_pos);
            if (player == 1)
            {
                BoardPosition front_right = new BoardPosition(pawn_pos.Row- 1, pawn_pos.Col - 1);
                if (ChessBoard.PositionInBounds(front_right))
                    pawn_moves.Add(front_right);
                //front-left
                BoardPosition one_left = new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col + 1);
                if (ChessBoard.PositionInBounds(one_left))
                    pawn_moves.Add(one_left);
                /*
                 *  if (pawn_pos.Row == 1)
                {
                    BoardPosition two_front = new BoardPosition(pawn_pos.Row - 2, pawn_pos.Col);
                    pawn_moves.Add(two_front);
                }
                 */

            }
            else
            {
                BoardPosition front_right = new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col + 1);
                if (ChessBoard.PositionInBounds(front_right))
                    pawn_moves.Add(front_right);
                //front-left
                BoardPosition one_left = new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col - 1);
                if (ChessBoard.PositionInBounds(one_left))
                    pawn_moves.Add(one_left);
                /*
                 * 
                 *  if (pawn_pos.Row == 1)
                {
                    BoardPosition two_front = new BoardPosition(pawn_pos.Row + 2, pawn_pos.Col);
                    pawn_moves.Add(two_front);
                }
                */
               
            }
            
            return pawn_moves;
        }
        public static IEnumerable<BoardPosition> GetKnightMoves(BoardPosition pos, ChessBoard board)
        {
            List<BoardPosition> moves = new List<BoardPosition>();
            // front left and front right
            for (int i = -2; i <= 2; i += 4)
            {
                //front and back
                for (int j = -1; j <= 1; j += 2)
                {
                    BoardPosition back = new BoardPosition(pos.Row + i, pos.Col + j);
                    if (ChessBoard.PositionInBounds(back))
                        moves.Add(back);
                }
                //right and left
                for (int j = -1; j <= 1; j += 2)
                {
                    BoardPosition right = new BoardPosition(pos.Row + j, pos.Col + i);
                    if (ChessBoard.PositionInBounds(right))
                        moves.Add(right);
                }
            }
            return moves;
        }
        public static IEnumerable<BoardPosition> GetKingMoves(BoardPosition pos, ChessBoard board)
        {
            List<BoardPosition> moves = new List<BoardPosition>();
            BoardPosition left = new BoardPosition(pos.Row, pos.Col - 1);
            if (ChessBoard.PositionInBounds(left))
                moves.Add(left);
            BoardPosition right = new BoardPosition(pos.Row, pos.Col + 1);
            if (ChessBoard.PositionInBounds(right))
                moves.Add(right);

            BoardPosition front = new BoardPosition(pos.Row + 1, pos.Col);
            if (ChessBoard.PositionInBounds(front))
                moves.Add(front);

            BoardPosition back = new BoardPosition(pos.Row - 1, pos.Col);
            if (ChessBoard.PositionInBounds(back))
                moves.Add(back);

            //diagonal
            BoardPosition front_left = new BoardPosition(pos.Row + 1, pos.Col - 1);
            if (ChessBoard.PositionInBounds(front_left))
                moves.Add(front_left);
            BoardPosition front_right = new BoardPosition(pos.Row + 1, pos.Col + 1);
            if (ChessBoard.PositionInBounds(front_right))
                moves.Add(front_right);

            BoardPosition back_left = new BoardPosition(pos.Row - 1, pos.Col - 1);
            if (ChessBoard.PositionInBounds(back_left))
                moves.Add(back_left);

            BoardPosition back_right = new BoardPosition(pos.Row - 1, pos.Col + 1);
            if (ChessBoard.PositionInBounds(back_right))
                moves.Add(back_right);


            return moves;
        }

        public static IEnumerable<BoardPosition> GetRookMoves(BoardPosition pos, ChessBoard board)
        {
            
            List<BoardPosition> moves = new List<BoardPosition>();
            int row_adder = 1;
            int col_adder = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i == 1) { row_adder = -1; }
                if (i == 2) { row_adder = 0;col_adder = 1; }
                if (i == 3) { col_adder = -1; }
                BoardPosition temp = new BoardPosition(pos.Row +row_adder, pos.Col+col_adder);
                while (ChessBoard.PositionInBounds(temp))
                {
                    moves.Add(temp);
                    if (!board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))
                        break;
                    temp = new BoardPosition(temp.Row +row_adder, temp.Col+ col_adder);
                }
            }
            //BoardPosition temp = new BoardPosition(pos.Row + 1, pos.Col);
            //while (ChessBoard.PositionInBounds(temp))
            //{
            //    moves.Add(temp);
            //    if (! board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))   
            //        break; 
            //    temp = new BoardPosition(temp.Row+1, temp.Col);
            //}
            ////while loop right
            //temp = new BoardPosition(pos.Row, pos.Col + 1);
            //while (ChessBoard.PositionInBounds(temp))
            //{
            //    moves.Add(temp);
            //    if (!board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))
            //        break;
            //    temp = new BoardPosition(temp.Row, temp.Col + 1);
            //}
            ////while loop back
            //temp = new BoardPosition(pos.Row - 1, pos.Col);
            //while (ChessBoard.PositionInBounds(temp))
            //{
            //    moves.Add(temp);
            //    if (!board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))
            //        break;
            //    temp = new BoardPosition(temp.Row - 1, temp.Col);
            //}
            ////while loop left
            //temp = new BoardPosition(pos.Row, pos.Col-1);
            //while (ChessBoard.PositionInBounds(temp))
            //{
            //    moves.Add(temp);
            //    if (!board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))
            //        break;
            //    temp = new BoardPosition(temp.Row, temp.Col-1);
            //}

            return (IEnumerable<BoardPosition>)moves;
        }
        public static IEnumerable<BoardPosition> GetQueenMoves(BoardPosition pos, ChessBoard board)
        {
            List<BoardPosition> moves = new List<BoardPosition>();
            //while loop for front
            foreach( BoardPosition vertical_horizontal in GetRookMoves(pos,board))
                moves.Add(vertical_horizontal);
            foreach(BoardPosition diagonal in GetBishopMoves(pos, board))
                moves.Add(diagonal);
            return (IEnumerable<BoardPosition>)moves;
        }
        public static IEnumerable<BoardPosition> GetBishopMoves(BoardPosition pos, ChessBoard board)
        {
            List<BoardPosition> moves = new List<BoardPosition>();
            //while loop front-right
            int row_adder = -1;
            int col_adder = -1;
            for (int i = 0; i < 4; i++)
            {
                if (i == 1) { col_adder = 1; }
                if (i == 2) { row_adder = 1; col_adder = -1;}
                if (i==3) { col_adder = 1; }

                BoardPosition temp = new BoardPosition(pos.Row +row_adder, pos.Col + col_adder);
                while (ChessBoard.PositionInBounds(temp))
                {
                    moves.Add(temp);
                    if (!board.GetPieceAtPosition(temp).PieceType.Equals(ChessPieceType.Empty))
                        break;
                    temp = new BoardPosition(temp.Row +row_adder, temp.Col +col_adder);
                }

            }
            return (IEnumerable<BoardPosition>)moves;
        }

        public override string ToString()
        {
            return $"{StartPosition} to {EndPosition}";
        }

    }
}

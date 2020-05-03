using System;
using System.Collections.Generic;
using System.Text;

namespace Cecs475.BoardGames.Chess.Model {
    /// <summary>
    /// Represents a chess piece owned by a particular player.
    /// </summary>
    public struct ChessPiece {
        public ChessPieceType PieceType { get; }
        public sbyte Player { get; }

        public ChessPiece(ChessPieceType pieceType, int player) {
            PieceType = pieceType;
            Player = (sbyte)player;
        }

        public static ChessPiece Empty { get; } = new ChessPiece(ChessPieceType.Empty, 0);

        public override string ToString()
        {
            string piece_type = PieceType.ToString();
            int a_player = (int)Player;
            string s_player;
            if(a_player == 1)
            {
                s_player = "white";
            }
            else
            {
                s_player = "black";
            }

            return s_player + " " + piece_type;
        }
    }
}

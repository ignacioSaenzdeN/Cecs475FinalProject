using System;
using System.Collections.Generic;
using System.Text;
using Cecs475.BoardGames.Model;
using System.Linq;

namespace Cecs475.BoardGames.Chess.Model
{
    /// <summary>
    /// Represents the board state of a game of chess. Tracks which squares of the 8x8 board are occupied
    /// by which player's pieces.

    /// </summary>\
    public class ChessBoard : IGameBoard
    {
        #region Member fields.
        // The history of moves applied to the board.
        private List<ChessMove> mMoveHistory = new List<ChessMove>();

        public const int BoardSize = 8;

        // TODO: create a field for the board position array. You can hand-initialize
        // the starting entries of the array, or set them in the constructor.
        //DONE

        public byte[] Cboard = new byte[32];

        // TODO: Add a means of tracking miscellaneous board state, like captured pieces and the 50-move rule.


        // TODO: add a field for tracking the current player and the board advantage.		
        public struct CurrentGameState
        {
            public ChessPiece piece_captured;
            public bool white_left_rook;
            public bool white_right_rook;
            public bool black_left_rook;
            public bool black_right_rook;
            public bool white_king;
            public bool black_king;
            public int draw_counter;
            public int advantage;
            public ChessPiece promoted_piece;
            public bool checkmate;
            public CurrentGameState(CurrentGameState cs)
            {
                piece_captured = cs.piece_captured;
                white_left_rook = cs.white_left_rook;
                white_right_rook = cs.white_right_rook;
                black_left_rook = cs.black_left_rook;
                black_right_rook = cs.black_right_rook;
                white_king = cs.white_king;
                black_king = cs.black_king;
                draw_counter = cs.draw_counter;
                advantage = cs.advantage;
                promoted_piece = cs.promoted_piece;
                checkmate = cs.checkmate;
            }


            //this might be a problem
            //public int current_player;

        }
        // public static CurrentGameState currentGame = new CurrentGameState(new ChessPiece(0, 0), false, false, false, false, false, false, 0, 0);
        public static CurrentGameState currentGame = new CurrentGameState();
        //public int currentplayer;



        public List<CurrentGameState> ListCurrentGameStates = new List<CurrentGameState>() { currentGame };

        #endregion

        #region Properties.
        // TODO: implement these properties.
        // You can choose to use auto properties, computed properties, or normal properties 
        // using a private field to back the property.



        // You can add set bodies if you think that is appropriate, as long as you justify
        // the access level (public, private).

        public bool IsFinished
        {
            get
            {
                if (IsCheckmate||IsStalemate||IsDraw)
                    return true;
                return false;
            }
        }

        public int CurrentPlayer
        {
            get
            { 
                return (mMoveHistory.Count % 2 == 0) ? 1 : 2;
            }
        }

        public GameAdvantage CurrentAdvantage
        {
            get
            {

                int game_advantage = ListCurrentGameStates[ListCurrentGameStates.Count - 1].advantage;
                if (game_advantage < 0)
                    return new GameAdvantage(2, -1 * game_advantage);
                else if (game_advantage > 0)
                    return new GameAdvantage(1, game_advantage);
                else
                    return new GameAdvantage(0, 0);
            }
        }

        public IReadOnlyList<ChessMove> MoveHistory => mMoveHistory;

        // TODO: implement IsCheck, IsCheckmate, IsStalemate
        public bool IsCheck
        {
            get
            {
                if (ListCurrentGameStates[ListCurrentGameStates.Count() - 1].checkmate == true)
                    return false;

                int enemy_player = (CurrentPlayer == 1) ? 2 : 1;
                    BoardPosition king_pos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();
                    return PositionIsAttacked(king_pos, enemy_player);
                return false;
               
            }
        }

        public bool IsEnemyCheck
        {
            get
            {
                int enemy_player = (CurrentPlayer == 1) ? 2 : 1;
                BoardPosition king_pos = GetPositionsOfPiece(ChessPieceType.King, enemy_player).First();
                return PositionIsAttacked(king_pos, CurrentPlayer);
            }
        }

        public bool IsCheckmate
        {
            get
            {
                //if (inCheckRun() || inCheckBlock() || inCheckAttack())
                //    return false;
                //return true;
                int enemy_player = (CurrentPlayer == 1) ? 2 : 1;
                BoardPosition king_pos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();
                if (PositionIsAttacked(king_pos,enemy_player) &&GetPossibleMoves().Count() == 0)
                    return true;
                    
                return false;
            }
        }

        public bool IsStalemate
        {
            get
            {
                if (ListCurrentGameStates[ListCurrentGameStates.Count() - 1].checkmate == true)
                    return false;
                int enemy_player = (CurrentPlayer == 1) ? 2 : 1;
                if (GetPossibleMoves().Count ()== 0&&!IsCheck)
                    return true;
                return false;
            }
        }

        public bool IsDraw
        {
            get
            {
                if (ListCurrentGameStates[ListCurrentGameStates.Count() - 1].checkmate == true)
                    return false;
                return DrawCounter == 100;
            }
        }

        /// <summary>
        /// Tracks the current draw counter, which goes up by 1 for each non-capturing, non-pawn move, and resets to 0
        /// for other moves. If the counter reaches 100 (50 full turns), the game is a draw.
        /// </summary>
        public int DrawCounter
        {
            get
            {
                return ListCurrentGameStates.ElementAt(ListCurrentGameStates.Count - 1).draw_counter;
            }
        }
        #endregion


        #region Public methods.
        public IEnumerable<ChessMove> GetPossibleMoves()
        {
            List<ChessMove> possible_moves_total = new List<ChessMove>();
            //king_possible_moves = new List<ChessMove>();
            int enemy_player = (CurrentPlayer == 1) ? 2 : 1;

            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    BoardPosition current_position = new BoardPosition(row, col);
                    ChessPiece current_piece = GetPieceAtPosition(current_position);
                    if (current_piece.PieceType.Equals(ChessPieceType.Empty))
                        continue;

                    if (!current_piece.PieceType.Equals(ChessPieceType.Pawn) && !current_piece.PieceType.Equals(ChessPieceType.King) && GetPlayerAtPosition(current_position).Equals(CurrentPlayer))
                        foreach (BoardPosition attack_pos in ChessMove.GetMoves4Piece(current_position, this))
                            if (!GetPieceAtPosition(attack_pos).Player.Equals(current_piece.Player))
                                possible_moves_total.Add(new ChessMove(current_position, attack_pos, ChessMoveType.Normal));
                }
            // possible moves for pawn player 1 and 2

            if (CurrentPlayer == 1)
            {
                foreach (BoardPosition pawn_pos in GetPositionsOfPiece(ChessPieceType.Pawn, 1))
                {
                    bool front_pos_1 = GetPieceAtPosition(new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col)).PieceType.Equals(ChessPieceType.Empty);
                    bool front_pos_2 = false;
                    if (pawn_pos.Row - 2 > 0)
                        front_pos_2 = GetPieceAtPosition(new BoardPosition(pawn_pos.Row - 2, pawn_pos.Col)).PieceType.Equals(ChessPieceType.Empty);
                  
                    // en passant movement left white player capturing black pawn
                    if (mMoveHistory.Count > 0)
                    {
                        ChessMove last_move = mMoveHistory[mMoveHistory.Count - 1];
                        if (pawn_pos.Col > 0)
                        {
                            ChessPiece side_piece_left = GetPieceAtPosition(new BoardPosition(pawn_pos.Row, pawn_pos.Col - 1));
                            ChessMove passant_trigger_move_left = new ChessMove(new BoardPosition(pawn_pos.Row - 2, pawn_pos.Col - 1),
                                new BoardPosition(pawn_pos.Row, pawn_pos.Col - 1), ChessMoveType.Normal);
                            if (!(side_piece_left.Player.Equals(CurrentPlayer)) && side_piece_left.PieceType.Equals(ChessPieceType.Pawn)
                                && last_move.Equals(passant_trigger_move_left))
                            {
                                BoardPosition passant_attack = new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col - 1);
                                possible_moves_total.Add(new ChessMove(pawn_pos, passant_attack, ChessMoveType.EnPassant));
                            }
                        }
                        // en passant movement right of white player capturing black pawn
                        if ( pawn_pos.Col < 7)
                        {
                            ChessPiece side_piece_right = GetPieceAtPosition(new BoardPosition(pawn_pos.Row, pawn_pos.Col + 1));
                            ChessMove passant_trigger_move_right = new ChessMove(new BoardPosition(pawn_pos.Row - 2, pawn_pos.Col + 1),
                                new BoardPosition(pawn_pos.Row, pawn_pos.Col + 1), ChessMoveType.Normal);
                            if (!(side_piece_right.Player.Equals(CurrentPlayer)) && side_piece_right.PieceType.Equals(ChessPieceType.Pawn)
                                && last_move.Equals(passant_trigger_move_right))
                            {
                                BoardPosition passant_attack = new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col + 1);
                                possible_moves_total.Add(new ChessMove(pawn_pos, passant_attack, ChessMoveType.EnPassant));
                            }
                        }
                    }
                    if (pawn_pos.Row.Equals(6) && front_pos_1 && front_pos_2)
                    {
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessMoveType.Normal));
                        if (pawn_pos.Row - 2 > 0)
                            possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 2, pawn_pos.Col), ChessMoveType.Normal));
                    }
                    else if (pawn_pos.Row.Equals(6) && front_pos_1)
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessMoveType.Normal));
                    else if (front_pos_1 && !pawn_pos.Row.Equals(1))
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessMoveType.Normal));
                    else if (pawn_pos.Row == 1 && front_pos_1)
                    {
                        //possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessPieceType.Queen, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessPieceType.Knight, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row - 1, pawn_pos.Col), ChessPieceType.Rook, ChessMoveType.PawnPromote));
                    }

                    foreach (BoardPosition attack_pos in ChessMove.GetPawnMoves(pawn_pos, this))
                    {
                        if (GetPlayerAtPosition(attack_pos).Equals(enemy_player)&&!pawn_pos.Row.Equals(1))
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessMoveType.Normal));
                        else if (GetPlayerAtPosition(attack_pos).Equals(enemy_player) && pawn_pos.Row.Equals(1))
                        {
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Queen, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Rook, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Knight, ChessMoveType.PawnPromote));
                            /*  ChessMove chessMove = new ChessMove(pawn_pos, attack_pos, ChessPieceType.Queen, ChessMoveType.PawnPromote);

                              if (chessMove.promoteTo.Equals(ChessPieceType.Empty))
                              {
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Rook, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Knight, ChessMoveType.PawnPromote));
                              }

                              else
                              {
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Queen, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Rook, ChessMoveType.PawnPromote));
                                  possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Knight, ChessMoveType.PawnPromote));
                              }*/
                        }

                    }
                }
            }
            else
            {
                foreach (BoardPosition pawn_pos in GetPositionsOfPiece(ChessPieceType.Pawn, 2))
                {
                    bool front_pos_1 = GetPieceAtPosition(new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col)).PieceType.Equals(ChessPieceType.Empty);
                    bool front_pos_2 = false;
                    if (pawn_pos.Row+2<8)
                        front_pos_2 = GetPieceAtPosition(new BoardPosition(pawn_pos.Row + 2, pawn_pos.Col)).PieceType.Equals(ChessPieceType.Empty);

                    if (mMoveHistory.Count > 0)
                    {
                        ChessMove last_move = mMoveHistory[mMoveHistory.Count - 1];
                        // en passant movement left black player capturing white pawn
                        if (pawn_pos.Col > 0)
                        {
                            ChessPiece side_piece_left = GetPieceAtPosition(new BoardPosition(pawn_pos.Row, pawn_pos.Col - 1));
                            ChessMove passant_trigger_move_left = new ChessMove(new BoardPosition(pawn_pos.Row + 2, pawn_pos.Col - 1),
                                new BoardPosition(pawn_pos.Row, pawn_pos.Col - 1), ChessMoveType.Normal);
                            if (!(side_piece_left.Player.Equals(CurrentPlayer)) && side_piece_left.PieceType.Equals(ChessPieceType.Pawn)
                                && last_move.Equals(passant_trigger_move_left))
                            {
                                BoardPosition passant_attack = new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col - 1);
                                possible_moves_total.Add(new ChessMove(pawn_pos, passant_attack, ChessMoveType.EnPassant));
                            }
                        }
                        // en passant movement right of black player capturing white pawn
                        if (pawn_pos.Col < 7)
                        {
                            ChessPiece side_piece_right = GetPieceAtPosition(new BoardPosition(pawn_pos.Row, pawn_pos.Col + 1));
                            ChessMove passant_trigger_move_right = new ChessMove(new BoardPosition(pawn_pos.Row + 2, pawn_pos.Col + 1),
                                new BoardPosition(pawn_pos.Row, pawn_pos.Col + 1), ChessMoveType.Normal);

                            if (!(side_piece_right.Player.Equals(CurrentPlayer)) && side_piece_right.PieceType.Equals(ChessPieceType.Pawn)
                                && last_move.Equals(passant_trigger_move_right))
                            {
                                BoardPosition passant_attack = new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col + 1);
                                possible_moves_total.Add(new ChessMove(pawn_pos, passant_attack, ChessMoveType.EnPassant));
                            }
                        }
                    }
                    if (pawn_pos.Row.Equals(1) && front_pos_1 && front_pos_2)
                    {
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessMoveType.Normal));
                        if (pawn_pos.Row + 2 < 8)
                            possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 2, pawn_pos.Col), ChessMoveType.Normal));
                    }
                    else if (pawn_pos.Row.Equals(1) && front_pos_1)
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessMoveType.Normal));
                    else if (front_pos_1 && !pawn_pos.Row.Equals(6))
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessMoveType.Normal));
                    else if (pawn_pos.Row == 6)
                    {

                        //possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessPieceType.Queen, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessPieceType.Knight, ChessMoveType.PawnPromote));
                        possible_moves_total.Add(new ChessMove(pawn_pos, new BoardPosition(pawn_pos.Row + 1, pawn_pos.Col), ChessPieceType.Rook, ChessMoveType.PawnPromote));
                    }
                    foreach (BoardPosition attack_pos in ChessMove.GetPawnMoves(pawn_pos, this))
                    {
                        if (GetPlayerAtPosition(attack_pos).Equals(enemy_player)&& !pawn_pos.Row.Equals(6))
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessMoveType.Normal));
                        else if (GetPlayerAtPosition(attack_pos).Equals(enemy_player) && pawn_pos.Row.Equals(6)) {
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessMoveType.PawnPromote));
                            //possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Queen, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Bishop, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Rook, ChessMoveType.PawnPromote));
                            possible_moves_total.Add(new ChessMove(pawn_pos, attack_pos, ChessPieceType.Knight, ChessMoveType.PawnPromote));
                        }
                            
                    }
                }

            }
            BoardPosition white_king_position = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();
            if (CurrentPlayer == 1 &&white_king_position.Equals(new BoardPosition(7,4 )))
            {
                //WHITE KING SIDE CASTLING  
                ChessPiece right_one_piece = GetPieceAtPosition(new BoardPosition(white_king_position.Row, white_king_position.Col + 1));
                ChessPiece right_two_piece = GetPieceAtPosition(new BoardPosition(white_king_position.Row, white_king_position.Col + 2));
                BoardPosition right_one_pos = new BoardPosition(white_king_position.Row, white_king_position.Col + 1);
                BoardPosition right_two_pos = new BoardPosition(white_king_position.Row, white_king_position.Col + 2);

                if (right_one_piece.PieceType.Equals(ChessPieceType.Empty) && right_two_piece.PieceType.Equals(ChessPieceType.Empty) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].white_right_rook) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].white_king) && !(IsCheck) &&
                                                    !PositionIsAttacked(right_one_pos, enemy_player) && !PositionIsAttacked(right_two_pos, enemy_player)&&
                                                    GetPieceAtPosition(new BoardPosition(7, 7)).PieceType.Equals(ChessPieceType.Rook))
                {
                    ChessMove king_side_castle_move = new ChessMove(white_king_position, right_two_pos, ChessMoveType.CastleKingSide);
                    possible_moves_total.Add(king_side_castle_move);
                }

                //WHITE QUEEN SIDE CASTLING
                ChessPiece left_one_piece = GetPieceAtPosition(new BoardPosition(white_king_position.Row, white_king_position.Col - 1));
                ChessPiece left_two_piece = GetPieceAtPosition(new BoardPosition(white_king_position.Row, white_king_position.Col - 2));
                ChessPiece left_three_piece = GetPieceAtPosition(new BoardPosition(7,1));

                BoardPosition left_one_pos = new BoardPosition(white_king_position.Row, white_king_position.Col - 1);
                
                BoardPosition left_two_pos = new BoardPosition(white_king_position.Row, white_king_position.Col - 2);
                BoardPosition left_three_pos = new BoardPosition(white_king_position.Row, white_king_position.Col - 3);


                if (left_one_piece.PieceType.Equals(ChessPieceType.Empty) && left_two_piece.PieceType.Equals(ChessPieceType.Empty) &&
                                                    left_three_piece.PieceType.Equals(ChessPieceType.Empty) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].white_left_rook) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].white_king) && !(IsCheck) &&
                                                    !PositionIsAttacked(left_one_pos, enemy_player) && !PositionIsAttacked(left_two_pos, enemy_player) &&
                                                    //!PositionIsAttacked(left_three_pos, enemy_player)&&
                                                     GetPieceAtPosition(new BoardPosition(7, 0)).PieceType.Equals(ChessPieceType.Rook))

                {
                    ChessMove queen_side_castle_move = new ChessMove(white_king_position, left_two_pos, ChessMoveType.CastleQueenSide);
                    possible_moves_total.Add(queen_side_castle_move);
                }

            }
           
            else if (CurrentPlayer==2&& GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First().Equals(new BoardPosition(0,4)))   
            {
                //BLACK KING SIDE CASTLING// left = queen side=right
                BoardPosition black_king_position = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();
                ChessPiece b_right_one_piece = GetPieceAtPosition(new BoardPosition(black_king_position.Row, black_king_position.Col + 1));
                ChessPiece b_right_two_piece = GetPieceAtPosition(new BoardPosition(black_king_position.Row, black_king_position.Col + 2));
                BoardPosition b_right_one_pos = new BoardPosition(black_king_position.Row, black_king_position.Col + 1);
                BoardPosition b_right_two_pos = new BoardPosition(black_king_position.Row, black_king_position.Col + 2);

                if (b_right_one_piece.PieceType.Equals(ChessPieceType.Empty)
                    && b_right_two_piece.PieceType.Equals(ChessPieceType.Empty) &&
                   !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].black_right_rook) &&
                   !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].black_king) && !(IsCheck) &&
                   !PositionIsAttacked(b_right_one_pos, enemy_player) && !PositionIsAttacked(b_right_two_pos, enemy_player)
                   && GetPieceAtPosition(new BoardPosition(0, 7)).PieceType.Equals(ChessPieceType.Rook))
                {
                    ChessMove b_king_side_castle_move = new ChessMove(black_king_position, b_right_two_pos, ChessMoveType.CastleKingSide);
                    possible_moves_total.Add(b_king_side_castle_move);
                }

                //BLACK QUEEN SIDE CASTLING
                ChessPiece b_left_one_piece = GetPieceAtPosition(new BoardPosition(black_king_position.Row, black_king_position.Col - 1));
                ChessPiece b_left_two_piece = GetPieceAtPosition(new BoardPosition(black_king_position.Row, black_king_position.Col - 2));
                ChessPiece b_left_three_piece = GetPieceAtPosition(new BoardPosition(black_king_position.Row, black_king_position.Col - 3));
                BoardPosition b_left_one_pos = new BoardPosition(black_king_position.Row, black_king_position.Col - 1);
                BoardPosition b_left_two_pos = new BoardPosition(black_king_position.Row, black_king_position.Col - 2);
                BoardPosition b_left_three_pos = new BoardPosition(black_king_position.Row, black_king_position.Col - 3);

                if (b_left_one_piece.PieceType.Equals(ChessPieceType.Empty) && b_left_two_piece.PieceType.Equals(ChessPieceType.Empty) &&
                                                     b_left_three_piece.PieceType.Equals(ChessPieceType.Empty) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].black_left_rook) &&
                                                    !(ListCurrentGameStates[ListCurrentGameStates.Count - 1].black_king) && !(IsCheck) &&
                                                    !PositionIsAttacked(b_left_one_pos, enemy_player) && !PositionIsAttacked(b_left_two_pos, enemy_player) &&
                                                    //!PositionIsAttacked(b_left_three_pos, enemy_player)&&
                                                    GetPieceAtPosition(new BoardPosition(0, 0)).PieceType.Equals(ChessPieceType.Rook))
                {
                    ChessMove b_queen_side_castle_move = new ChessMove(black_king_position, b_left_two_pos, ChessMoveType.CastleQueenSide);
                    possible_moves_total.Add(b_queen_side_castle_move);
                }
            }
            // king normal move
            
            BoardPosition king_pos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).First();
            foreach (BoardPosition attack_position in ChessMove.GetMoves4Piece(king_pos, this))
            {
                bool isEnemy_and_not_attacked = !PositionIsAttacked(attack_position, enemy_player) && !GetPlayerAtPosition(attack_position).Equals(CurrentPlayer);
                bool isEmpty_and_no_attacked = PositionIsEmpty(attack_position) && !PositionIsAttacked(attack_position, enemy_player);
                if (isEnemy_and_not_attacked|| isEmpty_and_no_attacked)
                    possible_moves_total.Add(new ChessMove(king_pos, attack_position, ChessMoveType.Normal));
            }

            List<ChessMove> final_moves = new List<ChessMove>();
            foreach (ChessMove possible_move in possible_moves_total)
            {
                ApplyMove(possible_move);
                if (!IsEnemyCheck)
                    final_moves.Add(possible_move);
                UndoLastMove();
            }
            CurrentGameState currentgame = ListCurrentGameStates[ListCurrentGameStates.Count() - 1];
            if (IsCheck && final_moves.Count() == 0)
                currentgame.checkmate = true;

            ListCurrentGameStates.RemoveAt(ListCurrentGameStates.Count() - 1);
            ListCurrentGameStates.Add(currentgame);
            //final_moves.Add(new ChessMove(new BoardPosition(7, 4), new BoardPosition(7, 2)));
            return final_moves;
        }

        public int ValueOfPiece(ChessPiece piece)
        {
            if (piece.PieceType.Equals(ChessPieceType.Pawn))
                return 1;
            else if (piece.PieceType.Equals(ChessPieceType.Knight)|| piece.PieceType.Equals(ChessPieceType.Bishop))
                return 3;
            else if (piece.PieceType.Equals(ChessPieceType.Rook))
                return 5;
            else if (piece.PieceType.Equals(ChessPieceType.Queen))
                return 9;
            else
                return 0;
        }

        public void ApplyMove(ChessMove m)
        {
            // STRONG RECOMMENDATION: any mutation to the board state should be run
            // through the method SetPieceAtPosition.

            BoardPosition start = m.StartPosition;
            BoardPosition end = m.EndPosition;
            ChessPiece moving_piece = GetPieceAtPosition(start);
            ChessPiece target_piece = GetPieceAtPosition(end);
            SetPieceAtPosition(start, new ChessPiece(ChessPieceType.Empty, 0));
            SetPieceAtPosition(end, moving_piece);
            CurrentGameState nCurrentGame;

            if (ListCurrentGameStates.Count == 0)
                nCurrentGame = currentGame;
            else
                nCurrentGame = ListCurrentGameStates[ListCurrentGameStates.Count - 1];

            if (m.MoveType.Equals(ChessMoveType.EnPassant) && CurrentPlayer == 1)
            {
                SetPieceAtPosition(new BoardPosition(end.Row + 1, end.Col), ChessPiece.Empty);
                nCurrentGame.advantage += 1;
                nCurrentGame.draw_counter = 0;

            }
            else if (m.MoveType.Equals(ChessMoveType.EnPassant) && CurrentPlayer == 2)
            {
                SetPieceAtPosition(new BoardPosition(end.Row - 1, end.Col), ChessPiece.Empty);
                nCurrentGame.advantage -= 1;
                nCurrentGame.draw_counter = 0;
            }

            //Castling
            if (m.MoveType.Equals(ChessMoveType.CastleKingSide)&& CurrentPlayer == 1)
            {
                //ChessPiece white_rook_right = GetPieceAtPosition(new BoardPosition(7, 7));
                ChessPiece white_rook_right = new ChessPiece(ChessPieceType.Rook, 1);
                SetPieceAtPosition(new BoardPosition(7, 5), white_rook_right);
                SetPieceAtPosition(new BoardPosition(7, 7), ChessPiece.Empty);
            }else if (m.MoveType.Equals(ChessMoveType.CastleQueenSide) && CurrentPlayer == 1)
            {
                //ChessPiece white_rook_left = GetPieceAtPosition(new BoardPosition(7, 0));
                ChessPiece white_rook_left = new ChessPiece(ChessPieceType.Rook, 1);
                SetPieceAtPosition(new BoardPosition(7, 3), white_rook_left);
                SetPieceAtPosition(new BoardPosition(7, 0), ChessPiece.Empty);
            }else if (m.MoveType.Equals(ChessMoveType.CastleKingSide) && CurrentPlayer == 2) {
                ChessPiece black_rook_right = new ChessPiece(ChessPieceType.Rook, 2);
                SetPieceAtPosition(new BoardPosition(0, 5), black_rook_right);
                SetPieceAtPosition(new BoardPosition(0, 7), ChessPiece.Empty);

            }
            else if (m.MoveType.Equals(ChessMoveType.CastleQueenSide) && CurrentPlayer == 2)
            {
                ChessPiece black_rook_left = new ChessPiece(ChessPieceType.Rook, 2);
                SetPieceAtPosition(new BoardPosition(0, 3), black_rook_left);
                SetPieceAtPosition(new BoardPosition(0, 0), ChessPiece.Empty);

            }

            //pawn promotion
            if (m.MoveType.Equals(ChessMoveType.PawnPromote))
            {
                ChessPiece promotion;
                if (m.promoteTo.Equals(ChessPieceType.Empty))
                    promotion = new ChessPiece(ChessPieceType.Queen, CurrentPlayer);
                else 
                    promotion = new ChessPiece(m.promoteTo, CurrentPlayer);

                SetPieceAtPosition(end, promotion);
                if (CurrentPlayer.Equals(1))
                    nCurrentGame.advantage += ValueOfPiece(promotion) - 1;
                else
                    nCurrentGame.advantage += ValueOfPiece(promotion) + 1;
                nCurrentGame.promoted_piece = promotion;
            }

            // if piece at end position is empty the draw counter is increased by 1
            if (target_piece.PieceType.Equals(ChessPieceType.Empty) && !moving_piece.PieceType.Equals(ChessPieceType.Pawn))
                nCurrentGame.draw_counter += 1;
            else
                nCurrentGame.draw_counter = 0;

            if (GetPlayerAtPosition(end).Equals(1))
                nCurrentGame.advantage += ValueOfPiece(target_piece);
            else
                nCurrentGame.advantage -= ValueOfPiece(target_piece);


            // puts piece at end position in the piece captured field of the struct
            nCurrentGame.piece_captured = target_piece;
            nCurrentGame = CheckCastling(moving_piece, m, nCurrentGame);
            ListCurrentGameStates.Add(nCurrentGame);
            mMoveHistory.Add(m);
        }

        public void UndoLastMove()
        {
            if (mMoveHistory.Count() == 0)
                throw new System.InvalidOperationException();
            ChessMove lastMove = mMoveHistory[mMoveHistory.Count - 1];
            BoardPosition startPosition = lastMove.StartPosition;
            BoardPosition endPosition = lastMove.EndPosition;
            ChessMoveType moveType = lastMove.MoveType;
            CurrentGameState nCurrentGame;
            nCurrentGame = ListCurrentGameStates[ListCurrentGameStates.Count - 1];

            if (GetPlayerAtPosition(startPosition).Equals(1))
                nCurrentGame.advantage -= ValueOfPiece(GetPieceAtPosition(endPosition));
            else
                nCurrentGame.advantage += ValueOfPiece(GetPieceAtPosition(endPosition));
            SetPieceAtPosition(startPosition, GetPieceAtPosition(endPosition));
            SetPieceAtPosition(endPosition, ListCurrentGameStates[ListCurrentGameStates.Count - 1].piece_captured);

            //update draw counter
            if (GetPieceAtPosition(startPosition).PieceType.Equals(ChessPieceType.Pawn) || !GetPieceAtPosition(endPosition).Equals(ChessPieceType.Empty))
                nCurrentGame.draw_counter = ListCurrentGameStates[ListCurrentGameStates.Count - 2].draw_counter;
            else
                nCurrentGame.draw_counter -= 1;

            mMoveHistory.RemoveAt(mMoveHistory.Count - 1);
            ListCurrentGameStates.RemoveAt(ListCurrentGameStates.Count - 1);
                

            // Undo castling in general
            if (moveType.Equals(ChessMoveType.CastleKingSide))
            {
                int row = (CurrentPlayer == 1) ? 7 : 0;
                ChessPiece rook_right = new ChessPiece(ChessPieceType.Rook, CurrentPlayer);
                SetPieceAtPosition(new BoardPosition(row, 5), ChessPiece.Empty);
                SetPieceAtPosition(new BoardPosition(row, 7), rook_right);

            }
            else if (moveType.Equals(ChessMoveType.CastleQueenSide))
            {
                int row = (CurrentPlayer == 1) ? 7 : 0;
                ChessPiece rook_left = new ChessPiece(ChessPieceType.Rook, CurrentPlayer);
                SetPieceAtPosition(new BoardPosition(row, 3), ChessPiece.Empty);
                SetPieceAtPosition(new BoardPosition(row, 0), rook_left);
            }    
            //undo en passant
           else if (moveType.Equals(ChessMoveType.EnPassant))
            {
                int capture_direction = ((endPosition.Col - startPosition.Col) > 0) ? 1 : -1;
                int pawn_enemy_player = (CurrentPlayer == 1) ? 2 : 1;
                ChessPiece enemy_pawn = new ChessPiece(ChessPieceType.Pawn, pawn_enemy_player);
                SetPieceAtPosition(new BoardPosition(startPosition.Row, startPosition.Col + capture_direction), enemy_pawn);
            }

           else if (moveType.Equals(ChessMoveType.PawnPromote))
            {
                
                ChessPiece pawn= new ChessPiece(ChessPieceType.Pawn, CurrentPlayer);
                SetPieceAtPosition(startPosition, pawn);
                if (CurrentPlayer == 1)
                    nCurrentGame.advantage -= ValueOfPiece(GetPieceAtPosition(startPosition)) + 1;              
                else  
                    nCurrentGame.advantage += ValueOfPiece(GetPieceAtPosition(startPosition)) - 1;
            }
        }

        /// <summary>
        /// Returns whatever chess piece is occupying the given position.
        /// </summary>
        public ChessPiece GetPieceAtPosition(BoardPosition position)
        {

            int a = position.Row * 4;
            int b = (position.Col / 2);
            byte byte_at_pos = Cboard[a + b];

            int piece_val = (position.Col % 2 == 0) ? byte_at_pos >> 4 : (byte_at_pos & 0b1111);
            if (piece_val == 0)
                return new ChessPiece((ChessPieceType)0, 0);
            else if (piece_val < 7)
                return new ChessPiece((ChessPieceType)piece_val, 1);
            //technically the las two lanes should be wrapped in the else statement. However, the result is the same
            else
                piece_val -= 8;
                return new ChessPiece((ChessPieceType)piece_val, 2);
        }

        /// <summary>
        /// Returns whatever player is occupying the given position.
        /// </summary>
        public int GetPlayerAtPosition(BoardPosition pos) => GetPieceAtPosition(pos).Player;

        /// <summary>
        /// Returns true if the given position on the board is empty.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEmpty(BoardPosition pos)
        {
            if (!PositionInBounds(pos))
                return false;
            return GetPieceAtPosition(pos).PieceType.Equals(ChessPieceType.Empty);
        }

        /// <summary>
        /// Returns true if the given position contains a piece that is the enemy of the given player.
        /// </summary>
        /// <remarks>returns false if the position is not in bounds</remarks>
        public bool PositionIsEnemy(BoardPosition pos, int player)
        {
            if (!PositionInBounds(pos) || GetPieceAtPosition(pos).Player.Equals(0))
                return false;
            return !GetPieceAtPosition(pos).Player.Equals(player);
        }

        /// <summary>
        /// Returns true if the given position is in the bounds of the board.
        /// </summary>
        public static bool PositionInBounds(BoardPosition pos)=> ((pos.Col >= 0 && pos.Col < 8) && (pos.Row >= 0 && pos.Row < 8));

        /// <summary>
        /// Returns all board positions where the given piece can be found.
        /// </summary>
        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType pieceType, int player)
        {
            List<BoardPosition> positions_of_piece = new List<BoardPosition>();
            BoardPosition boardPosition;
            ChessPiece chessPiece;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    boardPosition = new BoardPosition(i, j);
                    chessPiece = GetPieceAtPosition(boardPosition);
                    if (chessPiece.PieceType == pieceType && chessPiece.Player == player)
                        positions_of_piece.Add(boardPosition);
                }
            return (IEnumerable<BoardPosition>)positions_of_piece;

        }

        /// <summary>
        /// Returns true if the given player's pieces are attacking the given position.
        /// </summary>
        public bool PositionIsAttacked(BoardPosition position, int byPlayer)
        {
            if (!PositionInBounds(position))
                return false;
            return GetAttackedPositions(byPlayer).Contains(position);
        }

        /// <summary>
        /// Returns a set of all BoardPositions that are attacked by the given player.
        /// </summary>
        public ISet<BoardPosition> GetAttackedPositions(int byPlayer)
        {
            List<BoardPosition> list_attack_position = new List<BoardPosition>();
            for (int i = 0; i < Cboard.Length * 2; i++)
            {
                int column = i % 8;
                int row = i / 8;
                BoardPosition curr = new BoardPosition(row, column);
                if (GetPieceAtPosition(curr).Player == byPlayer)
                {
                    IEnumerable<BoardPosition> attack_positions = ChessMove.GetMoves4Piece(curr, this);
                    foreach (BoardPosition current_pos in attack_positions)
                        list_attack_position.Add(current_pos);
                }
            }
            ISet<BoardPosition> ans = new HashSet<BoardPosition>(list_attack_position);
            return ans;
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Mutates the board state so that the given piece is at the given position.
        /// </summary>
        private void SetPieceAtPosition(BoardPosition position, ChessPiece piece)
        {
            int a = position.Row * 4;
            int b = (position.Col / 2);
            byte byte_at_pos = Cboard[a + b];
            int base_to_power = (piece.Player == 1 || piece.Player == 0) ? 0 : 1;

            byte int_val = (byte)((base_to_power * 8) + (int)piece.PieceType);

            if (position.Col % 2 == 0)
            {
                int_val = (byte)(int_val << 4);
                byte_at_pos = (byte)(byte_at_pos & 0b1111);
            }
            else
                byte_at_pos = (byte)(byte_at_pos & 0b11110000);

            Cboard[a + b] = (byte)(int_val | byte_at_pos);
        }

        #endregion

        #region Explicit IGameBoard implementations.
        IEnumerable<IGameMove> IGameBoard.GetPossibleMoves()=> GetPossibleMoves();
        void IGameBoard.ApplyMove(IGameMove m) =>ApplyMove(m as ChessMove);      
        IReadOnlyList<IGameMove> IGameBoard.MoveHistory => mMoveHistory;
        // You will need to change this later.
        public long BoardWeight => CurrentAdvantage.Player == 1 ?
            CurrentAdvantage.Advantage : -CurrentAdvantage.Advantage;
        #endregion

        // You may or may not need to add code to this constructor.
        public ChessBoard()
        {
            BoardPosition boardPosition;
            ChessPiece chessPiece;
            string[] pieces = { "010", "011", "100", "101", "110", "100", "011", "010" };
            string pawnPiece = "001";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i == 0)
                    {
                        boardPosition = new BoardPosition(i, j);
                        string a = pieces[j];
                        chessPiece = new ChessPiece((ChessPieceType)Convert.ToInt32(a, 2), 2);
                        SetPieceAtPosition(boardPosition, chessPiece);
                    }

                    else if (i == 1)
                    {
                        boardPosition = new BoardPosition(i, j);
                        chessPiece = new ChessPiece((ChessPieceType)Convert.ToInt32(pawnPiece, 2), 2);
                        SetPieceAtPosition(boardPosition, chessPiece);
                    }
                    else if (i == 6)
                    {
                        boardPosition = new BoardPosition(i, j);
                        chessPiece = new ChessPiece((ChessPieceType)Convert.ToInt32(pawnPiece, 2), 1);
                        SetPieceAtPosition(boardPosition, chessPiece);
                    }
                    else if (i == 7)
                    {
                        boardPosition = new BoardPosition(i, j);
                        chessPiece = new ChessPiece((ChessPieceType)Convert.ToInt32(pieces[j], 2), 1);
                        SetPieceAtPosition(boardPosition, chessPiece);
                    }
                }
            }
        }

        public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiece>> startingPositions)
            : this()
        {
            var king1 = startingPositions.Where(t => t.Item2.Player == 1 && t.Item2.PieceType == ChessPieceType.King);
            var king2 = startingPositions.Where(t => t.Item2.Player == 2 && t.Item2.PieceType == ChessPieceType.King);
            if (king1.Count() != 1 || king2.Count() != 1)
                throw new ArgumentException("A chess board must have a single king for each player");

            foreach (var position in BoardPosition.GetRectangularPositions(8, 8))
                SetPieceAtPosition(position, ChessPiece.Empty);

            int[] values = { 0, 0 };
            foreach (var pos in startingPositions)
                SetPieceAtPosition(pos.Item1, pos.Item2);

            int value_player_1 = 0;
            int value_player_2 = 0;

            value_player_1 += GetPositionsOfPiece(ChessPieceType.Pawn, 1).Count();
            value_player_1 += 5 * GetPositionsOfPiece(ChessPieceType.Rook, 1).Count();
            value_player_1 += 3 * GetPositionsOfPiece(ChessPieceType.Knight, 1).Count();
            value_player_1 += 3 * GetPositionsOfPiece(ChessPieceType.Bishop, 1).Count();
            value_player_1 += 9 * GetPositionsOfPiece(ChessPieceType.Queen, 1).Count();

            value_player_2 += GetPositionsOfPiece(ChessPieceType.Pawn, 2).Count();
            value_player_2 += 5 * GetPositionsOfPiece(ChessPieceType.Rook, 2).Count();
            value_player_2 += 3 * GetPositionsOfPiece(ChessPieceType.Knight, 2).Count();
            value_player_2 += 3 * GetPositionsOfPiece(ChessPieceType.Bishop, 2).Count();
            value_player_2 += 9 * GetPositionsOfPiece(ChessPieceType.Queen, 2).Count();

            CurrentGameState currentGame = ListCurrentGameStates[ListCurrentGameStates.Count - 1];
            currentGame.advantage = value_player_1 - value_player_2;
            ListCurrentGameStates.RemoveAt(ListCurrentGameStates.Count - 1);
            ListCurrentGameStates.Add(currentGame);

        }
        public CurrentGameState CheckCastling(ChessPiece moving, ChessMove m, CurrentGameState currentGame)
        {
            BoardPosition start = m.StartPosition;
            // checks Castling\
            // check if user want king to move from e1 to g1, or the other side
            //If so, check flags for rook has moved. If everything is fine, apply the moves of king and rook
            if (CurrentPlayer == 1)
            {
                if (moving.PieceType.Equals(ChessPieceType.Rook) && start.Equals(new BoardPosition(7, 0)))
                    currentGame.white_left_rook = true;
                else if (moving.PieceType.Equals(ChessPieceType.Rook) && start.Equals(new BoardPosition(7, 7)))
                    currentGame.white_right_rook = true;
                else if (moving.PieceType.Equals(ChessPieceType.King) && start.Equals(new BoardPosition(7, 4)))
                    currentGame.white_king = true;
            }
            else
            {
                if (moving.PieceType.Equals(ChessPieceType.Rook) && start.Equals(new BoardPosition(0, 0)))
                    currentGame.black_left_rook = true;
                else if (moving.PieceType.Equals(ChessPieceType.Rook) && start.Equals(new BoardPosition(0, 7)))
                    currentGame.black_right_rook = true;
                else if (moving.PieceType.Equals(ChessPieceType.King) && start.Equals(new BoardPosition(0, 4)))
                    currentGame.black_king = true;
            }
            return currentGame;
        }
    }
}
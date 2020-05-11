using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Cecs475.BoardGames.ComputerOpponent;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.Chess.WpfView
{

    public class ChessSquare : INotifyPropertyChanged
    {
        private ChessPiece chess_piece;
        /// <summary>
        /// The player that has a piece in the given square, or 0 if empty.
        /// </summary>
        /// 
        public ChessPiece Chess_Piece
        {
            get { return chess_piece;}
            set
            {
                chess_piece = value;
                OnPropertyChanged(nameof(Chess_Piece));
            }
        }
        /// <summary>
        /// The position of the square.
        /// </summary>
        public BoardPosition Position
        {
            get; set;
        }

        private bool isSelected;
        private bool isHovered;
        private bool isChecked;
 
        /// <summary>
        /// Whether the square should be highlighted because of a user action.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        /// <summary>
        /// Whether the square should be highlighted because of a user action.
        /// </summary>
        public bool IsHovered
        {
            get { return isHovered; }
            set
            {
                if (value != isHovered)
                {
                    isHovered = value;
                    OnPropertyChanged(nameof(IsHovered));
                }
            }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class ChessViewModel : INotifyPropertyChanged, IGameViewModel
    {
        private ChessBoard mBoard;
        private ObservableCollection<ChessSquare> mSquares;
        public event EventHandler GameFinished;
        private ChessPiece queen;
        private ChessPiece bishop;
        private ChessPiece rook;
        private ChessPiece knight;
        private const int MAX_AI_DEPTH = 4;
        private IGameAi mGameAi = new MinimaxAi(MAX_AI_DEPTH);

        public ChessPiece Queen
        {
            get { return queen; }
            set
            {
                queen = value;
                OnPropertyChanged(nameof(Queen));
            }
        }
        public ChessPiece Bishop
        {
            get { return bishop; }
            set
            {
                bishop = value;
                OnPropertyChanged(nameof(Bishop));
            }
        }
        public ChessPiece Rook
        {
            get { return rook; }
            set
            {
                rook = value;
                OnPropertyChanged(nameof(Rook));
            }
        }
        public ChessPiece Knight
        {
            get { return knight; }
            set
            {
                knight = value;
                OnPropertyChanged(nameof(Knight));
            }
        }
        public ChessViewModel()
        {
            mBoard = new ChessBoard();
        
            // Initialize the squares objects based on the board's initial state.
            mSquares = new ObservableCollection<ChessSquare>(
                BoardPosition.GetRectangularPositions(8, 8)
                .Select(pos => new ChessSquare()
                {
                    Position = pos,
                    Chess_Piece = mBoard.GetPieceAtPosition(pos),
                    IsChecked = false
                })
            );
          
         PossibleMoves = new HashSet<ChessMove>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m
            );


        }

        /// <summary>
        /// Applies a move for the current player at the given position.
        /// </summary>
        public async Task ApplyMove(ChessMove cmove)
        {
            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
            // Validate the move as possible.
            if (possMoves.Contains(cmove))
                mBoard.ApplyMove(cmove);

            if (Players == NumberOfPlayers.One && !mBoard.IsFinished)
            {
                var bestMove =  Task.Run(()=> { return mGameAi.FindBestMove(mBoard); } );
                var temp = await bestMove;
                if (bestMove != null)
                {
                    
                    mBoard.ApplyMove(temp as ChessMove);
                }
            }
            RebindState();
            if (mBoard.IsFinished)
            {
                GameFinished?.Invoke(this, new EventArgs());
            }
           
        }


        private void RebindState()
        {
            Queen = new ChessPiece(ChessPieceType.Queen, CurrentPlayer);
            Bishop = new ChessPiece(ChessPieceType.Bishop, CurrentPlayer);
            Knight = new ChessPiece(ChessPieceType.Knight, CurrentPlayer);
            Rook = new ChessPiece(ChessPieceType.Rook, CurrentPlayer);
            // Rebind the possible moves, now that the board has changed.
            PossibleMoves = new HashSet<ChessMove>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m
            );

            // Update the collection of squares by examining the new board state.
            var newSquares = BoardPosition.GetRectangularPositions(8, 8);
            int i = 0;
            foreach (var pos in newSquares)
            {
                mSquares[i].Chess_Piece = mBoard.GetPieceAtPosition(pos);
                i++;
            }
            foreach (var square in mSquares)
            {
                ChessPiece square_piece = square.Chess_Piece;
                square.IsChecked = false;
                if (square_piece.PieceType.Equals(ChessPieceType.King) && square_piece.Player == mBoard.CurrentPlayer && mBoard.IsCheck)
                    square.IsChecked = true;

            }
            OnPropertyChanged(nameof(BoardAdvantage));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
        }

        /// <summary>
        /// A collection of 64 OthelloSquare objects representing the state of the 
        /// game board.
        /// </summary>
        public ObservableCollection<ChessSquare> Squares
        {
            get { return mSquares; }
        }

        /// <summary>
        /// A set of board positions where the current player can move.
        /// </summary>
        public HashSet<ChessMove> PossibleMoves
        {
            get; private set;
        }

        /// <summary>
        /// The player whose turn it currently is.
        /// </summary>
        public int CurrentPlayer
        {
            get { return mBoard.CurrentPlayer; }
        }


        /// <summary>
        /// The value of the othello board.
        /// </summary>
        public long BoardWeight => mBoard.BoardWeight;
        public GameAdvantage BoardAdvantage => mBoard.CurrentAdvantage;

        public bool CanUndo => mBoard.MoveHistory.Any();

        public NumberOfPlayers Players { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void UndoMove()
        {
            if (CanUndo)
            {
                mBoard.UndoLastMove();

                if (Players == NumberOfPlayers.One && CanUndo)
                    mBoard.UndoLastMove();
                RebindState();
            }
        }

    }
}

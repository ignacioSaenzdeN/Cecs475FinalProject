using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.WpfView
{
    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    /// 
    public partial class ChessView : UserControl, IWpfGameView
    {
        private ChessSquare selected_square = new ChessSquare();
        private ChessSquare hovered_square = new ChessSquare();
        public ChessView()
        {
            InitializeComponent();
        }

        private void Border_LeftClickDown(object sender, MouseEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            if (selected_square != null)
                selected_square.IsSelected = false;
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            selected_square = square;
            var vm = FindResource("vm") as ChessViewModel;
            if (vm.CurrentPlayer.Equals(square.Chess_Piece.Player))
                square.IsSelected = true;
        }

        private void Border_HoverMouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            hovered_square = square;
            var vm = FindResource("vm") as ChessViewModel;
  
            foreach (var move in vm.PossibleMoves)
            {
                if (selected_square != null && move.StartPosition.Equals(selected_square.Position) && move.EndPosition.Equals(square.Position))
                    square.IsHovered = true;
                /*              else if(selected_square != null && selected_square.IsSelected == false && move.EndPosition.Equals(square.Position))
                                  square.IsHovered = true;*/
               
                else if(selected_square != null && selected_square.IsSelected == false && move.StartPosition.Equals(square.Position))
                    square.IsHovered = true;
            }
        }
        private void Border_HoverMouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsEnabled)
                return;
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHovered = false;
        }

        private async void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            foreach (var move in vm.PossibleMoves)
            {
                if (move.StartPosition.Equals(selected_square.Position) && move.EndPosition.Equals(hovered_square.Position))
                {
                    if (move.MoveType.Equals(ChessMoveType.PawnPromote))
                    {
                        var pawn_promote_window = new PawnPromotion(vm, move.StartPosition, move.EndPosition);
                        pawn_promote_window.Show();                                             
                        break;
                    }
                    this.IsEnabled = false;
                    await vm.ApplyMove(move);
                    this.IsEnabled = true;
                    selected_square.IsSelected = false;
                }
            }
            MessageBox.Show(vm.BoardWeight.ToString());
        }

        public ChessViewModel ChessViewModel => FindResource("vm") as ChessViewModel;

        public Control ViewControl => this;

        public IGameViewModel ViewModel => ChessViewModel;
    }
}

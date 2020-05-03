using Cecs475.BoardGames.Chess.Model;
using Cecs475.BoardGames.Model;
using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for PawnPromotion.xaml
    /// </summary>
    public partial class PawnPromotion : Window, INotifyPropertyChanged
    {
        public ChessViewModel avm;
        public BoardPosition start_;
        public BoardPosition end_;

       

        public PawnPromotion(ChessViewModel vm, BoardPosition start, BoardPosition end)
        {
            avm = vm;
            this.Resources.Add("avm", vm);
            start_ = start;
            end_ = end;
            InitializeComponent();
            //DataContext = avm;


        }
        private void button_Click(object sender, EventArgs e)
        {
            String chess_piece_num = ((Button)sender).Name.Substring(1, 1);
            ChessPieceType aChessPiece = (ChessPieceType)Int32.Parse(chess_piece_num);
            ChessMove aMove = new ChessMove(start_, end_, aChessPiece);
            avm.ApplyMove(aMove);
            for(int i = 0; i < avm.Squares.Count; i++)
            {
                if (avm.Squares[i].Position.Equals(start_))
                {
                    avm.Squares[i].IsSelected = false;
                }
            }
            this.Hide();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}

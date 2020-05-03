using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Cecs475.BoardGames.Chess.Model;

namespace Cecs475.BoardGames.Chess.WpfView
{
    public class ChessPieceImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
    
                ChessPiece c = (ChessPiece)value;
                if (!c.Equals(null))
                {
                    string src = c.ToString().ToLower().Replace(' ', '_');
                    return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.WpfView;component/Resources/" + src + ".png", UriKind.Relative));
                }
                else
                {
                    return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.WpfView;component/Resources/black_pawn.png", UriKind.Relative));
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

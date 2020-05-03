using Cecs475.BoardGames.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.WpfView
{
    class ChessSquareBackgroundConverter : IMultiValueConverter
    {
        private static SolidColorBrush SELECT_BRUSH= Brushes.Red;
        private static SolidColorBrush HOVER_BRUSH = Brushes.LightGreen;
        private static SolidColorBrush CHECK_BRUSH = Brushes.Yellow;
        private static SolidColorBrush MOVE_BRUSH = Brushes.Green;
        private static SolidColorBrush KING_CHECK = Brushes.Yellow;
        private static SolidColorBrush ALLOWED_MOVE_BRUSH = Brushes.Green;
        private static SolidColorBrush BLACK_BRUSH = Brushes.SaddleBrown;
        private static SolidColorBrush WHITE_BRUSH = Brushes.White;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
			BoardPosition pos = (BoardPosition)values[0];
			bool IsSelected = (bool)values[1];
            bool IsHovered = (bool)values[2];
            bool IsChecked = (bool)values[3];

            // Hovered squares have a specific color.
            if (IsSelected)
                return SELECT_BRUSH;
            if (IsHovered)
                return HOVER_BRUSH;
            if (IsChecked)
                return CHECK_BRUSH;
            if (pos.Row % 2 == 0 && pos.Col % 2 == 0)
                return BLACK_BRUSH;
            else if (pos.Row % 2 != 0 && pos.Col % 2 != 0)
                return BLACK_BRUSH;
            else
                return WHITE_BRUSH;      
		}

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

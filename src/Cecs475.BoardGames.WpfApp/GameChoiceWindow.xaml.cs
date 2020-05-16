using Cecs475.BoardGames.WpfView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Cecs475.BoardGames.WpfApp
{
    /// <summary>
    /// Interaction logic for GameChoiceWindow.xaml
    /// </summary>
    public partial class GameChoiceWindow : Window
    {
        public GameChoiceWindow()
        {
            Type IWpfGameFactory_Type = typeof(IWpfGameFactory);
            IEnumerable<Type> tempGameTypes = new List<Type>();
            List<IWpfGameFactory> GameTypes = new List<IWpfGameFactory>();
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\games\\";

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                var file_name = Path.GetFileNameWithoutExtension(dll);
                Assembly.Load(file_name + ", Version=\"1.0.0.0\", Culture = \"neutral\", PublicKeyToken=\"68e71c13048d452a\"");
                tempGameTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => IWpfGameFactory_Type.IsAssignableFrom(t) && t.IsClass);

            }
            foreach (var temp in tempGameTypes)
            {
                GameTypes.Add((IWpfGameFactory)Activator.CreateInstance(temp, new Object[] { }));
            }
            IWpfGameFactory[] gamest = GameTypes.ToArray();
            this.Resources["GameTypes"] = gamest;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            IWpfGameFactory gameType = b.DataContext as IWpfGameFactory;
            var gameWindow = new GameWindow(gameType,
                mHumanBtn.IsChecked.Value ? NumberOfPlayers.Two : NumberOfPlayers.One)
            {
                Title = gameType.GameName
            };
            gameWindow.Closed += GameWindow_Closed;
            gameWindow.Show();
            this.Hide();
        }

        private void GameWindow_Closed(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}

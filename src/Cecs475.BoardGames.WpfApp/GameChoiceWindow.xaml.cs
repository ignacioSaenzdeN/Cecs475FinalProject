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

namespace Cecs475.BoardGames.WpfApp {
	/// <summary>
	/// Interaction logic for GameChoiceWindow.xaml
	/// </summary>
	public partial class GameChoiceWindow : Window {
		public GameChoiceWindow() {
            Type IWpfGameFactory_Type = typeof(IWpfGameFactory);
            //List<Assembly> allAssemblies = new List<Assembly
            IEnumerable<Type> tempGameTypes = new List<Type>();
            List<IWpfGameFactory> GameTypes = new List<IWpfGameFactory>();
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\games\\";
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                Assembly.LoadFrom(dll);
                tempGameTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => IWpfGameFactory_Type.IsAssignableFrom(t) && t.IsClass);

            }
            foreach (var temp in tempGameTypes)
            {
                //temp is the more specific type of each IWpfGameFactory i.e. ChessGameFactory (types of classes that extend IWpfGameFactory)
                //proof: Console.WriteLine(temp);
                //Activator.CreateInstance Creates an instance of the specified type using the constructor that best matches the specified parameters.
                GameTypes.Add((IWpfGameFactory)Activator.CreateInstance(temp, new Object[] { }));
            }
            //Convert list GameTypes into an array to give the resources of window a sequence of objects  
            IWpfGameFactory[] gamest = GameTypes.ToArray();
            //Give resources an array with key GameTypes
            this.Resources["GameTypes"] = gamest;
            InitializeComponent();
        }

		private void Button_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			// Retrieve the game type bound to the button
			IWpfGameFactory gameType = b.DataContext as IWpfGameFactory;
            // Construct a GameWindow to play the game.
            var gameWindow = new GameWindow(gameType,
                mHumanBtn.IsChecked.Value ? NumberOfPlayers.Two : NumberOfPlayers.One)
            {
                Title = gameType.GameName
			};
			// When the GameWindow closes, we want to show this window again.
			gameWindow.Closed += GameWindow_Closed;

			// Show the GameWindow, hide the Choice window.
			gameWindow.Show();
			this.Hide();
		}

		private void GameWindow_Closed(object sender, EventArgs e) {
			this.Show();
		}
	}
}

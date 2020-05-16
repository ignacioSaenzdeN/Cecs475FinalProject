using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
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

namespace Cecs475.BoardGames.WpfApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
        }
        public async Task<Boolean> downloadFile()
        {
            var restClient = new RestClient("https://cecs475-boardamges.herokuapp.com/api/games");
            var request = new RestRequest(Method.GET);
            var response = restClient.Execute(request);
            GameInfo gameinfo = JsonConvert.DeserializeObject<GameInfo>(response.Content.Substring(1, response.Content.Length - 2));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                Console.WriteLine("Success!");
            else
                Console.WriteLine("Game not found!");

            using (WebClient wc = new WebClient())
            {
                foreach (var gameurl in gameinfo.Files)
                {
                    string path = "..\\Debug\\games\\" + gameurl.FileName;
                    Console.WriteLine(path);
                    await  wc.DownloadFileTaskAsync(
                    new System.Uri(gameurl.Url), path);
                }

            }
            return true;
        }

        async void OnLoad(object sender, RoutedEventArgs e)
        {
            var temp = downloadFile();
            var temp2 = await temp;
            GameChoiceWindow gcw = new GameChoiceWindow();
            gcw.Show();
            this.Close();
           
        }
    }
}

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
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;

namespace LivestreamerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String channel;
        private readonly String path;
        private TwitchFollowing following;
        private WebClient webClient;

        public MainWindow()
        {
            InitializeComponent();
            
            channel = StreamInputBox.Text;
            path = LivestreamerPathBox.Text;
            webClient = new WebClient();
            

            StreamInputBox.Focus();
        }

        public void test()
        {
            var url = "https://api.twitch.tv/kraken/users/fallenadvocate/follows/channels";
            string json = webClient.DownloadString(url);

            TwitchFollowing twitch = JsonConvert.DeserializeObject<TwitchFollowing>(json);
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            channel = StreamInputBox.Text;

            OpenLivestreamer();
        }

        private void StreamInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Submit_Click(this, new RoutedEventArgs());
            }
        }

        private string BuildString(string channel)
        {
            return "livestreamer.exe twitch.tv/" + channel + " source";
        }
      
        private void OpenLivestreamer()
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            
            p.StandardInput.WriteLine(BuildString(channel));
        }
    }
}

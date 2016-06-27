using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
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
        private WebClient webClient;
        private List<TwitchFollowing> twitchFollowing;

        public MainWindow()
        {
            InitializeComponent();
            
            channel = StreamInputBox.Text;
            path = LivestreamerPathBox.Text;
            webClient = new WebClient();

            twitchFollowing = new List<TwitchFollowing>();

            StreamInputBox.Focus();
        }

        public void GetFollowing()
        {
            var url = "https://api.twitch.tv/kraken/users/fallenadvocate/follows/channels";
            string json = webClient.DownloadString(url);
            TwitchFollowing followers = new TwitchFollowing();
            followers = JsonConvert.DeserializeObject<TwitchFollowing>(json);
            
            // can only grab 25 channels you're following at a time, this loops until all are grabbed
            do
            {
                twitchFollowing.Add(followers);
                json = webClient.DownloadString(followers._links.next);
                followers = JsonConvert.DeserializeObject<TwitchFollowing>(json);
            }
            while (followers.follows.Count != 0);

        }

        public void GetOnline()
        {
            foreach (var list in twitchFollowing)
            {
                for(int c = 0; c < list.follows.Count; c++)
                {
                    FollowingTextBox.Text += list.follows[c].channel.name + " " + list.follows[c].channel.game + "\n";
                }
            }
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            GetFollowing();
            GetOnline();
            //var button = sender as Button;
            //channel = StreamInputBox.Text;

            //OpenLivestreamer();
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

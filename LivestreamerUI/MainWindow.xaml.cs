using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

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
        private readonly BackgroundWorker worker;
        private List<string> onlineStreams;

        public MainWindow()
        {
            InitializeComponent();
            
            channel = StreamInputBox.Text;
            path = LivestreamerPathBox.Text;
            webClient = new WebClient();
            twitchFollowing = new List<TwitchFollowing>();
            onlineStreams = new List<string>();

            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            //LivestreamerGUI.Show();
            ShowFollowing();
        }

        public void ShowFollowing()
        {
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetFollowing();
            GetOnline();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WriteStream(onlineStreams);
        }
        
        public void GetFollowing()
        {
            try {
                var followersUrl = "https://api.twitch.tv/kraken/users/fallenadvocate/follows/channels";
                string json = webClient.DownloadString(followersUrl);
                TwitchFollowing followerData = new TwitchFollowing();
                followerData = JsonConvert.DeserializeObject<TwitchFollowing>(json);
                 
                // can only grab 25 channels you're following at a time, this loops until all are grabbed
                do
                {
                    twitchFollowing.Add(followerData);
                    json = webClient.DownloadString(followerData._links.next);
                    followerData = JsonConvert.DeserializeObject<TwitchFollowing>(json);
                } while (followerData.follows.Count != 0);

            } catch (Exception) { FollowingTextBox.AppendText("error"); }
        }

        public void GetOnline()
        {
            var streamUrl = "https://api.twitch.tv/kraken/streams/";

            foreach (var list in twitchFollowing)
            { 
                for(int c = 0; c < list.follows.Count; c++)
                {

                    string json = webClient.DownloadString(streamUrl + list.follows[c].channel.name);
                    TwitchStream streamData = new TwitchStream();
                    streamData = JsonConvert.DeserializeObject<TwitchStream>(json);

                    if (streamData.stream != null)
                    { 
                        onlineStreams.Add(list.follows[c].channel.name + " " + list.follows[c].channel.game);
                    }
                }
            }
        }

        public void WriteStream(List<string> streams)
        {
            FollowingTextBox.Document.Blocks.Clear(); 
            foreach(var stream in streams)
            {
                FollowingTextBox.AppendText(stream + "\n"); 
            }
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            OpenLivestreamer();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FollowingTextBox.SelectAll();
            FollowingTextBox.Selection.Text = "";

            ShowFollowing();
        }

        private void StreamInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
            
            p.StandardInput.WriteLine(BuildString(StreamInputBox.Text));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Windows.Threading;
using System.Linq;

namespace LivestreamerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string channel;
        private readonly String path;
        private WebClient webClient;
        private List<TwitchFollowing> twitchFollowing;
        private readonly BackgroundWorker worker;
        private List<List<string>> onlineStreams;
        private List<TwitchFollowing> online;
        private string username;

        public MainWindow()
        {
            InitializeComponent();
            
            webClient = new WebClient();
            twitchFollowing = new List<TwitchFollowing>();
            onlineStreams = new List<List<string>>();
            online = new List<TwitchFollowing>();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            ShowFollowing();
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetFollowing();
            GetOnline();
            twitchFollowing.Clear();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<List<string>> sorted = new List<List<string>>();
            var list = onlineStreams.Select(x => x).ToList();
            
        
            foreach(var item in list)
            {
                //do
                //{

                //}
            }

            //onlineStreams = onlineStreams.OrderByDescending(a => a.Max(x => a.ElementAt[x][3])).ToList();

            WriteStream(onlineStreams);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //ProgressBar.Value = e.ProgressPercentage;
        }
        
        public void ShowFollowing()
        {
            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public void GetFollowing()
        {
            Thread.Sleep(200);
            LoadXMLData();
            
            //if a username is entered, access their followers and add their data to twitchFollowing
            if (username != null) {
                try
                {
                    var followersUrl = "https://api.twitch.tv/kraken/users/" + username + "/follows/channels";
                    string json = webClient.DownloadString(followersUrl);
                    TwitchFollowing followerData = new TwitchFollowing();
                    followerData = JsonConvert.DeserializeObject<TwitchFollowing>(json);

                    worker.ReportProgress(40, null);

                    // can only grab 25 channels you're following at a time, this loops until all are grabbed
                    do
                    {
                        twitchFollowing.Add(followerData);
                        json = webClient.DownloadString(followerData._links.next);
                        followerData = JsonConvert.DeserializeObject<TwitchFollowing>(json);
                    }
                    while (followerData.follows.Count != 0);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            else
            {
                MessageBox.Show("Enter your Twitch Username, save, and then reload");
;           }

        }

        public void GetOnline()
        {
            var streamUrl = "https://api.twitch.tv/kraken/streams/";
            onlineStreams.Clear();
            
            foreach (var list in twitchFollowing)
            { 
                for(int c = 0; c < list.follows.Count; c++)
                {
                    string json = webClient.DownloadString(streamUrl + list.follows[c].channel.name);
                    TwitchStream streamData = new TwitchStream();
                    streamData = JsonConvert.DeserializeObject<TwitchStream>(json);

                    if (streamData.stream != null)
                    {
                        //onlineStreams.Add(list.follows[c].channel.display_name + " - " + list.follows[c].channel.game + " " + list.follows[c].channel.status + " " + streamData.stream.viewers + "" + " \r\n ");
                        List<string> l = new List<string>() { list.follows[c].channel.display_name, list.follows[c].channel.game, list.follows[c].channel.status, streamData.stream.viewers };
                        
                        onlineStreams.Add(l);
                    }

                    worker.ReportProgress(80, null);
                }
            }
        }

        public void WriteStream(List<List<string>> streams)
        {
            FollowingListBox.Items.Clear();

            foreach (List<string> list in streams) {
                foreach (string s in list)
                {
                    FollowingListBox.Items.Add(s);
                }
                FollowingListBox.Items.Add("\r\n");
            }
        }
        
        private void View_Click(object sender, RoutedEventArgs e)
        {
            channel = StreamInputBox.Text;
            OpenLivestreamer();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                LoadXMLData();
                FollowingListBox.Items.Clear();
                onlineStreams.Clear();
                ShowFollowing();
            }
            else
            {
                MessageBox.Show("Retry");
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();

            settings.Show();
        }
        
        private void StreamInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                View_Click(this, new RoutedEventArgs());
            }
        }

        private string BuildString()
        {
            return "livestreamer.exe twitch.tv/" + channel + " source";
        }

        private void LoadXMLData()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                if (File.Exists("data.xml"))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Data));
                    FileStream read = new FileStream("data.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                    Data data = (Data)xs.Deserialize(read);
                    
                    username = data.Username;
                }
            }));
        }

        private void LivestreamerGui_Load(object sender, EventArgs e)
        {
            LoadXMLData();
        }
      
        private void OpenLivestreamer()
        {
            Thread.Sleep(100); //without this, sometimes clicking view will randomly not work

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            
            p.StandardInput.WriteLine(BuildString());
        }
    }
}

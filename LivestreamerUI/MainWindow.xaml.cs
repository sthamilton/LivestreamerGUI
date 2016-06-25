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

namespace LivestreamerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var channel = StreamInputBox.Text;
            var path = LivestreamerPathBox.Text;

            if(button != null)
            {
                OpenLivestreamer(path, channel);
            }
        }

        private string BuildString(string channel)
        {
            return "livestreamer.exe twitch.tv/" + channel + " source";
        }

        private string BuildString(string channel, string quality)
        {
            return "livestreamer.exe twitch.tv/" + channel + " " + quality;
        }

        private void OpenLivestreamer(string path, string channel)
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

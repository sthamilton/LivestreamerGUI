using System;
using System.Windows;
using System.IO;
using System.Xml.Serialization;

namespace LivestreamerUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            LoadXMLData();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data data = new Data();
                data.LivestreamerPath = LivestreamerPathBox.Text;
                data.Username = UsernameBox.Text;
                SaveData.Save(data, "data.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Close();
        }

        private void LoadXMLData()
        {
           
            if (File.Exists("data.xml"))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Data));
                FileStream read = new FileStream("data.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
                Data data = (Data)xs.Deserialize(read);

                LivestreamerPathBox.Text = data.LivestreamerPath;
                UsernameBox.Text = data.Username;
            }
        }
    }
}

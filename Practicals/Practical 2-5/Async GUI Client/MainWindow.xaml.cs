using Business_tier;
using Library;
using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Async_GUI_Client
{
   

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BusinessServerInterface channel;
        private bool indexBoxLastChanged = true;


        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<BusinessServerInterface> serverInterface;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8101/BusinessServer";
            serverInterface = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            channel = serverInterface.CreateChannel();

            NumberEntriesBox.Text = "Database entries: " + channel.GetNumEntries().ToString();

        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string firstName = "", lastName = "";
            int balance = 0;
            uint acctNo = 0, pin = 0;

            // Search for index
            if (indexBoxLastChanged)
            {
                try
                {
                    index = Int32.Parse(IndexBox.Text);
                    var result = channel.GetValuesForEntry(index);

                    UpdateUI(result);
                }

                catch (FormatException)
                {
                    MessageBox.Show("Please enter a valid index", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Search for last name
            else
            {
                string lastname = LNameSearchBox.Text;
                try
                {
                    var result = await Task.Run(() => SearchDBForLastName(lastname));
                    UpdateUI(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    SetUIState(true);
                }

            }

        }

        private DataStruct SearchDBForLastName(string name)
        {
            return channel.SearchForLastName(name);
        }

        private void SetUIState(bool enabled)
        {
            IndexBox.IsReadOnly = !enabled;
            LNameSearchBox.IsReadOnly = !enabled;
            SearchBtn.IsEnabled = enabled;
            Progress.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateUI(DataStruct result)
        {
            FNameBox.Text = result.firstName;
            LNameBox.Text = result.lastName;
            BalBox.Text = result.balance.ToString();
            AccNoBox.Text = result.acctNo.ToString();
            PINBox.Text = result.pin.ToString("D4");
        }


        private void LNameSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            indexBoxLastChanged = false;
            SearchBtn.Content = "Search for " + LNameSearchBox.Text;

        }

        private void IndexBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            indexBoxLastChanged = true;
            SearchBtn.Content = "Search for " + IndexBox.Text;
        }
    }
}

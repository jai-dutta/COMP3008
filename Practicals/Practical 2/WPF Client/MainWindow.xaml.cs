using Business_tier;
using Grpc.Core;
using Library;
using ServerDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
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

namespace WPF_Client
{
    public delegate DataStruct DelegateSearchLastName(string searchTerm);

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

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string firstName = "", lastName = "";
            int balance = 0;
            uint acctNo = 0, pin = 0;

            

            if (indexBoxLastChanged) 
            {
                
                try
                { 
                    index = Int32.Parse(IndexBox.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter a valid index", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                channel.GetValuesForEntry(index, out firstName, out lastName, out pin, out acctNo, out balance);

                FNameBox.Text = firstName;
                LNameBox.Text = lastName;
                BalBox.Text = balance.ToString();
                AccNoBox.Text = acctNo.ToString();
                PINBox.Text = pin.ToString("D4");
            }

            if (!indexBoxLastChanged)
            {
               
                
                DelegateSearchLastName delegateSearchLastName;
                delegateSearchLastName = new DelegateSearchLastName(SearchDBForLastName);
                AsyncCallback callbackSearch = this.SearchCompletion;
                delegateSearchLastName.BeginInvoke(LNameSearchBox.Text, callbackSearch, delegateSearchLastName);

                IndexBox.IsReadOnly = true;
                LNameSearchBox.IsReadOnly = true;

                SearchBtn.IsEnabled = false;

                Progress.Visibility = Visibility.Visible;

                
            }

        }

        private DataStruct SearchDBForLastName(string lastName)
        {
            return channel.SearchForLastName(lastName);
        }

        private void SearchCompletion(IAsyncResult asyncResult)
        {

            DelegateSearchLastName delegateSearchLastName = (DelegateSearchLastName)asyncResult.AsyncState;
            AsyncResult asyncobj = (AsyncResult)asyncResult;

            if (asyncobj.EndInvokeCalled == false)
            {

                DataStruct result = delegateSearchLastName.EndInvoke(asyncobj);

                Dispatcher.Invoke(() =>
                {
                    FNameBox.Text = result.firstName;
                    LNameBox.Text = result.lastName;
                    BalBox.Text = result.balance.ToString();
                    AccNoBox.Text = result.acctNo.ToString();
                    PINBox.Text = result.pin.ToString("D4");


                    IndexBox.IsReadOnly = false;
                    LNameSearchBox.IsReadOnly = false;

                    SearchBtn.IsEnabled = true;

                    Progress.Visibility = Visibility.Collapsed;

                });
                asyncResult.AsyncWaitHandle.Close();
            }
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

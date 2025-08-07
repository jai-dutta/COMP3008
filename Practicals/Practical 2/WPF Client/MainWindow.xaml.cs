using Grpc.Core;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ServerInterface channel;
         
        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<ServerInterface> serverInterface;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            serverInterface = new ChannelFactory<ServerInterface>(tcp, URL);
            channel = serverInterface.CreateChannel();
            //Also, tell me how many entries are in the DB.
            var totalEntries = channel.GetNumEntries();

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            string firstName = "", lastName = "";
            int balance = 0;
            uint acctNo = 0, pin = 0;

            index = Int32.Parse(IndexBox.Text);

            channel.GetValuesForEntry(index, out firstName, out lastName, out pin, out acctNo, out balance);
            FNameBox.Text = firstName;
            LNameBox.Text = lastName;
            BalBox.Text = balance.ToString();
            AccNoBox.Text = acctNo.ToString();
            PINBox.Text = pin.ToString("D4");


        }
    }
}


using Newtonsoft.Json;
using RestSharp;
using System;
using System.ServiceModel;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DTOS;


namespace Client
{

    public partial class MainWindow : Window
    {
        private bool indexBoxLastChanged = true;
        private readonly RestClient _client;

        public MainWindow()
        {
            InitializeComponent();

            _client = new RestClient("http://localhost:5055/api/"); // Business REST API

            LoadNumEntriesAsync();
        }

        private async void LoadNumEntriesAsync()
        {
            try
            {
                var request = new RestRequest("entries", Method.Get);
                var response = await _client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    int entries = JsonConvert.DeserializeObject<int>(response.Content!);
                    NumberEntriesBox.Text = $"Database entries: {entries}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching number of entries: {ex.Message}");
            }
        }


        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;

            // Search for index
            if (indexBoxLastChanged)
            {
                try
                {
                    index = Int32.Parse(IndexBox.Text);
                    var response = await _client.ExecuteAsync(new RestRequest($"GetValues/{index}", Method.Get));

                    if (!response.IsSuccessful)
                    {
                        var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                        string errorMessage = errorObj["message"];
                        MessageBox.Show($"Error: {errorMessage}", response.StatusCode.ToString());
                        return;
                        return;
                    }

                    var result = JsonConvert.DeserializeObject<DataStructDto>(response.Content!);

                    UpdateUI(result!);
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
                SetUIState(false);
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

        private async Task<DataStructDto> SearchDBForLastName(string name)
        {
            SearchQueryDto searchDto = new SearchQueryDto
            {
                searchQuery = name
            };
            var response = await _client.ExecuteAsync(new RestRequest($"search", Method.Post).AddJsonBody(searchDto));
            if (!response.IsSuccessful)
            {
                throw new Exception($"{response.StatusCode}");
            }
            return JsonConvert.DeserializeObject<DataStructDto>(response.Content!)!;
        }

        private void SetUIState(bool enabled)
        {
            IndexBox.IsReadOnly = !enabled;
            LNameSearchBox.IsReadOnly = !enabled;
            SearchBtn.IsEnabled = enabled;
            Progress.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateUI(DataStructDto result)
        {
            FNameBox.Text = result.fname;
            LNameBox.Text = result.lname;
            BalBox.Text = result.bal.ToString();
            AccNoBox.Text = result.acct.ToString();
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

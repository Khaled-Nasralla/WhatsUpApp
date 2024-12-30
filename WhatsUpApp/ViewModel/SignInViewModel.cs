using System.ComponentModel;
using System.Text;
using WhatsUpApp.View;
using WhatsUpApp.Model;


namespace WhatsUpApp.ViewModel
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private int _userId; // To store userId for the session

        // Define the Username property
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        // Define the Password property
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        // Define the ErrorMessage property to show login errors
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        // This will be triggered when properties change
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to notify property change
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Define the SignInCommand, which will be invoked when the user clicks the sign-in button
        public Command SignInCommand => new Command(async () => await SignInAsync());

        // Async method for sign-in functionality
        public Command SignUpCommand => new Command(async () => await SignUpAsync());

        private async Task SignUpAsync()
        {
            // Navigate to the SignUpPage
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpPage());
        }

        private async Task SignInAsync()
        {
            // Using the Singleton pattern for SocketService
            const string serverAddress = "127.0.0.1";
            const int serverPort = 5000;

            var socketService = await SocketService.InstanceAsync(serverAddress, serverPort); // Use the singleton instance

            try
            {
                // Format the login request
                string loginRequest = $"LOGIN:{Username}:{Password}";
                // Send the request using SocketService and get the response
                string response = await socketService.SendRequestAsync(loginRequest);

                if (response.StartsWith("OK"))
                {
                    // Extract userId from the response (e.g., "OK:123")
                    string[] responseParts = response.Split(':');
                    _userId = int.Parse(responseParts[1]);


                    Application.Current.MainPage.Navigation.PushAsync(new MainPage(_userId));


                }
                else
                {
                    ErrorMessage = response; // Display error message from the server
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error connecting to server: {ex.Message}"; // Handle connection errors
            }


        }
    }
}

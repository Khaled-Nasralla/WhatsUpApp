using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using WhatsUpApp.View;

namespace WhatsUpApp.ViewModel
{
    public class SignUpViewModel : INotifyPropertyChanged
    {
        public string _username { get; set; }
        public string _password { get; set; }
        public string _confirmPassword { get; set; }

        public string _email { get; set; }

        public string _errorMessage { get; set; }



        public string Username
        {
            get => _username;
            set
            {
                _username = value; OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {

            get => _password;
            set
            {
                _password = value; OnPropertyChanged(nameof(Password));
            }
        }


        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value; OnPropertyChanged(nameof(Email));
            }
        }


        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        // This will be triggered when properties change
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to notify property change
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Command SignUpCommand => new Command(async () => await SignUpAsync());

        private async Task SignUpAsync()
        {
            const string serverAddress = "127.0.0.1";
            const int serverPort = 5000;

            try
            {
                var socketserver = await SocketService.InstanceAsync(serverAddress, serverPort);

                string signUpRequest = $"SIGN_UP:{Username}:{Password}:{Email}";

                string response = await socketserver.SendRequestAsync(signUpRequest);

                if (response == "User registered successfully")
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new SignInPage());
                }


            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

    }

}

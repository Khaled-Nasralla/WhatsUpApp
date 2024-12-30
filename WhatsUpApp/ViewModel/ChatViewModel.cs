using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using WhatsUpApp.Model;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using WhatsUpApp.View;


namespace WhatsUpApp.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        // Collection des messages
        public ObservableCollection<MessageModel> Messages { get; set; }
        private SocketService _socketService;
        private string _currentUsername;
        private ChatModel _selectedFriend;

        // Propriété IsLoading
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }



        // Nom de l'utilisateur actuel
        private string _newMessage;
        public string NewMessage
        {
            get => _newMessage;
            set
            {
                _newMessage = value;
                OnPropertyChanged(nameof(NewMessage));
            }
        }

        // Commande pour envoyer un message
        public ICommand SendMessageCommand { get; }

        public ChatViewModel()
        {
        }

        // Constructeur principal
        public ChatViewModel(ChatModel chatModel)
        {
            Messages = new ObservableCollection<MessageModel>();
            _selectedFriend = chatModel;

            _ = InitializeAsync();

            SendMessageCommand = new Command(SendMessage);
        }

        // Initialisation du service de socket
        private async Task InitializeAsync()
        {
            _socketService = await SocketService.InstanceAsync("127.0.0.1", 5000);
        }

        // Chargement des messages depuis le serveur
        private async Task LoadMessagesFromServerAsync()
        {
            IsLoading = true;
            try
            {
                string request = $"FETCH_MESSAGES:{_selectedFriend.UniqueId}";
                string response = await _socketService.SendRequestAsync(request);
                Debug.WriteLine($"Response: {response}");


                if (response == "NO_MESSAGES")
                {
                    Debug.WriteLine("No messages found.");
                    IsLoading = false;
                    return;
                }

                Messages.Clear();
                var messageList = response.Split(',');
                foreach (var messageData in messageList)
                {
                    if (string.IsNullOrWhiteSpace(messageData)) continue;
                    var parts = messageData.Split('|');
                    if (parts.Length < 4)
                    {
                        Debug.WriteLine("Invalid message format.");
                        continue; // Vérifier qu'il y a suffisamment de parties
                    }
                    if (int.TryParse(parts[0], out int senderId) && senderId == _selectedFriend.SenderId)
                    {
                        _currentUsername = "Me";
                    }
                    else
                    {
                        _currentUsername = _selectedFriend.ReceiverName;
                    }
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Messages.Add(new MessageModel
                        {
                            Sender = _currentUsername,
                            Message = parts[2],
                            Time = DateTime.Parse(parts[3])
                        });
                        Debug.WriteLine($"Message added:{_currentUsername} {parts[2]}");
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load messages: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Envoi d'un message
        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessage) || _selectedFriend == null)
            {
                Debug.WriteLine("NewMessage is empty or SelectedFriend is null.");
                return;
            }

            try
            {
                string request = $"SEND_MESSAGE:{_selectedFriend.SenderId}:{_selectedFriend.UniqueId}:{NewMessage}";
                Debug.WriteLine($"Sending request: {request}");
                string response = await _socketService.SendRequestAsync(request);
                Debug.WriteLine($"Response: {response}");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Messages.Add(new MessageModel
                    {
                        Sender = "Me",
                        Message = NewMessage,
                        Time = DateTime.Now
                    });
                    NewMessage = string.Empty;
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send message: {ex.Message}");
            }
        }

        // Gestion du bouton retour
        public async void OnBackButtonPressed()
        {
            _ = Application.Current.MainPage.Navigation.PushAsync(new MainPage(_selectedFriend.SenderId));
        }

        public void OnAppearing()
        {
            _ = LoadMessagesFromServerAsync();

        }

        // Gestion des notifications de changement de propriété
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

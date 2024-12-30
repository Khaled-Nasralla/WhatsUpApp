using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using WhatsUpApp.Model;
using WhatsUpApp.View;

using System.Windows.Input;

namespace WhatsUpApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FriendModel> Friends { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ChatModel> Chats { get; set; }

        private int _userId;
        private SocketService _socketService;

        private ChatModel _selectedFriend;
        public ChatModel SelectedFriend
        {
            get => _selectedFriend;
            set
            {
                _selectedFriend = value;
                OnPropertyChanged(nameof(SelectedFriend));
                if (_selectedFriend != null)
                {
                    Application.Current.MainPage.Navigation.PushAsync(new ChatPage(_selectedFriend));
                }
            }
        }

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

        public ICommand SearchCommand { get; }





        public MainPageViewModel() { }



        public MainPageViewModel(int userId)
        {
            Friends = new ObservableCollection<FriendModel>();
            Messages = new ObservableCollection<MessageModel>();
            Chats = new ObservableCollection<ChatModel>();
            SearchCommand= new Command(OnSearchFriend);
            _userId = userId;
        }

        private async Task InitializeAsync()
        {
            _socketService = await SocketService.InstanceAsync("127.0.0.1", 5000);

        }

        private async Task LoadFriendsFromServerAsync(int userId)
        {
            try
            {

                string request = $"FETCH_FRIENDS:{userId}";
                Debug.WriteLine($"Request: {request}");
                string response = await _socketService.SendRequestAsync(request);
                Debug.WriteLine($"Response: {response}");

                var friendInfos = response.Split(',');
                foreach (var friendInfo in friendInfos)
                {
                    if (!string.IsNullOrWhiteSpace(friendInfo))
                    {
                        var parts = friendInfo.Split('|');
                        if (parts.Length < 4) continue;

                        if (Guid.TryParse(parts[0], out Guid uniqueId))
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                             {
                                 _selectedFriend = new ChatModel
                                 {
                                     UniqueId = uniqueId,
                                     SenderId = int.Parse(parts[1]),
                                     ReceiverId = int.Parse(parts[2]),
                                     ReceiverName = parts[3],
                                 };
                                 Chats.Add(_selectedFriend);
                             });


                        }

                        else
                        {
                            Debug.WriteLine($"Invalid GUID format: {parts[0]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load friends: {ex.Message}");
                await ShowErrorAsync("Unable to load friends. Please try again.");
            }
        }


        public void OnSearchFriend()
        {
            Application.Current.MainPage.Navigation.PushAsync(new FriendsPage(_userId));
        }


        public void OnAppearing()
        {

            _ = InitializeAsync();

            _=LoadFriendsFromServerAsync(_userId);
        }



        private async Task ShowErrorAsync(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

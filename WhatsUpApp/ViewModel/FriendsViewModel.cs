using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using WhatsUpApp.View;

using WhatsUpApp.Model;


namespace WhatsUpApp.ViewModel
{
    public class FriendsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FriendModel> Friends { get; set; } = new();

        private SocketService _socketService;
        private int _userId;

        private string _searchFriends;
        public string SearchFriends
        {
            get => _searchFriends;
            set
            {
                _searchFriends = value;
                OnPropertyChanged(nameof(SearchFriends));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddFriendCommand { get; }

        public ICommand BackButton { get; }

        public FriendsViewModel()
        {
        }

        public FriendsViewModel(int userId)
        {
            _userId = userId;
            Friends = new ObservableCollection<FriendModel>();
            _ = InitializeAsync();

            SearchCommand = new Command(async () => await SearchFriend());
            AddFriendCommand = new Command<FriendModel>(async (friend) => await AddFriend(friend));

        }

        private async Task InitializeAsync()
        {
            _socketService = await SocketService.InstanceAsync("127.0.0.1", 5000);
        }

        private async Task SearchFriend()
        {
            try
            {
                string request = $"SEARCH_FRIENDS:{SearchFriends}:{_userId}";
                string response = await _socketService.SendRequestAsync(request);
                if (response == "NOT_FOUND")
                {
                    Debug.WriteLine("Error: No friends found");
                    return;
                }
                var friendList = response.Split(':');

                Friends.Clear();


                foreach (var friend in friendList)
                {
                    if (!string.IsNullOrWhiteSpace(friend))
                    {
                        var parts = friend.Split('|');
                        if (parts.Length < 2) continue;

                        var newFriend = new FriendModel
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1],
                            IsAdded = false
                        };
                        if (!Friends.Any(f => f.Id == newFriend.Id))
                        {
                            Friends.Add(newFriend);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task AddFriend(FriendModel friend)
        {
            string request = $"ADD_FRIEND:{_userId}:{friend.Id}";
            string response = await _socketService.SendRequestAsync(request);
            if (response == "OK")
            {
                friend.IsAdded = true;
            }
            else
            {
                friend.IsAdded = false;
                Debug.WriteLine($"Error: {response}");


            }
        }


        public async void OnBackButtonPressed()
        {
            _ = Application.Current.MainPage.Navigation.PushAsync(new MainPage(_userId));
        }

        public async void OnAppearing()
        {
            Friends.Clear();
            _ = InitializeAsync();
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

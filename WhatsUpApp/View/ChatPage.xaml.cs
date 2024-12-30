using WhatsUpApp.Model;
using WhatsUpApp.ViewModel;

namespace WhatsUpApp.View
{
    public partial class ChatPage : ContentPage
    {
        private ChatViewModel _viewModel;
        private ChatModel _chatModel;


        public ChatPage(ChatModel chatModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = new ChatViewModel(chatModel);
            _chatModel = chatModel;

        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.OnBackButtonPressed();
            return true;
        }

        //override the OnAppearing method
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}



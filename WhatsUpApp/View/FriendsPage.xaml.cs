
using WhatsUpApp.ViewModel;


namespace WhatsUpApp.View
{


    public partial class FriendsPage : ContentPage
    {
        private FriendsViewModel _viewModel;
        public FriendsPage(int userId)
        {
            InitializeComponent();
            BindingContext = _viewModel = new FriendsViewModel(userId);
        }

        protected override bool OnBackButtonPressed()
        {
            _viewModel.OnBackButtonPressed();
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using WhatsUpApp.Model;
using WhatsUpApp.ViewModel;
namespace WhatsUpApp.View;

public partial class MainPage : ContentPage
{
    private MainPageViewModel _viewModel;
    public MainPage(int userId)
    {
        InitializeComponent();
        BindingContext =_viewModel =new MainPageViewModel(userId);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }



}
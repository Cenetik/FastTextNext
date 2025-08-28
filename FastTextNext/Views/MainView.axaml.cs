using Avalonia.Controls;
using FastTextNext.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System;

namespace FastTextNext.Views;

public partial class MainView : UserControl
{

    public MainView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;

        Loaded += MainView_Loaded;
        
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainText.Focus();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.SetFocusOnMainText += ViewModel_SetFocusOnMainText;
        }
    }

    private void ViewModel_SetFocusOnMainText()
    {
        MainText.Focus();
    }
}

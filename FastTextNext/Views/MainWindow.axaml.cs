using Avalonia.Controls;
using FastTextNext.ViewModels;
using System;

namespace FastTextNext.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Подписываемся на события ViewModel
        DataContextChanged += OnDataContextChanged;
        
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.OnOpenSettingsDialog += OnOpenSettingsDialog;
            viewModel.OnSetTopMost += ViewModel_OnSetTopMost;
        }
    }

    private void ViewModel_OnSetTopMost()
    {
        this.Topmost = !this.Topmost;
    }

    private async void OnOpenSettingsDialog()
    {
        var settingsWindow = new ListTextWindow
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        await settingsWindow.ShowDialog(this);
    }
}

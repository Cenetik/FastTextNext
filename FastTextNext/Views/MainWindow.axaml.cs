using Avalonia.Controls;
using Avalonia.Platform.Storage;
using FastTextNext.ViewModels;
using Microsoft.Extensions.Logging;
using System;

namespace FastTextNext.Views;

public partial class MainWindow : Window
{
    private MainViewModel mainViewModel;
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
            mainViewModel = viewModel;
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
            IsEnabled = true,
            WindowStartupLocation = WindowStartupLocation.CenterOwner            
        };
        var listTextModel = new ListTextModel(mainViewModel.TextManageService, mainViewModel.CurrentTextCategory);
        settingsWindow.DataContext = listTextModel;

        await settingsWindow.ShowDialog(this);

        if(listTextModel.ClosedByGoto)
            mainViewModel.GotoText(listTextModel.SelectedText);
    }
}

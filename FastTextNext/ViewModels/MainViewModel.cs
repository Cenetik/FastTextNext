using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public RelayCommand ShowListTextCommand { get; }

    public event Action? OnOpenSettingsDialog;
    public event Action? OnSetTopMost;

    public MainViewModel()
    {
        ShowListTextCommand = new RelayCommand(ExecuteOpenSettings);
    }

    private void ExecuteOpenSettings()
    {
        OnOpenSettingsDialog?.Invoke();
    }

    [ObservableProperty] 
    private string _textContent = string.Empty;

    public string Greeting => "Welcome to Avalonia!";

    [RelayCommand]
    public void SetText()
    {
        TextContent = "Hello, world!";
    }

    [RelayCommand]
    public void SetTopMost()
    {
        OnSetTopMost?.Invoke();
    }

    [RelayCommand]
    public void SetFavoriteText()
    {

    }

    [RelayCommand]
    public void SetTaskText() 
    {
    }

    [RelayCommand]
    public void SetDoneTaskText()
    {

    }
}

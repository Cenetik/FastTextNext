using Application;
using Application.Helpers;
using Application.Services;
using Application.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FastTextNext.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    public RelayCommand ShowListTextCommand { get; }
    private bool _wasChanged = false;
    private bool _withoutActivity = true;

    public event Action? OnOpenSettingsDialog;
    public event Action? OnSetTopMost;       
    
    public event Action TextContentChanged;

    public MainViewModel(ITextStorageService textStorageService, IConfiguration configuration, IBaseTimer timer)
        : base()
    {
        ShowListTextCommand = new RelayCommand(ExecuteOpenSettings);
        
        this.textStorageService = textStorageService;
        this.timer = timer;

        InitAndStartTimer();
    }

    private void InitAndStartTimer()
    {
        timer.SetIntervalAndAction(1000, OnTimerTick);
        timer.Start();
    }

    private void OnTimerTick()
    {
        if (_withoutActivity)
            Saving();
        else
            _withoutActivity = true;
    }

    private void Saving()
    {
        var textContent = TextContent;
        textStorageService.Save("myfile.txt", textContent);
    }

    private void ExecuteOpenSettings()
    {
        OnOpenSettingsDialog?.Invoke();
    }

    
    [ObservableProperty] 
    private string _textContent = string.Empty;
    private readonly IBaseTimer timer;
    private readonly ITextStorageService textStorageService;

    partial void OnTextContentChanged(string? value)
    {
        TextContentChanged?.Invoke();
        _wasChanged = true;
        _withoutActivity = false;
    }    

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

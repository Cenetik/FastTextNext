using Application.Services;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FastTextNext.Services;
using Microsoft.Extensions.Configuration;
using System;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public RelayCommand ShowListTextCommand { get; }

    public event Action? OnOpenSettingsDialog;
    public event Action? OnSetTopMost;
    private bool _wasChanged = false;
    DispatcherTimer timer = null;
    private bool _withoutActivity = true;
    private readonly ITextStorageService textStorageService;

    public MainViewModel(ITextStorageService textStorageService,IConfiguration configuration)
    {
        ShowListTextCommand = new RelayCommand(ExecuteOpenSettings);
        this.textStorageService = textStorageService;        
        var proc = configuration.GetSection("TextProcessing");

        DispatcherTimer timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(3); 
        timer.Tick += OnTimerTick; 
        timer.Start(); 
    }

    private void OnTimerTick(object? sender, EventArgs e)
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
    partial void OnTextContentChanged(string? value)
    {
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

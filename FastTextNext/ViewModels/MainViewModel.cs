using Application;
using Application.Helpers;
using Application.Services;
using Application.UseCases;
using Application.UseCases.SavingLogic;
using Application.ViewModels;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FastTextNext.Services;
using FastTextNext.Views;
using Microsoft.Extensions.Configuration;
using System;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    public RelayCommand ShowListTextCommand { get; }
    private bool _wasChanged = false;
    private bool _withoutActivity = true;
    private string _filename;
    private bool _fromLoadFile = true;
    private string _maintext = "";
    private string _currentFolder = "";
    private bool _checkButtonFavoriteClicked;
    private bool _checkButtonTaskClicked;
    private bool _checkButtonDoneTaskClicked;
    private bool _noteModeChanged;
    private bool _changeFontStyle;
    private int _firstFileIndex;
    private int _secondFileIndex;
    private bool _isTopMost;
    

    public event Action? OnOpenSettingsDialog;
    public event Action? OnSetTopMost;       
    
    public event Action TextContentChanged;

    public MainViewModel(ITextStorageService textStorageService, IConfiguration configuration, IBaseTimer timer, SavingLogicUseCase savingLogicUseCase, IActionsManager actionsManager)
        : base()
    {
        ShowListTextCommand = new RelayCommand(ExecuteOpenSettings);
        
        this.textStorageService = textStorageService;
        this.timer = timer;
        this.savingLogicUseCase = savingLogicUseCase;
        InitAndStartTimer();
        actionsManager.ChangeFilename += (string filename) => ChangeFilename(filename);
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
        var saveRequest = new SavingLogicRequest(_changeFontStyle, _noteModeChanged, _wasChanged);

        var response = savingLogicUseCase.Save(saveRequest);

        ChangeFilename(response.Filename);

        //var textContent = TextContent;
        //textStorageService.Save("myfile.txt", textContent);
    }

    // ну тут вообще много. Надо это как-то вызывать. Возможно в параметре надо передавать Action'ы? Надо подумоть... Или Subcase сделать
    private void LoadPrevTexts()
    {
        var files = GetFiles(_view.SelectedTextGroup);
        SetVisibleTaskButtons(files);
        int visibleButtons = GetNumberVisibleTaskButtons();
        _secondFileIndex = _firstFileIndex + visibleButtons;

        SetHotkeyTexts(files, _firstFileIndex, _secondFileIndex, visibleButtons);
        SetButtonsLabels(files);
    }

    private void ChangeFilename(string replace)
    {
        _filename = replace;
        //_fontSystemService.SetStandartFont();

        if (_filename != null && _filename.Contains("_f"))
            _view.CheckButtonFavoriteChecked = true;
        else
        {
            _view.CheckButtonFavoriteChecked = false;
        }

        if (_filename != null && _filename.Contains("_t"))
            IsTaskButtonChecked = true;            
        else
        {
            IsTaskButtonChecked = false;
        }

        if (_filename != null && _filename.Contains("_d"))
        {
            _view.CheckButtonDoneTaskChecked = true;
            //richEditControl1.Font = new Font(richEditControl1.Font, FontStyle.Strikeout);
        }
        else
        {
            _view.CheckButtonDoneTaskChecked = false;
        }
        ShowFileName();
    }

    private void ExecuteOpenSettings()
    {
        OnOpenSettingsDialog?.Invoke();
    }

    [ObservableProperty]
    private bool _isTaskButtonChecked;

    [ObservableProperty] 
    private string _textContent = string.Empty;
    private readonly IBaseTimer timer;
    private readonly SavingLogicUseCase savingLogicUseCase;
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

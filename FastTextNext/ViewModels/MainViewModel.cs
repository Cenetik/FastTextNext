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
using Microsoft.Extensions.Options;
using System;
using System.Globalization;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    private bool _isTaskButtonChecked;
    [ObservableProperty]
    private bool _isFavoriteButtonChecked;
    [ObservableProperty]
    private bool _isDoneTaskButtonChecked;
    [ObservableProperty]
    private bool _isResaveThisButtonChecked;
    [ObservableProperty]
    private string _textContent = string.Empty;
    [ObservableProperty]
    private string _logText = string.Empty;

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
    private readonly IBaseTimer timer;
    private readonly SavingLogicUseCase savingLogicUseCase;
    private readonly ITextStorageService textStorageService;

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
        var saveRequest = new SavingLogicRequest(_changeFontStyle, _noteModeChanged, _wasChanged, TextContent,_filename,IsResaveThisButtonChecked,IsFavoriteButtonChecked,IsTaskButtonChecked,IsTaskButtonChecked,LogText,_firstFileIndex);

        var response = savingLogicUseCase.Save(saveRequest);
        LogText = response.LogText;
        _firstFileIndex = response.FirstFileIndex;
        _wasChanged = response.WasChanged;
        _noteModeChanged = response.NoteModeChanged;

        if (response.TextNameChanged)
        {
            IsResaveThisButtonChecked = false;           

            ChangeFilename(response.TextName);
            // todo: много логики, реализовать потом (загрузка хоткеев для быстрого доступа к текстам, в основном для избранных и задач)
            //LoadPrevTextsHotKeys();
        }        
    }
    
    // Прогрузить кнопки быстрого доступа к текстам
    /*private void LoadPrevTextsHotKeys()
    {
        var files = GetFiles(_view.SelectedTextGroup);
        SetVisibleTaskButtons(files);
        int visibleButtons = GetNumberVisibleTaskButtons();
        _secondFileIndex = _firstFileIndex + visibleButtons;

        SetHotkeyTexts(files, _firstFileIndex, _secondFileIndex, visibleButtons);
        SetButtonsLabels(files);
    }*/

    private void ChangeFilename(string replace)
    {
        _filename = replace;
        //_fontSystemService.SetStandartFont();

        if (_filename != null && _filename.Contains("_f"))
            IsFavoriteButtonChecked = true;
        else
        {
            IsFavoriteButtonChecked = false;
        }

        if (_filename != null && _filename.Contains("_t"))
            IsTaskButtonChecked = true;            
        else
        {
            IsTaskButtonChecked = false;
        }

        if (_filename != null && _filename.Contains("_d"))
        {
            IsDoneTaskButtonChecked = true;            
        }
        else
        {
            IsDoneTaskButtonChecked = false;
        }
        ShowTextName();
    }

    private void ShowTextName()
    {
        var mode = "";
        if (_filename == null)
            _filename = "";

        if (_filename.Contains("_t"))
            mode += " - Задача ";
        if (_filename.Contains("_d"))
            mode += " - Задача выполнена";
        if (_filename.Contains("_f"))
            mode += " - Избранная";

        var dateTimeOfFile = GetDateTimeByTextName(_filename);
        var filename = _filename;
        if (dateTimeOfFile != null)
            filename += string.Format(" [{0:dd.MM.yyyy HH:mm:ss.fff}]", dateTimeOfFile.Value);

        _view.HeadText = string.Format("{0} - {1}{2}", _maintext, filename, mode);
    }

    private DateTime? GetDateTimeByTextName(string filename)
    {
        var onlyDate = filename.Split(new[] { '_', '.' });
        DateTime dt;
        if (DateTime.TryParseExact(onlyDate[0], "yyyyMMddHHmmssfff", null, DateTimeStyles.None, out dt))
            return dt;
        return null;
    }

    private void ExecuteOpenSettings()
    {
        OnOpenSettingsDialog?.Invoke();
    }


    partial void OnTextContentChanged(string? value)
    {
        TextContentChanged?.Invoke();
        _wasChanged = true;
        _withoutActivity = false;
    }    

        

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

using Application;
using Application.Enums;
using Application.Helpers;
using Application.Services;
using Application.UseCases;
using Application.UseCases.NextText;
using Application.UseCases.PrevText;
using Application.UseCases.SavingLogic;
using Application.ViewModels;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Entities;
using FastTextNext.Commands;
using FastTextNext.Services;
using FastTextNext.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace FastTextNext.ViewModels;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    public string Greeting => "Welcome to Avalonia!";

    #region private fields
    private bool _wasChanged = false;
    private bool _withoutActivity = true;
    private string _textname;
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
    private readonly ISavingLogicUseCase savingLogicUseCase;
    private readonly INextTextUseCase nextTextUseCase;    
    private readonly IPrevTextUseCase prevTextUseCase;
    private readonly ITextStorageService textStorageService;
    #endregion

    #region ObservableProperties
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
    [ObservableProperty]
    private string _headerText = string.Empty;
    [ObservableProperty]
    private bool _isTaskButtonEnabled;
    [ObservableProperty]
    private bool _isDoneTaskButtonEnabled;
    [ObservableProperty]
    private bool _isFavoriteButtonEnabled;
    [ObservableProperty]
    private bool _isFastText1Visible;
    [ObservableProperty]
    private bool _isFastText2Visible;


    public TextCategory CurrentTextCategory { get; internal set; }
    #endregion

    public MainViewModel(ITextStorageService textStorageService, IConfiguration configuration, IBaseTimer timer, ISavingLogicUseCase savingLogicUseCase, INextTextUseCase nextTextUseCase,
        ITextManageService textManageService, IPrevTextUseCase prevTextUseCase)
        : base()
    {
        this.textStorageService = textStorageService;
        this.timer = timer;
        this.savingLogicUseCase = savingLogicUseCase;
        this.nextTextUseCase = nextTextUseCase;
        this.TextManageService = textManageService;
        this.prevTextUseCase = prevTextUseCase;
        ShowListTextCommand = new RelayCommand(ExecuteOpenSettings);        
        //ShowListTextCommand = new ShowListTextRelayCommand(OnOpenSettingsDialog,CanOpenSettingsDialog);

        CurrentTextCategory = TextCategory.AllTexts;

        InitAndStartTimer();
    }

    private bool CanOpenSettingsDialog(object arg)
    {
        return true;
    }


    #region RelayCommands    
    public RelayCommand ShowListTextCommand { get; }
    //public ShowListTextRelayCommand ShowListTextCommand { get; }
    public ITextManageService TextManageService { get; internal set; }

    [RelayCommand]
    public void SetNewText()
    {
        Saving();
        ChangeFilename("");
        TextContent = "";
        _wasChanged = false;
        IsDoneTaskButtonEnabled = IsTaskButtonEnabled = IsFavoriteButtonEnabled = false;
        LogText = string.Format("[{0:HH:mm:ss}] Создание новой заметки", DateTime.Now);
        ShowTextName();
        SetFocusOnMainText?.Invoke();
    }

    [RelayCommand]
    public void SetTopMost()
    {
        OnSetTopMost?.Invoke();
    }

    [RelayCommand]
    public void SetFavoriteText()
    {
        _noteModeChanged = true;
        Saving();
    }

    partial void OnIsFavoriteButtonCheckedChanged(bool value)
    {
        // тут нельзя делать логику сейва, т.к. событие вызывается даже когда переходим со старых заметок на новые. И из-за логики происходит пересохранение и зацикливание.
    }

    [RelayCommand]
    public void SetTaskText()
    {
        _noteModeChanged = true;
        if (IsDoneTaskButtonChecked && IsTaskButtonChecked)
            IsDoneTaskButtonChecked = false;

        _checkButtonTaskClicked = false;
        _noteModeChanged = true;
        if (IsTaskButtonChecked)
            TextContent += Environment.NewLine + "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "] - задача создана";
        Saving();
    }

    [RelayCommand]
    public void SetDoneTaskText()
    {
        _checkButtonDoneTaskClicked = false;
        _noteModeChanged = true;

        IsTaskButtonChecked = !IsDoneTaskButtonChecked;

        if (IsDoneTaskButtonChecked)
            TextContent += Environment.NewLine + "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "] - задача выполнена";
        Saving();
    }

    [RelayCommand]
    public void SetResaveMode()
    {
        if (IsResaveThisButtonChecked)
            Saving();
    }

    [RelayCommand]
    public void RemoveText()
    {
        //todo: реализовать
        throw new NotImplementedException();
    }

    [RelayCommand]
    public void NextText()
    {
        Saving();

        var nextTextRequest = new NextTextRequest(_textname);
        var response = nextTextUseCase.GetNextText(nextTextRequest);
        if (response.NextTextExists)
        {
            _fromLoadFile = true;
            TextContent = response.NextTextContent;
            _textname = response.TextName;
            ShowTextName();
            ChangeFilename(response.TextName);
        }
        SetFocusOnMainText?.Invoke();
    }

    [RelayCommand]
    public void PrevText()
    {
        Saving();
        
        var prevTextRequest = new PrevTextRequest(_textname);
        var response = prevTextUseCase.GetPrevText(prevTextRequest);       

        if (response.PrevTextExists)
        {
            _fromLoadFile = true;
            TextContent = response.PrevTextContent;
            _textname = response.TextName;            
            ShowTextName();  
            ChangeFilename(response.TextName);
        }
        SetFocusOnMainText?.Invoke();
    }    

    [RelayCommand]
    public void FastText1()
    {

    }

    [RelayCommand]
    public void FastText2()
    {

    }


    #endregion



    #region Actions
    public event Action? OnOpenSettingsDialog;
    public event Action? OnSetTopMost;           
    public event Action TextContentChanged;
    public event Action? SetFocusOnMainText;
    #endregion

    public MainViewModel()
    {

    }
    

    private void ExecuteOpenSettings()
    {
        OnOpenSettingsDialog?.Invoke();        
    }

    private void ChangeFilename(string replace)
    {
        _textname = replace;
        //_fontSystemService.SetStandartFont();

        if (_textname != null && _textname.Contains("_f"))
            IsFavoriteButtonChecked = true;
        else
        {
            IsFavoriteButtonChecked = false;
        }

        if (_textname != null && _textname.Contains("_t"))
            IsTaskButtonChecked = true;
        else
        {
            IsTaskButtonChecked = false;
        }

        if (_textname != null && _textname.Contains("_d"))
        {
            IsDoneTaskButtonChecked = true;
        }
        else
        {
            IsDoneTaskButtonChecked = false;
        }
        ShowTextName();
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
        //Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
    }

    private void Saving()
    {
        var saveRequest = new SavingLogicRequest(_changeFontStyle, _noteModeChanged, _wasChanged, TextContent,_textname,IsResaveThisButtonChecked,
                                                 IsFavoriteButtonChecked,IsTaskButtonChecked,IsDoneTaskButtonChecked,LogText,_firstFileIndex);

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
            LoadPrevTextsHotKeys();
        }        
    }
    
    // Прогрузить кнопки быстрого доступа к текстам
    private void LoadPrevTextsHotKeys()
    {
        // todo: надо ещё на форму добавить выпадающий список с тремя типами категорий, и тогда вот здесь нужно выбирать из них
        /*var textList = TextManageService.GetTextEntities(FastTextCategory - это комбо-бокс)        
        SetVisibleTaskButtons(textList);
        int visibleButtons = GetNumberVisibleTaskButtons();
        _secondFileIndex = _firstFileIndex + visibleButtons;

        SetHotkeyTexts(files, _firstFileIndex, _secondFileIndex, visibleButtons);
        SetButtonsLabels(files);*/
    }


    private void ShowTextName()
    {        
        if (_textname == null)
            _textname = "";        

        var dateTimeOfFile = GetDateTimeByTextName(_textname);
        var textNameForHeader = _textname;
        if (dateTimeOfFile != null)
            textNameForHeader += string.Format(" [{0:dd.MM.yyyy HH:mm:ss.fff}]", dateTimeOfFile.Value);

        var mode = TextManageService.GetTextCategory(_textname);        

        HeaderText = GetHeaderText(textNameForHeader, mode); 
        
    }

    private string GetHeaderText(string filename, TextCategory textCategory)
    {
        string suffix = "";
        if (filename.Contains("_f"))
            suffix += " Избранная";
        if (filename.Contains("_t"))
            suffix += " Задача";
        if (filename.Contains("_d"))
            suffix += " Выполненная задача";        

        if (!string.IsNullOrEmpty(suffix))
            suffix = " -" + suffix;

        return string.Format("{0} - {1}{2}", _maintext, filename, suffix);
    }

    private DateTime? GetDateTimeByTextName(string filename)
    {
        var onlyDate = filename.Split(new[] { '_', '.' });
        DateTime dt;
        if (DateTime.TryParseExact(onlyDate[0], "yyyyMMddHHmmssfff", null, DateTimeStyles.None, out dt))
            return dt;
        return null;
    }   


    partial void OnTextContentChanged(string? value)
    {
        TextContentChanged?.Invoke();       

        if (_fromLoadFile)
        {
            _fromLoadFile = false;            
        }
        else
        {
            _wasChanged = true;
            _withoutActivity = false;
        }
            

        IsDoneTaskButtonEnabled = IsTaskButtonEnabled = IsFavoriteButtonEnabled = !string.IsNullOrEmpty(TextContent);
        //checkButtonTask.Enabled = checkButtonDoneTask.Enabled = checkButtonFavorite.Enabled = !string.IsNullOrEmpty(richEditControl1.Text);
    }

    public void GotoText(TextEntity selectedText)
    {
        Saving();

        _fromLoadFile = true;
        TextContent = selectedText.TextContent;
        _textname = selectedText.Name;
        ShowTextName();
        SetFocusOnMainText?.Invoke();

        ChangeFilename(selectedText.Name);
    }
}

using Application.Enums;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastTextNext.ViewModels
{
    public partial class ListTextModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TextEntity> _loadedTexts = new();
        [ObservableProperty]
        private TextEntity _selectedText;
        [ObservableProperty]
        private string _textToFind = string.Empty;
        private readonly ITextManageService textManageService;
        private TextCategory _textCategory;
        public event Action? CloseThisView;
        public bool ClosedByGoto = false;

        public ListTextModel(ITextManageService textManageService,TextCategory textCategory) : base()
        {
            GotoTextCommand = new RelayCommand(GotoText);
            ClosedByGoto = false;
            this.textManageService = textManageService;
            _textCategory = textCategory;
            _loadedTexts = new ObservableCollection<TextEntity>(textManageService.GetTextEntities(textCategory));
        }

        public RelayCommand GotoTextCommand { get; }
        
        public void GotoText()
         {
            ClosedByGoto = true;
            CloseThisView?.Invoke();
           // int a = 0;
           // a = a + 1;
        }

        [RelayCommand]
        public void FindText()
        {
            var text = TextToFind;
            int a = 0;
            a = a + 1;
        }

        
    }
}

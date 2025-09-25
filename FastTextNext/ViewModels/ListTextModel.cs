using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastTextNext.ViewModels
{
    public partial class ListTextModel : ObservableObject
    {
        public ListTextModel() : base()
        {
            GotoTextCommand = new RelayCommand(GotoText1);            
        }

        public RelayCommand GotoTextCommand { get; }
        
        public void GotoText1()
        {
            int a = 0;
            a = a + 1;
        }

        [RelayCommand]
        public void FindText()
        {
            var text = TextToFind;
            int a = 0;
            a = a + 1;
        }

        [ObservableProperty]
        private string _textToFind = string.Empty;
    }
}

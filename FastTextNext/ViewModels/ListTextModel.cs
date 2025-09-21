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
        [RelayCommand]
        public void GotoText()
        {
          
        }

        [RelayCommand]
        public void Find()
        {

        }

        [ObservableProperty]
        private string _findText = string.Empty;
    }
}

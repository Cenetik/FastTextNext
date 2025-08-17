using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SavingLogic
{
    public record SavingLogicRequest(bool ChangeFontStyle, bool NoteModeChanged, bool WasChanged, string TextContent, string CurrentFolder, 
        string Filename, bool ButtonResaveThisChecked,bool CheckButtonFavoriteChecked,bool CheckButtonTaskChecked,bool CheckButtonDoneTaskChecked,
         string LogText, int FirstFileIndex);
    

    public record SavingLogicResult(bool WasChanged, bool ChangeFontStyle, string LogText, int FirstFileIndex, bool ButtonResaveThisChecked, string NameOfFile);
    
}

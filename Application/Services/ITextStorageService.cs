using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITextStorageService
    {
        string GetText(string textName);
        string GetPrevTextName();
        void Save(string textName, string textValue);
        void ChangeTextName(string oldNameOfFile, string nameOfFile);
        List<string> GetAllTextsNames();
    }
}

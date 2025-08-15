using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastTextNext.Services
{
    public interface ITextStorageService
    {
        void SaveText(string textName, string text);
    }
}

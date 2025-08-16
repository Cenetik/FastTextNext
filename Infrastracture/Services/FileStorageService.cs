using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Services
{
    public class FileStorageService : ITextStorageService
    {
        private string _folder = "C:/texts/";
        public void Save(string textName, string textValue)
        {
            var fullpath = Path.Combine(_folder, textName);
            if(!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
            File.WriteAllText(fullpath, textValue);
        }
    }
}

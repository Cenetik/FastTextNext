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
        public FileStorageService()
        {
            if (string.IsNullOrEmpty(GlobalData.SaveTextStorageFolder))
                throw new FileStorageWithFolderCheckServiceNotDefinedStorageFolderException();
            _folder = GlobalData.SaveTextStorageFolder;
        }

        public string GetText(string textName)
        {
            var text = File.ReadAllText(Path.Combine(_folder,textName));
            return text;
        }

        public string GetPrevTextName()
        {
            var files = Directory.GetFiles(_folder);
            if (files.Length == 0)
                return "";

            var path = "";
            path = files[files.Length - 1];

            //todo: проверить, что это работает! Можно написать тесты (надо подключить проект тестов!)
            return Path.GetFileNameWithoutExtension(path);            
        }
        
        public void Save(string textName, string textContent)
        {
            var fullpath = Path.Combine(_folder, textName);           
            File.WriteAllText(fullpath, textContent);
        }

        public void ChangeTextName(string oldNameOfText, string newNameOfText)
        {
            File.Move(Path.Combine(_folder, oldNameOfText), Path.Combine(_folder, newNameOfText));
        }
    }    

    public class FileStorageWithFolderCheckService : ITextStorageService
    {
        private readonly FileStorageService fileStorageService;
        private readonly string _folder;

        public FileStorageWithFolderCheckService(FileStorageService fileStorageService)
        {
            this.fileStorageService = fileStorageService;
            if (string.IsNullOrEmpty(GlobalData.SaveTextStorageFolder))
                throw new FileStorageWithFolderCheckServiceNotDefinedStorageFolderException();
            _folder = GlobalData.SaveTextStorageFolder;
        }

        public void ChangeTextName(string oldNameOfFile, string nameOfFile)
        {
            EnsureMainDirectoryExists();
            fileStorageService.ChangeTextName(oldNameOfFile, nameOfFile);
        }

        public string GetPrevTextName()
        {
            EnsureMainDirectoryExists();
            return fileStorageService.GetPrevTextName();
        }

        public string GetText(string textName)
        {
            EnsureMainDirectoryExists();
            return fileStorageService.GetText(textName);
        }

        public void Save(string textName, string textValue)
        {
            EnsureMainDirectoryExists();
            fileStorageService.Save(textName, textValue);
        }

        private void EnsureMainDirectoryExists()
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
        }
    }
}

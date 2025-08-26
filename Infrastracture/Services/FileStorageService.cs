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
        private string _extension = ".txt";
        public FileStorageService()
        {
            if (string.IsNullOrEmpty(GlobalData.SaveTextStorageFolder))
                throw new FileStorageWithFolderCheckServiceNotDefinedStorageFolderException("Не задана папка для хранения файлов!");
            _folder = GlobalData.SaveTextStorageFolder;
        }

        public string GetText(string textName)
        {
            var text = File.ReadAllText(FullFileName(textName));
            return text;
        }

        private string FullFileName(string textName)
        {
            return Path.Combine(_folder, textName) + _extension;
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
            var fullpath = FullFileName(textName);
            File.WriteAllText(fullpath, textContent);
        }

        public void ChangeTextName(string oldNameOfText, string newNameOfText)
        {
            File.Move(FullFileName(oldNameOfText), FullFileName(newNameOfText));
        }
    }    

    public class FileStorageWithFolderCheckService : ITextStorageService
    {
        private readonly ITextStorageService textStorageService;
        private readonly string _folder;

        public FileStorageWithFolderCheckService(ITextStorageService textStorageService)
        {
            this.textStorageService = textStorageService;
            if (string.IsNullOrEmpty(GlobalData.SaveTextStorageFolder))
                throw new FileStorageWithFolderCheckServiceNotDefinedStorageFolderException();
            _folder = GlobalData.SaveTextStorageFolder;
        }

        public void ChangeTextName(string oldNameOfFile, string nameOfFile)
        {
            EnsureMainDirectoryExists();
            textStorageService.ChangeTextName(oldNameOfFile, nameOfFile);
        }

        public string GetPrevTextName()
        {
            EnsureMainDirectoryExists();
            return textStorageService.GetPrevTextName();
        }

        public string GetText(string textName)
        {
            EnsureMainDirectoryExists();
            return textStorageService.GetText(textName);
        }

        public void Save(string textName, string textValue)
        {
            EnsureMainDirectoryExists();
            textStorageService.Save(textName, textValue);
        }

        private void EnsureMainDirectoryExists()
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);
        }
    }
}

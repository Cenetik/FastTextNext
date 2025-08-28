using Application.Services;

namespace Infrastracture.Services
{
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

        public List<string> GetAllTextsNames()
        {
            EnsureMainDirectoryExists();
            return textStorageService.GetAllTextsNames();
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

using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SavingLogic
{
    public class SavingLogicUseCase
    {
        private readonly ITextStorageService textStorageService;

        public SavingLogicUseCase(ITextStorageService textStorageService)
        {
            this.textStorageService = textStorageService;
        }

        public SavingLogicResult Save(SavingLogicRequest request)
        {
            if (request.ChangeFontStyle && request.NoteModeChanged)
            {
                return new SavingLogicResult(false,false, request.LogText, request.FirstFileIndex,request.ButtonResaveThisChecked,request.Filename);
            }

            var logText = request.LogText;
            var firstFileIndex = request.FirstFileIndex;
            var buttonResaveThisChecked = request.ButtonResaveThisChecked;
            var nameOfFile = request.Filename;
            if (!string.IsNullOrWhiteSpace(request.TextContent) && (request.WasChanged || request.NoteModeChanged))
            {
                
                textStorageService.EnsureMainDirectoryExists(request.CurrentFolder);
                var prevFile = string.IsNullOrEmpty(request.Filename) ? GetLastFilePath(request.CurrentFolder) : Path.Combine(request.CurrentFolder, request.Filename);

                nameOfFile = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var oldNameOfFile = "";
                if (!string.IsNullOrEmpty(prevFile))
                {
                    var text = GetFileText(prevFile);
                    // Проверяем, не содержит ли новый текст в начале текст предыдущего файла. Тогда мы будем перезаписывать файл, а не создавать новый, ибо это излишество
                    if (request.TextContent.Trim().StartsWith(text.Trim()) || request.ButtonResaveThisChecked)
                    {
                        FileInfo fileInfo = new FileInfo(prevFile);
                        oldNameOfFile = fileInfo.Name;
                    }
                }

                // Не баг, а фича - закомментировал специально: пусть лучше создаётся больше случайных задач и избранных, 
                // чем потеряются какие-то задачи или избранные
                // Добавлено позже: коммент выше непонятен, но, видимо, раньше задачи, избранные и выполненные не дублировались при изменении. 
                // Т.е. если мы теперь вставим какой-то текст в середину текста уже существующей задачи, то будет создана новая задача, а не перезаписана старая.
                // Но вообще, тогда закомментированное можно убрать...
                //if (!string.IsNullOrEmpty(oldNameOfFile))
                //{
                if (request.CheckButtonFavoriteChecked)
                    nameOfFile += "_f";
                if (request.CheckButtonTaskChecked)
                    nameOfFile += "_t";
                if (request.CheckButtonDoneTaskChecked)
                    nameOfFile += "_d";
                //}

                nameOfFile += ".txt";

                // Если был ранее создан файл с таким текстом в начале, и мы его дополняем
                if (!string.IsNullOrEmpty(oldNameOfFile))
                {
                    // Записываем текст в этот старый файл
                    File.WriteAllText(Path.Combine(request.CurrentFolder, oldNameOfFile), request.TextContent);
                    // Переименовываем его по-новому
                    File.Move(Path.Combine(request.CurrentFolder, oldNameOfFile), Path.Combine(request.CurrentFolder, nameOfFile));
                    logText = string.Format("[{0:HH:mm:ss}] Заметка дополнена и перезаписана под новым именем {1}", DateTime.Now, nameOfFile);
                }
                else
                {
                    File.WriteAllText(Path.Combine(request.CurrentFolder, nameOfFile), request.TextContent);
                    logText = string.Format("[{0:HH:mm:ss}] Создана новая заметка {1}", DateTime.Now, nameOfFile);
                }

                //ChangeFilename(nameOfFile);

                firstFileIndex = 0;
                
               // todo: реализовать согласно старому проекту
               // LoadPrevTexts();
                buttonResaveThisChecked = false;
            }

            return new SavingLogicResult(false, false,logText,firstFileIndex, buttonResaveThisChecked,nameOfFile);
        }

        private string GetFileText(string path)
        {
            var text = File.ReadAllText(path);
            return text;
        }

        private string GetLastFilePath(string currentFolder)
        {
            if (!Directory.Exists(currentFolder))
                Directory.CreateDirectory(currentFolder);

            var files = Directory.GetFiles(currentFolder);
            if (files.Length == 0)
                return "";

            var path = "";
            path = files[files.Length - 1];

            return path;
        }       
    }
}

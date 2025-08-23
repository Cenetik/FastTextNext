using Application.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                return new SavingLogicResult(false, request.LogText, request.FirstFileIndex,request.ButtonResaveThisChecked,request.Filename,false,request.NoteModeChanged);
            }

            var logText = request.LogText;
            var firstFileIndex = request.FirstFileIndex;
            var buttonResaveThisChecked = request.ButtonResaveThisChecked;
            var nameOfFile = request.Filename;
            var wasChanged = request.WasChanged;
            var noteModeChanged = request.NoteModeChanged;
            var textNameChanged = false;

            if (!string.IsNullOrWhiteSpace(request.TextContent) && (request.WasChanged || request.NoteModeChanged))
            {                
                var prevFile = string.IsNullOrEmpty(request.Filename) ? textStorageService.GetPrevTextName() 
                                                                      : request.Filename;

                nameOfFile = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var oldNameOfFile = "";
                if (!string.IsNullOrEmpty(prevFile))
                {
                    var text = textStorageService.GetText(prevFile);
                    // Проверяем, не содержит ли новый текст в начале текст предыдущего файла. Тогда мы будем перезаписывать файл, а не создавать новый, ибо это излишество
                    if (request.TextContent.Trim().StartsWith(text.Trim()) || request.ButtonResaveThisChecked)
                    {
                        // тут какая-то фигня была, типа извлечение имени из полного пути. Но по идее я это делаю в методе textStorageService.GetPrevTextName()
                        //FileInfo fileInfo = new FileInfo(prevFile);
                        oldNameOfFile = prevFile;// fileInfo.Name;
                    }
                }
                
                if (request.CheckButtonFavoriteChecked)
                    nameOfFile += "_f";
                if (request.CheckButtonTaskChecked)
                    nameOfFile += "_t";
                if (request.CheckButtonDoneTaskChecked)
                    nameOfFile += "_d";               
                
                // Если был ранее создан файл с таким текстом в начале, и мы его дополняем
                if (!string.IsNullOrEmpty(oldNameOfFile))
                {
                    textStorageService.Save(oldNameOfFile, request.TextContent);
                    // Переименовываем его по-новому
                    textStorageService.ChangeTextName(oldNameOfFile,nameOfFile);
                    // вот эти логи бы сделать событиями какими-то. Раз уж надо нам такое логгировать.
                    logText = string.Format("[{0:HH:mm:ss}] Заметка дополнена и перезаписана под новым именем {1}", DateTime.Now, nameOfFile);
                }
                else
                {
                    textStorageService.Save(nameOfFile, request.TextContent);                    
                    logText = string.Format("[{0:HH:mm:ss}] Создана новая заметка {1}", DateTime.Now, nameOfFile);
                }

                textNameChanged = true;              
                firstFileIndex = 0;                
                buttonResaveThisChecked = false;
            }

            wasChanged = false;
            noteModeChanged = false;

            return new SavingLogicResult(wasChanged, logText,firstFileIndex, buttonResaveThisChecked,nameOfFile, textNameChanged, noteModeChanged);
        }        
    }
}

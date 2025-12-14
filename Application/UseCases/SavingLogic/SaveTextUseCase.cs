using Application.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SavingLogic
{
    public class SaveTextUseCase : ISaveTextUseCase
    {
        private readonly ITextStorageService textStorageService;        

        public SaveTextUseCase(ITextStorageService textStorageService)
        {
            this.textStorageService = textStorageService;
        }

        public SaveTextUseCaseResult Execute(SaveTextUseCaseParams args)
        {
            try
            {

                if (args.ChangeFontStyle && args.NoteModeChanged)
                {
                    return new SaveTextUseCaseResult(WasChanged: false,
                                                 LogText: args.LogText,
                                                 FirstFileIndex: args.FirstFileIndex,
                                                 ButtonResaveThisChecked: args.ButtonResaveThisChecked,
                                                 TextName: args.Filename,
                                                 TextNameChanged: false,
                                                 NoteModeChanged: args.NoteModeChanged);
                }

                var logText = args.LogText;
                var firstFileIndex = args.FirstFileIndex;
                var buttonResaveThisChecked = args.ButtonResaveThisChecked;
                var nameOfFile = args.Filename;
                var wasChanged = args.WasChanged;
                var noteModeChanged = args.NoteModeChanged;
                var textNameChanged = false;

                if (!string.IsNullOrWhiteSpace(args.TextContent) && (args.WasChanged || args.NoteModeChanged))
                {
                    var prevTextName = textStorageService.GetPrevTextName();
                    var prevFile = string.IsNullOrEmpty(args.Filename) ? prevTextName
                                                                          : args.Filename;

                    nameOfFile = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var oldNameOfFile = "";
                    if (!string.IsNullOrEmpty(prevFile))
                    {
                        var text = textStorageService.GetText(prevFile);
                        // Проверяем, не содержит ли новый текст в начале текст предыдущего файла. Тогда мы будем перезаписывать файл, а не создавать новый, ибо это излишество
                        if (args.TextContent.Trim().StartsWith(text.Trim()) || args.ButtonResaveThisChecked)
                        {
                            // тут какая-то фигня была, типа извлечение имени из полного пути. Но по идее я это делаю в методе textStorageService.GetPrevTextName()
                            //FileInfo fileInfo = new FileInfo(prevFile);
                            oldNameOfFile = prevFile;// fileInfo.Name;
                        }
                    }

                    if (args.CheckButtonFavoriteChecked)
                        nameOfFile += "_f";
                    if (args.CheckButtonTaskChecked)
                        nameOfFile += "_t";
                    if (args.CheckButtonDoneTaskChecked)
                        nameOfFile += "_d";

                    // Если был ранее создан файл с таким текстом в начале, и мы его дополняем
                    if (!string.IsNullOrEmpty(oldNameOfFile))
                    {
                        textStorageService.Save(textName: oldNameOfFile,
                                                textValue: args.TextContent);
                        // Переименовываем его по-новому
                        textStorageService.ChangeTextName(oldNameOfFile: oldNameOfFile,
                                                          nameOfFile: nameOfFile);
                        // вот эти логи бы сделать событиями какими-то. Раз уж надо нам такое логгировать.
                        logText = string.Format("[{0:HH:mm:ss}] Заметка дополнена и перезаписана под новым именем {1}", DateTime.Now, nameOfFile);
                    }
                    else
                    {
                        textStorageService.Save(textName: nameOfFile,
                                                textValue: args.TextContent);
                        logText = string.Format("[{0:HH:mm:ss}] Создана новая заметка {1}", DateTime.Now, nameOfFile);
                    }

                    textNameChanged = true;
                    firstFileIndex = 0;
                    buttonResaveThisChecked = false;
                }

                wasChanged = false;
                noteModeChanged = false;
                return new SaveTextUseCaseResult(WasChanged: wasChanged,
                                             LogText: logText,
                                             FirstFileIndex: firstFileIndex,
                                             ButtonResaveThisChecked: buttonResaveThisChecked,
                                             TextName: nameOfFile,
                                             TextNameChanged: textNameChanged,
                                             NoteModeChanged: noteModeChanged);
            }
            catch (Exception ex)
            {
                return new SaveTextUseCaseResult(false, 
                                            "Критическая ошибка при сохранении: "+ex.Message, 
                                            args.FirstFileIndex, 
                                            args.ButtonResaveThisChecked, 
                                            args.Filename, 
                                            false, 
                                            args.NoteModeChanged);
            }
        }
    }
}

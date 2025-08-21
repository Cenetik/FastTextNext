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
                return new SavingLogicResult(false,false, request.LogText, request.FirstFileIndex,request.ButtonResaveThisChecked,request.Filename);
            }

            var logText = request.LogText;
            var firstFileIndex = request.FirstFileIndex;
            var buttonResaveThisChecked = request.ButtonResaveThisChecked;
            var nameOfFile = request.Filename;
            var wasChanged = request.WasChanged;
            var noteModeChanged = request.NoteModeChanged;

            if (!string.IsNullOrWhiteSpace(request.TextContent) && (request.WasChanged || request.NoteModeChanged))
            {                
                var prevFile = string.IsNullOrEmpty(request.Filename) ? textStorageService.GetPrevTextName() 
                                                                      : Path.Combine(request.CurrentFolder, request.Filename);

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
                
                // Вот тут наверное не стоит добавлять расширение. Это задача всё-таки fileTextService реализации ITextStorageService
                //nameOfFile += ".txt";

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

                ChangeFilename(nameOfFile);

                firstFileIndex = 0;
                
               // todo: реализовать согласно старому проекту
                LoadPrevTexts();
                buttonResaveThisChecked = false;
            }

            wasChanged = false;
            noteModeChanged = false;

            return new SavingLogicResult(false, false,logText,firstFileIndex, buttonResaveThisChecked,nameOfFile);
        }

        // Вот этот метод нужно вынести куда-то отдельно, т.к. он вызывается на разные действия пользователя, а не только при сохранении
        private void ChangeFilename(string replace)
        {
            _filename = replace;
            _fontSystemService.SetStandartFont();

            if (_filename != null && _filename.Contains("_f"))
                _view.CheckButtonFavoriteChecked = true;
            else
            {
                _view.CheckButtonFavoriteChecked = false;
            }

            if (_filename != null && _filename.Contains("_t"))
                _view.CheckButtonTaskChecked = true;
            else
            {
                _view.CheckButtonTaskChecked = false;
            }

            if (_filename != null && _filename.Contains("_d"))
            {
                _view.CheckButtonDoneTaskChecked = true;
                //richEditControl1.Font = new Font(richEditControl1.Font, FontStyle.Strikeout);
            }
            else
            {
                _view.CheckButtonDoneTaskChecked = false;
            }
            ShowFileName();
        }

        // Этот метод тоже нужно вынести отдельно
        private void ShowFileName()
        {
            var mode = "";
            if (_filename == null)
                _filename = "";

            if (_filename.Contains("_t"))
                mode += " - Задача ";
            if (_filename.Contains("_d"))
                mode += " - Задача выполнена";
            if (_filename.Contains("_f"))
                mode += " - Избранная";

            var dateTimeOfFile = GetDateTimeByFileName(_filename);
            var filename = _filename;
            if (dateTimeOfFile != null)
                filename += string.Format(" [{0:dd.MM.yyyy HH:mm:ss.fff}]", dateTimeOfFile.Value);

            _view.HeadText = string.Format("{0} - {1}{2}", _maintext, filename, mode);
        }

        // и этот...
        private DateTime? GetDateTimeByFileName(string filename)
        {
            var onlyDate = filename.Split(new[] { '_', '.' });
            DateTime dt;
            if (DateTime.TryParseExact(onlyDate[0], "yyyyMMddHHmmssfff", null, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        // ну тут вообще много. Надо это как-то вызывать. Возможно в параметре надо передавать Action'ы? Надо подумоть... Или Subcase сделать
        private void LoadPrevTexts()
        {
            var files = GetFiles(_view.SelectedTextGroup);
            SetVisibleTaskButtons(files);
            int visibleButtons = GetNumberVisibleTaskButtons();
            _secondFileIndex = _firstFileIndex + visibleButtons;

            SetHotkeyTexts(files, _firstFileIndex, _secondFileIndex, visibleButtons);
            SetButtonsLabels(files);
        }
    }
}

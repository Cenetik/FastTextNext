using Application.Enums;
using Application.Services;

namespace Application.UseCases.PrevText
{
    public class PrevTextUseCase : IPrevTextUseCase
    {
        private readonly ITextManageService textManageService;
        private readonly ITextStorageService textStorageService;

        public PrevTextUseCase(ITextManageService textManageService, ITextStorageService textStorageService)
        {
            this.textManageService = textManageService;
            this.textStorageService = textStorageService;
        }

        public PrevTextResponse GetPrevText(PrevTextRequest request)
        {       
            List<string> textNames = textManageService.GetTexts(TextCategory.AllTexts);
            if (textNames.Count == 0)
                return new PrevTextResponse(PrevTextExists: false, PrevTextContent: null, TextCategory: TextCategory.AllTexts, TextName: null);

            var prevTextName = textManageService.GetNexOrPrevPath(textNames, request.CurrentTextName, -1);

            if (string.IsNullOrWhiteSpace(prevTextName))
                return new PrevTextResponse(PrevTextExists: false, PrevTextContent: null, TextCategory: TextCategory.AllTexts, TextName: null);

            var textContent = textStorageService.GetText(prevTextName);

            var textCategory = textManageService.GetTextCategory(prevTextName);

            return new PrevTextResponse(PrevTextContent: textContent, TextCategory: textCategory, PrevTextExists: true, TextName: prevTextName);
        }
    }

}

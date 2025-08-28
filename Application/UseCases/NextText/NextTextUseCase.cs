using Application.Enums;
using Application.Services;

namespace Application.UseCases.NextText
{
    public class NextTextUseCase : INextTextUseCase
    {
        private readonly ITextManageService textManageService;
        private readonly ITextStorageService textStorageService;

        public NextTextUseCase(ITextManageService textManageService, ITextStorageService textStorageService)
        {
            this.textManageService = textManageService;
            this.textStorageService = textStorageService;
        }

        public NextTextResponse GetNextText(NextTextRequest nextTextRequest)
        {
            List<string> textNames = textManageService.GetTexts(TextCategory.AllTexts);
            if (textNames.Count == 0)
                return new NextTextResponse(NextTextExists: false, NextTextContent: null, TextCategory: TextCategory.AllTexts, TextName: null);
            
            var nextTextName = textManageService.GetNexOrPrevPath(textNames, nextTextRequest.CurrentTextName, 1);

            if (string.IsNullOrWhiteSpace(nextTextName))
                return new NextTextResponse(NextTextExists: false, NextTextContent: null, TextCategory: TextCategory.AllTexts, TextName: null );

            var textContent = textStorageService.GetText(nextTextName);

            var textCategory = textManageService.GetTextCategory(nextTextName);

            return new NextTextResponse( NextTextContent: textContent, TextCategory: textCategory, NextTextExists: true, TextName: nextTextName );
        }
    }
}

using Application.Enums;

namespace Application.Services
{
    public interface ITextManageService
    {
        string? GetNexOrPrevPath(List<string> textNames, string currentTextName, int offset);
        TextCategory GetTextCategory(string textName);
        List<string> GetTexts(TextCategory textGroup);
    }
}

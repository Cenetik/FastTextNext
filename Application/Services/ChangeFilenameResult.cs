namespace Application.Services
{
    public record ChangeFilenameResult(string Filename, bool CheckButtonFavoriteChecked, bool CheckButtonTaskChecked, bool CheckButtonDoneTaskChecked);
}

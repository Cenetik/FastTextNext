namespace Application.Services
{
    public interface IChangeFileNameService
    {
        ChangeFilenameResult Change(string filename);
    }

    public class ChangeFileNameService : IChangeFileNameService
    {
        public ChangeFilenameResult Change(string filename)
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
        }
    }
}

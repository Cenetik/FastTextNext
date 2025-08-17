using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

    public class FileNameGenerateService : IFileNameGenerateService
    {
        public string GenerateFileNameForShowing(string filename)
        {
            var mode = "";
            if (filename == null)
                filename = "";

            if (filename.Contains("_t"))
                mode += " - Задача ";
            if (filename.Contains("_d"))
                mode += " - Задача выполнена";
            if (filename.Contains("_f"))
                mode += " - Избранная";

            var dateTimeOfFile = GetDateTimeByFileName(filename);
            var outFilename = filename;
            if (dateTimeOfFile != null)
                outFilename += string.Format(" [{0:dd.MM.yyyy HH:mm:ss.fff}]", dateTimeOfFile.Value);
            return outFilename;
        }

        private DateTime? GetDateTimeByFileName(string filename)
        {
            var onlyDate = filename.Split(new[] { '_', '.' });
            DateTime dt;
            if (DateTime.TryParseExact(onlyDate[0], "yyyyMMddHHmmssfff", null, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }
    }
}

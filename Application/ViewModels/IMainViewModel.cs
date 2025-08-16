using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public interface IMainViewModel
    {
        event Action TextContentChanged;
        string TextContent { get;set;}

    }
}

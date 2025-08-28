using Application.Enums;
using Application.UseCases.PrevText;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PrevText
{
    public record PrevTextRequest(string CurrentTextName);
    public record PrevTextResponse(bool PrevTextExists, string PrevTextContent, TextCategory TextCategory, string TextName);

    public interface IPrevTextUseCase
    {
        PrevTextResponse GetPrevText(PrevTextRequest request);
    }

}

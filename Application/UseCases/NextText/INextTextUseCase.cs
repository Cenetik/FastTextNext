using Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.NextText
{
    public record NextTextRequest(string CurrentTextName);
    public record NextTextResponse(bool NextTextExists, string NextTextContent, string TextName, TextCategory TextCategory);

    public interface INextTextUseCase
    {
        NextTextResponse GetNextText(NextTextRequest nextTextRequest);
    }
}

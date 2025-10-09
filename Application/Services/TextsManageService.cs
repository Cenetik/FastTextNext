using Application.Enums;
using Application.Helpers;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

    public class TextsManageService : ITextManageService
    {
        private readonly ITextStorageService textStorageService;

        public TextsManageService(ITextStorageService textStorageService)
        {
            this.textStorageService = textStorageService;
        }

        public string? GetNexOrPrevPath(List<string> textNames, string currentTextName, int offset)
        {
            if (string.IsNullOrWhiteSpace(currentTextName))
            {
                if (offset > 0)
                    return null;
                if (textNames.Count > 0)
                    return textNames[textNames.Count - 1];
                return null;
            }
            
            var index = textNames.IndexOf(currentTextName);
            if (index < 0)
            {
                if (textNames.Count > 0)
                    return textNames[textNames.Count - 1];
                return null;
            }

            var nextIndex = index + offset;
            if ((offset > 0 && textNames.Count <= nextIndex) ||
                (offset < 0 && nextIndex < 0))
                return null;

            return textNames[nextIndex];
        }

        public TextCategory GetTextCategory(string textName)
        {
            var category = TextCategory.AllTexts;
            if (textName == null)
                textName = "";

            if (textName.Contains("_t"))
                category = TextCategory.Tasks;
            if (textName.Contains("_d"))
                category = TextCategory.DoneTasks;
            if (textName.Contains("_f"))
                category = TextCategory.Favoites;
            return category;
        }

        public List<TextEntity> GetTextEntities(TextCategory textGroup)
        {
            var textNames = GetTexts(textGroup);

            var textEntities = new List<TextEntity>();
            foreach (var textName in textNames)
            {
                var textEntity = new TextEntity();
                textEntity.Name = textName;
                textEntity.DateCreate = TextNameGenerator.GetDateTimeFromFileName(textName)??DateTime.Now;
                textEntity.TextContent = textStorageService.GetText(textName);
                textEntities.Add(textEntity);
            }
            return textEntities;
        }

        public List<string> GetTexts(TextCategory textGroup)
        {
            List<string> textNames = textStorageService.GetAllTextsNames();            
            
            if (textGroup == TextCategory.Favoites)
                textNames = textNames.Where(p => p.Contains("_f")).ToList();
            if (textGroup == TextCategory.Tasks)
                textNames = textNames.Where(p => p.Contains("_t")).ToList();
            if (textGroup == TextCategory.DoneTasks)
                textNames = textNames.Where(p => p.Contains("_d")).ToList();
            return textNames;
        }
    }
}

using Application.Helpers;
using Application.Services;
using Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class MainViewBusinessLogic
    {
        private readonly IMainViewModel vm;
        private readonly IBaseTimer timer;
        private readonly ITextStorageService textStorageService;
        private bool _wasChanged = false;
        private bool _withoutActivity = true;

        public MainViewBusinessLogic(IMainViewModel vm, IBaseTimer timer, ITextStorageService textStorageService)
        {
            this.vm = vm;
            this.timer = timer;
            this.textStorageService = textStorageService;
            vm.TextContentChanged += () => TextContentChanged();

            InitAndStartTimer();
        }

        private void InitAndStartTimer()
        {
            timer.SetIntervalAndAction(1000, OnTimerTick);
            timer.Start();
        }

        private void TextContentChanged()
        {
            _wasChanged = true;
            _withoutActivity = false;
        }

        private void OnTimerTick()
        {
            if (_withoutActivity)
                Saving();
            else
                _withoutActivity = true;
        }

        private void Saving()
        {
            var textContent = vm.TextContent;
            textStorageService.Save("myfile.txt", textContent);
        }
    }
}

using Application.Helpers;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Helpers
{
    public class AvaloniaTimer : Application.Helpers.IBaseTimer
    {
        private Action? onTickFunc;
        DispatcherTimer timer = null;

        public AvaloniaTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            onTickFunc?.Invoke();
        }
        
        public void SetIntervalAndAction(int milliseconds,Action onTickFunc)
        {
            timer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            this.onTickFunc = onTickFunc;
        }

        public void Start()
        {
            timer.Start();
        }
    }
}


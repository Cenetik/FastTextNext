using Application;
using Application.Helpers;
using Application.Services;
using Application.UseCases.NextText;
using Application.UseCases.PrevText;
using Application.UseCases.SavingLogic;
using Application.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FastTextNext.Services;
using FastTextNext.Utils;
using FastTextNext.ViewModels;
using FastTextNext.Views;
using Infrastracture;
using Infrastracture.Helpers;
using Infrastracture.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace FastTextNext;

public partial class App : Avalonia.Application
{
    private IHost? _host;

    // Добавляем статическое свойство для доступа к сервисам
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);


        // Настройка DI контейнера
        _host = HostInitializer.InitHostAndServices();

        GlobalData.SaveTextStorageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FastTextNext");

        Services = _host.Services;
        var vm = Services.GetRequiredService<MainViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };            
            
            desktop.Exit += Desktop_Exit;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm
            };            
        }

        

        base.OnFrameworkInitializationCompleted();
    }

    private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _host?.Dispose();
    }

    
}

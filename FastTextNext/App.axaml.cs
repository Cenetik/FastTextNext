using Application;
using Application.Helpers;
using Application.Services;
using Application.UseCases.SavingLogic;
using Application.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using FastTextNext.Services;
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
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            })
            .Build();

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

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Конфигурация
        services.AddSingleton(configuration);

        services.AddSingleton<IMainViewModel,MainViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ISavingLogicUseCase, SavingLogicUseCase>();

        // Для декорирования можно также юзать dotnet add package Scrutor
        // и потом 
        // services.AddScoped<ITextStorageService, FileStorageService>();
        // services.Decorate<ITextStorageService, FileStorageWithFolderCheckService>();
        services.AddTransient<FileStorageService>();
        services.AddTransient<ITextStorageService>(p =>
        {
            var fileStorageService = p.GetService<FileStorageService>();
            return new FileStorageWithFolderCheckService(fileStorageService);
        });        
        services.AddTransient<IBaseTimer, AvaloniaTimer>();


        //var str = configuration.GetConnectionString("DefaultConnection");
        //// База данных
        //services.AddDbContext<AppDbContext>(options =>
        //    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        //// Репозитории и Unit of Work
        //services.AddScoped<IWordRepository, WordRepository>();
        //services.AddScoped<IRepository<AppSettings>, Repository<AppSettings>>();
        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Бизнес-сервисы

        //services.AddSingleton<IStopWordsService, StopWordsService>();
        //services.AddScoped<ITextProcessingService, TextProcessingService>();
        //services.AddScoped<IWordDictionaryService, WordDictionaryService>();
        //services.AddScoped<ITranslatorService, TranslatorService>();
        //services.AddScoped<IExportService, ExportService>();
        //services.AddScoped<IConfigurationService, ConfigurationService>();

        //// ViewModels
        //services.AddTransient<MainWindowViewModel>();
        //services.AddTransient<WordDictionaryViewModel>();
        //services.AddTransient<SettingsViewModel>();

        //// Инициализация БД при старте
        //services.AddHostedService<DatabaseInitializationService>();

        //// Инициализация сервисов
        //services.AddHostedService<ServiceInitializationService>();
    }
}

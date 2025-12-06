using Application.Helpers;
using Application.Services;
using Application.UseCases.NextText;
using Application.UseCases.PrevText;
using Application.UseCases.SavingLogic;
using Application.ViewModels;
using FastTextNext.ViewModels;
using Infrastracture.Helpers;
using Infrastracture.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastTextNext.Utils
{
    public static class HostInitializer
    {
        public static IHost? InitHostAndServices()
        {
            var host = Host.CreateDefaultBuilder()
           .ConfigureAppConfiguration((context, config) =>
           {
               config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
           })
           .ConfigureServices((context, services) =>
           {
               ConfigureServices(services, context.Configuration);
           })
           .Build();

            return host;
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Конфигурация
            services.AddSingleton(configuration);

            services.AddSingleton<IMainViewModel, MainViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<ISavingLogicUseCase, SavingLogicUseCase>();
            services.AddTransient<INextTextUseCase, NextTextUseCase>();
            services.AddTransient<IPrevTextUseCase, PrevTextUseCase>();
            services.AddTransient<ITextManageService, TextsManageService>();

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
}

using Avalonia.Rendering;
using FastTextNext.Utils;
using FastTextNext.ViewModels;
using Infrastracture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.IntegrationalTests
{
    /// <summary>
    /// Тестируем работу приложения для случая с файловым хранилищем
    /// </summary>
    public class FileStorageTests
    {
        private IHost _host;
        [SetUp]
        public void Setup()
        {
            SetTestStorage();
            _host = HostInitializer.InitHostAndServices();            
        }

        private void SetTestStorage()
        {
            GlobalData.SaveTextStorageFolder = "TestStorage";
            if (Directory.Exists(GlobalData.SaveTextStorageFolder))
                Directory.Delete(GlobalData.SaveTextStorageFolder,recursive:true);

            Directory.CreateDirectory(GlobalData.SaveTextStorageFolder);            
        }

        [Test]
        public async Task TestSetFavorite()
        {
            // Arrange
            var textContent = "test_text";

            var services = _host.Services;
            var vm = services.GetRequiredService<MainViewModel>();
            vm.TextContent = textContent;            

           // await Task.Delay(TimeSpan.FromSeconds(5));

            // Act            
            // Нужно оба действия, т.к. сначала мы присваиваем значение свойству, а потом эмулируем нажатие на кнопку
            vm.IsFavoriteButtonChecked = true;             
            vm.SetFavoriteText();

            // Assert
            Assert.IsTrue(vm.IsFavoriteButtonChecked);

            var files = Directory.GetFiles(GlobalData.SaveTextStorageFolder);
            Assert.That(files.Count(), Is.EqualTo(1));

            var text = File.ReadAllText(files[0]);
            Assert.That(text, Is.EqualTo(textContent));

            Assert.That(vm.TextContent, Is.EqualTo(textContent));

            Assert.IsTrue(files[0].Contains("_f"));
        }

        [TearDown]
        public void Dispose()
        {
            _host.Dispose();
        }
    }
}

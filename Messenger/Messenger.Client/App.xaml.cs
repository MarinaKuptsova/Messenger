using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Messenger.Client.ViewModel;

namespace Messenger.Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainViewModel model = new MainViewModel();
            Application.Current.MainWindow = model.Window;
            model.SetScreen();
            model.Window.Show();
        }
    }
}

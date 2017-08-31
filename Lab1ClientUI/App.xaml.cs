using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lab1ClientUI.View;
using Lab1ClientUI.ViewModel;

namespace Lab1ClientUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var window = new HostView();
            var vm = new HostViewModel();
            window.DataContext = vm;
            window.Show();
        }
    }
}

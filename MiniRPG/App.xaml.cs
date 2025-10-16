using System;
using System.IO;
using System.Windows;
using MiniRPG.Models;
using MiniRPG.Services;
using MiniRPG.ViewModels;

namespace MiniRPG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Player player;
            if (File.Exists("player_save.json"))
            {
                player = SaveLoadService.LoadPlayer() ?? new Player();
            }
            else
            {
                player = new Player();
            }
            var mainWindow = new MainWindow();
            if (mainWindow.DataContext is MainViewModel vm)
            {
                vm.CurrentPlayer = player;
            }
            mainWindow.Show();
            // TODO: Replace with proper title screen offering "New Game" or "Continue"
        }
    }
}

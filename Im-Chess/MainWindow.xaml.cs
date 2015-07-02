using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Datalayer;
using Datalayer.Entities;

namespace Im_Chess
{
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var db = ImChessModel.Instance;
            if (db.ApplicationSettings.Any())
            {
                var settings = db.ApplicationSettings.ToList();
                Left = settings.First(s => s.Name == "Window Left").Value;
                Top = settings.First(s => s.Name == "Window Top").Value;
                Width = settings.First(s => s.Name == "Window Width").Value;
                Height = settings.First(s => s.Name == "Window Height").Value;
            }
            MainBoard.NewGame();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var db = ImChessModel.Instance;

            List<ApplicationSetting> settings;
            if (db.ApplicationSettings.Any())
            {
                settings = db.ApplicationSettings.ToList();
                settings.First(s => s.Name == "Window Left").Value = Convert.ToInt32(Left);
                settings.First(s => s.Name == "Window Top").Value = Convert.ToInt32(Top);
                settings.First(s => s.Name == "Window Width").Value = Convert.ToInt32(Width);
                settings.First(s => s.Name == "Window Height").Value = Convert.ToInt32(Height);
            }
            else
            {
                settings = new List<ApplicationSetting>
                {
                    new ApplicationSetting() { Name = "Window Left", Value = Convert.ToInt32(Left) },
                    new ApplicationSetting() { Name = "Window Top", Value = Convert.ToInt32(Top) },
                    new ApplicationSetting() { Name = "Window Width", Value = Convert.ToInt32(Width) },
                    new ApplicationSetting() { Name = "Window Height", Value = Convert.ToInt32(Height) }
                };
                db.ApplicationSettings.AddRange(settings);
            }
            db.SaveChanges();

            MainBoard.SaveGame();
        }

        private void NewGame_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = false;
        }

        private void NewGameBlack_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = false;
            MainBoard.MakeEngineMove();
        }

        private void NewGameTwoPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = true;
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FlipBoard_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.FlipBoard();
        }

        private void ToggleAutoFlip(object sender, RoutedEventArgs e)
        {
            MainBoard.AutoFlip = !MainBoard.AutoFlip;
        }
    }
}

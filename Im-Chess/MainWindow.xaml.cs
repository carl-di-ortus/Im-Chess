using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Datalayer;
using Datalayer.Entities;
using Microsoft.Win32;

namespace Im_Chess
{
    public partial class MainWindow : RibbonWindow
    {
        private ImChessModel _db;

        public MainWindow()
        {
            InitializeComponent();
            _db = ImChessModel.Instance;
            if (_db.ApplicationSettings.Any())
            {
                var settings = _db.ApplicationSettings.ToList();
                Left = Convert.ToDouble(settings.First(s => s.Name == "Window Left").Value);
                Top = Convert.ToDouble(settings.First(s => s.Name == "Window Top").Value);
                Width = Convert.ToDouble(settings.First(s => s.Name == "Window Width").Value);
                Height = Convert.ToDouble(settings.First(s => s.Name == "Window Height").Value);
                
                var setting = _db.ApplicationSettings.FirstOrDefault(s => s.Name == "Engine Path");
                if (setting != null)
                {
                    var engineName = MainBoard.SetEngine(setting.Value);
                    SelectEngineButton.Label = engineName;
                    EngineTab.Header = engineName;
                }
            }
            MainBoard.NewGame();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            List<ApplicationSetting> settings;
            if (_db.ApplicationSettings.Any())
            {
                settings = _db.ApplicationSettings.ToList();
                settings.First(s => s.Name == "Window Left").Value = Left.ToString();
                settings.First(s => s.Name == "Window Top").Value = Top.ToString();
                settings.First(s => s.Name == "Window Width").Value = Width.ToString();
                settings.First(s => s.Name == "Window Height").Value = Height.ToString();
            }
            else
            {
                settings = new List<ApplicationSetting>
                {
                    new ApplicationSetting() { Name = "Window Left", Value = Left.ToString() },
                    new ApplicationSetting() { Name = "Window Top", Value = Top.ToString() },
                    new ApplicationSetting() { Name = "Window Width", Value = Width.ToString() },
                    new ApplicationSetting() { Name = "Window Height", Value = Height.ToString() }
                };
                _db.ApplicationSettings.AddRange(settings);
            }
            _db.SaveChanges();

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

        private void SelectEngine_OnClick(object sender, RoutedEventArgs e)
        {
            var setting = _db.ApplicationSettings.FirstOrDefault(s => s.Name == "Engine Path");

            var fileChooser = new OpenFileDialog { Filter = "Executables (*.exe)|*.exe", Multiselect = false, Title = "Select engine executable file" };
            fileChooser.ShowDialog();
            
            var filepath = fileChooser.FileName;
            if (String.IsNullOrEmpty(filepath))
            {
                return;
            }

            try
            {
                var engineName = MainBoard.SetEngine(filepath);
                SelectEngineButton.Label = engineName;
                EngineTab.Header = engineName;

                if (setting != null)
                {
                    setting.Value = filepath;
                }
                else
                {
                    _db.ApplicationSettings.Add(new ApplicationSetting() { Name = "Engine Path", Value = filepath });
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error reading executable", "Unrecognized engine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

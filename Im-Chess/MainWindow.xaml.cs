using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Ribbon;
using Microsoft.Win32;

namespace Im_Chess
{
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var engine = Properties.Settings.Default.EnginePath;
            if (!String.IsNullOrEmpty(engine))
            {
                var engineName = MainBoard.SetEngine(engine);
                SelectEngineButton.Label = engineName;
                EngineTab.Header = engineName;
            }
            MainBoard.NewGame();
        }

        public void AppendEngineOutput(string message)
        {
            EngineOuput.Text = message + EngineOuput.Text;
        }

        public void ClearEngineOutput()
        {
            EngineOuput.Text = String.Empty;
        }

        private void EngineGo_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.MakeEngineMove();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            MainBoard.SaveGame();
        }

        private void NewGame_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = false;
            EngineOuput.Text = "";
        }

        private void NewGameBlack_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = false;
            EngineOuput.Text = "";
            MainBoard.MakeEngineMove();
        }

        private void NewGameTwoPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.NewGame();
            MainBoard.TwoPlayer = true;
            EngineOuput.Text = "";
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
                Properties.Settings.Default.EnginePath = filepath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error reading executable", "Unrecognized engine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using Datalayer.Entities;
using Application = System.Windows.Application;
using Control = System.Windows.Controls.Control;
using FlowDirection = System.Windows.FlowDirection;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Forms.Label;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;

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
                InitEngineOptions();
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
                InitEngineOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error reading executable", "Unrecognized engine", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitEngineOptions()
        {
            EngineOptions.Items.Clear();
            EngineOptions.GroupSizeDefinitions = new RibbonGroupSizeDefinitionBaseCollection();
            var options = MainBoard.GetEngineOptions();
            var oplist = new List<object>();
            double max;
            foreach (var option in options)
            {
                if (option.Type == EngineOptionType.Check)
                {
                    oplist.Add(new RibbonCheckBox
                    {
                        Label = option.Name,
                        IsChecked = Convert.ToBoolean(option.Value),
                        Margin = new Thickness(5, 0, 5, 0),
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        FlowDirection = FlowDirection.RightToLeft,
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    });
                }
                if (option.Type == EngineOptionType.Spin)
                {
                    oplist.Add(new Spinner
                    {
                        SpinnerBlock = { Text = option.Name },
                        IntegerUpDown =
                        {
                            Value = Convert.ToInt32(option.Value),
                            Increment = 1,
                            Minimum = Convert.ToInt32(option.MinValue),
                            Maximum = Convert.ToInt32(option.MaxValue)
                        },
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    });
                }
                if (option.Type == EngineOptionType.Combo)
                {
                    //todo
                }
                if (option.Type == EngineOptionType.Button)
                {
                    oplist.Add(new RibbonButton
                    {
                        Label = option.Name,
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50
                    });
                }
                if (option.Type == EngineOptionType.String)
                {
                    oplist.Add(new RibbonTextBox
                    {
                        Label = option.Name,
                        Text = option.Value,
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    });
                }

                if (oplist.Count == 3)
                {
                    max = oplist.Max(i => ((Control)i).MinWidth);
                    foreach (Control item in oplist)
                    {
                        item.MinWidth = max;
                        EngineOptions.Items.Add(item);
                    }
                    oplist.Clear();
                }
            }

            if (oplist.Count > 0)
            {
                max = oplist.Max(i => ((Control)i).MinWidth);
                foreach (Control item in oplist)
                {
                    item.MinWidth = max;
                    EngineOptions.Items.Add(item);
                }
            }
        }
    }
}

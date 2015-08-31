using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Forms;
using Datalayer.Entities;
using Xceed.Wpf.Toolkit;
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

        private void EngineOptionChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as RibbonCheckBox;
            if (checkbox != null)
            {
                MainBoard.SetEngineOption(checkbox.Label, checkbox.IsChecked.ToString());
                return;
            }

            var spinner = sender as IntegerUpDown;
            if (spinner != null)
            {
                MainBoard.SetEngineOption(spinner.Uid, spinner.Value.ToString());
                return;
            }

            // todo
            //var combo = sender as Combo;
            //if (combo != null)
            //{
            //    MainBoard.SetEngineOption(combo.Text, combo.Value.ToString());
            //    return;
            //}

            var button = sender as RibbonButton;
            if (button != null)
            {
                MainBoard.SetEngineOption(button.Label, String.Empty);
                return;
            }

            var textbox = sender as RibbonTextBox;
            if (textbox != null)
            {
                MainBoard.SetEngineOption(textbox.Label, textbox.Text);
            }
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FlipBoard_OnClick(object sender, RoutedEventArgs e)
        {
            MainBoard.FlipBoard();
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
                    var checkbox = new RibbonCheckBox
                    {
                        Label = option.Name,
                        IsChecked = Convert.ToBoolean(option.Value),
                        Margin = new Thickness(5, 0, 5, 0),
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        FlowDirection = FlowDirection.RightToLeft,
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    };
                    checkbox.Checked += EngineOptionChanged;
                    checkbox.Unchecked += EngineOptionChanged;
                    oplist.Add(checkbox);
                }
                if (option.Type == EngineOptionType.Spin)
                {
                    var spinner = new Spinner
                    {
                        SpinnerBlock = { Text = option.Name },
                        IntegerUpDown = { Uid = option.Name, Value = Convert.ToInt32(option.Value), Increment = 1, Minimum = Convert.ToInt32(option.MinValue), Maximum = Convert.ToInt32(option.MaxValue) },
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    };
                    spinner.IntegerUpDown.ValueChanged += EngineOptionChanged;
                    oplist.Add(spinner);
                }
                if (option.Type == EngineOptionType.Combo)
                {
                    //todo
                }
                if (option.Type == EngineOptionType.Button)
                {
                    var button = new RibbonButton
                    {
                        Label = option.Name,
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50
                    };
                    button.Click += EngineOptionChanged;
                    oplist.Add(button);
                }
                if (option.Type == EngineOptionType.String)
                {
                    var textbox = new RibbonTextBox
                    {
                        Label = option.Name,
                        Text = option.Value,
                        Margin = new Thickness(5, 0, 5, 0),
                        MinWidth = TextRenderer.MeasureText(option.Name, (new Label()).Font).Width + 50,
                    };
                    textbox.TextChanged += EngineOptionChanged;
                    oplist.Add(textbox);
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

        private void ToggleAutoFlip(object sender, RoutedEventArgs e)
        {
            MainBoard.AutoFlip = !MainBoard.AutoFlip;
        }
    }
}

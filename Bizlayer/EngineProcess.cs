using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Datalayer.Entities;
using Datalayer.Repositories;

namespace Bizlayer
{
    public class EngineProcess
    {
        private readonly EngineRepo _repo;
        private readonly Process _engineProcess;

        public EngineProcess(string path)
        {
            _repo = new EngineRepo();
            _engineProcess = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    CreateNoWindow = true,
                    UseShellExecute = false, 
                    RedirectStandardInput = true, 
                    RedirectStandardOutput = true
                }
            };
            _engineProcess.Start();

            Engine = new Engine();
            StartUciMode();
        }

        public Engine Engine { get; private set; }

        public EngineOption Option
        {
            set
            {
                _engineProcess.StandardInput.WriteLine("setoption name " + value.Name + " value " + value.Value);
                _repo.SetOption(Engine.Name, value.Name, value.Value);
            }
        }

        public List<EngineOption> Options
        {
            get
            {
                return _repo.Get(Engine.Name).Options;
            }
            set
            {
                var existingEngine = _repo.Get(Engine.Name);
                foreach (var option in existingEngine.Options.Where(o => value.Any(on => on.Name == o.Name && on.Value != o.Value)))
                {
                    Option = option;
                }
            }
        }

        public void NewGame()
        {
            _engineProcess.StandardInput.WriteLine("ucinewgame");
        }

        public IEnumerable<string> Go(string position)
        {
            _engineProcess.StandardInput.WriteLine("ucinewgame");
            _engineProcess.StandardInput.WriteLine(position);
            _engineProcess.StandardInput.WriteLine("go");
            var line = "";
            while (!line.StartsWith("bestmove"))
            {
                line = _engineProcess.StandardOutput.ReadLineAsync().Result;
                yield return line;
            }
        }

        private void StartUciMode()
        {
            _engineProcess.StandardInput.WriteLineAsync("uci");
            var line = "";
            while (line != "uciok")
            {
                line = _engineProcess.StandardOutput.ReadLineAsync().Result;
                switch (line.Split(' ')[0])
                {
                    case "id":
                        ReadEngineId(line);
                        break;
                    case "option":
                        var option = ReadOptionFromStdin(line);
                        if (option != null)
                        {
                            if (Engine.Options == null)
                            {
                                Engine.Options = new List<EngineOption>();
                            }
                            Engine.Options.Add(option);
                        }
                        break;
                    case "uciok":
                        Console.WriteLine("uciok");
                        break;
                }
            }

            if (_repo.Exists(Engine.Name))
            {
                Options = _repo.Get(Engine.Name).Options;
            }
            else
            {
                _repo.Create(Engine);
            }
        }

        private void ReadEngineId(string line)
        {
            switch (line.Split(' ')[1])
            {
                case "name":
                    Engine.Name = line.Substring(8);
                    break;
                case "author":
                    Engine.Author = line.Substring(10);
                    break;
            }
        }

        private EngineOption ReadOptionFromStdin(string stdin)
        {
            var option = new EngineOption { Name = stdin.Substring(12, stdin.IndexOf(" type ") - 12) };
            var type = stdin.Split(' ')[Array.IndexOf(stdin.Split(' '), "type") + 1];
            var value = stdin.Split(' ')[Array.IndexOf(stdin.Split(' '), "default") + 1];

            var comboVars = stdin.IndexOf(" var ");
            var comboString = "";
            if (comboVars != -1)
            {
                comboString = stdin.Substring(comboVars);
            }

            switch (type)
            {
                case "check":
                    option.Type = EngineOptionType.Check;
                    option.Value = value;
                    break;
                case "spin":
                    option.Type = EngineOptionType.Spin;
                    option.Value = value;
                    option.MinValue = stdin.Split(' ')[Array.IndexOf(stdin.Split(' '), "min") + 1];
                    option.MaxValue = stdin.Split(' ')[Array.IndexOf(stdin.Split(' '), "max") + 1];
                    break;
                case "combo":
                    option.Type = EngineOptionType.Combo;
                    option.Value = value;
                    var combos = comboString.Split(new[] { " var " }, StringSplitOptions.RemoveEmptyEntries);
                    option.ComboOption = new List<EngineOptionCombo>();
                    foreach (var combo in combos)
                    {
                        option.ComboOption.Add(new EngineOptionCombo
                        {
                            Engine = Engine,
                            Option = option,
                            ComboValue = combo
                        });
                    }
                    break;
                case "button":
                    option.Type = EngineOptionType.Button;
                    break;
                case "string":
                    option.Type = EngineOptionType.String;
                    option.Value = value;
                    break;
                default:
                    Console.WriteLine("Warning! unrecognised engine option type recieved, ignoring - following engine setting will be unconfigurable:");
                    Console.WriteLine(stdin);
                    return null;
            }
            return option;
        }
    }
}
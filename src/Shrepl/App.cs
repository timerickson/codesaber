using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class App {

        public App()
        {
            _newLine = Environment.NewLine;

            _commands = new CommandCollection(this);

            _script = new Script();

            _display = new Display(_newLine, _script);
            _executor = new Executor(_commands);
            _inputService = new InputService(_display, _executor);
        }

        private readonly string _newLine;

        private readonly CommandCollection _commands;

        private readonly InputService _inputService;
        private readonly Script _script;
        private readonly Executor _executor;
        private readonly Display _display;

        private bool _isRunning;

        public const ConsoleColor DefaultConsoleColor = ConsoleColor.Gray;

        public void Run()
        {
            PrintHeader();

            _isRunning = true;

            _inputService.Buffer("using System;\r\nvar theNumber = 42;\r\ntheNumber\r\nSystem.Console.WriteLine(theNumber);\r\n");

            while (_isRunning)
                _inputService.Read();
        }

        private void PrintHeader()
        {
            var text = new StringBuilder();
            text.AppendLine("CodeSaber C# REPL by Tim Erickson (in2bits.org)");
            text.AppendLine("based on Microsoft (R) Roslyn C# Compiler version 1.2.20906.1");
            text.AppendLine("Type \"#help\" for more information.");
            _display.OutputFeedback(text.ToString());
        }

        public void Exit()
        {
            _isRunning = false;
        }
    }
}
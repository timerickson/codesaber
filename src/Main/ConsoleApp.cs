using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using CodeSaber.Shrepl;

namespace CodeSaber
{
    public class ConsoleApp
    {
        static void Main(string[] args)
        {
            var app = new ConsoleApp();
            app.Run();
        }

        public ConsoleApp()
        {
            NewLine = Environment.NewLine;
        }

        private Script _script;

        private readonly string NewLine;
        private const string InputCaret = "> ";

        private readonly Commands _commands = new Commands();

        public void Run()
        {
            const ConsoleColor headerColor = ConsoleColor.Gray;
            PrintOutput("CodeSaber C# REPL by Tim Erickson (in2bits.org)", headerColor);
            PrintOutput("based on Microsoft (R) Roslyn C# Compiler version 1.2.20906.1", headerColor);
            PrintOutput("Type \"#help\" for more information.", headerColor);

            _script = new Script(NewLine);

            Console.Write(InputCaret);

            AddExampleInput();

            while (true)
            {
                var keyInfo = Console.ReadKey();
                var c = keyInfo.KeyChar;
                var key = keyInfo.Key;

                if (key == ConsoleKey.Backspace)
                    ProcessBackspace();
                else if (key == ConsoleKey.Enter)
                    ProcessInput();
                else
                    _script.Append(c.ToString(CultureInfo.CurrentCulture));
            }
        }

        private void AddExampleInput()
        {
            var testInputLines = 
                "using System;\r\nvar theNumber = 42;\r\ntheNumber\r\nSystem.Console.WriteLine(theNumber);\r\n"
                .Split(new string[] { NewLine }, StringSplitOptions.None);

            for (var i = 0; i < testInputLines.Length; i++)
            {
                var testInputLine = testInputLines[i];
                _script.Append(testInputLine);
                Console.Write(testInputLine);
                if (i < (testInputLines.Length - 1))
                    ProcessInput();
            }
        }

        private void ProcessBackspace()
        {
            _script.RemoveInput(1);
            Console.Write(" \b");
        }

        private void ProcessInput()
        {
            Console.Write(NewLine);
            var command = GetCommand();
            if (command == null)
            {
                _script.Process();
                var state = _script.State;
                if (state.Result != null)
                {
                    PrintOutput(state.Result.ToString(), ConsoleColor.Cyan);
                }
                else if (state.RunTimeException != null)
                {
                    PrintOutput(state.RunTimeException.Message, ConsoleColor.DarkRed);
                }
                else if (state.CompileTimeException != null)
                {
                    PrintOutput(state.CompileTimeException.Message, ConsoleColor.Red);
                    if (state.CompileTimeException.Message.Contains("CS1024: Preprocessor directive expected"))
                        PrintOutput("Try '#help'...", ConsoleColor.Red);
                }

                if (state.IsExpectingClosingChar.HasValue)
                    Console.Write(new string(' ', InputCaret.Length));
                else
                    Console.Write(InputCaret);
            }
            else
            {
                var output = command.Execute();
                if (output != null)
                    PrintOutput(output.ToString(), ConsoleColor.Green);
                _script.ClearInput();
                Console.Write(InputCaret);
            }
        }

        private void PrintOutput(string output, ConsoleColor color)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            foreach (var line in output.Split(new string[] {NewLine}, StringSplitOptions.None))
                Console.WriteLine(line);
            Console.ForegroundColor = currentColor;
        }

        private ShreplCommand GetCommand()
        {
            var pendingLine = _script.PendingLine;
            if (pendingLine == null)
                return null;

            if (!pendingLine.StartsWith("#"))
                return null;

            var name = pendingLine.Substring(1).ToLowerInvariant();

            return _commands.Get(name);
        }
    }

    public class Commands
    {
        public readonly ShreplCommand ExitCommand = new ExitCommand();
        public readonly ShreplCommand PrintHelpCommand = new HelpCommand();
        public readonly ShreplCommand StartIceCommand = new StartIceCommand();
        
        private readonly Dictionary<string, ShreplCommand> _commands = new Dictionary<string, ShreplCommand>(); 

        public Commands()
        {
            Register(ExitCommand);
            Register(PrintHelpCommand);
            Register(StartIceCommand);
        }

        public void Register(ShreplCommand command)
        {
            if (_commands.ContainsKey(command.Name))
                throw new Exception("Duplicate command found");
            _commands[command.Name] = command;
        }

        public ShreplCommand Get(string name)
        {
            return _commands[name];
        }
    }

    public abstract class ShreplCommand
    {
        public abstract string Name { get; }
        public abstract object Execute(object parameter = null);
    }

    public class ExitCommand : ShreplCommand
    {
        public override string Name { get { return "exit"; } }

        public override object Execute(object parameter = null)
        {
            Environment.Exit(0);
            return null;
        }
    }

    public class HelpCommand : ShreplCommand
    {
        public override string Name { get { return "help"; } }

        public override object Execute(object parameter = null)
        {
            var help = new StringBuilder();
            help.AppendLine("REPL commands:");
            help.AppendLine("  help    Display all available REPL commands");
            help.AppendLine("  ice     Open GUI (ICE - Integrated Code Environment)");
            help.Append("  exit    Exit");
            return help.ToString();
        }
    }

    public class StartIceCommand : ShreplCommand
    {
        public override string Name { get { return "ice"; } }

        private const string IceExe = "CodeSaber.Ice.exe";

        public override object Execute(object parameter = null)
        {
            if (!File.Exists(IceExe))
            {
                Console.WriteLine("{0} not found.", IceExe);
                return null;
            }

            Process.Start(IceExe);
            return null;
        }
    }
}

using System;
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
            if (command == CommandInfo.Empty)
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

        private CommandInfo GetCommand()
        {
            var pendingLine = _script.PendingLine;
            if (pendingLine == null)
                return CommandInfo.Empty;

            if (!pendingLine.StartsWith("#"))
                return CommandInfo.Empty;

            var name = pendingLine.Substring(1).ToLowerInvariant();

            if (name == "exit")
                return Commands.ExitCommand;
            else if (name == "ice")
                return Commands.StartIceCommand;
            else if (name == "help")
                return Commands.PrintHelpCommand;

            return CommandInfo.Empty;
        }
    }

    public static class Commands
    {
        public static readonly CommandInfo ExitCommand = new CommandInfo(Exit);
        public static readonly CommandInfo PrintHelpCommand = new CommandInfo(PrintHelp);
        public static readonly CommandInfo StartIceCommand = new CommandInfo(StartIce);

        private static object Exit(object parameter)
        {
            Environment.Exit(0);
            return null;
        }

        private static object PrintHelp(object parameter)
        {
            var help = new StringBuilder();
            help.AppendLine("REPL commands:");
            help.AppendLine("  help    Display all available REPL commands");
            help.AppendLine("  ice     Open GUI (ICE - Integrated Code Environment)");
            help.Append("  exit    Exit");
            return help.ToString();
        }

        private const string IceExe = "CodeSaber.Ice.exe";

        private static object StartIce(object parameter)
        {
            if (!File.Exists(IceExe))
            {
                Console.WriteLine("{0} not found.", IceExe);
                return null;
            }

            Process.Start(IceExe);
            return null;
        }

        private static object UnknownCommand(object parameter)
        {
            var description = parameter as string;
            return description;
        }
    }

    public class CommandInfo
    {
        public Func<object, object> Command;
        public object Parameter;
        public static readonly CommandInfo Empty = new CommandInfo();

        private CommandInfo()
        {
        }

        public CommandInfo(Func<object, object> command) : this(command, null)
        {
        }

        public CommandInfo(Func<object, object> command, object parameter)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            Command = command;
            Parameter = parameter;
        }

        public object Execute()
        {
            return Command(Parameter);
        }
    }
}

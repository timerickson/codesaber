using System;
using System.Globalization;
using CodeSaber.Shrepl;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber
{
    public class App {

        public App()
        {
            NewLine = Environment.NewLine;
        }

        private Script _script;

        private readonly string NewLine;
        private const string InputCaret = "> ";

        private readonly CommandCollection _commands = new CommandCollection();

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
}
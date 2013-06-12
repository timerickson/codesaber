using System;
using System.Text;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class App {

        public App()
        {
            NewLine = Environment.NewLine;

            Script = new Script();
            _commands = new CommandCollection(this);

            Script.AppendMemberNames(_commands.GetAllNames());

            Display = new Display(this);
            Executor = new Executor(_commands);
            InputService = new InputService(this);
        }

        public readonly string NewLine;

        private readonly CommandCollection _commands;

        public InputService InputService { get; private set; }
        public Script Script { get; private set; }
        public Executor Executor { get; private set; }
        public Display Display { get; private set; }

        private bool _isRunning;

        public const ConsoleColor DefaultConsoleColor = ConsoleColor.Gray;

        public void Run()
        {
            PrintHeader();

            _isRunning = true;

            //_inputService.Buffer("using System;\r\nvar theNumber = 42;\r\ntheNumber\r\nSystem.Console.WriteLine(theNumber);\r\n");
            //_inputService.Buffer("class Foo { public void DoNothing(){}}\r\nvar foo = new Foo();\r\nfoo.DoNothing()");

            while (_isRunning)
                InputService.Read();
        }

        private void PrintHeader()
        {
            var text = new StringBuilder();
            text.AppendLine("CodeSaber C# REPL by Tim Erickson (in2bits.org)");
            text.AppendLine("based on Microsoft (R) Roslyn C# Compiler version 1.2.20906.1");
            text.AppendLine("Type \"#help\" for more information.");
            Display.OutputFeedback(text.ToString());
        }

        public void Exit()
        {
            _isRunning = false;
        }
    }
}
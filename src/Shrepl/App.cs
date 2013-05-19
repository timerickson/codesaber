using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class App {

        public App()
        {
            _newLine = Environment.NewLine;

            Commands = new CommandCollection(this);
            Script = new Script(_newLine);

            Reader = new Reader(this);
            Executor = new Executor(Commands, Script);
            Printer = new Printer(this);
        }

        private readonly CommandCollection Commands;

        private readonly Reader Reader;
        private readonly Executor Executor;
        private readonly Printer Printer;

        private readonly string _newLine;
        public string NewLine
        {
            get { return _newLine; }
        }

        public const ConsoleColor DefaultConsoleColor = ConsoleColor.Gray;

        public bool IsRunning { get; private set; }

        public Script Script { get; private set; }

        public void Run()
        {
            PrintHeader();

            IsRunning = true;

            Reader.Buffer("using System;\r\nvar theNumber = 42;\r\ntheNumber\r\nSystem.Console.WriteLine(theNumber);\r\n");

            Loop();
        }

        private void Loop()
        {
            while (IsRunning)
            {
                var input = Reader.Read();
                var output = Executor.Execute(input);
                Printer.Print(output);
            }
        }

        private void PrintHeader()
        {
            const ConsoleColor headerColor = ConsoleColor.Gray;
            Printer.PrintLine("CodeSaber C# REPL by Tim Erickson (in2bits.org)", headerColor);
            Printer.PrintLine("based on Microsoft (R) Roslyn C# Compiler version 1.2.20906.1", headerColor);
            Printer.PrintLine("Type \"#help\" for more information.", headerColor);
        }

        public void Exit()
        {
            IsRunning = false;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CodeSaber.Shrepl;

namespace CodeSaber
{
    public class ConsoleApp
    {
        private const string TestInput = "using System;\r\nvar num = 42;\r\nConsole.WriteLine(num);";

        private static Script _script;

        static void Main(string[] args)
        {
            Console.WriteLine("CodeSaber C# REPL by Tim Erickson (in2bits.org)");
            Console.WriteLine("based on Microsoft (R) Roslyn C# Compiler version 1.2.20906.1");
            Console.WriteLine("Type \"#help\" for more information.");

            _script = new Script();

            _script.AddInput(TestInput);
            Console.Write(TestInput);

            while (true)
            {
                while (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey();
                    var c = keyInfo.KeyChar;
                    var key = keyInfo.Key;
                    if (key == ConsoleKey.Backspace)
                    {
                        _script.RemoveInput(1);
                        Console.Write(" \b");
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        Console.Write("\r\n");
                        if (!ProcessCommand())
                            _script.Process();
                    }
                    else
                    {
                        _script.AddInput(c.ToString());
                    }
                }
                Thread.Sleep(0);
            }
        }

        private static bool ProcessCommand()
        {
            var pendingLine = _script.PendingLine;
            if (pendingLine == null)
                return false;

            if (pendingLine.StartsWith("#"))
            {
                var command = pendingLine.Substring(1).ToLowerInvariant();

                _script.AddInput("\r\n");

                if (command == "exit")
                    Environment.Exit(0);
                else if (command == "ice")
                    StartIce();
                else if (command == "help")
                    PrintHelp();
                else
                    Console.WriteLine(string.Format("--Unknown command: {0}", command));
                
                return true;
            }

            return false;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("REPL commands:");
            Console.WriteLine("  help    Display all available REPL commands");
            Console.WriteLine("  ice     Open GUI (ICE - Integrated Code Environment)");
            Console.WriteLine("  exit    Exit");
        }

        private const string IceExe = "CodeSaber.Ice.exe";

        private static void StartIce()
        {
            if (!File.Exists(IceExe))
            {
                Console.WriteLine("{0} not found.", IceExe);
                return;
            }

            Process.Start(IceExe);
        }
    }
}

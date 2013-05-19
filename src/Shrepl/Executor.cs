using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class Executor
    {
        private readonly CommandCollection _commands;
        private readonly Script _script;

        public Executor(CommandCollection commands, Script script)
        {
            _commands = commands;
            _script = script;
        }

        public OutputInfo Execute(string input)
        {
            var command = _commands.Get(input);
            if (command != null)
                return ProcessCommand(command, input);

            return ProcessScript(input);
        }

        private OutputInfo ProcessCommand(ShreplCommand command, string commandLine)
        {
            var output = command.Execute();
            _script.AppendExecutedCommandLine(commandLine);
            if (output == null)
                return OutputInfo.Empty;

            return new OutputInfo(output.ToString(), ConsoleColor.Green);
        }

        private OutputInfo ProcessScript(string input)
        {
            _script.AppendLine(input);
            _script.Process();
            var state = _script.State;
            if (state.Result != null)
                return new OutputInfo(state.Result.ToString(), ConsoleColor.Cyan);

            if (state.RunTimeException != null)
                return new OutputInfo(state.RunTimeException.Message, ConsoleColor.DarkRed);

            if (state.CompileTimeException != null)
            {
                var output = state.CompileTimeException.Message;
                if (state.CompileTimeException.Message.Contains("CS1024: Preprocessor directive expected"))
                    output += _script.NewLine + _script.NewLine + "Try '#help'...";
                return new OutputInfo(output, ConsoleColor.Red);
            }

            return OutputInfo.Empty;
        }
    }
}

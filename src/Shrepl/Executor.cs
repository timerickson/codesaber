using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CodeSaber.Shrepl.Commands;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace CodeSaber.Shrepl
{
    public class Executor
    {
        private readonly CommandCollection _commands;
        private readonly Display _display;
        private readonly Session _runtimeSession;
        private readonly ScriptEngine _engine;

        private readonly List<string> _members = new List<string>
            {
                "foo",
                "bar"
            };

        public Executor(CommandCollection commands, Display display)
        {
            _commands = commands;
            _display = display;
            _engine = InitEngine();
            _runtimeSession = _engine.CreateSession();
        }

        private ScriptEngine InitEngine()
        {
            var engine = new ScriptEngine();
            engine.AddReference("System");

            var currentDirectory = Environment.CurrentDirectory;
            var bin = Path.Combine(currentDirectory, "bin");
            engine.BaseDirectory = bin;

            if (!Directory.Exists(bin))
                Directory.CreateDirectory(bin);

            return engine;
        }

        public IEnumerable<string> Suggest(string start)
        {
            return _members.Where(x => x.StartsWith(start, StringComparison.CurrentCulture));
        } 

        public bool Execute(string input)
        {
            var command = _commands.Get(input);
            if (command != null)
                return ProcessCommand(command, input);

            return ProcessScript(input);
        }

        private bool ProcessCommand(ShreplCommand command, string commandLine)
        {
            try
            {
                var output = command.Execute();

                if (output != null)
                    _display.OutputResult(output);
            }
            catch (Exception ex)
            {
                _display.OutputRunTimeException(ex);
            }

            return true;
        }

        private bool ProcessScript(string input)
        {
            var state = ExecuteScript(input);

            if (state.ReturnValue != null)
            {
                _display.OutputResult(state.ReturnValue);
                return true;
            }

            if (state.RunTimeException != null)
            {
                _display.OutputRunTimeException(state.RunTimeException);
                return true;
            }

            if (state.IsExpectingClosingChar.HasValue)
            {
                return false;
            }

            if (state.CompileTimeException != null)
            {
                _display.OutputCompileTimeException(state.CompileTimeException);
                return true;
            }

            return true;
        }

        private ExecutionResult ExecuteScript(string scriptChunk)
        {
            var result = new ExecutionResult();

            try
            {
                result.Submission = _runtimeSession.CompileSubmission<object>(scriptChunk);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result.UpdateClosingExpectation(ex);
                if (!result.IsExpectingClosingChar.HasValue)
                    result.CompileTimeException = ex;
            }

            if (result.Submission != null)
            {
                try
                {
                    result.ReturnValue = result.Submission.Execute();
                }
                catch (Exception ex)
                {
                    result.RunTimeException = ex;
                }
            }

            return result;
        }
    }
}

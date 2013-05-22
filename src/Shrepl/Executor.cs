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
        private readonly Session _runtimeSession;
        private readonly ScriptEngine _engine;

        public Executor(CommandCollection commands)
        {
            _commands = commands;
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

        public ScriptChunkResult Process(string input)
        {
            var result = new ScriptChunkResult();

            var command = _commands.Get(input);
            if (command != null)
            {
                try
                {
                    result.SetExecuteAction(() => command.Execute());
                }
                catch (Exception ex)
                {
                    result.RunTimeException = ex;
                }

                return result;
            }

            try
            {
                var submission = _runtimeSession.CompileSubmission<object>(input ?? "");
                if (submission.Compilation != null && 
                    submission.Compilation.ScriptClass != null && 
                    submission.Compilation.ScriptClass.MemberNames != null)
                result.NewMemberNames = submission.Compilation.ScriptClass.MemberNames;
                result.SetExecuteAction(submission.Execute);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                result.UpdateClosingExpectation(ex);
                if (!result.IsExpectingClosingChar.HasValue)
                    result.CompileTimeException = ex;
            }

            return result;
        }
    }
}

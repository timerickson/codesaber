using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Roslyn.Compilers.CSharp;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace CodeSaber.Shrepl
{
    public class Script
    {
        public Script(string newLine)
        {
            if (string.IsNullOrEmpty(newLine))
                throw new ArgumentException("must be populated", "newLine");

            NewLine = newLine;

            Chunks = new List<string>();

            Engine = new ScriptEngine();

            Engine.AddReference("System");

            var currentDirectory = Environment.CurrentDirectory;
            var bin = Path.Combine(currentDirectory, "bin");
            Engine.BaseDirectory = bin;

            if (!Directory.Exists(bin))
                Directory.CreateDirectory(bin);

            RuntimeSession = Engine.CreateSession();

            State = new ScriptState();
        }

        private ScriptEngine Engine { get; set; }
        private Session RuntimeSession { get; set; }

        public string NewLine { get; set; }

        private List<string> Chunks { get; set; }

        private void AppendChunk(string chunk)
        {
            Chunks.Add(chunk);
        }

        public void AppendExecutedCommand(string commandInput)
        {
            AppendChunk(commandInput);
        }

        public ScriptState Process(string scriptChunk)
        {
            UpdateState(scriptChunk);

            return State;
        }

        public ScriptState State { get; set; }

        private void UpdateState(string scriptChunk)
        {
            State.Reset();

            try
            {
                State.Submission = RuntimeSession.CompileSubmission<object>(scriptChunk);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                State.UpdateClosingExpectation(ex);
                if (!State.IsExpectingClosingChar.HasValue)
                    State.CompileTimeException = ex;
            }

            if (State.Submission != null)
            {
                try
                {
                    State.Result = State.Submission.Execute();
                }
                catch (Exception ex)
                {
                    State.RunTimeException = ex;
                }
            }
        }

/*
        private static MemberDeclarationSyntax GetFinalInteractiveStatement(Submission<object> testSubmission)
        {
            var syntaxNodes = testSubmission.Compilation.ScriptClass.DeclaringSyntaxNodes;
            var lastNodeMembers = ((CompilationUnitSyntax)syntaxNodes.Last()).Members;
            if (lastNodeMembers.Any())
            {
                var lastMember = lastNodeMembers.Last();
                if (lastMember.Kind == SyntaxKind.GlobalStatement)
                {
                    var stmt = lastMember.ToString();
                    if (!stmt.EndsWith(";"))
                    {
                        return lastMember;
                    }
                }
            }
            return null;
        }
*/
    }
}

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

            Lines = new List<string>();

            Engine = new ScriptEngine();

            Engine.AddReference("System");

            var currentDirectory = Environment.CurrentDirectory;
            var bin = Path.Combine(currentDirectory, "bin");
            Engine.BaseDirectory = bin;

            if (!Directory.Exists(bin))
                Directory.CreateDirectory(bin);

            RuntimeSession = Engine.CreateSession();

            State = new InteractionState();
        }

        private ScriptEngine Engine { get; set; }
        private Session RuntimeSession { get; set; }

        public string NewLine { get; set; }

        private List<string> Lines { get; set; }
        private int _lastExecutedLineIndex = -1;

        public void AppendLine(string input)
        {
            var newLines = input.GetLines().ToList();
            if (newLines.Count > 1)
                throw new Exception("Can't add multiple lines of input");

            Lines.Add(newLines[0]);
        }

        public void AppendExecutedCommandLine(string input)
        {
            AppendLine(input);
            _lastExecutedLineIndex = Lines.Count - 1;
        }

        public InteractionState Process()
        {
            UpdateState();

            return State;
        }

        public InteractionState State { get; set; }

        private void UpdateState()
        {
            State.Reset();

            string scriptChunk = string.Join(NewLine, Lines.Skip(_lastExecutedLineIndex + 1));
            try
            {
                State.Submission = RuntimeSession.CompileSubmission<object>(scriptChunk);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                UpdateClosingExpectation(ex);
                if (!State.IsExpectingClosingChar.HasValue)
                    State.CompileTimeException = ex;
            }
            if (!State.IsExpectingClosingChar.HasValue)
                _lastExecutedLineIndex = Lines.Count - 1;

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

        private void UpdateClosingExpectation(Exception ex)
        {
            var message = ex.Message;
            char? closingChar = null;

            if (message.Contains("CS1026: ) expected"))
                closingChar = ')';
            else if (message.Contains("CS1513: } expected"))
                closingChar = '}';
            else if (message.Contains("CS1003: Syntax error, ']' expected"))
                closingChar = ']';

            if (closingChar.HasValue)
                State.IsExpectingClosingChar = closingChar.Value;
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

    public class InteractionState
    {
        public Submission<object> Submission { get; set; }
        public Exception CompileTimeException { get; set; }
        public char? IsExpectingClosingChar { get; set; }
        public object Result { get; set; }
        public Exception RunTimeException { get; set; }

        public void Reset()
        {
            Submission = null;
            CompileTimeException = null;
            IsExpectingClosingChar = null;
            Result = null;
            RunTimeException = null;
        }
    }
}

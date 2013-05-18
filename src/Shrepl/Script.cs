using System;
using System.Collections.Generic;
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

        private string NewLine { get; set; }

        private List<string> Lines { get; set; }
        public string PendingLine { get; private set; }
        private int _lastExecutedLineIndex = -1;


        public void RemoveInput(int charCount)
        {
            if (PendingLine == null)
                return;
            if (PendingLine.Length < charCount)
                return;
            PendingLine = PendingLine.Substring(0, PendingLine.Length - charCount);
        }

        public void Append(string input)
        {
            var lines = (PendingLine + input).Split(new[] { NewLine }, StringSplitOptions.None);
            var lineCount = lines.Length;
            if (lineCount > 1)
                throw new Exception("Can't add multiple lines of input");

            PendingLine = lines[0];
        }

        public InteractionState Process()
        {
            if (PendingLine != null)
            {
                Lines.Add(PendingLine);
                PendingLine = null;
            }

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
            else if (message.Contains("CS1002: ; expected"))
                closingChar = ';';

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

        public void ClearInput()
        {
            PendingLine = "";
        }
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

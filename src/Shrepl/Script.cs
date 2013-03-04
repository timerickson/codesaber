using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace CodeSaber.Shrepl
{
    public class Script
    {
        public Script()
        {
            Lines = new List<string>();
            CompiledChunks = new List<string>();
            Submissions = new List<Submission<object>>();

            Engine = new ScriptEngine();
            Engine.AddReference("System");
            var currentDirectory = Environment.CurrentDirectory;
            var bin = Path.Combine(currentDirectory, "bin");
            Engine.BaseDirectory = bin;

            if (!Directory.Exists(bin))
                Directory.CreateDirectory(bin);

            RuntimeSession = Engine.CreateSession();
        }

        private const string NewLine = "\r\n";

        private ScriptEngine Engine { get; set; }

        private Session RuntimeSession { get; set; }

        private string Input { get; set; }
        private List<string> Lines { get; set; }
        private List<string> CompiledChunks { get; set; }
        private List<Submission<object>> Submissions { get; set; } 
        private int _lastCompiledLineIndex = -1;

        public string PendingLine { get; private set; }

        public void RemoveInput(int charCount)
        {
            if (PendingLine == null)
                return;
            if (PendingLine.Length < charCount)
                return;
            PendingLine = PendingLine.Substring(0, PendingLine.Length - charCount);
            Input = Input.Substring(0, Input.Length - charCount);
        }

        public void AddInput(string input)
        {
            Input += input;

            var lines = (PendingLine + input).Split(new[] { NewLine }, StringSplitOptions.None);
            var lineCount = lines.Length;
            if (lineCount == 1)
            {
                PendingLine = lines[0];
            }
            else
            {
                Lines.AddRange(lines.Take(lineCount - 1));
                PendingLine = lines[lineCount - 1];
            }
        }

        public void Process()
        {
            if (PendingLine != null)
            {
                Lines.Add(PendingLine);
                PendingLine = null;
                Input += NewLine;
            }

            var newSubmission = GetNewSubmission();
            if (newSubmission == null)
                return;

            Submissions.Add(newSubmission);

            newSubmission.Execute();
        }

        private Submission<object> GetNewSubmission()
        {
            var previous = string.Join(NewLine, CompiledChunks);
            for (var startIndex = _lastCompiledLineIndex + 1; startIndex < Lines.Count; startIndex++)
            {
                var chunk = string.Join(NewLine, Lines.Skip(startIndex));
                try
                {
                    var script = previous + NewLine + chunk;
                    var testSession = Engine.CreateSession();
                    var submission = testSession.CompileSubmission<object>(script);
                    CompiledChunks.Add(chunk);
                    _lastCompiledLineIndex = Lines.Count - 1;
                    submission = RuntimeSession.CompileSubmission<object>(chunk);
                    return submission;
                }
                catch{}
            }

            return null;
        }
    }
}

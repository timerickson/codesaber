using System;
using System.Linq;

namespace CodeSaber.Shrepl
{
    public class ScriptTextArea : TextArea
    {
        private readonly Script _script;
        private readonly Executor _executor;
        private readonly Display _display;

        public ScriptTextArea(string newLine, int displayLineIndex, Script script, Executor executor, Display display) : base(newLine, displayLineIndex)
        {
            _script = script;
            _executor = executor;
            _display = display;

            MarkInputStart();
        }

        private const string InputStartMarker = "> ";

        private void MarkInputStart()
        {
            Console.Write(InputStartMarker);
        }

        public override void Process(ConsoleKeyInfo keyInfo)
        {
            base.Process(keyInfo);

            Suggest();
        }

        public override void LeftArrow(ConsoleKey key, ConsoleModifiers modifiers)
        {
            if (Console.CursorLeft <= InputStartMarker.Length)
                return;
            base.LeftArrow(key, modifiers);
        }

        private void Suggest()
        {
            string start = null;
            if (string.IsNullOrEmpty(Text))
                return;
            var startIndex = Text.LastIndexOf(" ");
            if (startIndex == -1)
                start = Text;
            else
                start = Text.Substring(startIndex + 1);
            if (string.IsNullOrEmpty(start))
                return;
            var suggestions = _executor.Suggest(start);
            if (!suggestions.Any())
                return;
            var first = suggestions.First();
            var end = first.Substring(start.Length);
            var col = Console.CursorLeft;
            Print(end, ConsoleColor.Yellow);
            Console.CursorLeft = col;
        }

        public override void Enter()
        {
            base.Enter();

            var result = _executor.Process(Text);
            Process(result);
        }

        private void Process(ScriptChunkResult result)
        {
            if (result.IsPendingClosing)
            {
                Print(new string(' ', InputStartMarker.Length));
                return;
            }

            IsComplete = true;
            _script.AppendChunk(this);

            if (result.CompileTimeException != null)
            {
                _display.OutputCompileTimeException(result.CompileTimeException);
                return;
            }

            result.Execute();

            if (result.RunTimeException != null)
            {
                _display.OutputRunTimeException(result.RunTimeException);
                return;
            }

            if (result.ReturnValue != null)
            {
                _display.OutputResult(result.ReturnValue);
            }
        }
    }
}
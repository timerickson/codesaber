using System;
using System.Linq;

namespace CodeSaber.Shrepl
{
    public class ScriptTextArea : TextArea
    {
        private readonly Script _script;
        private readonly Executor _executor;

        public ScriptTextArea(string newLine, int displayLineIndex, Script script, Executor executor) : base(newLine, displayLineIndex)
        {
            _script = script;
            _executor = executor;

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

            //Suggest();
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
            Print(end, ConsoleColor.Yellow);
        }

        public override void Enter()
        {
            base.Enter();
            if (_executor.Execute(Text))
            {
                IsComplete = true;
                _script.AppendChunk(this);
            }
            else
            {
                Print(new string(' ', InputStartMarker.Length));
            }
        }
    }
}
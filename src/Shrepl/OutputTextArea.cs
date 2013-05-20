using System;

namespace CodeSaber.Shrepl
{
    public class OutputTextArea : TextArea
    {
        private readonly ConsoleColor _color;

        public OutputTextArea(string newLine, int displayLineIndex, ConsoleColor color) : base(newLine, displayLineIndex)
        {
            _color = color;
        }

        public override void Append(string input)
        {
            if (IsComplete)
                throw new Exception("Can't Append to OutputTextArea more than once");
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = _color;
            base.Append(input);
            Console.ForegroundColor = currentColor;
            Print(NewLine);
            IsComplete = true;
        }
    }
}
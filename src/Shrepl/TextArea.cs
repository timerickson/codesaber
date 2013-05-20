using System;

namespace CodeSaber.Shrepl
{
    public abstract class TextArea
    {
        private readonly int _displayLineIndex;
        protected readonly string NewLine;

        protected TextArea(string newLine, int displayLineIndex)
        {
            NewLine = newLine;
            _displayLineIndex = displayLineIndex;
        }

        public bool IsComplete { get; protected set; }

        public string Text { get; set; }

        public virtual void Process(ConsoleKeyInfo keyInfo)
        {
            var key = keyInfo.Key;

            if (HandleArrow(key))
                return;

            if (IsComplete)
                return;

            if (key == ConsoleKey.Enter)
                Enter();
            else if (key == ConsoleKey.Backspace)
                Backspace();
            else
                Append(new string(keyInfo.KeyChar, 1));
        }

        private bool HandleArrow(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    LeftArrow();
                    break;
                case ConsoleKey.RightArrow:
                    RightArrow();
                    break;
                case ConsoleKey.UpArrow:
                    UpArrow();
                    break;
                case ConsoleKey.DownArrow:
                    DownArrow();
                    break;
                default:
                    return false;
            }

            return true;
        }

        public virtual void LeftArrow()
        {
            
        }

        public virtual void RightArrow()
        {
            
        }

        public virtual void UpArrow()
        {
            
        }

        public virtual void DownArrow()
        {
            
        }

        public virtual void Append(string input)
        {
            Text += input;
            Print(input);
        }

        public void Backspace()
        {
            if (string.IsNullOrEmpty(Text))
                return;
            Text = Text.Substring(0, Text.Length - 1);
            Console.Write("\b \b");
        }

        public virtual void Enter()
        {
            Print(NewLine);
        }

        protected void Print(string text, ConsoleColor? color = null)
        {
            var currentColor = Console.ForegroundColor;
            if (color.HasValue)
                Console.ForegroundColor = color.Value;
            Console.Write(text);
            Console.ForegroundColor = currentColor;
        }
    }
}
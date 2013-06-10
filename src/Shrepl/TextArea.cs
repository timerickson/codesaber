using System;
using System.Linq;

namespace CodeSaber.Shrepl
{
    public abstract class TextArea
    {
        private readonly int _displayLineIndex;
        protected readonly string NewLine;
        protected readonly string TabString = "  ";

        protected TextArea(string newLine, int displayLineIndex)
        {
            NewLine = newLine;
            _displayLineIndex = displayLineIndex;
        }

        public bool IsComplete { get; protected set; }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (string.Equals(_text, value))
                    return;
                _text = value;
                //UpdateLines();
            }
        }

        //private string[] Lines { get; set; }

        //private void UpdateLines()
        //{
        //    Lines = Text.GetLines().ToArray();
        //}

        public virtual void Process(ConsoleKeyInfo keyInfo)
        {
            var key = keyInfo.Key;

            if (HandleArrow(keyInfo))
                return;

            if (IsComplete)
                return;

            if (key == ConsoleKey.Enter)
                Enter();
            else if (key == ConsoleKey.Backspace)
                Backspace();
            else
                AppendTyping(new string(keyInfo.KeyChar, 1));
        }

        private bool HandleArrow(ConsoleKeyInfo keyInfo)
        {
            var key = keyInfo.Key;
            switch (keyInfo.Key)
            {
                case ConsoleKey.Tab:
                    Tab(key, keyInfo.Modifiers);
                    break;
                case ConsoleKey.Escape:
                    Escape(key, keyInfo.Modifiers);
                    break;
                case ConsoleKey.LeftArrow:
                    LeftArrow(key, keyInfo.Modifiers);
                    break;
                case ConsoleKey.RightArrow:
                    RightArrow(key, keyInfo.Modifiers);
                    break;
                case ConsoleKey.UpArrow:
                    UpArrow(key, keyInfo.Modifiers);
                    break;
                case ConsoleKey.DownArrow:
                    DownArrow(key, keyInfo.Modifiers);
                    break;
                default:
                    return false;
            }

            return true;
        }

        protected virtual void Tab(ConsoleKey key, ConsoleModifiers modifiers)
        {
            Append(TabString);
        }

        public virtual void Escape(ConsoleKey key, ConsoleModifiers modifiers)
        {
            
        }

        public virtual void LeftArrow(ConsoleKey key, ConsoleModifiers modifiers)
        {
            Console.CursorLeft--;
        }

        public virtual void RightArrow(ConsoleKey key, ConsoleModifiers modifiers)
        {
            Console.CursorLeft++;
        }

        public virtual void UpArrow(ConsoleKey key, ConsoleModifiers modifiers)
        {
            if (Console.CursorTop == _displayLineIndex)
                return;
            Console.CursorTop--;
        }

        public virtual void DownArrow(ConsoleKey key, ConsoleModifiers modifiers)
        {
            if (Console.CursorTop == (_displayLineIndex + Text.GetLines().Count() - 1))
                return;
            Console.CursorTop++;
        }

        public virtual void AppendTyping(string input)
        {
            Append(input);
        }

        public virtual void Append(string input)
        {
            Text += input;
            Print(input);
        }

        public virtual void Backspace()
        {
            if (string.IsNullOrEmpty(Text))
                return;
            Text = Text.Substring(0, Text.Length - 1);
            Print("\b \b");
        }

        public virtual void Enter()
        {
            //Text += NewLine;
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
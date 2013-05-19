using System;

namespace CodeSaber.Shrepl
{
    public class InputArea
    {
        private readonly string _newLine;

        public InputArea(string newLine)
        {
            _newLine = newLine;
            MarkInputStart();
        }

        private const string InputStartMarker = "> ";

        public bool IsComplete { get; set; }

        public string Text { get; set; }

        private void MarkInputStart()
        {
            Console.Write(InputStartMarker);
        }

        public void Append(BufferedInput input)
        {
            Text += input.Input;
            Console.Write(input.Input);
            if (input.IsComplete)
                Enter();
        }

        public void Append(ConsoleKeyInfo keyInfo)
        {
            Text += new string(keyInfo.KeyChar, 1);
            Console.Write(keyInfo.KeyChar);
        }

        public void Backspace()
        {
            if (string.IsNullOrEmpty(Text))
                return;
            Text = Text.Substring(0, Text.Length - 1);
            Console.Write("\b \b");
        }

        public void Enter()
        {
            IsComplete = true;
            Console.Write(_newLine);
        }

        public void Continue()
        {
            IsComplete = false;
            Console.Write(new string(' ', InputStartMarker.Length));
        }
    }
}
using System;

namespace CodeSaber.Shrepl
{
    public class InputArea
    {
        private readonly string _newLine;

        public InputArea(string newLine)
        {
            _newLine = newLine;
        }

        private const string InputStartMarker = "> ";

        public bool IsComplete { get; set; }

        public string Text { get; set; }

        private bool _hasMarkedInputStart;

        public void ReadKey()
        {
            MarkInputStart();

            var keyInfo = Console.ReadKey(true);
            var key = keyInfo.Key;
            if (key == ConsoleKey.Enter)
                Enter();
            else if (key == ConsoleKey.Backspace)
                Backspace();
            else
                Append(keyInfo);
        }

        private void MarkInputStart()
        {
            if (!_hasMarkedInputStart)
            {
                Console.Write(InputStartMarker);
                _hasMarkedInputStart = true;
            }
        }

        public void Append(BufferedInput input)
        {
            MarkInputStart();

            Text += input.Input;
            Console.Write(input.Input);
            if (input.IsComplete)
                Enter();
        }

        private void Append(ConsoleKeyInfo keyInfo)
        {
            Text += new string(keyInfo.KeyChar, 1);
            Console.Write(keyInfo.KeyChar);
        }

        private void Backspace()
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

        public void Reset()
        {
            IsComplete = false;
            Text = null;
            _hasMarkedInputStart = false;
        }
    }
}
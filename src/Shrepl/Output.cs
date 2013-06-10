using System;

namespace CodeSaber.Shrepl
{
    public class Output
    {
        public string Text;
        public ConsoleColor Color = App.DefaultConsoleColor;

        private Output(){}

        public Output(char c) : this(new string(c, 1))
        {
        }

        public Output(string s)
        {
            Text = s;
        }

        public Output(string s, ConsoleColor color) : this(s)
        {
            Color = color;
        }

        public static readonly Output Empty = new Output();
    }
}
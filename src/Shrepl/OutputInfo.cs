using System;

namespace CodeSaber.Shrepl
{
    public class OutputInfo
    {
        public string Text;
        public ConsoleColor Color = App.DefaultConsoleColor;

        private OutputInfo(){}

        public OutputInfo(char c) : this(new string(c, 1))
        {
        }

        public OutputInfo(string s)
        {
            Text = s;
        }

        public OutputInfo(string s, ConsoleColor color) : this(s)
        {
            Color = color;
        }

        public static readonly OutputInfo Empty = new OutputInfo();
    }
}
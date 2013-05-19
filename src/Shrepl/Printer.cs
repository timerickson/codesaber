using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSaber.Shrepl
{
    public class Printer
    {
        private readonly App _app;

        public Printer(App app)
        {
            _app = app;
        }

        public void Print(OutputInfo output)
        {
            var text = output.Text;
            if (text == null)
                return;
            foreach (var line in text.GetLines())
                PrintLine(line, output.Color);
        }

        public void PrintLine(string line, ConsoleColor? color = null)
        {
            Print(line + _app.NewLine, color);
        }

        private void Print(string text, ConsoleColor? color = null)
        {
            var currentColor = Console.ForegroundColor;
            if (color.HasValue)
                Console.ForegroundColor = color.Value;
            Console.Write(text);
            Console.ForegroundColor = currentColor;
        }
    }
}

using System;
using System.Collections.Generic;

namespace CodeSaber.Shrepl
{
    public class Display
    {
        private readonly App _app;
        private readonly List<TextArea> _chunks = new List<TextArea>();

        public Display(App app)
        {
            _app = app;
        }

        public TextArea CurrentTextArea { get; private set; }

        public TextArea NextScriptInput(Executor executor)
        {
            var newTextArea = new ScriptTextArea(_app, Console.CursorTop);
            _chunks.Add(newTextArea);
            CurrentTextArea = newTextArea;
            return newTextArea;
        }

        public void OutputResult(object obj)
        {
            //http://stackoverflow.com/questions/1820243/how-to-test-if-methodinfo-returntype-is-type-of-system-void
            //var text = obj.GetType() == typeof(void) ? "<void>" : obj.ToString();
            Output(new Output(obj.ToString(), ConsoleColor.Green));
        }

        public void OutputRunTimeException(Exception ex)
        {
            Output(new Output(ex.Message, ConsoleColor.Red));
        }

        public void OutputCompileTimeException(Exception ex)
        {
            var message = ex.Message;
            if (message.Contains("CS1024: Preprocessor directive expected"))
                message += _app.NewLine + _app.NewLine + "Try '#help'...";
            Output(new Output(message, ConsoleColor.Red));
        }

        public void OutputFeedback(string feedback)
        {
            Output(new Output(feedback));
        }

        private void Output(Output output)
        {
            var area = new OutputTextArea(_app.NewLine, Console.CursorTop, output.Color);
            area.Append(output.Text);
            _chunks.Add(area);
        }
    }
}

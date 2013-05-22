using System;
using System.Collections.Generic;

namespace CodeSaber.Shrepl
{
    public class Display
    {
        private readonly string _newLine;
        private readonly Script _script;
        private readonly List<TextArea> _chunks = new List<TextArea>();

        public Display(string newLine, Script script)
        {
            _newLine = newLine;
            _script = script;
        }

        public TextArea CurrentTextArea { get; private set; }

        public TextArea NextScriptInput(Executor executor)
        {
            var newTextArea = new ScriptTextArea(_newLine, Console.CursorTop, _script, executor, this);
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
                message += _newLine + _newLine + "Try '#help'...";
            Output(new Output(message, ConsoleColor.Red));
        }

        public void OutputFeedback(string feedback)
        {
            Output(new Output(feedback));
        }

        private void Output(Output output)
        {
            var area = new OutputTextArea(_newLine, Console.CursorTop, output.Color);
            area.Append(output.Text);
            _chunks.Add(area);
        }
    }
}

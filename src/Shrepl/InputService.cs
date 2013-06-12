using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSaber.Shrepl
{
    public class InputService
    {
        private readonly App _app;
        private readonly Queue<BufferedInput> _inputBuffer = new Queue<BufferedInput>();

        public InputService(App app)
        {
            _app = app;
        }

        private string _rawInputBuffer;

        public void Read()
        {
            EnqueuRawInputBuffer();

            var display = _app.Display;
            var textArea = display.CurrentTextArea;

            if (textArea == null || textArea.IsComplete)
                textArea = display.NextScriptInput(_app.Executor);

            if (_inputBuffer.Count > 0)
            {
                var input = _inputBuffer.Dequeue();
                    textArea.Append(input.Text);
                if (input.IncludesEnter)
                    textArea.Enter();
            }
            else
            {
                var keyInfo = Console.ReadKey(true);
                textArea.Process(keyInfo);
            }
        }

        public void Buffer(string raw)
        {
            _rawInputBuffer += raw;
        }

        private void EnqueuRawInputBuffer()
        {
            if (_rawInputBuffer == null)
                return;
            var lines = _rawInputBuffer.GetLines().ToArray();
            _rawInputBuffer = null;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                _inputBuffer.Enqueue(new BufferedInput(line, i < (lines.Length - 1)));
            }
        }
    }
}

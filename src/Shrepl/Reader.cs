using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSaber.Shrepl
{
    public class Reader
    {
        private readonly App _app;
        private string _rawInputBuffer = null;
        private readonly Queue<BufferedInput> _inputBuffer = new Queue<BufferedInput>();

        public Reader(App app)
        {
            _app = app;
        }

        private InputArea Input;

        public string Read()
        {
            EnqueuRawInputBuffer();

            Input = new InputArea(_app.NewLine);

            while (_app.IsRunning && !Input.IsComplete)
            {
                if (_inputBuffer.Count > 0)
                    Input.Append(_inputBuffer.Dequeue());
                else
                    Input.ReadKey();
            }

            var result = Input.Text;

            Input.Reset();

            return result;
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
                _inputBuffer.Enqueue(new BufferedInput {Input = line, IsComplete = i < (lines.Length - 1)});
            }
        }
    }
}

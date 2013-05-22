using System;
using Roslyn.Scripting;

namespace CodeSaber.Shrepl
{
    public class ScriptChunkResult
    {
        public Exception CompileTimeException { get; set; }
        public char? IsExpectingClosingChar { get; set; }
        public object ReturnValue { get; set; }
        public Exception RunTimeException { get; set; }

        private Func<object> _executeAction = null;
        public void SetExecuteAction(Func<object> executeAction)
        {
            _executeAction = executeAction;
        }

        public void Execute()
        {
            if (_executeAction == null)
                throw new Exception("ExecuteAction has not been set!");

            try
            {
                ReturnValue = _executeAction();
            }
            catch (Exception ex)
            {
                RunTimeException = ex;
            }
        }

        public bool IsPendingClosing
        {
            get { return IsExpectingClosingChar.HasValue; }
        }

        public void UpdateClosingExpectation(Exception ex)
        {
            var message = ex.Message;
            char? closingChar = null;

            if (message.Contains("CS1026: ) expected"))
                closingChar = ')';
            else if (message.Contains("CS1513: } expected"))
                closingChar = '}';
            else if (message.Contains("CS1003: Syntax error, ']' expected"))
                closingChar = ']';

            if (closingChar.HasValue)
                IsExpectingClosingChar = closingChar.Value;
        }
    }
}
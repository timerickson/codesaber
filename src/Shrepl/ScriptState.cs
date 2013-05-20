using System;
using Roslyn.Scripting;

namespace CodeSaber.Shrepl
{
    public class ExecutionResult
    {
        public Submission<object> Submission { get; set; }
        public Exception CompileTimeException { get; set; }
        public char? IsExpectingClosingChar { get; set; }
        public object ReturnValue { get; set; }
        public Exception RunTimeException { get; set; }

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
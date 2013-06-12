using System;
using System.Collections.Generic;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class ScriptChunkResult
    {
        public Exception CompileTimeException { get; set; }
        public char? IsExpectingClosingChar { get; set; }
        public object ReturnValue { get; set; }
        public Type ReturnType { get; set; }
        public Exception RunTimeException { get; set; }
        public IEnumerable<string> NewMemberNames { get; set; }

        public ScriptChunkResult(string chunk)
        {
            ScriptChunk = chunk;
            NewMemberNames = new string[0];
        }

        private Func<App, object> _executeAction;
        public void SetExecuteAction(Func<App, object> executeAction)
        {
            _executeAction = executeAction;
        }

        public void Execute(App app)
        {
            if (_executeAction == null)
                throw new Exception("ExecuteAction has not been set!");

            try
            {
                ReturnValue = _executeAction(app);
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

        public string ScriptChunk { get; private set; }

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

        public void UpdateScriptChunk(ShreplCommand command)
        {
            ScriptChunk = command.GetModifiedScriptChunk(ScriptChunk);
        }
    }
}
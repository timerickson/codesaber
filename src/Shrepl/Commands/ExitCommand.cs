using System;

namespace CodeSaber.Shrepl.Commands
{
    public class ExitCommand : ShreplCommand
    {
        private readonly App _app;
        public override string Name { get { return "exit"; } }

        public ExitCommand(App app)
        {
            _app = app;
        }

        public override object Execute(object parameter = null)
        {
            _app.Exit();
            return null;
        }
    }
}
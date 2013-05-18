using System;

namespace CodeSaber.Shrepl.Commands
{
    public class ExitCommand : ShreplCommand
    {
        public override string Name { get { return "exit"; } }

        public override object Execute(object parameter = null)
        {
            Environment.Exit(0);
            return null;
        }
    }
}
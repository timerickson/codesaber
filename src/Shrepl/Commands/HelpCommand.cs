using System.Collections.Generic;
using System.Text;

namespace CodeSaber.Shrepl.Commands
{
    public partial class CommandCollection
    {
        private class HelpCommand : ShreplCommand
        {
            private readonly IEnumerable<ShreplCommand> _commandList;

            public HelpCommand(IEnumerable<ShreplCommand> commandList)
            {
                _commandList = commandList;
            }

            public override string Name { get { return "help"; } }

            public override string Description { get { return "Display all available REPL commands"; } }

            public override object Execute(App app)
            {
                var help = new StringBuilder();
                help.AppendLine("REPL commands:");
                foreach (var command in _commandList)
                    help.AppendLine(string.Format("  {0}    {1}", command.Name, command.Description));
                return help.ToString();
            }
        }
    }
}
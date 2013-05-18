using System.Text;

namespace CodeSaber.Shrepl.Commands
{
    public class HelpCommand : ShreplCommand
    {
        public override string Name { get { return "help"; } }

        public override object Execute(object parameter = null)
        {
            var help = new StringBuilder();
            help.AppendLine("REPL commands:");
            help.AppendLine("  help    Display all available REPL commands");
            help.AppendLine("  ice     Open GUI (ICE - Integrated Code Environment)");
            help.Append("  exit    Exit");
            return help.ToString();
        }
    }
}
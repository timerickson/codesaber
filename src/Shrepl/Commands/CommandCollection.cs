using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSaber.Shrepl.Commands
{
    public partial class CommandCollection
    {
        private readonly List<ShreplCommand> _commands = new List<ShreplCommand>();

        public CommandCollection(App app)
        {
            Register(new HelpCommand(_commands));
            Register(new StartIceCommand());
            Register(new SaveCommand());
            Register(new GistCommand());
            Register(new ExitCommand());
        }

        public void Register(ShreplCommand command)
        {
            if (_commands.Any(x => x.Name == command.Name))
                throw new Exception("Duplicate command found");
            _commands.Add(command);
        }

        public ShreplCommand Get(string input)
        {
            if (input == null)
                return null;

            if (!input.StartsWith("#"))
                return null;

            string parameters = null;
            var spaceIndex = input.IndexOf(' ');
            if (spaceIndex != -1)
            {
                parameters = input.Substring(spaceIndex + 1).Trim();
                input = input.Substring(0, spaceIndex);
            }

            var name = input.Substring(1).ToLowerInvariant();

            var command = _commands.FirstOrDefault(x => x.Name == name);
            if (command != null)
                command.Parameters = parameters;
            return command;
        }

        public IEnumerable<string> GetAllNames()
        {
            return _commands.Select(x => "#" + x.Name);
        }
    }
}
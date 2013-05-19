using System;
using System.Collections.Generic;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class CommandCollection
    {
        private readonly Dictionary<string, ShreplCommand> _commands = new Dictionary<string, ShreplCommand>(); 

        public CommandCollection(App app)
        {
            Register(new ExitCommand(app));
            Register(new HelpCommand());
            Register(new StartIceCommand());
        }

        public void Register(ShreplCommand command)
        {
            if (_commands.ContainsKey(command.Name))
                throw new Exception("Duplicate command found");
            _commands[command.Name] = command;
        }

        public ShreplCommand Get(string input)
        {
            if (input == null)
                return null;

            if (!input.StartsWith("#"))
                return null;

            var name = input.Substring(1).ToLowerInvariant();

            if (!_commands.ContainsKey(name))
                return null;

            return _commands[name];
        }
    }
}
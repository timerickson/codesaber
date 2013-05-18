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

        public ShreplCommand Get(string name)
        {
            return _commands[name];
        }
    }
}
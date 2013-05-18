using System;
using System.Collections.Generic;
using CodeSaber.Shrepl.Commands;

namespace CodeSaber.Shrepl
{
    public class CommandCollection
    {
        public readonly ShreplCommand ExitCommand = new ExitCommand();
        public readonly ShreplCommand PrintHelpCommand = new HelpCommand();
        public readonly ShreplCommand StartIceCommand = new StartIceCommand();
        
        private readonly Dictionary<string, ShreplCommand> _commands = new Dictionary<string, ShreplCommand>(); 

        public CommandCollection()
        {
            Register(ExitCommand);
            Register(PrintHelpCommand);
            Register(StartIceCommand);
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
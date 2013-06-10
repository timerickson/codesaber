using System;
using System.Globalization;
using System.IO;

namespace CodeSaber.Shrepl.Commands
{
    public class SaveCommand : ShreplCommand
    {
        private readonly string _newLine;
        private readonly Script _script;

        public SaveCommand(string newLine, Script script)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            _newLine = newLine;
            _script = script;
        }

        public override string Name { get { return "save"; } }

        public override string Description
        {
            get { return "Save Script to file (.csx) in current directory"; }
        }

        public override object Execute()
        {
            var fileName = Parameters;
            if (File.Exists(fileName))
            {
                Console.WriteLine("Overwrite y/[n]?");
                var response = Console.ReadKey(true);
                if (response.KeyChar.ToString(CultureInfo.CurrentCulture).ToLowerInvariant() != "y")
                    return null;
            }
            File.WriteAllText(fileName, _script.GetAllText(_newLine));
            return null;
        }

        public override string GetModifiedScriptChunk(string scriptChunk)
        {
            return "";
        }
    }
}
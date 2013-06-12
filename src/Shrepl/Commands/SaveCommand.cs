using System;
using System.Globalization;
using System.IO;

namespace CodeSaber.Shrepl.Commands
{
    public class SaveCommand : ShreplCommand
    {
        public override string Name { get { return "save"; } }

        public override string Description
        {
            get { return "Save Script to file (.csx) in current directory"; }
        }

        public override object Execute(App app)
        {
            var fileName = Parameters;
            if (File.Exists(fileName))
            {
                Console.WriteLine("Overwrite y/[n]?");
                var response = Console.ReadKey(true);
                if (response.KeyChar.ToString(CultureInfo.CurrentCulture).ToLowerInvariant() != "y")
                    return null;
            }
            File.WriteAllText(fileName, app.Script.GetAllText(app.NewLine));
            return null;
        }

        public override string GetModifiedScriptChunk(string scriptChunk)
        {
            return "";
        }
    }
}
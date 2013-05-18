using System;
using System.Diagnostics;
using System.IO;

namespace CodeSaber.Shrepl.Commands
{
    public class StartIceCommand : ShreplCommand
    {
        public override string Name { get { return "ice"; } }

        private const string IceExe = "CodeSaber.Ice.exe";

        public override object Execute(object parameter = null)
        {
            if (!File.Exists(IceExe))
            {
                Console.WriteLine("{0} not found.", IceExe);
                return null;
            }

            Process.Start(IceExe);
            return null;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;

namespace CodeSaber.Shrepl.Commands
{
    public class StartIceCommand : ShreplCommand
    {
        public override string Name { get { return "ice"; } }

        public override string Description { get { return "Open GUI (ICE - Integrated Code Environment)"; } }

        private const string IceExe = "CodeSaber.Ice.exe";

        public override object Execute(App app)
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
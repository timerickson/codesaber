using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IronGitHub;
using IronGitHub.Entities;

namespace CodeSaber.Shrepl.Commands
{
    public class GistCommand : ShreplCommand
    {
        private readonly string _newLine;
        private readonly Script _script;

        public GistCommand(string newLine, Script script)
        {
            _newLine = newLine;
            _script = script;
        }

        public override string Name
        {
            get { return "gist"; }
        }

        public override string Description
        {
            get { return "GitHub Gist (save, load, list, signin, signout)"; }
        }

        public override object Execute()
        {
            Parameters = Parameters ?? "";
            var parms = Parameters.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parms.Length == 0)
                return Usage();
            
            return Execute(parms).Result;
        }

        private async Task<object> Execute(string[] parms)
        {
            InitApi();

            var text = _script.GetAllText(_newLine);
            var files = new Dictionary<string, Gist.NewGistPost.NewGistFile>
                {
                    {"Script20130607.csx", new Gist.NewGistPost.NewGistFile{Content=text}}
                };
            var gist = await _api.Gists.New(files, "Created by CodeSaber - https://github.com/in2bits/codesaber");
            var url = gist.HtmlUrl;
            SetClipboardText(url, TextDataFormat.Text);
            Console.Write("Launch in browser? [n]/y");
            var key = Console.ReadKey(true);
            if (key.KeyChar.ToString(CultureInfo.CurrentCulture).ToLower(CultureInfo.CurrentCulture) == "y")
                Process.Start(url);
            Console.Write(_newLine);
            return null;
        }

        private void SetClipboardText(string text, TextDataFormat format)
        {
            var sta = new Thread(() =>
                {
                    Clipboard.SetText(text, format);
                    Console.WriteLine("[copied to your clipboard] {0}", text);
                });
            sta.SetApartmentState(ApartmentState.STA);
            sta.Start();
            sta.Join();
        }

        private GitHubApi _api;
        private void InitApi()
        {
            if (_api != null)
                return;

            _api = GitHubApi.Create();
        }

        private string Usage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("#gist save [private] (private only if signed in, anonymous if not)");
            usage.AppendLine("#gist open [url | gistId]");
            usage.AppendLine("#gist list (only when signed in)");
            usage.AppendLine("#gist signin myGitHubUserName myGitHubPassword");
            usage.AppendLine("#gist signout");
            return usage.ToString();
        }
    }
}

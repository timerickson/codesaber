using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        public override string Name
        {
            get { return "gist"; }
        }

        public override string Description
        {
            get { return "GitHub Gist (save, load, list, signin, signout)"; }
        }

        public override object Execute(App app)
        {
            Parameters = Parameters ?? "";
            var parms = Parameters.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parms.Length == 0)
                return Usage();
            
            return Execute(app, parms).Result;
        }

        public override string GetModifiedScriptChunk(string scriptChunk)
        {
            return "";
        }

        private async Task<object> Execute(App app, string[] parms)
        {
            InitApi();

            var command = parms[0].ToLowerInvariant();
            if (command == "save")
                return await SaveToGist(app);
            else if (command == "open")
                return await OpenFromGist(app, parms[1]);
            else
                return Usage();
        }

        async private Task<object> OpenFromGist(App app, string idOrUrl)
        {
            long id;
            if (!long.TryParse(idOrUrl, out id))
                id = Gist.ParseIdFromUrl(idOrUrl);
            var gist = await _api.Gists.Get(id);
            if (gist.Files.Count == 0)
            {
                Console.WriteLine("No file found in gist " + id);
                return null;
            }
            else if (gist.Files.Count > 1)
            {
                Console.WriteLine("Multi-file gists not yet supported");
                return null;
            }
            var text = gist.Files[gist.Files.Keys.First()].Content;
            app.InputService.Buffer(text);
            return null;
        }

        async private Task<object> SaveToGist(App app)
        {
            var text = app.Script.GetAllText(app.NewLine);
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
            Console.Write(app.NewLine);
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
            //usage.AppendLine("#gist insert [url | gistId]");
            //usage.AppendLine("#gist list (only when signed in)");
            //usage.AppendLine("#gist signin myGitHubUserName myGitHubPassword");
            //usage.AppendLine("#gist signout");
            return usage.ToString();
        }
    }
}

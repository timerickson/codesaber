namespace CodeSaber.Shrepl.Commands
{
    public class ExitCommand : ShreplCommand
    {
        private readonly App _app;

        public override string Name { get { return "exit"; } }

        public override string Description
        {
            get { return "Exit"; }
        }

        public ExitCommand(App app)
        {
            _app = app;
        }

        public override object Execute()
        {
            _app.Exit();
            return null;
        }
    }
}
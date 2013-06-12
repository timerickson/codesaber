namespace CodeSaber.Shrepl.Commands
{
    public class ExitCommand : ShreplCommand
    {
        public override string Name { get { return "exit"; } }

        public override string Description
        {
            get { return "Exit"; }
        }

        public override object Execute(App app)
        {
            app.Exit();
            return null;
        }
    }
}
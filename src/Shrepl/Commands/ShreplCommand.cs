namespace CodeSaber.Shrepl.Commands
{
    public abstract class ShreplCommand
    {
        public abstract string Name { get; }
        public abstract object Execute(object parameter = null);
    }
}
namespace CodeSaber.Shrepl.Commands
{
    public abstract class ShreplCommand
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public string Parameters { get; set; }

        public abstract object Execute(App app);

        public virtual string GetModifiedScriptChunk(string scriptChunk)
        {
            return scriptChunk;
        }
    }
}
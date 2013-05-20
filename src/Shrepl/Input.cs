namespace CodeSaber.Shrepl
{
    public class BufferedInput
    {
        public string Text { get; set; }
        public bool IncludesEnter { get; set; }

        public BufferedInput(string text, bool includesEnter)
        {
            Text = text;
            IncludesEnter = includesEnter;
        }
    }
}
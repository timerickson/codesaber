using System.Collections.Generic;

namespace CodeSaber.Shrepl
{
    public class Script
    {
        private readonly List<TextArea> _chunks = new List<TextArea>();

        public void AppendChunk(TextArea textArea)
        {
            _chunks.Add(textArea);
        }

/*
        private static MemberDeclarationSyntax GetFinalInteractiveStatement(Submission<object> testSubmission)
        {
            var syntaxNodes = testSubmission.Compilation.ScriptClass.DeclaringSyntaxNodes;
            var lastNodeMembers = ((CompilationUnitSyntax)syntaxNodes.Last()).Members;
            if (lastNodeMembers.Any())
            {
                var lastMember = lastNodeMembers.Last();
                if (lastMember.Kind == SyntaxKind.GlobalStatement)
                {
                    var stmt = lastMember.ToString();
                    if (!stmt.EndsWith(";"))
                    {
                        return lastMember;
                    }
                }
            }
            return null;
        }
*/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSaber.Shrepl
{
    public class Script
    {
        private readonly List<TextArea> _chunks = new List<TextArea>();

        private readonly List<string> _memberNames = new List<string>(); 

        public void AppendChunk(TextArea textArea)
        {
            _chunks.Add(textArea);
        }

        public void AppendMemberNames(IEnumerable<string> newMembers)
        {
            _memberNames.AddRange(newMembers);
        }

        public IEnumerable<string> SuggestCompletions(string start)
        {
            return _memberNames.Where(x => x.StartsWith(start, StringComparison.CurrentCulture));
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

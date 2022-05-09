using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;

namespace Thorn
{
    public class Parser
    {
        List<Token> Tokens;
        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }
        public static Dictionary<string, NamedGraph<Token>> GenerateTrees()
        {
            return new Dictionary<string, NamedGraph<Token>>();
        }
        public static List<Operation> GenerateSyntaxTree()
        {
            //temp retutrn empty list
            return new List<Operation>();
        }
    }
}

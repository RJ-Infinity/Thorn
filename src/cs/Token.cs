using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public enum TokenType
    {
        VariableName,

        ClassDeclaration,
        FunctionDeclaration,
        
        OpenBrackets,
        CloseBrackets,
        
        newClassOperator,
        Assignment,
        TypeDecleration,

        StringLitteral,
        NumberLitteral,
    }
    public class Token
    {
        public int LineNumber;
        public int CharPos;
        public TokenType Type;
        public string Data;

    }
}

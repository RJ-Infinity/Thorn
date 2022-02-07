using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public enum TokenType
    {
        Statement,

        ClassDeclaration,
        FunctionDeclaration,
        
        OpenBrackets,
        CloseBrackets,
        ExpresionEnd,

        newClassOperator,
        Assignment,
        TypeDecleration,

        StringLitteral,
        NumberLitteral,
    }
    public class Token
    {
        public Token(string data, TokenType type, int lineNumber, int charPos)
        {
            Data = data;
            Type = type;
            LineNumber = lineNumber;
            CharPos = charPos;
        }
        public Token() { }
        public string Data;
        public TokenType Type;
        public int LineNumber;
        public int CharPos;
    }
}

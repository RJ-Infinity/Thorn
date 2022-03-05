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
        Return,
        Seperator,
        ListSeperator,

        NumberLitteralDeclaration,
        StringLetteralDeclaration,

        ClassDeclaration,
        Namespace,
        Include,
        
        OpenBrackets,
        CloseBrackets,
        ExpresionEnd,

        newClassOperator,
        Assignment,

        ByteLitteral,
        FunctionLitteral,
        StringLitteral,
        CharLitteral,
        NumberLitteral,

        NULL,// for when there isnt a token needed
    }
    public class Token
    {
        public Token(string data, TokenType type)
        {
            Data = data;
            Type = type;
        }
        public Token() { }

        public string Data;
        public TokenType Type;
        public override string ToString() => ToString(false);
        public virtual string ToString(bool addQuotes)
        {
            return "[" + Type + "]\t'" + Data + "'";
        }
    }
    public class FileToken : Token
    {
        public FileToken(string data, TokenType type, int lineNumber, int charPos) : base(data,type)
        {
            LineNumber = lineNumber;
            CharPos = charPos;
        }
        public FileToken() : base() { }

        public int LineNumber;
        public int CharPos;
        public override string ToString(bool addQuotes)
        {
            return LineNumber + ":" + CharPos + base.ToString(addQuotes);
        }
    }
    public class GrammarToken : Token
    {
        public GrammarToken(string data, TokenType type, string location) : base(data, type)
        {
            Location = location;
            Link = location == null;
        }
        public GrammarToken(string data, TokenType type) : base(data, type)
        {
            Location = null;
            Link = false;
        }

        public GrammarToken() : base() { }

        public bool Link;
        public string Location;
        public override string ToString(bool addQuotes)
        {
            if (Link)
            {
                return base.ToString(addQuotes);
            }
            return "Link To " + Location;
        }
    }
}

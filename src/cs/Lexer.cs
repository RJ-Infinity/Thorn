using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    class Lexer
    {
        private static readonly char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public Lexer(string code)
        {
            Code = code;
        }
        public int LineNumber = 0;
        public int CharPos = 0;
        public string Code;
        private int Index = 0;
        private List<Token> BufferedTokens = new List<Token>();
        public List<Token> Tokens = new List<Token>();
        public void GenerateNextToken()
        {
            if (BufferedTokens.Count > 0)
            {
                Tokens.Add(BufferedTokens[0]);
                return;
            }
            int initialLN = LineNumber;
            int initialCP = CharPos;
            string currExpr = "";
            bool inStr = false;
            TokenType? typeGuess = null;
            while (Index < Code.Length)
            {
                if (!inStr)
                {
                    if (Code[Index] == '=')
                    {
                        Tokens.Add(new Token(currExpr, TokenType.Statement, initialLN, initialCP));
                        BufferedTokens.Add(new Token("=", TokenType.Assignment, LineNumber, CharPos));
                        return;
                    }
                    else if (Code[Index] == '"')
                    {
                        inStr = true;
                        typeGuess = TokenType.StringLitteral;
                    }
                    else if (numbers.Contains(Code[Index]) && (typeGuess == null))
                    {

                    }
                }
                else if (Code[Index] == '"')
                {
                    inStr = false;
                }
                if (Code[Index] == '\n')
                {
                    LineNumber++;
                    CharPos = 0;
                }
                currExpr += Code[Index];
                CharPos++;
                Index++;
            }
            if (Index < Code.Length)
            {
                //we reached the end of the code block for now just return
                return;
            }
        }
    }
}

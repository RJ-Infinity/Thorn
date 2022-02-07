using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public class Lexer
    {
        private static readonly char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly char[] whitespace = { ' ', '\t', '\n', '\r' };
        private static readonly char[] exprTerm = { '.', ';', '(', '=', '{', '}', ')', '[', ']','"'};
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
        public bool GenerateNextToken()
        {
            if (BufferedTokens.Count > 0)
            {
                Tokens.Add(BufferedTokens[0]);
                BufferedTokens.RemoveAt(0);
                return true;
            }
            string currExpr = "";
            bool inStr = false;
            TokenType? typeGuess = null;
            int initialLN=-1;//assigned so the complier doesent complain
            int initialCP=-1;//assigned so the complier doesent complain
            bool resetExprStart = true;
            while (Index < Code.Length)
            {
                if (resetExprStart)
                {
                    initialLN=LineNumber;
                    initialCP=CharPos;
                    resetExprStart = false;
                }
                if (!inStr)
                {
                    if (Code[Index] == '=')
                    {
                        Tokens.Add(new Token(currExpr, TokenType.Statement, initialLN, initialCP));
                        BufferedTokens.Add(new Token("=", TokenType.Assignment, LineNumber, CharPos));
                        Index++;//must increment index when returning so the same char doesent get reevaled
                        CharPos++;
                        return true;
                    }
                    else if (Code[Index] == '"')
                    {
                        inStr = true;
                        typeGuess = TokenType.StringLitteral;
                    }
                    else if (numbers.Contains(Code[Index]) && (typeGuess == null))
                    {
                        typeGuess = TokenType.NumberLitteral;
                    }
                    else if (Code[Index] == ';')
                    {
                        if (typeGuess == null)
                        {
                            typeGuess = TokenType.Statement;
                        }
                        Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                        BufferedTokens.Add(new Token(";", TokenType.ExpresionEnd, LineNumber, CharPos));
                        Index++;//must increment index when returning so the same char doesent get reevaled
                        CharPos++;
                        return true;
                    }
                    else if (whitespace.Contains(Code[Index]))
                    {
                        resetExprStart = currExpr.Length == 0;//if the expresion hasnt started yet reset the LN and CP
                        int i = Index;
                        while (i < Code.Length && whitespace.Contains(Code[i])) { i++; }
                        if (i == Code.Length)
                        {
                            if(currExpr.Length > 0)
                            {
                                Console.WriteLine(currExpr.Length);
                                Console.WriteLine("'"+currExpr +"'");
                                Debug.Assert(typeGuess != null, "The File Ended without a valid expression");
                                BufferedTokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                                return true;
                            }
                            return false;
                        }
                        //Debug.Assert(exprTerm.Contains(Code[i]), "There are two expresions back to back");
                    }
                    if (!whitespace.Contains(Code[Index]))
                    {
                        currExpr += Code[Index];
                    }
                }
                else 
                {
                    currExpr += Code[Index];
                    if (Code[Index] == '"')
                    {
                        inStr = false;
                    }
                }
                if (Code[Index] == '\n')
                {
                    LineNumber++;
                    CharPos = 0;
                }
                else
                {
                    CharPos++;
                }
                Index++;
            }
            //we reached the end of the code block for now just return
            return false;
        }
    }
}

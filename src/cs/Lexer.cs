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
        private static readonly Dictionary<char, TokenType> keyChars = new Dictionary<char, TokenType>
        {
            {'=', TokenType.Assignment},
            {'$', TokenType.Namespace},
            {'@', TokenType.Include},
            {'.', TokenType.Seperator},
            {',', TokenType.ListSeperator},
            {'£', TokenType.ClassDeclaration},
            {'#', TokenType.ByteLitteral},
            {'~', TokenType.NumberLitteralDeclaration},
            {'`', TokenType.StringLetteralDeclaration},
            {';', TokenType.ExpresionEnd},
            {'¬', TokenType.CharLitteral},
            {'§', TokenType.FunctionLitteral},
            {'(', TokenType.OpenBrackets},
            {'{', TokenType.OpenBrackets},
            {'[', TokenType.OpenBrackets},
            {')', TokenType.CloseBrackets},
            {'}', TokenType.CloseBrackets},
            {']', TokenType.CloseBrackets},
            {'‡', TokenType.newClassOperator},
            {'→', TokenType.Return},
        };
        public Lexer(string code)
        {
            Code = code;
        }
        public int LineNumber = 1;// this starts at one as files dont have a line 0
        public int CharPos = 0;
        public string Code;
        private int Index = 0;
        private List<Token> BufferedTokens = new List<Token>();
        public List<Token> Tokens = new List<Token>();
        public void GenerateAllTokens()
        {
            while (GenerateNextToken()) { };
        }
        public bool GenerateNextToken()
        {
            if (BufferedTokens.Count > 0)
            {
                Tokens.Add(BufferedTokens[0]);
                BufferedTokens.RemoveAt(0);
                return true;
            }
            string currExpr = "";//assigned so the complier doesent complain
            bool inStr = false;
            TokenType? typeGuess = null;
            int initialLN=-1;//assigned so the complier doesent complain
            int initialCP=-1;//assigned so the complier doesent complain
            bool resetExprStart = true;
            bool comment = false;
            while (Index < Code.Length)
            {
                if (resetExprStart)
                {
                    initialLN=LineNumber;
                    initialCP=CharPos;
                    currExpr = "";
                    resetExprStart = false;
                }
                if (!inStr && !comment)
                {
                    if (keyChars.ContainsKey(Code[Index]))
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new FileToken(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new FileToken(Code[Index].ToString(), keyChars[Code[Index]], LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new FileToken(Code[Index].ToString(), keyChars[Code[Index]], LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (Code[Index] == '"')
                    {
                        inStr = true;
                        typeGuess = TokenType.StringLitteral;
                    }
                    else if (numbers.Contains(Code[Index]) && typeGuess == null && currExpr.Length == 0)
                    {
                        typeGuess = TokenType.NumberLitteral;
                    }
                    else if (whitespace.Contains(Code[Index]))
                    {
                        resetExprStart = currExpr.Length == 0;//if the expresion hasnt started yet reset the LN and CP
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new FileToken(currExpr, typeGuess.Value, initialLN, initialCP));
                            return true;
                        }
                    }
                    else if (
                        Index > 0 &&
                        Code[Index] == '/' &&
                        Code[Index - 1] == '/'
                    )
                    {
                        Debug.Assert(currExpr == "/", "The comment is not // and also contains '" + currExpr + "'");
                        comment = true;
                    }
                    if (!whitespace.Contains(Code[Index]))
                    {
                        currExpr += Code[Index];
                    }
                }
                else 
                {
                    if (!comment)
                    {
                        currExpr += Code[Index];
                    }
                    if (Code[Index] == '"')
                    {
                        inStr = false;
                    }
                }
                if (Code[Index] == '\n')
                {
                    resetExprStart = resetExprStart || comment;//reset if the last line was a comment or keep the same
                    comment = false;
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

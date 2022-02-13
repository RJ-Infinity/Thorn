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
        private static readonly char[] openBrackets = { '(', '{', '[', };
        private static readonly char[] closeBrackets = { ')', '}', ']', };
        public Lexer(string code)
        {
            Code = code;
        }
        public int LineNumber = 1;// this starts at one as files dont have a line 0-
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
                    if (Code[Index] == '=')
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token("=", TokenType.Assignment, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token("=", TokenType.Assignment, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (Code[Index] == '$')
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token("$", TokenType.Namespace, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token("$", TokenType.Namespace, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (Code[Index] == '£')
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token("£", TokenType.ClassDeclaration, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token("£", TokenType.ClassDeclaration, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (Code[Index] == '*')
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token("*", TokenType.ByteLitteral, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token("*", TokenType.ByteLitteral, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (openBrackets.Contains(Code[Index]))
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token(Code[Index].ToString(), TokenType.OpenBrackets, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token(Code[Index].ToString(), TokenType.OpenBrackets, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
                    }
                    else if (closeBrackets.Contains(Code[Index]))
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token(Code[Index].ToString(), TokenType.CloseBrackets, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token(Code[Index].ToString(), TokenType.CloseBrackets, LineNumber, CharPos));
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
                    else if (Code[Index] == ';')
                    {
                        if (currExpr.Length > 0)
                        {
                            if (typeGuess == null)
                            {
                                typeGuess = TokenType.Statement;
                            }
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            BufferedTokens.Add(new Token(";", TokenType.ExpresionEnd, LineNumber, CharPos));
                        }
                        else
                        {
                            Tokens.Add(new Token(";", TokenType.ExpresionEnd, LineNumber, CharPos));
                        }
                        Index++;//must increment index when returning so the same char doesent get reevaled
                        CharPos++;
                        return true;
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
                            Tokens.Add(new Token(currExpr, typeGuess.Value, initialLN, initialCP));
                            return true;
                        }
                    }
                    else if (Code[Index] == '+' && currExpr.Length == 0)
                    {
                        Tokens.Add(new Token("+", TokenType.newClassOperator, LineNumber, CharPos));
                        Index++;//must increment index when returning so the same char doesent get re-evaled
                        CharPos++;
                        return true;
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

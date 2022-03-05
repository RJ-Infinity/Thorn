using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public enum OperationTypes
    {
        VarDecleration,
        FunctionDecleration,
        ClassDecleration,
        Namespace,

        StringLitteral,
        CharLitteral,
        NumberLitteral,        
    }
    public class Operation
    {
        public Operation(OperationTypes type, List<Token> tokens)
        {
            Type = type;
            Tokens = tokens;
        }
        public OperationTypes Type;
        public List<Token> Tokens;
    }
}

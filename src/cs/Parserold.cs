using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJJSON;

namespace Thorn
{
    public class Parser
    {
        public string Code;
        Data Statement;
        JSONTypes Raw;
        public Parser(string code)
        {
            Code = code;
        }
        public void Formater()
        {
            Code = Code.Replace("\r\n", "\n").Replace("\n", "");//remove all types of line ending
            bool InString = false;
            bool InEsc = false;
            for (int ichar = 0; ichar < Code.Length; ichar++)//this loop removes whitespace except in strings
            {
                if (!InString)
                {
                    while (Code.Substring(ichar, 1) == " ")//while the current character is a whitespace remove it
                    {
                        Code = Code.Remove(ichar, 1);//while is neededd not if as otherise it fails to remove two whitespace in a row
                    }
                    while (Code.Substring(ichar, 1) == "\t")
                    {
                        Code = Code.Remove(ichar, 1);
                    }
                }
                if (!InEsc)//stops ending strings if the " was preceded with a \
                {
                    if (Code.Substring(ichar, 1) == "\"")
                    {
                        InString = !InString;
                    }
                }
                if (InEsc) { InEsc = false; }
                if (Code.Substring(ichar, 1) == "\\")
                {
                    InEsc = true;
                }
            }
        }
        public void Tokeniser()
        {
            Statement = emptyBrackets(Code);
        }
        public void Execute()
        {
            executeFunc(ref Statement);
            Raw = Statement.Raw;
        }
        private Data emptyBrackets(string Exp, int TotalPos = 0)
        {
            Data dat = new Data();
            if (double.TryParse(Exp, out double tryParse))
            {
                dat.Raw = new JSONFloat(tryParse);
                dat.ExpresionType.Raw = true;
                return dat;
            }
            int brackets = 0;
            bool index = false;
            bool inString = false;
            string currExp = "";
            if (Exp[0] == '"')
            {
                for (int ichar = 0; ichar < Exp.Length; ichar++)
                {
                    currExp += Exp[ichar];
                    if (Exp[ichar] == '"' && !inString)
                    {
                        inString = true;
                    }
                    else if (Exp[ichar] == '"' && inString && ichar > 0 && Exp[ichar] != '\\')
                    {
                        inString = false;
                        if (ichar + 1 < Exp.Length)
                        {
                            if (Exp[ichar + 1] == '+')
                            {
                                dat.Concatination.Add(emptyBrackets(currExp, TotalPos));
                                Data d = emptyBrackets(Exp.Substring(ichar + 2), TotalPos + ichar + 1);
                                if (d.Concatination.Count > 0)
                                {
                                    dat.Concatination.AddRange(d.Concatination);
                                }
                                else
                                {
                                    dat.Concatination.Add(d);
                                }
                                dat.ExpresionType.Concatination = true;
                            }
                            else
                            {
                                throw new ParserExeption("string statetment at char " + TotalPos + " must end with a quote (\")");
                            }
                        }
                        else
                        {
                            dat.Raw = JSON.StringToObject(currExp);
                            dat.ExpresionType.Raw = true;
                        }
                        return dat;
                    }
                }
            }
            dat.ExpresionType.UnProcesedData = true;
            for (int ichar = 0; ichar < Exp.Length; ichar++)
            {
                if (Exp[ichar] == '"' && !inString)
                {
                    inString = true;
                }
                else if (Exp[ichar] == '"' && inString && ichar > 0 && Exp[ichar] != '\\')
                {
                    inString = false;
                }

                if (index)
                {
                    if (Exp[ichar] == '[' && !inString)
                    {
                        dat.ExpresionType.Index = true;
                    }
                    else if (Exp[ichar] == ']' && !inString)
                    {
                        bool isString = !double.TryParse(currExp, out _);
                        dat.Index.Add(new Tuple<bool, string>(isString, isString ? currExp.Substring(1, currExp.Length - 2) : currExp));
                        currExp = "";
                        if (ichar + 1 < Exp.Length && Exp[ichar + 1] == '+')
                        {
                            Data concatDat = new Data();
                            concatDat.Concatination.Add(dat);
                            concatDat.ExpresionType.Concatination = true;

                            Data d = emptyBrackets(Exp.Substring(ichar + 1), TotalPos + ichar + 1);
                            if (d.ExpresionType.Concatination)
                            {
                                dat.Concatination.AddRange(d.Concatination);
                            }
                            else
                            {
                                dat.Concatination.Add(d);
                            }
                            return concatDat;
                        }
                    }
                    else
                    {
                        currExp += Exp[ichar];
                    }
                }
                else
                {
                    if (Exp[ichar] == '(' && !inString)
                    {
                        if (brackets == 0)
                        {
                            dat.UnProcesedData.Name = currExp;
                            currExp = "";
                        }
                        else
                        {
                            currExp += Exp[ichar];
                        }
                        brackets++;
                    }
                    else if (Exp[ichar] == ')' && !inString)
                    {
                        brackets--;
                        if (brackets == 0)
                        {
                            if (currExp.Length > 0)
                            {
                                dat.UnProcesedData.Args.Add(emptyBrackets(currExp, TotalPos + ichar));
                            }
                            index = true;
                            currExp = "";
                            if (ichar + 1 < Exp.Length && Exp[ichar + 1] == '+')
                            {
                                Data concatDat = new Data();
                                concatDat.Concatination.Add(dat);
                                concatDat.ExpresionType.Concatination = true;

                                Data d = emptyBrackets(currExp.Substring(ichar + 1), TotalPos + ichar + 1);
                                if (d.ExpresionType.Concatination)
                                {
                                    dat.Concatination.AddRange(d.Concatination);
                                }
                                else
                                {
                                    dat.Concatination.Add(d);
                                }
                                return concatDat;
                            }
                        }
                        else
                        {
                            currExp += Exp[ichar];
                        }
                    }
                    else if (Exp[ichar] == ',' && brackets == 1 && !inString)
                    {
                        dat.UnProcesedData.Args.Add(emptyBrackets(currExp, TotalPos + ichar));
                        currExp = "";
                    }
                    else
                    {
                        currExp += Exp[ichar];
                    }
                }
            }
            return dat;
        }
        private void executeFunc(ref Data dat)
        {
            if (dat.ExpresionType.Raw) { return; }
            if (dat.ExpresionType.Concatination)
            {
                for (int i = 0; i < dat.Concatination.Count; i++)
                {
                    Data d = dat.Concatination[i];
                    executeFunc(ref d);
                    dat.Concatination[i] = d;
                }
                string rawT = "";
                for (int i = 0; i < dat.Concatination.Count; i++)
                {
                    rawT += dat.Concatination[i];
                }
                dat.Raw = JSON.StringToObject("\"" + rawT + "\"");
                dat.ExpresionType.Raw = true;
                return;
            }
            if (dat.ExpresionType.UnProcesedData)
            {

            }
            if (dat.ExpresionType.Index)//must come after UnProcDat
            {
                for (int i = 0; i < dat.Index.Count; i++)
                {
                    if (dat.Index[i].Item1)
                    {
                        dat.Raw = dat.Raw[dat.Index[i].Item2];
                    }
                    else
                    {
                        dat.Raw = dat.Raw[double.Parse(dat.Index[i].Item2)];
                    }
                }
            }
        }
        private static string RemoveEscChars(string str)
        {
            bool InEsc = false;
            string newStr = "";
            for (int ichar = 0; ichar < str.Length; ichar++)
            {
                if (str.Substring(ichar, 1) == "\\" && !InEsc)
                {
                    InEsc = true;
                }
                else if (InEsc)
                {
                    InEsc = false;
                    switch (str.Substring(ichar, 1))
                    {
                        case "b":
                            newStr += "\b";
                            break;
                        case "f":
                            newStr += "\f";
                            break;
                        case "n":
                            newStr += "\n";
                            break;
                        case "r":
                            newStr += "\r";
                            break;
                        case "t":
                            newStr += "\t";
                            break;
                        case "\"":
                            newStr += "\"";
                            break;
                        case "\\":
                            newStr += "\\";
                            break;
                        default:
                            newStr += "\\" + str.Substring(ichar, 1);
                            break;
                    }
                }
                else
                {
                    newStr += str.Substring(ichar, 1);
                }
            }
            return newStr;
        }
    }
    class Func
    {
        public string Name;
        public List<Data> Args = new List<Data>();
        public override string ToString() => ToString();
        public string ToString(bool newline = true)
        {
            string rv = "";
            rv += Name;
            rv += newline ? "(\n" : "(";
            foreach (Data i in Args)
            {
                rv += "{";
                if (newline)
                {
                    rv += "\n";
                    foreach (string j in i.ToString(newline).Split('\n'))
                    {
                        rv += "\t";
                        rv += j;
                        rv += "\n";
                    }
                }
                else
                {
                    rv += i.ToString(newline);
                }
                rv += newline ? "},\n" : "},";
            }
            if (Args.Count > 0)
            {
                rv = rv.Remove(rv.Length - (newline ? 2 : 1), (newline ? 2 : 1));//remove last char & newline
            }
            rv += newline ? "\n)" : ")";
            return rv;
        }
    }
    class Data
    {
        public Data()
        {
            ExpresionType.Raw = false;
            ExpresionType.UnProcesedData = false;
            ExpresionType.Concatination = false;
            ExpresionType.Index = false;
        }
        public Func UnProcesedData = new Func();
        public JSONTypes Raw;
        public List<Tuple<bool, string>> Index = new List<Tuple<bool, string>>();
        public List<Data> Concatination = new List<Data>();
        public DataExpresionType ExpresionType = new DataExpresionType();
        public override string ToString() => ToString();
        public string ToString(bool newline = true)
        {
            string rv = "";
            if (ExpresionType.Raw)
            {
                rv += Raw.Data;
                if (newline && (ExpresionType.Index || ExpresionType.Concatination || ExpresionType.UnProcesedData))
                {
                    rv += "\n";
                }
            }
            if (ExpresionType.Concatination)
            {
                foreach (Data i in Concatination)
                {
                    rv += "+";
                    rv += i.ToString(newline);
                    if (newline)
                    {
                        rv += "\n";
                    }
                }
            }
            else if (ExpresionType.UnProcesedData)
            {
                rv += UnProcesedData.ToString(newline);
            }
            if (ExpresionType.Index)
            {
                foreach (Tuple<bool, string> i in Index)
                {
                    rv += "[";
                    if (i.Item1)
                    {
                        rv += "\"";
                    }
                    rv += i.Item2;
                    if (i.Item1)
                    {
                        rv += "\"";
                    }
                    rv += "]";
                }
            }
            rv += ExpresionType;
            return rv;
        }
    }
    struct DataExpresionType
    {
        public bool Raw;
        public bool UnProcesedData;
        public bool Concatination;
        public bool Index;
        public string ToString(bool _) => ToString();
        public override string ToString()
        {
            string rv = "";
            rv += "!";
            rv += Raw ? "Raw|" : "";
            rv += UnProcesedData ? "UnProcesedData|" : "";
            rv += Concatination ? "Concatination|" : "";
            rv += Index ? "Index|" : "";
            rv = rv.Remove(rv.Length - 1, 1);
            return rv;
        }
    }
    public abstract class Function
    {
        public abstract Type Execute(List<JSONTypes> Arguments);
        public abstract System.Type ReturnType { get; }
    }
    class Rand : Function
    {
        public override Type Execute(List<JSONTypes> Arguments)
        {
            Random rnd = new Random();
            Type rv = new Type();
            rv.Data = Arguments[rnd.Next(0, Arguments.Count)].Data;
            return rv;
        }
        public override System.Type ReturnType => typeof(Type);
    }
    public abstract class Type
    {
        public abstract string Name { get; }
        public abstract string Data { get; set; }
    }
    public abstract class BasicType<CSType> : Type
    {
        public abstract CSType DataAsCSType { get; set; }
    }
    public class String : BasicType<string>
    {
        public override string Name { get => "String"; }
        public override string Data { get; set; }
        public override string DataAsCSType { get => Data; set => Data = value; }
    }
    public class Number : BasicType<double>
    {
        public override string Name { get => "Number"; }
        public override string Data
        {
            get => DataAsCSType.ToString();
            set
            {
                if (!double.TryParse(value, out double i))
                {
                    throw new InvalidTypeCastException("Could not cast the typeless \"" + value + "\" to a Number.");
                }
                DataAsCSType = i;
            }
        }
        public override double DataAsCSType { get; set; }
    }
    public class Image : BasicType<System.Drawing.Image>
    {
        public override string Name { get => "Number"; }
        public override string Data { get; set; }
        public override System.Drawing.Image DataAsCSType
        {
            get => System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Data)));
            set
            {
                using (MemoryStream m = new MemoryStream())//https://stackoverflow.com/questions/21325661/convert-an-image-selected-by-path-to-base64-string
                {
                    value.Save(m, value.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    Data = Convert.ToBase64String(imageBytes);
                }
            }
        }
    }
}

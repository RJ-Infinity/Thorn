using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJJSON;

namespace Thorn
{
    class Program
    {
        public static void Assert(bool statement, string message)
        {
            if (statement)
            {
                Console.Error.WriteLine(message);
                Environment.Exit(1);
            }
        }
        public static void Main(string[] args)
        {
            Assert(args.Length == 0 || !File.Exists(args[0]),"You need to provide a JSON config file");
            JSONType obj;
            try
            {
                obj = JSON.StringToObject(System.IO.File.ReadAllText(args[0]));
            }
            catch (Exception ex)
            {
                Console.Error.Write("Invalid Json File\n");
                Console.Error.Write(ex.ToString());
                Environment.Exit(1);
                return; //so the compiler dosent complain
            }
            Assert(obj.Type != JSON.Types.DICT,"The root of the Json file must be a Dict");
            Assert(!obj.DictData.ContainsKey("Files"),"The Json file must contain a files list");
            Assert(obj["Files"].Type != JSON.Types.LIST,"The Json file must contain a files list");
            Assert(obj["Files"].ListData.Count == 0,"The files list must contain files");
            Assert(
                !obj.DictData.ContainsKey("EntryPoint") && //it doesnt contain an entry point and...
                (
                    !obj.DictData.ContainsKey("OutputType") ||// ... it doesetn contain an output type or ...
                    (
                        obj["OutputType"].Type != JSON.Types.STRING &&
                        obj["OutputType"].StringData != "Libary"// ... the output type isnt a libary
                    )
                ),
                "The Json file must either contain an entry point or have an output type of \"Libary\""
            );
            foreach(JSONType file in obj["Files"])
            {
                Assert(file.Type == JSON.Types.STRING, "The Json files must be a string");
            }
        }
    }
}

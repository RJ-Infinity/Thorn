using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJJSON;

namespace Thorn
{
    class Program
    {
        public void Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.Error.Write("You need to provide a JSON config file\n");
                Environment.Exit(1);
            }
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
            if (
                obj.Type != JSON.Types.DICT ||
                !obj.DictData.ContainsKey("Files") ||
                obj["Files"].Type != JSON.Types.LIST ||
                obj["Files"].ListData.Count > 0 ||
                (
                    !obj.DictData.ContainsKey("EntryPoint") && //it doesnt contain an entry point and...
                    (
                        !obj.DictData.ContainsKey("OutputType") ||// ... it doesetn contain an output type or ...
                        (
                            obj["OutputType"].Type != JSON.Types.STRING &&
                            obj["OutputType"].StringData != "Libary"// ... the output type isnt a libary
                        )
                    )
                )

            )
            {
                Console.Error.Write("The Json file has the wrong format\n");
                Environment.Exit(1);
            }
        }
    }
}

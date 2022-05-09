using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJJSON;
using Graph;

namespace Thorn
{
    class Program
    {
        public static void Main(string[] args)
        {
            Debug.Assert(args.Length != 0 && File.Exists(args[0]), "You need to provide a JSON config file");
            Directory.SetCurrentDirectory(Path.GetDirectoryName(args[0]));//put us in the directory of the project file
            JSONType obj;
            try
            {
                obj = JSON.StringToObject(File.ReadAllText(args[0]));
            }
            catch (Exception ex)
            {
                Console.Error.Write("Invalid Json File\n");
                Console.Error.Write(ex.ToString());
                Environment.Exit(1);
                return; //so the compiler dosent complain
            }
            Debug.Assert(obj.Type == JSON.Types.DICT, "The root of the Json file must be a Dict");
            Debug.Assert(obj.DictData.ContainsKey("Files"), "The Json file must contain a files list");
            Debug.Assert(obj["Files"].Type == JSON.Types.LIST, "The Json file must contain a files list");
            Debug.Assert(obj["Files"].ListData.Count != 0, "The files list must contain files");
            Debug.Assert(
                obj.DictData.ContainsKey("EntryPoint") || //it contain an entry point or...
                (
                    obj.DictData.ContainsKey("OutputType") &&// ... it contains an output type and ...
                    (
                        obj["OutputType"].Type == JSON.Types.STRING &&
                        obj["OutputType"].StringData == "Libary"// ... the output type isnt a libary
                    )
                ),
                "The Json file must either contain an entry point or have an output type of \"Libary\""
            );
            foreach (JSONType file in obj["Files"])
            {
                Debug.Assert(file.Type == JSON.Types.STRING, "The Json files must be a string");
                Debug.Assert(File.Exists(file.StringData),"The File given "+file.StringData+" wasnt found");
                Lexer lexer = new Lexer(File.ReadAllText(file.StringData));
                lexer.GenerateAllTokens();
                foreach (Token token in lexer.Tokens)
                {
                    Console.WriteLine(file.StringData+":"+token.ToString(true));
                }
            }
        }
        public static void tMain(string[] args)
        {
            NamedGraph<string> ng = new NamedGraph<string>
            ("root", "this is the root", new List<string>
                {
                    "child0",
                    "child1",
                    "child2",
                    "child3"
                }
            )
            {
                {
                    "child0",
                    "this is a child to the root",
                    new List<string> {
                        "grandchild0,0",
                        "grandchild0,1",
                        "grandchild0,2",
                        "grandchild0,3"
                    }
                },
                {
                    "child1",
                    "this is the second child",
                    new List<string>
                    {
                        "grandchild1,0",
                        "grandchild1,1"
                    }
                },
                {
                    "grandchild0,1",
                    "this is the second grandchild of the root",
                    new List<string>()
                }
            };
            Console.WriteLine(ng.Name);
            Console.WriteLine(ng.Data);
            Console.WriteLine(ng[0].Name);
            Console.WriteLine(ng[0].Data);
            Console.WriteLine(ng[0][1].Name);
            Console.WriteLine(ng[0][1].Data);
        }
    }
}

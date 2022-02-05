using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    class Program
    {
        public void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Write("you need to provide a JSON config file\n");
                System.Environment.Exit(1);
            }
            if (File.Exists(args[0]))
            {

            }
        }
    }
}

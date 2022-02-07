using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thorn
{
    public static class Debug
    {
        public static void Assert(bool statement, string message)
        {
            if (!statement)
            {
                Console.Error.WriteLine(message);
                Environment.Exit(1);
            }
        }
    }
}

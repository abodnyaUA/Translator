using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translators.Lab01
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler.sharedCompiler.CompileFile("C:\\Developer\\Projects\\Translators.Lab01\\Translators.Lab01\\TextFile1.txt");
            
            Console.ReadKey();
        }
    }
}

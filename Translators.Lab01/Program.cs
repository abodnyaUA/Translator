using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace Translators
{
    class Program
    {
        static void Main(string[] args)
        {
			//Compiler.sharedCompiler.CompileFile("C:\\Developer\\Projects\\Translators.Lab01\\Translators.Lab01\\TextFile1.txt");
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
			//Console.ReadKey();
        }
    }
}

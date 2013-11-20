using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using System.Threading;

namespace Translators
{
	class Program
    {
		public static MainWindow window;
		public static Thread mainthread = Thread.CurrentThread;
        static void Main(string[] args)
        {
			//Compiler.sharedCompiler.CompileFile("C:\\Developer\\Projects\\Translators.Lab01\\Translators.Lab01\\TextFile1.txt");
			Application.Init ();
			window = new MainWindow ();
			window.Show ();
			Application.Run ();
			//Console.ReadKey();
        }
    }
}

using System;
using Gtk;
using System.Threading;
using System.Threading.Tasks;

namespace Translators
{
	class Program
	{
		public static RootWindow window;
		public static Thread mainthread = Thread.CurrentThread;
		public static void Main (string[] args)
		{
			Application.Init ();
			window = new RootWindow ();
			window.Show ();
			Application.Run ();
		}
	}
}

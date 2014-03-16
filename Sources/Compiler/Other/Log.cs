using System;
using System.Diagnostics;

namespace Translators
{
	public class Out
	{
		public enum State {
			ApplicationError,
			ApplicationOutput,
			LogInfo,
			LogDebug,
			LogVerbose
		}

		public static State LogState = State.ApplicationOutput;
		public static void LogOneLine(State LogState, string str)
		{
			if (LogState <= Out.LogState) 
			{
				Gtk.Application.Invoke(delegate {
					Program.window.Console.Buffer.Text += str; });
				Out.Write(str);
			}
		}
		public static void Log(State LogState, string str)
		{
			if (LogState <= Out.LogState) 
			{
				Gtk.Application.Invoke(delegate {
					Program.window.Console.Buffer.Text += str + "\n"; });
				Out.WriteLine(str);
			}
		}
		
		public static void WriteLine(String fmt, params Object[] args)
		{
			Out.Write(fmt);
			Out.Write("\n");
		}
		public static void Write(String fmt, params Object[] args)
		{
			string op;
			if (fmt == null)
				op = String.Empty;
			else if (args == null || args.Length == 0)
				op = fmt;
			else
				op = String.Format(fmt, args);
			Trace.Write(op);
			Console.Write(op);
		}
	}
}


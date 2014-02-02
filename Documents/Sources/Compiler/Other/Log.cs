using System;

namespace Translators
{
	public class Out
	{
		public enum State {
			LogInfo,
			LogDebug,
			LogVerbose
		}

		public static State LogState = State.LogDebug;
		public static void LogOneLine(State LogState, string str)
		{
			if (LogState <= Out.LogState) 
			{
				Program.window.Console.Buffer.Text += str;
			}
		}
		public static void Log(State LogState, string str)
		{
			if (LogState <= Out.LogState) 
			{
				Program.window.Console.Buffer.Text += str + "\n";
			}
		}
	}
}


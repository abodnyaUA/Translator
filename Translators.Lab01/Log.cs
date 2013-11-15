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

		public static State LogState = State.LogInfo;
		public static void Log(State LogState, string str)
		{
			if (LogState <= Out.LogState) 
			{
				System.Console.WriteLine(str);
			}
		}
	}
}


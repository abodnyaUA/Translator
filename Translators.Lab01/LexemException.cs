using System;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class LexemException : Exception
	{
		public LexemException(int lineNumber, string comment)
		{
			string line = Parser.sharedParser.RealLines[lineNumber];
			Console.WriteLine("LINE = "+line);
			for (int i=0; i< Parser.sharedParser.RealLines.Count; i++)
				Console.WriteLine("Line_ "+i+":"+Parser.sharedParser.RealLines[i]);

			userInfo = "Line " + lineNumber + ": " + line + "\n" + 
					   "Error: " + comment;
		}
		private string userInfo;
		public string UserInfo { get { return userInfo; } }
	}
}


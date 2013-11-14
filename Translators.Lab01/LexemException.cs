using System;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class LexemException : Exception
	{
		public LexemException(int lineNumber, string comment)
		{
			string line = Parser.sharedParser.RealLines[lineNumber];
			userInfo = "Line " + lineNumber + ": " + line + "\n" + 
					   "Error: " + comment;
		}
		private string userInfo;
		public string UserInfo { get { return userInfo; } }
	}
}


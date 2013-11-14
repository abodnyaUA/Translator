using System;
using System.Collections.Generic;

namespace Translators.Lab01
{
	public class LexemException : Exception
	{
		public LexemException(int lineNumber, string comment)
		{
			userInfo = "Line: " + lineNumber + "\n"+ "Error: " + comment;
		}
		private string userInfo;
		public string UserInfo { get { return userInfo; } }
	}
}


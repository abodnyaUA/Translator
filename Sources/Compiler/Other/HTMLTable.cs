using System;
using System.IO;

namespace Translators
{
	public class HTMLTable
	{
		private string htmlContent;
		public int columsCount = 0;
		public int rowsCount = 1;
		public string style = "table { font-size: 8pt;}";
		public HTMLTable (params string[] lineHeaders)
		{
			htmlContent = "<html>\n<head>\n<style type=\"text/css\">\n" +
				style + 
				"\n</style>\n</head>\n" +
					"<body>\n<table border=\"1\">\n<tr>";
			foreach (string lineHeader in lineHeaders)
			{
				htmlContent += "<td>" + lineHeader + "</td>\n";
				columsCount++;
			}
			htmlContent += "</tr>";
		}

		public static HTMLTable Table(params string[] lineHeaders)
		{
			return new HTMLTable(lineHeaders);
		}

		public void AddLine(params string[] lineContent)
		{
			if (lineContent.Length > columsCount)
			{
				// TODO: Error handling
			}
			else
			{
				rowsCount++;
				htmlContent += "<tr>";
				foreach (string lineContentColum in lineContent)
				{
					string formatted = lineContentColum.Replace("<","&lt;").Replace(">","&gt;");
					htmlContent += "<td>" + formatted + "</td>\n";
				}
				htmlContent += "</tr>";
			}
		}

		public void WriteToFile(string filepath)
		{
			string htmlTable = htmlContent + "</table>\n</body>\n</html>";
			File.WriteAllLines(filepath,new string[] { htmlTable } );
		}
	}
}


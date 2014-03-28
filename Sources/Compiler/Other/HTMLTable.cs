using System;
using System.IO;

namespace Translators
{
	public class HTMLTable
	{
		private string htmlContent;
		public int columsCount = 0;
		public int rowsCount = 1;
		public string style = 
			"table " +
			"{ " +
			  "left: 0em; " +
			  "border-collapse: collapse; " +
			  "width: 100%; " +
			  "height: 100%;" +
			  "font-size: 8pt;" +
			  "font-family: Helvetica" +
			"} " +
			"td.head" +
			"{" +
			  "background: #F7F7F7;" +
			"}" +
			"td " +
			"{ " +
			  "border: 1px solid #DDD; " +
			  "min-width: 120px; " +
			  "padding: 5px;" +
			"}";
		public HTMLTable(params string[] lineHeaders)
		{
			htmlContent = "<html>\n<head>\n<meta charset=\"UTF-8\">\n<style type=\"text/css\">\n" +
				style + 
				"\n</style>\n</head>\n" +
					"<body>\n<table>\n<tr>";
			foreach (string lineHeader in lineHeaders)
			{
				htmlContent += "<td class=\"head\"><b>" + lineHeader + "</b></td>\n";
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


using System;
using System.Collections.Generic;

namespace Translators
{
	public class PolizAnalyzer
	{
		private static PolizAnalyzer _sharedAnalyzer = null;
		public static PolizAnalyzer sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new PolizAnalyzer();
				return _sharedAnalyzer;
			}
		}
		private PolizAnalyzer ()
		{
		}

		public PolizOperarionsList operationList = new PolizOperarionsList();

		public void CalculatePoliz(List<string> poliz)
		{
			poliz = FilteredPolizList(poliz);
			LogPoliz(poliz);
			CalculateExpression(poliz,0,poliz.Count-1);
		}

		private List<string> FilteredPolizList(List<string> poliz)
		{
			poliz.RemoveAll((string polizElement) => { return polizElement == "(" || polizElement == ")"; });
			return poliz;
		}

		private void LogPoliz(List<string> poliz)
		{
			Out.LogOneLine(Out.State.LogInfo,"POLIZ: ");
			foreach (string polizString in poliz)
			{
				Out.LogOneLine(Out.State.LogInfo,polizString.Replace("CONST_","")+" ");
			}
			Out.Log(Out.State.LogInfo,"");
		}

		public void CalculateExpression(List<string> poliz, int start, int end)
		{
			for (int i = start; i <= end; i++)
			{
				string polizElement = poliz[i];
				if (PolizAnalyzer.sharedAnalyzer.operationList.isOperation(polizElement))
				{
					int operand1 = Convert.ToInt32(poliz[i-2].Replace("CONST_",""));
					int operand2 = Convert.ToInt32(poliz[i-1].Replace("CONST_",""));
					int result = resultCalculation(operand1,operand2,polizElement);
					i -= 2;
					end -= 2;
					poliz.RemoveRange(i,3);
					poliz.Insert(i,result.ToString());
				}
			}
			Out.Log(Out.State.LogInfo,"Result calculation is: "+poliz[0].Replace("CONST_",""));
		}
		
		private int resultCalculation(int operand1, int operand2, string calcOperator)
		{
			int result = 0;
			switch (calcOperator)
			{
				case "+": result = operand1 + operand2; break;
				case "*": result = operand1 * operand2; break;
				case "/": result = operand1 / operand2; break;
				case "^": result = (int) (Math.Pow (operand1,operand2)); break;
				case "-": result = operand1 - operand2; break;
			}
			return result;
		}
	}
}


using System;
using System.Collections.Generic;

namespace Translators
{
	public class PolizCompiler
	{
		#region Singletone
		private static PolizCompiler _sharedCompiler = null;
		public static PolizCompiler sharedCompiler
		{
			get
			{
				if (_sharedCompiler == null) _sharedCompiler = new PolizCompiler();
				return _sharedCompiler;
			}
		}
		private PolizCompiler () {	}
		#endregion

		List<Lexem> poliz;
		public void Compile()
		{
			this.poliz = PolizAnalyzer.sharedAnalyzer.CompletePoliz;
			for (int i = 0; i < this.poliz.Count; i++)
			{
				PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
				if (poliz[i].Command == "=")
				{
					CalculateExpression(poliz,1,i-1);
					poliz[0].Value = poliz[1].Value;
					poliz.RemoveRange(0,3);
					i = -1;
				}

				else if (poliz[i].Key == PolizOperarionsList.kLexemKeyUPL) // if
				{
					CalculateLogicalExpression(poliz,0,i-2);
					if (poliz[0].Value == 0) // false. go to "else" block
					{
						string destination = poliz[1].Command;
						poliz.RemoveRange(0,3);
						while (destination != poliz[0].Command)
						{
							poliz.RemoveAt(0);
							PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
						}
						poliz.RemoveAt(0); // closed label. "else" block
						PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
					}
					else
					{
						poliz.RemoveRange(0,3);
					}
					i = -1;
				}

				else if (poliz[0].Key == PolizOperarionsList.kLexemKeyLabelStart &&
				         poliz[1].Key == PolizOperarionsList.kLexemKeyBP) // unused "else" block
				{
					string destination = poliz[0].Command;
					poliz.RemoveRange(0,3);
					PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
					while (destination != poliz[0].Command)
					{
						poliz.RemoveAt(0);
						PolizAnalyzer.sharedAnalyzer.LogLexems("PoliZ", this.poliz);
					}
					poliz.RemoveAt(0);
					PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
					i = -1;
				}

				else if (poliz[i].Key == PolizOperarionsList.kLexemKeyLabelEnd) // endif
				{
					poliz.RemoveAt(i);
					i = -1;
				}
			}
		}
		
		// Calculate expression //
		HashSet<string> mathOperations = new HashSet<string>()
		{ "+", "-", "*", "^", "/" };
		private void CalculateExpression(List<Lexem> poliz, int start, int end)
		{
			for (int i = start; i <= end; i++)
			{
				if (mathOperations.Contains(poliz[i].Command)) //operator
				{
					Lexem result = new Lexem(poliz[start].LineNumber,"0",Lexem.kConstKey);
					string operation = poliz[i].Command;
					int operand1 = poliz[i-2].Value;
					int operand2 = poliz[i-1].Value;
					result.Value = resultCalculation(operand1,operand2,operation);
					i -= 2;
					end -= 2;
					poliz.RemoveRange(i,3);
					poliz.Insert(i,result);
					PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
				}
			}
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
		
		// Calculate logical expression //
		HashSet<string> logicalOperations = new HashSet<string>()
		{ ">", "<", ">=", "<=", "equ", "!=", "or", "and" };
		private void CalculateLogicalExpression(List<Lexem> poliz, int start, int end)
		{
			// Convert all math expressions to single numbers //
			for (int i = start; i <= end; i++)
			{
				if (logicalOperations.Contains(poliz[i].Command)) //operator
				{
					string operation = poliz[i].Command;
					int lengthBefore = poliz.Count;
					
					// Calculate Math expression //
					CalculateExpression(poliz,start,i);
					int lengthAfter = poliz.Count;
					
					i -= (lengthBefore - lengthAfter);
					end -= (lengthBefore - lengthAfter);
					
					// Calculate logical expression
					Lexem result = new Lexem(poliz[start].LineNumber,"0",Lexem.kConstKey);
					int operand1 = poliz[i - 2].Value;
					int operand2 = poliz[i - 1].Value;
					result.Value = resultLogicalCalculation(operand1,operand2,operation);
					
					i -= 2;
					end -= 2;
					
					poliz.RemoveRange(i,3);
					poliz.Insert(i,result);
					PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
				}
			}
		}
		private int resultLogicalCalculation(int operand1, int operand2, string operation)
		{
			bool result = false;
			switch (operation)
			{
			case ">": result = operand1 > operand2; break;
			case ">=": result = operand1 >= operand2; break;
			case "<=": result = operand1 <= operand2; break;
			case "<": result = operand1 < operand2; break;
			case "!=": result = operand1 != operand2;  break;
			case "equ": result = operand1 == operand2;  break;
			case "and": result = operand1 == operand2;  break; // binary compare //
			case "or": result = operand1 + operand2 > 0; break;  // binary compare // 
			}
			return result ? 1 : 0;
		}
	}
}


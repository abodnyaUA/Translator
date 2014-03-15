using System;
using System.Collections.Generic;

namespace Translators
{
	public class PolizAnalyzer
	{
		#region Singletone
		private static PolizAnalyzer _sharedAnalyzer = null;
		public static PolizAnalyzer sharedAnalyzer
		{
			get
			{
				if (_sharedAnalyzer == null) _sharedAnalyzer = new PolizAnalyzer();
				return _sharedAnalyzer;
			}
		}
		private PolizAnalyzer () {	}

		#endregion

		#region Processing
		public void AnalyzeLexems()
		{
			this.stack = new List<Lexem>();
			this.poliz = new List<Lexem>();
			this.lexems = new List<Lexem>(LexemList.Instance.Lexems);
			UseOperatorsBlock();

			while (lexems.Count > 0)
			{
				ProcessLexem();
			}
			while (stack.Count > 0)
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			LogLexems("Poliz",poliz);
		}

		private void UseOperatorsBlock()
		{
			while (lexems[1].Command != "@implementation")
			{
				lexems.RemoveAt(0);
			}
			lexems.RemoveAt(0); // "\n"
			lexems.RemoveAt(0); // "@implementation"
			lexems.RemoveAt(0); // "\n"
			lexems.RemoveAt(lexems.Count-1); // "\n"
			lexems.RemoveAt(lexems.Count-1); // "@end"
		}

		private void ProcessLexem()
		{
			if (lexems[0].isIDorCONST())
			{
				this.poliz.Add(lexems[0]);
				this.lexems.RemoveAt(0);
			}
			else
			{
				if (this.stack.Count == 0)
				{
					this.stack.Add(lexems[0]);
					this.lexems.RemoveAt(0);
				}
				else 
				{
					if (operationList.LexemPriority(this.stack[stack.Count-1]) >= 
				        operationList.LexemPriority(lexems[0]) && 
					    !operationList.OpenScobe(lexems[0]))
					{
						Lexem lastStack = this.stack[stack.Count-1];
						stack.RemoveAt(stack.Count-1);
						this.poliz.Add(lastStack);
					}

					else if ((operationList.LexemPriority(this.stack[stack.Count-1]) < 
					     operationList.LexemPriority(lexems[0]) || 
					     operationList.OpenScobe(lexems[0])) &&
					    !operationList.CloseScobe(lexems[0]))
					{
						this.stack.Add(lexems[0]);
						this.lexems.RemoveAt(0);
					}

					else if (operationList.CloseScobe(lexems[0]))
					{
						this.lexems.RemoveAt(0);
						while (!operationList.OpenScobe(stack[stack.Count-1]) && stack.Count >= 0)
						{
							Lexem lastStack = this.stack[stack.Count-1];
							stack.RemoveAt(stack.Count-1);
							this.poliz.Add(lastStack);
						}
						stack.RemoveAt(stack.Count-1); // "(" "["
					}
				}
			}
			LogLexems("Stack",stack);
			LogLexems("Source",lexems);
			LogLexems("Poliz",poliz);
			Out.Log(Out.State.LogInfo,"");
		}

		public PolizOperarionsList operationList = new PolizOperarionsList();
		private List<Lexem> poliz;
		private List<Lexem> stack;
		private List<Lexem> lexems;
		private void LogLexems(string name, List<Lexem> list)
		{
			Out.LogOneLine(Out.State.LogInfo,name+": ");
			foreach (Lexem polizString in list)
			{
				Out.LogOneLine(Out.State.LogInfo,polizString.Command+" ");
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
		#endregion
	}
}


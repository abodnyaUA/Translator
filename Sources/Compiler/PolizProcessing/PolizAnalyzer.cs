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
			this.allPoliz = new List<List<Lexem>>();
			this.allPoliz.Add(new List<Lexem>());
			this.lexems = new List<Lexem>(LexemList.Instance.Lexems);
			UseOperatorsBlock();

			while (lexems.Count > 0)
			{
				ProcessLexem();
			}
			FinishCurrentPoliz();
			if (this.poliz.Count == 0)
			{
				allPoliz.RemoveAt(allPoliz.Count-1);
			}
			foreach (List<Lexem> poliz in this.allPoliz)
			{
				LogLexems("Poliz",poliz);
			}
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
			//lexems.RemoveAt(lexems.Count-1); // "\n"
			lexems.RemoveAt(lexems.Count-1); // "@end"
		}

		private void ProcessPolizResult()
		{
			if (this.poliz.Count > 0)
			{
				if (this.poliz[poliz.Count-1].Command == "=")
				{
					ProcessSetter();
				}
			}
		}

		private void ProcessSetter()
		{
			CalculateExpression(poliz,1,poliz.Count-2);
			poliz[0].Value = poliz[1].Value;
			Out.Log(Out.State.LogInfo,"VALUE OF "+poliz[0].Value.ToString());
		}

		private void FinishCurrentPoliz()
		{
			// Clear stack //
			while (stack.Count > 0)
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}

			ProcessPolizResult();

			// Start new poliz //
			if (lexems.Count > 0)
			{
				allPoliz.Add(new List<Lexem>());
				lexems.RemoveAt(0);
			}
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
					// If separator (\n), start writing new poliz //
					if (lexems[0].isSeparator())
					{
						FinishCurrentPoliz();
					}

					else if (operationList.LexemPriority(this.stack[stack.Count-1]) >= 
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
		private List<List<Lexem>> allPoliz;
		private List<Lexem> poliz 
		{
			get 
			{
				return allPoliz[allPoliz.Count - 1];
			}
		}

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

		public void CalculateExpression(List<Lexem> poliz, int start, int end)
		{
			for (int i = start; i <= end; i++)
			{
				if (poliz[i].Value == int.MaxValue) //operator
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
					LogLexems("Poliz", this.poliz);
				}
			}
			Out.Log(Out.State.LogInfo,"Result calculation is: "+poliz[0].Command+" = "+poliz[1].Value.ToString());
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


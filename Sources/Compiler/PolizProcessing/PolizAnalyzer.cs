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

			this.labelIterator = 1;
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

			CreateCompletePoliz();
			LogLexems("Complete Poliz",this.completePoliz);

		}

		private void UseOperatorsBlock()
		{
			while (lexems[0].Command != "@implementation")
			{
				lexems.RemoveAt(0);
			}
			lexems.RemoveAt(0); // "@implementation"
			lexems.RemoveAt(0); // "\n"
			lexems.RemoveAt(lexems.Count-1); // "@end"
		}

		/* Process completed polizes */
		private Stack<int> labels = new Stack<int>();
		private void FinishCurrentPoliz()
		{
			// Clear stack //
			while (stack.Count > 0)
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				if (lastStack.Command == "if" || lastStack.Command == "{" || lastStack.Command == "}")
				{
				}
				else if (lastStack.Command == "then")
				{
					Lexem label1 = new Lexem(lastStack.LineNumber,labelIterator.ToString(),
					                         PolizOperarionsList.kLexemKeyLabelStart);
					this.poliz.Add(label1);
					Lexem UPL = new Lexem(lastStack.LineNumber,"УПЛ",
					                      PolizOperarionsList.kLexemKeyUPL);
					this.poliz.Add(UPL);
					labels.Push(labelIterator);
					labelIterator+=2;
				}
				else if (lastStack.Command == "else")
				{
					int iterator = labels.Peek();
					Lexem label1 = new Lexem(lastStack.LineNumber,(iterator + 1).ToString(),
					                         PolizOperarionsList.kLexemKeyLabelStart);
					this.poliz.Add(label1);
					Lexem BP = new Lexem(lastStack.LineNumber,"БП",
					                     PolizOperarionsList.kLexemKeyBP);
					this.poliz.Add(BP);
					Lexem label2 = new Lexem(lastStack.LineNumber,(iterator).ToString(),
					                         PolizOperarionsList.kLexemKeyLabelEnd);
					this.poliz.Add(label2);
				}
				else if (lastStack.Command == "endif")
				{
					int iterator = labels.Pop();
					Lexem label2 = new Lexem(lastStack.LineNumber,(iterator + 1).ToString(),
					                         PolizOperarionsList.kLexemKeyLabelEnd);
					this.poliz.Add(label2);
				}
				else 
				{
					this.poliz.Add(lastStack);
				}
			}
			
			LogLexems("Stack",stack);
			LogLexems("Source",lexems);
			LogLexems("Poliz",poliz);
			Out.Log(Out.State.LogDebug,"");
			
			ProcessPolizResult();
			
			// Start new poliz //
			if (lexems.Count > 0)
			{
				allPoliz.Add(new List<Lexem>());
				lexems.RemoveAt(0);
			}
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
		}

		/* PreAnalyze */
		private int labelIterator = 0;
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
					ProcessOperatorLexem();
				}
			}
			LogLexems("Stack",stack);
			LogLexems("Source",lexems);
			LogLexems("Poliz",poliz);
			Out.Log(Out.State.LogInfo,"");
		}
		private void ProcessOperatorLexem()
		{
			// If separator (\n), start writing new poliz //
			if (lexems[0].isSeparator())
			{
				FinishCurrentPoliz();
			}

			/*else if (this.lexems[0].Command == "then")
			{
				this.lexems.RemoveAt(0);
				while (!stack[stack.Count-1].Command.StartsWith("if"))
				{
					Lexem lastStack = this.stack[stack.Count-1];
					stack.RemoveAt(stack.Count-1);
					this.poliz.Add(lastStack);
				}
			}*/

			// For expressions. Check ")" and "]" //
			else if (operationList.CloseScobe(lexems[0]))
			{
				this.lexems.RemoveAt(0);
				while (!operationList.OpenScobe(stack[stack.Count-1]))
				{
					Lexem lastStack = this.stack[stack.Count-1];
					stack.RemoveAt(stack.Count-1);
					this.poliz.Add(lastStack);
				}
				stack.RemoveAt(stack.Count-1); // "(" "["
			}
			
			else if (operationList.LexemPriority(this.stack[stack.Count-1]) >= 
			         operationList.LexemPriority(lexems[0]) && 
			         !operationList.OpenScobe(lexems[0]))
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			
			else if (operationList.LexemPriority(this.stack[stack.Count-1]) < 
			         operationList.LexemPriority(lexems[0]) || 
			         operationList.OpenScobe(lexems[0]))
			{
				this.stack.Add(lexems[0]);
				this.lexems.RemoveAt(0);
			}
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
		private List<Lexem> completePoliz;
		private void CreateCompletePoliz()
		{
			completePoliz = new List<Lexem>();
			foreach (List<Lexem> poliz in this.allPoliz)
			{
				foreach (Lexem lexem in poliz)
				{
					completePoliz.Add(lexem);
				}
			}
		}

		private List<Lexem> stack;
		private List<Lexem> lexems;
		private void LogLexems(string name, List<Lexem> list)
		{
			Out.LogOneLine(Out.State.LogInfo,name+": ");
			foreach (Lexem polizString in list)
			{
				string str = "";
				if (polizString.Key == PolizOperarionsList.kLexemKeyLabelStart)
				{
					str = "m"+polizString.Command;
				}
				else if (polizString.Key == PolizOperarionsList.kLexemKeyLabelEnd)
				{
					str = "m"+polizString.Command+":";
				}
				else
				{
					str = polizString.Command.Replace("\n","ENTER");
				}
				Out.LogOneLine(Out.State.LogDebug,str+" ");
			}
			Out.Log(Out.State.LogDebug,"");
		}

		// Calculate expression //
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


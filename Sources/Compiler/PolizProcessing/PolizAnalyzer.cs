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
			this.stack = new Stack<Lexem>();
			this.poliz = new List<Lexem>();
			this.lexems = new List<Lexem>(LexemAnalyzer.sharedAnalyzer.Lexems);
			UseOperatorsBlock();

			while (lexems.Count > 0)
			{
				ProcessLexem();
			}
			foreach (Lexem stackElement in this.stack)
			{
				this.poliz.Add(stackElement);
			}

			FilteredPolizList();
			LogPoliz();
		}

		private void UseOperatorsBlock()
		{
			while (lexems[1].command != "@implementation")
			{
				lexems.RemoveAt(0);
			}
			lexems.RemoveAt(0); // "\n"
			lexems.RemoveAt(0); // "@implementation"
			lexems.RemoveAt(0); // "\n"
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
				int lastStackPriority = this.stack.Count > 0 ? 
					operationList.LexemPriority(this.stack.Peek()) : int.MaxValue;
				Out.Log(Translators.Out.State.LogInfo,"Priority: " + lastStackPriority + 
				        "; Element: "+lexems[0].command);
				if (lastStackPriority == int.MaxValue)
				{
					this.stack.Push(lexems[0]);
					this.lexems.RemoveAt(0);
				}
				else if (lastStackPriority >= operationList.LexemPriority(lexems[0]))
				{
					this.stack.Push(lexems[0]);
					this.lexems.RemoveAt(0);
				}
				else
				{
					Lexem lastStack = this.stack.Pop();
					this.poliz.Add(lastStack);
				}
			}
			LogPoliz();
		}

		public PolizOperarionsList operationList = new PolizOperarionsList();
		private List<Lexem> poliz;
		private Stack<Lexem> stack;
		private List<Lexem> lexems;

		public void CalculatePoliz(List<string> poliz)
		{
			//poliz = FilteredPolizList(poliz);
			LogPoliz();
			CalculateExpression(poliz,0,poliz.Count-1);
		}

		private List<Lexem> FilteredPolizList()
		{
			poliz.RemoveAll((Lexem polizElement) => { 
				return polizElement.command == "(" || polizElement.command == ")"; 
			});
			return poliz;
		}

		private void LogPoliz()
		{
			Out.LogOneLine(Out.State.LogInfo,"POLIZ: ");
			foreach (Lexem polizString in this.poliz)
			{
				Out.LogOneLine(Out.State.LogInfo,polizString.command+" ");
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


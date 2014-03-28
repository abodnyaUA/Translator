using System;
using System.Collections.Generic;
using System.Threading;
using Gtk;

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

		private int LastInputValue;
		void DidConfirmInputDialog (object obj, ResponseArgs args)
		{
			InputIDDialog dialog = (InputIDDialog)obj;
			LastInputValue = dialog.Result();
		}

		List<Lexem> poliz;
		public void Compile()
		{
			LastInputValue = int.MaxValue;
			int commandIterator = 0;
			this.poliz = PolizAnalyzer.sharedAnalyzer.Poliz;
			for (int i = 0; i < this.poliz.Count; i++)
			{
				PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
				if (poliz[i].Command == "=")
				{
					Out.Log(Out.State.LogDebug, "Will change value for ID "+poliz[commandIterator].Command+
					        " (old value "+poliz[commandIterator].Value+")");
					poliz[commandIterator].Value = CalculateExpression(new List<Lexem>(this.poliz),
					                                                   commandIterator+1,i-1);
					Out.Log(Out.State.LogDebug, "Did change value for ID "+poliz[commandIterator].Command+
					        " (new value "+poliz[commandIterator].Value+")");
					commandIterator = i+1;
				}

				else if (poliz[i].Command == "output")
				{
					for (int j = commandIterator; j < i; j++)
					{
						if (poliz[j].Command[0] == '"')
						{
							Out.Log(Out.State.ApplicationOutput,poliz[j].Command.
							        Replace("\"","").Replace("_"," ").Replace("   "," ").Remove(0,1));
						}
						else if (poliz[j].isCONST())
						{
							Out.Log(Out.State.ApplicationOutput,poliz[j].Command);
						}
						else
						{
							Out.Log(Out.State.ApplicationOutput,poliz[j].Command+" = "+poliz[j].Value);
						}
					}
					commandIterator = i+1;
				}
				
				else if (poliz[i].Command == "input")
				{
					for (int j = commandIterator; j < i; j++)
					{
						Gtk.Application.Invoke(delegate {
							InputIDDialog dialog = new InputIDDialog();
							dialog.Title = "Введите значение "+poliz[j].Command;
							dialog.Response += DidConfirmInputDialog;
							dialog.Run();
							dialog.Destroy();
						});
						// Synchronize threads. Better use Mutex or Semaphore //
						while (LastInputValue == int.MaxValue)
						{
							Thread.Sleep(100);
						}
						poliz[j].Value = LastInputValue;
						LastInputValue = int.MaxValue;
					}
					commandIterator = i+1;
				}

				else if (poliz[i].Key == PolizOperarionsList.kLexemKeyUPL) // if
				{
					bool ok = CalculateLogicalExpression(commandIterator,i-2);
					if (false == ok) // false. go to "else" block
					{
						string destinationLabel = poliz[i-1].Command;
						i = LabelFinder.PositionCloseLabel(poliz,destinationLabel);
					}
					commandIterator = i+1;
				}

				else if (poliz[commandIterator].Key == PolizOperarionsList.kLexemKeyLabelStart &&
				         poliz[commandIterator + 1].Key == PolizOperarionsList.kLexemKeyBP) // unused "else" block
				{
					string destinationLabel = poliz[commandIterator].Command;
					i  = LabelFinder.PositionCloseLabel(poliz, destinationLabel);
					commandIterator = i+1;
				}

				else if (poliz[i].Key == PolizOperarionsList.kLexemKeyLabelEnd) // endif
				{
					i++;
					commandIterator = i;
				}
			}
			PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", this.poliz);
			Out.Log(Out.State.ApplicationOutput,"\n" +
				"============================\n" +
				"Приложение отработало корректно\n" +
				"Результаты вычислений приблизительные, поскольку программа " +
				"работает только с целыми числами");
		}
		
		// Calculate expression //
		HashSet<string> mathOperations = new HashSet<string>()
		{ "+", "-", "*", "^", "/", "root" };
		private int CalculateExpression(List<Lexem> poliz, int start, int end)
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
					PolizAnalyzer.sharedAnalyzer.LogLexems("Poliz", poliz);
				}
			}
			return poliz[start].Value;
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
			case "root": result = (int) (Math.Pow (operand1,1.0/operand2));; break;
			}
			return result;
		}
		
		// Calculate logical expression //
		HashSet<string> logicalOperations = new HashSet<string>()
		{ ">", "<", ">=", "<=", "equ", "!=", "or", "and" };
		private bool CalculateLogicalExpression(int start, int end)
		{
			List<Lexem> poliz = new List<Lexem>(this.poliz);
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
			return poliz[start].Value == 1;
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
			case "and": result = operand1 + operand2 == 2;  break; // binary compare //
			case "or": result = operand1 + operand2 > 0; break;  // binary compare // 
			}
			return result ? 1 : 0;
		}
	}
}


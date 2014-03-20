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
			htmlTable = new HTMLTable("Полиз","Стек","Входная цепочка");

			this.labelIterator = 1;
			while (lexems.Count > 0)
			{
				ProcessLexem();
			}

			FinishCurrentPoliz();
		
			if (Out.LogState != Out.State.ApplicationOutput)
			{
			 	Out.LogState = Out.State.LogDebug;
			}
			LogLexems("Complete Poliz",this.poliz);
			htmlTable.WriteToFile(Constants.HTMLTablePath);
			//Out.LogState = Out.State.LogInfo;
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
				
				if (lastStack.Command == "{" || lastStack.Command == "}" || lastStack.Command == "if"){}
				else if (lastStack.Command == "then")
				{
					Lexem label1 = new Lexem(lastStack.LineNumber,"m"+labelIterator.ToString(),
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
					Lexem label1 = new Lexem(lastStack.LineNumber,"m"+(iterator + 1).ToString(),
					                         PolizOperarionsList.kLexemKeyLabelStart);
					this.poliz.Add(label1);
					Lexem BP = new Lexem(lastStack.LineNumber,"БП",
					                     PolizOperarionsList.kLexemKeyBP);
					this.poliz.Add(BP);
					Lexem label2 = new Lexem(lastStack.LineNumber,"m"+(iterator).ToString(),
					                         PolizOperarionsList.kLexemKeyLabelEnd);
					this.poliz.Add(label2);
				}
				else if (lastStack.Command == "endif")
				{
					int iterator = labels.Pop();
					Lexem label2 = new Lexem(lastStack.LineNumber,"m"+(iterator + 1).ToString(),
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

			// Remove "\n" //
			if (lexems.Count > 0)
			{
				lexems.RemoveAt(0);
			}
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
			
			LogToHTMLTable();
			LogLexems("Stack",stack);
			LogLexems("Source",lexems);
			LogLexems("Poliz",poliz);
			Out.Log(Out.State.LogDebug,"");
		}
		private void ProcessOperatorLexem()
		{
			// If separator (\n), start writing new poliz //
			if (lexems[0].isSeparator())
			{
				FinishCurrentPoliz();
			}

			// Remove "," for output and input operations
			else if (lexems[0].Command == ",")
			{
				this.lexems.RemoveAt(0);
			}

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
		private List<Lexem> poliz;
		public List<Lexem> Poliz { get { return poliz; } }

		private List<Lexem> stack;
		private List<Lexem> lexems;

		/* Logging */
		private HTMLTable htmlTable;
		string ListToString(List<Lexem> list)
		{
			string str = "";
			foreach (Lexem polizString in list)
			{
				if (polizString.Key == PolizOperarionsList.kLexemKeyLabelStart)
				{
					str += polizString.Command;
				}
				else if (polizString.Key == PolizOperarionsList.kLexemKeyLabelEnd)
				{
					str += polizString.Command+":";
				}
				else
				{
					str += polizString.Command.Replace("\n","ENTER");
				}
				str += " ";
			}
			return str;
		}

		private void LogToHTMLTable()
		{
			string stackLine = ListToString(this.stack);
			string polizLine = ListToString(this.poliz);
			string lexemsLine = ListToString(this.lexems);
			htmlTable.AddLine(polizLine,stackLine,lexemsLine);
		}
		public void LogLexems(string name, List<Lexem> list)
		{
			if (Out.LogState >= Out.State.LogDebug)
			{
				string str = ListToString(list);
				Out.Log(Out.State.LogInfo,name+": "+str);
			}
		}

		#endregion
	}
}


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

			ProcessSeparator();
		
			if (Out.LogState != Out.State.ApplicationOutput)
			{
			 	Out.LogState = Out.State.LogDebug;
			}
			LogLexems("Complete Poliz",this.poliz);
			htmlTable.WriteToFile(Constants.HTMLTablePath);
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
				ProcessOperatorLexem();
			}
			
			LogToHTMLTable();
			LogLexems("Stack",stack);
			LogLexems("Source",lexems);
			LogLexems("Poliz",poliz);
			Out.Log(Out.State.LogDebug,"");
		}

		private Lexem cycleIterator;
		bool OpenScobe()
		{
			return operationList.LexemPriority(this.stack[stack.Count-1]) < 
				operationList.LexemPriority(lexems[0]) || 
					operationList.OpenScobe(lexems[0]);
		}
		bool LowPriority()
		{
			return operationList.LexemPriority(this.stack[stack.Count-1]) >= 
				operationList.LexemPriority(lexems[0]) && 
					!operationList.OpenScobe(lexems[0]);
		}
		private void ProcessOperatorLexem()
		{
			if (lexems[0].isSeparator()) 			ProcessSeparator();
			else if (lexems[0].Command == ",")		ProcessComma();
			else if (lexems[0].Command == "for")	ProcessFor();
			else if (lexems[0].Command == "step")	ProcessStep();
			else if (lexems[0].Command == "to")		ProcessTo();
			else if (lexems[0].Command == "do") 	ProcessDo();
			else if (lexems[0].Command == "next")	ProcessNext();
			else if (this.stack.Count == 0) 		ProcessEmptyStack();
			else if (operationList.CloseScobe(lexems[0]))	ProcessCloseScobe();
			else if (LowPriority())					ProcessLowPriorityLexem();
			else if (OpenScobe()) 					ProcessOpenScobe();
		}
		private void ProcessSeparator()
		{
			// Clear stack //
			while (stack.Count > 0 && stack[stack.Count-1].Command != "for")
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				
				if (lastStack.Command == "{" || lastStack.Command == "}" || lastStack.Command == "if"){}
				else if (lastStack.Command == "then") ProcessThen(lastStack);
				else if (lastStack.Command == "else") ProcessElse(lastStack);
				else if (lastStack.Command == "endif")ProcessEndIf(lastStack);
				else this.poliz.Add(lastStack);
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
		private void ProcessThen(Lexem lastStack)
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
		private void ProcessElse(Lexem lastStack)
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
		private void ProcessEndIf(Lexem lastStack)
		{
			int iterator = labels.Pop();
			
			int position = LabelFinder.PositionOpenLabel(poliz,"m"+(iterator+1).ToString());
			
			if (position == -1) // without "else"
			{
				this.poliz.Add(new Lexem(lastStack.LineNumber,"m"+(iterator).ToString(),
				                         PolizOperarionsList.kLexemKeyLabelEnd));
			}
			else // with "else"
			{
				this.poliz.Add(new Lexem(lastStack.LineNumber,"m"+(iterator + 1).ToString(),
				                         PolizOperarionsList.kLexemKeyLabelEnd));
			}
		}
		private void ProcessComma()
		{
			// Remove "," for output and input operations
			this.lexems.RemoveAt(0);
		}
		private void ProcessFor()
		{
			this.labels.Push(this.labelIterator);
			
			this.stack.Add(lexems[0]);
			this.labelIterator += 3;
			
			this.lexems.RemoveAt(0);
			
			cycleIterator = this.lexems[0];
		}
		private void ProcessStep()
		{
			while (stack[stack.Count-1].Command != "for")
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			
			int line = lexems[0].LineNumber;
			int iterator = labels.Peek();
			
			this.poliz.Add(new Lexem(line,"r1",Lexem.kIDKey));
			this.poliz.Add(new Lexem(line,"1",Lexem.kConstKey));
			this.poliz.Add(new Lexem(line,"=",LexemList.Instance.KeyForOperator("=")));
			this.poliz.Add(new Lexem(line,"m"+(iterator).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelEnd));
			this.poliz.Add(new Lexem(line,"r2",Lexem.kIDKey));
			
			this.lexems.RemoveAt(0);
		}
		private void ProcessTo()
		{
			while (stack[stack.Count-1].Command != "for")
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			
			int line = lexems[0].LineNumber;
			int iterator = labels.Peek();
			
			this.poliz.Add(new Lexem(line,"=",LexemList.Instance.KeyForOperator("=")));
			this.poliz.Add(new Lexem(line,"r1",Lexem.kIDKey));
			this.poliz.Add(new Lexem(line,"0",Lexem.kConstKey));
			this.poliz.Add(new Lexem(line,"equ",LexemList.Instance.KeyForOperator("equ")));
			this.poliz.Add(new Lexem(line,"m"+(iterator+1).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelStart));
			this.poliz.Add(new Lexem(line,"УПЛ",PolizOperarionsList.kLexemKeyUPL));
			this.poliz.Add(cycleIterator);
			this.poliz.Add(cycleIterator);
			this.poliz.Add(new Lexem(line,"r2",Lexem.kIDKey));
			this.poliz.Add(new Lexem(line,"+",LexemList.Instance.KeyForOperator("+")));
			this.poliz.Add(new Lexem(line,"=",LexemList.Instance.KeyForOperator("=")));
			this.poliz.Add(new Lexem(line,"m"+(iterator+1).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelEnd));
			this.poliz.Add(new Lexem(line,"r1",Lexem.kIDKey));
			this.poliz.Add(new Lexem(line,"0",Lexem.kConstKey));
			this.poliz.Add(new Lexem(line,"=",LexemList.Instance.KeyForOperator("=")));
			this.poliz.Add(cycleIterator);
			
			this.lexems.RemoveAt(0);
		}
		private void ProcessDo()
		{
			while (stack[stack.Count-1].Command != "for")
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			
			int line = lexems[0].LineNumber;
			int iterator = labels.Peek();
			
			this.poliz.Add(new Lexem(line,"-",LexemList.Instance.KeyForOperator("-")));
			this.poliz.Add(new Lexem(line,"r2",Lexem.kIDKey));
			this.poliz.Add(new Lexem(line,"*",LexemList.Instance.KeyForOperator("*")));
			this.poliz.Add(new Lexem(line,"0",Lexem.kConstKey));
			this.poliz.Add(new Lexem(line,"<=",LexemList.Instance.KeyForOperator("<=")));
			this.poliz.Add(new Lexem(line,"m"+(iterator+2).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelStart));
			this.poliz.Add(new Lexem(line,"УПЛ",PolizOperarionsList.kLexemKeyUPL));
			
			this.lexems.RemoveAt(0);
		}
		private void ProcessNext()
		{
			while (stack[stack.Count-1].Command != "for")
			{
				Lexem lastStack = this.stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				this.poliz.Add(lastStack);
			}
			stack.RemoveAt(stack.Count-1); // for
			
			int line = lexems[0].LineNumber;
			int iterator = labels.Pop();
			
			this.poliz.Add(new Lexem(line,"m"+(iterator).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelStart));
			this.poliz.Add(new Lexem(line,"БП",PolizOperarionsList.kLexemKeyBP));
			this.poliz.Add(new Lexem(line,"m"+(iterator+2).ToString(),
			                         PolizOperarionsList.kLexemKeyLabelEnd));
			
			this.lexems.RemoveAt(0);
		}
		private void ProcessCloseScobe()
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
		private void ProcessEmptyStack()
		{			
			this.stack.Add(lexems[0]);
			this.lexems.RemoveAt(0);
		}
		private void ProcessOpenScobe()
		{
			this.stack.Add(lexems[0]);
			this.lexems.RemoveAt(0);
		}
		private void ProcessLowPriorityLexem()
		{
			Lexem lastStack = this.stack[stack.Count-1];
			stack.RemoveAt(stack.Count-1);
			this.poliz.Add(lastStack);
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

